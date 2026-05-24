// -----------------------------------------------------------------------
// <copyright file="ScopedActionRunner.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MyNet.Utilities.Execution;

/// <summary>
/// Runs an action with disposable scopes active for the duration, optional start/end hooks per subscriber,
/// and optional manual completion for work that outlives the initial call stack.
/// </summary>
public sealed class ScopedActionRunner : IDisposable
{
    private readonly Func<Action, bool> _run;
    private readonly Dictionary<object, List<Func<IDisposable>>> _scopeFactories = [];
    private readonly Dictionary<object, List<Action>> _startHandlers = [];
    private readonly Dictionary<object, List<Action>> _endHandlers = [];
    private readonly Stopwatch _stopwatch = new();
    private readonly bool _useStopwatch;
    private List<IDisposable> _activeScopes = [];
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedActionRunner"/> class.
    /// </summary>
    public ScopedActionRunner(Action action, bool useStopwatch = false)
        : this(_ =>
        {
            action();
            return true;
        },
        useStopwatch)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedActionRunner"/> class.
    /// The run ends when the callback is invoked, or automatically when the delegate returns if it did not invoke the callback.
    /// </summary>
    public ScopedActionRunner(Action<Action> action, bool useStopwatch = false)
        : this(complete =>
        {
            var ended = false;

            action(executeComplete);
            return !ended;

            void executeComplete()
            {
                if (ended)
                    return;

                ended = true;
                complete();
            }
        },
        useStopwatch)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedActionRunner"/> class with a custom run implementation.
    /// </summary>
    /// <param name="run">A function that executes the action and returns a boolean indicating whether the run should end automatically.</param>
    /// <param name="useStopwatch">A value indicating whether to use a stopwatch to measure the elapsed time of the run.</param>
    private ScopedActionRunner(Func<Action, bool> run, bool useStopwatch)
    {
        _run = run ?? throw new ArgumentNullException(nameof(run));
        _useStopwatch = useStopwatch;
    }

    /// <summary>
    /// Gets a value indicating whether a run is in progress.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets the elapsed time of the last completed run when stopwatch timing was enabled in the constructor.
    /// </summary>
    public TimeSpan LastElapsed => _stopwatch.Elapsed;

    /// <summary>
    /// Registers a factory that creates a disposable scope for each run.
    /// </summary>
    public void RegisterScope(object subscriber, Func<IDisposable> createScope)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(createScope);
        SubscriberLists.Add(_scopeFactories, subscriber, createScope);
    }

    /// <summary>
    /// Registers a handler invoked at the start of each run.
    /// </summary>
    public void RegisterOnStart(object subscriber, Action handler)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(handler);
        SubscriberLists.Add(_startHandlers, subscriber, handler);
    }

    /// <summary>
    /// Registers a handler invoked at the end of each run.
    /// </summary>
    public void RegisterOnEnd(object subscriber, Action handler)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(handler);
        SubscriberLists.Add(_endHandlers, subscriber, handler);
    }

    /// <summary>
    /// Removes all registrations for <paramref name="subscriber"/>.
    /// </summary>
    public void Unregister(object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        _ = _scopeFactories.Remove(subscriber);
        _ = _startHandlers.Remove(subscriber);
        _ = _endHandlers.Remove(subscriber);
    }

    /// <summary>
    /// Executes the bound action when no other run is active.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when a run is already in progress.</exception>
    public void Run()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsRunning)
            throw new InvalidOperationException("A run is already in progress.");

        IsRunning = true;
        InvokeHandlers(_startHandlers);

        _activeScopes = CreateScopes();
        var shouldEnd = true;

        try
        {
            if (_useStopwatch)
                _stopwatch.Restart();

            shouldEnd = _run(EndRun);
        }
        finally
        {
            if (_useStopwatch)
                _stopwatch.Stop();

            if (shouldEnd)
                EndRun();
        }
    }

    /// <summary>
    /// Invokes all handlers in the given dictionary, which may be either start or end handlers depending on the context. The method iterates through the values of the dictionary, which are lists of handlers, and invokes each handler in order. This allows for flexible registration of multiple handlers per subscriber, and ensures that all registered handlers are executed when a run starts or ends. Any exceptions thrown by the handlers will propagate up to the caller of Run, allowing for proper error handling during handler execution.
    /// </summary>
    /// <param name="handlers">The dictionary of handlers to invoke.</param>
    private static void InvokeHandlers(Dictionary<object, List<Action>> handlers)
    {
        foreach (var handler in handlers.Values.SelectMany(static list => list))
            handler.Invoke();
    }

    /// <summary>
    /// Ends the current run, invoking end handlers and disposing scopes. Safe to call multiple times and from any context, allowing for manual completion of the run when the initial action outlives the call stack. Does nothing if no run is active.
    /// </summary>
    private void EndRun()
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        InvokeHandlers(_endHandlers);
        DisposeScopes();
    }

    /// <summary>
    /// Creates the disposable scopes for the current run by invoking all registered scope factories. The resulting list contains the active scopes that will be disposed at the end of the run. Each factory is invoked to create a new scope instance, allowing for dynamic and flexible scope management based on the current subscribers and their registrations. The scopes are created in the order of registration, and any exceptions thrown by the factories will propagate up to the caller of Run, ensuring proper error handling during scope creation.
    /// </summary>
    /// <returns>A list of disposable scopes for the current run.</returns>
    private List<IDisposable> CreateScopes() =>
        [.. _scopeFactories.Values.SelectMany(static factories => factories).Select(static factory => factory.Invoke())];

    /// <summary>
    /// Disposes all active scopes for the current run, ensuring that any resources associated with the scopes are properly released. This method iterates through the list of active scopes and calls Dispose on each one, allowing for deterministic cleanup of resources at the end of the run. After disposing all scopes, the list of active scopes is cleared to prepare for the next run. This method is safe to call multiple times and will not throw exceptions if called when no run is active or if some scopes have already been disposed, making it robust for various execution scenarios.
    /// </summary>
    private void DisposeScopes()
    {
        foreach (var scope in _activeScopes)
            scope.Dispose();

        _activeScopes = [];
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        DisposeScopes();
    }
}

/// <summary>
/// Runs an action with input and result payloads, disposable scopes, and optional manual completion.
/// </summary>
public sealed class ScopedActionRunner<TIn, TOut> : IDisposable
{
    private readonly Func<TIn, Action<TOut>, bool> _run;
    private readonly Dictionary<object, List<Func<IDisposable>>> _scopeFactories = [];
    private readonly Dictionary<object, List<Action<TOut>>> _startHandlers = [];
    private readonly Dictionary<object, List<Action<TOut>>> _endHandlers = [];
    private readonly Stopwatch _stopwatch = new();
    private readonly bool _useStopwatch;
    private List<IDisposable> _activeScopes = [];
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedActionRunner{TIn, TOut}"/> class.
    /// </summary>
    public ScopedActionRunner(Action<TIn> action, bool useStopwatch = false)
        : this((input, _) =>
        {
            action(input);
            return true;
        },
        useStopwatch)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedActionRunner{TIn, TOut}"/> class.
    /// </summary>
    public ScopedActionRunner(Action<TIn, Action<TOut>> action, bool useStopwatch = false)
        : this((input, complete) =>
        {
            var ended = false;

            action(input, executeComplete);
            return !ended;

            void executeComplete(TOut value)
            {
                if (ended)
                    return;

                ended = true;
                complete(value);
            }
        },
        useStopwatch)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedActionRunner{TIn, TOut}"/> class with a custom run implementation.
    /// </summary>
    /// <param name="run">The custom run implementation.</param>
    /// <param name="useStopwatch">Indicates whether to use a stopwatch to measure elapsed time.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="run"/> is null.</exception>
    private ScopedActionRunner(Func<TIn, Action<TOut>, bool> run, bool useStopwatch)
    {
        _run = run ?? throw new ArgumentNullException(nameof(run));
        _useStopwatch = useStopwatch;
    }

    /// <summary>
    /// Gets a value indicating whether a run is in progress.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets the elapsed time of the last completed run when stopwatch timing was enabled.
    /// </summary>
    public TimeSpan LastElapsed => _stopwatch.Elapsed;

    /// <summary>
    /// Registers a factory that creates a disposable scope for each run.
    /// </summary>
    public void RegisterScope(object subscriber, Func<IDisposable> createScope)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(createScope);
        SubscriberLists.Add(_scopeFactories, subscriber, createScope);
    }

    /// <summary>
    /// Registers a handler invoked at the start of each run with the initial result value.
    /// </summary>
    public void RegisterOnStart(object subscriber, Action<TOut> handler)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(handler);
        SubscriberLists.Add(_startHandlers, subscriber, handler);
    }

    /// <summary>
    /// Registers a handler invoked at the end of each run with the final result value.
    /// </summary>
    public void RegisterOnEnd(object subscriber, Action<TOut> handler)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(handler);
        SubscriberLists.Add(_endHandlers, subscriber, handler);
    }

    /// <summary>
    /// Removes all registrations for <paramref name="subscriber"/>.
    /// </summary>
    public void Unregister(object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        _ = _scopeFactories.Remove(subscriber);
        _ = _startHandlers.Remove(subscriber);
        _ = _endHandlers.Remove(subscriber);
    }

    /// <summary>
    /// Executes the bound action when no other run is active.
    /// </summary>
    public void Run(TIn input, Func<TOut> resultFactory)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(resultFactory);

        if (IsRunning)
            throw new InvalidOperationException("A run is already in progress.");

        var result = resultFactory();
        IsRunning = true;
        InvokeHandlers(_startHandlers, result);

        _activeScopes = CreateScopes();
        var shouldEnd = true;

        try
        {
            if (_useStopwatch)
                _stopwatch.Restart();

            shouldEnd = _run(input, EndRun);
        }
        finally
        {
            if (_useStopwatch)
                _stopwatch.Stop();

            if (shouldEnd)
                EndRun(result);
        }
    }

    /// <summary>
    /// Invokes all handlers in the given dictionary with the provided value, which may be either start or end handlers depending on the context. The method iterates through the values of the dictionary, which are lists of handlers, and invokes each handler in order with the given value. This allows for flexible registration of multiple handlers per subscriber, and ensures that all registered handlers are executed with the appropriate result value when a run starts or ends. Any exceptions thrown by the handlers will propagate up to the caller of Run, allowing for proper error handling during handler execution.
    /// </summary>
    /// <param name="handlers">The dictionary of handlers to invoke.</param>
    /// <param name="value">The value to pass to each handler.</param>
    private static void InvokeHandlers(Dictionary<object, List<Action<TOut>>> handlers, TOut value)
    {
        foreach (var handler in handlers.Values.SelectMany(static list => list))
            handler.Invoke(value);
    }

    /// <summary>
    /// Ends the current run with the given result, invoking end handlers and disposing scopes. Safe to call multiple times and from any context, allowing for manual completion of the run when the initial action outlives the call stack. Does nothing if no run is active.
    /// </summary>
    /// <param name="result">The result of the run.</param>
    private void EndRun(TOut result)
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        InvokeHandlers(_endHandlers, result);
        DisposeScopes();
    }

    /// <summary>
    /// Creates the disposable scopes for the current run by invoking all registered scope factories. The resulting list contains the active scopes that will be disposed at the end of the run. Each factory is invoked to create a new scope instance, allowing for dynamic and flexible scope management based on the current subscribers and their registrations. The scopes are created in the order of registration, and any exceptions thrown by the factories will propagate up to the caller of Run, ensuring proper error handling during scope creation.
    /// </summary>
    /// <returns>The list of active scopes for the current run.</returns>
    private List<IDisposable> CreateScopes() =>
        [.. _scopeFactories.Values.SelectMany(static factories => factories).Select(static factory => factory.Invoke())];

    /// <summary>
    /// Disposes all active scopes for the current run, ensuring that any resources associated with the scopes are properly released. This method iterates through the list of active scopes and calls Dispose on each one, allowing for deterministic cleanup of resources at the end of the run. After disposing all scopes, the list of active scopes is cleared to prepare for the next run. This method is safe to call multiple times and will not throw exceptions if called when no run is active or if some scopes have already been disposed, making it robust for various execution scenarios.
    /// </summary>
    private void DisposeScopes()
    {
        foreach (var scope in _activeScopes)
            scope.Dispose();

        _activeScopes = [];
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        DisposeScopes();
    }
}
