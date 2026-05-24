// -----------------------------------------------------------------------
// <copyright file="SingleTaskDeferrer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNet.Utilities.Deferring;
using MyNet.Utilities.Suspending;

namespace MyNet.Utilities.Threading;

/// <summary>
/// Coordinates deferred, optionally throttled refresh requests with at-most-one asynchronous execution.
/// While a task runs, a new request cancels it and schedules a rerun when the current execution ends.
/// </summary>
/// <remarks>
/// Compose <see cref="Deferring.DeferredAction"/> (batch updates), <see cref="Suspender"/> (temporary mute),
/// and <see cref="SingleTaskRunner"/> (single in-flight task). Use <see cref="Request"/> from property setters
/// and <see cref="Defer"/> when applying several changes in one scope.
/// </remarks>
public sealed class SingleTaskDeferrer : IDisposable, IDeferrable, ISuspendable
{
    private readonly DeferredAction _deferredAction;
    private readonly Suspender _suspender = new();
    private readonly SingleTaskRunner _runner;
    private readonly Debouncer? _debouncer;
    private readonly Action<bool>? _onRunningChanged;
    private bool _mustRerun;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleTaskDeferrer"/> class using a synchronous action (wrapped as a completed task).
    /// </summary>
    public SingleTaskDeferrer(
        Action<CancellationToken> action,
        Action<bool>? onRunningChanged = null,
        Action? onCancelled = null,
        int throttleMilliseconds = 0,
        ILogger? logger = null)
        : this(
            cancellationToken =>
            {
                action(cancellationToken);
                return Task.CompletedTask;
            },
            onRunningChanged,
            onCancelled,
            throttleMilliseconds,
            logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleTaskDeferrer"/> class using an asynchronous action.
    /// </summary>
    public SingleTaskDeferrer(
        Func<CancellationToken, Task> action,
        Action<bool>? onRunningChanged = null,
        Action? onCancelled = null,
        int throttleMilliseconds = 0,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (throttleMilliseconds < 0)
            ArgumentOutOfRangeException.ThrowIfNegative(throttleMilliseconds);

        _deferredAction = new(FlushDeferredRequest);
        _debouncer = throttleMilliseconds > 0 ? new Debouncer(throttleMilliseconds) : null;
        _onRunningChanged = onRunningChanged;

        _runner = new(
            action,
            OnRunningChanged,
            onCancelled,
            logger);
    }

    private void OnRunningChanged(bool isRunning)
    {
        _onRunningChanged?.Invoke(isRunning);

        if (isRunning || !_mustRerun)
            return;

        _mustRerun = false;
        _ = _runner.RunAsync();
    }

    /// <summary>
    /// Gets a value indicating whether execution is currently deferred.
    /// </summary>
    public bool IsDeferred => _deferredAction.IsDeferred;

    /// <summary>
    /// Gets a value indicating whether refresh requests are currently suspended.
    /// </summary>
    public bool IsSuspended => _suspender.IsSuspended;

    /// <summary>
    /// Gets a value indicating whether the underlying task is running.
    /// </summary>
    public bool IsRunning => _runner.IsRunning;

    /// <inheritdoc />
    public IDisposable Defer() => _deferredAction.Defer();

    /// <inheritdoc />
    public IDisposable Suspend() => _suspender.Suspend();

    /// <summary>
    /// Requests execution of the bound work, honoring deferral, suspension, and throttle settings.
    /// </summary>
    public void Request() => _deferredAction.Request();

    /// <summary>
    /// Cancels the current execution if any.
    /// </summary>
    public void Cancel() => _runner.Cancel();

    private void FlushDeferredRequest()
    {
        if (_suspender.IsSuspended)
            return;

        if (_debouncer is null)
        {
            DispatchExecution();
            return;
        }

        _debouncer.Schedule(DispatchExecution);
    }

    private void DispatchExecution()
    {
        if (_runner.IsRunning)
        {
            _mustRerun = true;
            _runner.Cancel();
            return;
        }

        _ = _runner.RunAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _debouncer?.Dispose();
        _runner.Dispose();
    }
}
