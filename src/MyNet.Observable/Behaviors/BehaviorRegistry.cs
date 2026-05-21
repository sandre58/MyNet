// -----------------------------------------------------------------------
// <copyright file="BehaviorRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Registry for behaviors. Behaviors are registered in the registry and then executed in the order they were registered. The registry is used to store pipeline behaviors (property changing, property changed, disposing) and attached behaviors (behaviors that are attached to an observable object and can be disposed when the object is disposed).
/// </summary>
internal sealed class BehaviorRegistry : IDisposable
{
    private readonly List<IPropertyChangingBehavior> _changing = [];
    private readonly List<IPropertyChangedBehavior> _changed = [];
    private readonly List<IObservableBehavior> _attached = [];
    private readonly Lock _gate = new();
    private bool _disposed;

    /// <summary>
    /// Gets the registered property changing behaviors. These behaviors are executed in the order they were registered when a property is about to change. Each behavior can perform actions such as validation, logging, or canceling the change by throwing an exception. The array is rebuilt each time a new behavior is registered to ensure that the execution order is maintained.
    /// </summary>
    public IPropertyChangingBehavior[] Changing { get; private set; } = [];

    /// <summary>
    /// Gets the registered property changed behaviors. These behaviors are executed in the order they were registered after a property has changed. Each behavior can perform actions such as updating related properties, logging, or triggering side effects based on the new property value. The array is rebuilt each time a new behavior is registered to ensure that the execution order is maintained.
    /// </summary>
    public IPropertyChangedBehavior[] Changed { get; private set; } = [];

    /// <summary>
    /// Gets the registered attached behaviors. These behaviors are attached to an observable object and can be disposed when the object is disposed.
    /// </summary>
    public IObservableBehavior[] Attached { get; private set; } = [];

    #region Register (pipeline behaviors)

    /// <summary>
    /// Registers a behavior in the registry. This method takes an IObservableBehavior as a parameter and adds it to the list of attached behaviors. If the behavior also implements IPropertyChangedBehavior or IPropertyChangingBehavior, it is added to the respective lists for those types of behaviors as well. The pipeline cache is rebuilt each time a new behavior is registered to ensure that the execution order is maintained. This allows for efficient execution of the behaviors in the correct order when properties change or when an object is being disposed.
    /// </summary>
    /// <param name="behavior">The behavior to register.</param>
    public void Register(IObservableBehavior behavior)
    {
        ThrowIfDisposed();

        lock (_gate)
        {
            _attached.Add(behavior);

            switch (behavior)
            {
                case IPropertyChangedBehavior changedBehavior:
                    _changed.Add(changedBehavior);
                    break;
                case IPropertyChangingBehavior changingBehavior:
                    _changing.Add(changingBehavior);
                    break;
            }

            Rebuild();
        }
    }

    #endregion

    #region Lookup

    /// <summary>
    /// Tries to get a behavior of the specified type from the registry. This method searches through the lists of attached behaviors, property changing behaviors, property changed behaviors, and disposing behaviors in that order to find a behavior of the specified type. If a behavior of the specified type is found, it is returned through the out parameter and the method returns true. If no behavior of the specified type is found, the out parameter is set to null and the method returns false. This allows for efficient retrieval of behaviors based on their type, enabling dynamic access to registered behaviors as needed during the execution of observable objects.
    /// </summary>
    /// <param name="behavior">The behavior instance if found; otherwise, null.</param>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>True if a behavior of the specified type is found; otherwise, false.</returns>
    public bool TryGet<T>([NotNullWhen(true)] out T? behavior)
        where T : class
    {
        behavior = _attached.OfType<T>().FirstOrDefault();

        return behavior is not null;
    }

    /// <summary>
    /// Gets a behavior of the specified type from the registry. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, it is returned. If no behavior of the specified type is found, an InvalidOperationException is thrown with a message indicating that the behavior was not found. This method provides a convenient way to retrieve behaviors when you expect them to be present in the registry, while also providing clear error handling when they are not found.
    /// </summary>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>The behavior instance of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a behavior of the specified type is not found.</exception>
    public T Get<T>()
        where T : class
        => TryGet<T>(out var b) ? b : throw new InvalidOperationException($"Behavior {typeof(T).Name} not found");

    #endregion

    #region Api

    /// <summary>
    /// Checks if a behavior of the specified type is registered in the registry. This method calls TryGet to determine if a behavior of the specified type exists in the registry. It returns true if a behavior of the specified type is found, and false otherwise. This allows for quick checks to see if a particular behavior is available in the registry without needing to retrieve the behavior instance itself.
    /// </summary>
    /// <typeparam name="T">The type of the behavior to check.</typeparam>
    /// <returns>True if a behavior of the specified type is registered; otherwise, false.</returns>
    public bool Has<T>()
        where T : class
        => TryGet<T>(out _);

    /// <summary>
    /// Gets a behavior of the specified type from the registry, or returns null if no behavior of that type is found. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, it is returned; otherwise, null is returned. This provides a convenient way to access behaviors when their presence in the registry is optional, allowing for more flexible code that can handle cases where certain behaviors may not be registered without throwing exceptions.
    /// </summary>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>The behavior instance of the specified type, or null if not found.</returns>
    public T? GetOrDefault<T>()
        where T : class
    {
        TryGet<T>(out var behavior);

        return behavior;
    }

    /// <summary>
    /// Tries to execute an action on a behavior of the specified type if it is registered in the registry. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided action is executed with the behavior as its parameter, and the method returns true. If no behavior of the specified type is found, the method returns false and the action is not executed. This allows for conditional execution of actions based on the presence of specific behaviors in the registry, enabling more dynamic and flexible code that can adapt to different configurations of registered behaviors.
    /// </summary>
    /// <param name="action">The action to execute on the behavior if it is found.</param>
    /// <typeparam name="T">The type of the behavior to execute the action on.</typeparam>
    /// <returns>True if the action was executed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    public bool TryExecute<T>(Action<T> action)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!TryGet<T>(out var behavior))
            return false;

        action(behavior);
        return true;
    }

    /// <summary>
    /// Executes an action on a behavior of the specified type. This method calls Get to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided action is executed with the behavior as its parameter. If no behavior of the specified type is found, an InvalidOperationException is thrown with a message indicating that the behavior was not found. This method provides a convenient way to execute actions on behaviors when you expect them to be present in the registry, while also providing clear error handling when they are not found.
    /// </summary>
    /// <param name="action">The action to execute on the behavior.</param>
    /// <typeparam name="T">The type of the behavior to execute the action on.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the behavior is not found.</exception>
    public void Execute<T>(Action<T> action)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(action);

        action(Get<T>());
    }

    /// <summary>
    /// Tries to evaluate a selector function on a behavior of the specified type if it is registered in the registry. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided selector function is executed with the behavior as its parameter, and the result is returned through the out parameter. The method returns true if the selector was executed; otherwise, false if no behavior of the specified type was found. This allows for conditional evaluation of functions based on the presence of specific behaviors in the registry, enabling more dynamic and flexible code that can adapt to different configurations of registered behaviors.
    /// </summary>
    /// <param name="selector">The selector function to evaluate on the behavior.</param>
    /// <param name="result">The result of the selector function if the behavior is found; otherwise, the default value of TResult.</param>
    /// <typeparam name="TBehavior">The type of the behavior to evaluate the selector on.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the selector function.</typeparam>
    /// <returns>True if the selector was executed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    public bool TryEvaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, [MaybeNullWhen(false)] out TResult result)
        where TBehavior : class
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!TryGet<TBehavior>(out var behavior))
        {
            result = default;
            return false;
        }

        result = selector(behavior);

        return true;
    }

    /// <summary>
    /// Evaluates a selector function on a behavior of the specified type. This method calls Get to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided selector function is executed with the behavior as its parameter, and the result is returned. If no behavior of the specified type is found, the default value is returned. This method provides a convenient way to evaluate functions on behaviors when you expect them to be present in the registry, while also providing a fallback value when they are not found.
    /// </summary>
    /// <param name="selector">The selector function to evaluate on the behavior.</param>
    /// <param name="defaultValue">The default value to return if the behavior is not found.</param>
    /// <typeparam name="TBehavior">The type of the behavior to evaluate the selector on.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the selector function.</typeparam>
    /// <returns>The result of the selector function if the behavior is found; otherwise, the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    public TResult Evaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, TResult defaultValue = default!)
        where TBehavior : class
    {
        ArgumentNullException.ThrowIfNull(selector);

        return TryGet<TBehavior>(out var behavior) ? selector(behavior) : defaultValue;
    }

    #endregion

    #region Rebuild pipeline cache

    /// <summary>
    /// Rebuilds the pipeline cache for property changing, property changed, and disposing behaviors. This method is called each time a new behavior is registered to ensure that the execution order of the behaviors is maintained. The arrays for Changing, Changed, and Disposing are updated to reflect the current list of registered behaviors. This allows for efficient execution of the behaviors in the correct order when properties change or when an object is being disposed.
    /// </summary>
    private void Rebuild()
    {
        Changing = [.. _changing];
        Changed = [.. _changed];
        Attached = [.. _attached];
    }

    #endregion

    /// <summary>
    /// Disposes the behavior registry and all registered behaviors. This method is called when the registry is no longer needed, allowing for cleanup of resources and proper disposal of any behaviors that implement IDisposable. The method ensures that each behavior in the Changing, Changed, Disposing, and Attached lists is disposed if it implements IDisposable, and then clears the lists to release references to the behaviors. This helps prevent memory leaks and ensures that all resources are properly released when the registry is disposed.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        lock (_gate)
        {
            foreach (var b in _attached.OfType<IDisposable>())
                b.Dispose();

            _attached.Clear();
            _changing.Clear();
            _changed.Clear();
        }
    }

    /// <summary>
    /// Throws an ObjectDisposedException if the registry has been disposed. This method is called at the beginning of each public method to ensure that operations are not performed on a disposed registry, which could lead to undefined behavior or exceptions. By throwing an ObjectDisposedException, it provides a clear indication that the registry is no longer available for use and helps prevent potential issues caused by accessing disposed resources.
    /// </summary>
    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, nameof(BehaviorRegistry));
}
