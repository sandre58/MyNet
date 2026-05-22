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
    private readonly Dictionary<BehaviorKey, IObservableBehavior> _behaviors = [];
    private readonly List<IPropertyChangingBehavior> _changing = [];
    private readonly List<IPropertyChangedBehavior> _changed = [];
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
    /// Gets all registered behaviors in the registry. This property returns an array of all behaviors that have been registered in the registry, regardless of their type (property changing, property changed, disposing, or attached). The array is built by taking the values from the internal dictionary of behaviors, which allows for efficient retrieval of all registered behaviors. This can be useful for scenarios where you need to iterate over all behaviors or perform actions on all registered behaviors without needing to differentiate between their types.
    /// </summary>
    public IObservableBehavior[] All
    {
        get
        {
            lock (_gate)
            {
                return [.. _behaviors.Values];
            }
        }
    }

    #region Register (pipeline behaviors)

    /// <summary>
    /// Registers a behavior in the registry. This method takes an IObservableBehavior as a parameter and adds it to the list of attached behaviors. If the behavior also implements IPropertyChangedBehavior or IPropertyChangingBehavior, it is added to the respective lists for those types of behaviors as well. The pipeline cache is rebuilt each time a new behavior is registered to ensure that the execution order is maintained. This allows for efficient execution of the behaviors in the correct order when properties change or when an object is being disposed.
    /// </summary>
    /// <param name="behavior">The behavior to register.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement", Justification = "Switch statement would be less efficient here since we are checking for multiple interfaces on the same object.")]
    public void Register(IObservableBehavior behavior, string? propertyName = null, string? scope = null)
    {
        ThrowIfDisposed();

        lock (_gate)
        {
            _behaviors[new(behavior.GetType(), scope, propertyName)] = behavior;

            if (behavior is IPropertyChangedBehavior changedBehavior)
                _changed.Add(changedBehavior);

            if (behavior is IPropertyChangingBehavior changingBehavior)
                _changing.Add(changingBehavior);

            Rebuild();
        }
    }

    #endregion

    #region Lookup

    /// <summary>
    /// Tries to get a behavior of the specified type from the registry. This method searches through the lists of attached behaviors, property changing behaviors, property changed behaviors, and disposing behaviors in that order to find a behavior of the specified type. If a behavior of the specified type is found, it is returned through the out parameter and the method returns true. If no behavior of the specified type is found, the out parameter is set to null and the method returns false. This allows for efficient retrieval of behaviors based on their type, enabling dynamic access to registered behaviors as needed during the execution of observable objects.
    /// </summary>
    /// <param name="behavior">The behavior instance if found; otherwise, null.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>True if a behavior of the specified type is found; otherwise, false.</returns>
    public bool TryGet<T>([NotNullWhen(true)] out T? behavior, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        lock (_gate)
        {
            var key = !string.IsNullOrEmpty(propertyName) || !string.IsNullOrEmpty(scope) ? new BehaviorKey(typeof(T), propertyName, scope) : (BehaviorKey?)null;
            if (key is not null && _behaviors.TryGetValue(key.Value, out var found) && found is T typed)
            {
                behavior = typed;
                return true;
            }

            behavior = _behaviors.Values.OfType<T>().FirstOrDefault();
            return behavior is not null;
        }
    }

    /// <summary>
    /// Gets a behavior of the specified type from the registry. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, it is returned. If no behavior of the specified type is found, an InvalidOperationException is thrown with a message indicating that the behavior was not found. This method provides a convenient way to retrieve behaviors when you expect them to be present in the registry, while also providing clear error handling when they are not found.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>The behavior instance of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a behavior of the specified type is not found.</exception>
    public T Get<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => TryGet<T>(out var b, propertyName, scope)
            ? b
            : throw new InvalidOperationException($"Behavior {typeof(T).Name} not found for propertyName {propertyName ?? "<null>"} and scope {scope ?? "<null>"}.");

    #endregion

    #region Api

    /// <summary>
    /// Checks if a behavior of the specified type is registered in the registry. This method calls TryGet to determine if a behavior of the specified type exists in the registry. It returns true if a behavior of the specified type is found, and false otherwise. This allows for quick checks to see if a particular behavior is available in the registry without needing to retrieve the behavior instance itself.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to check.</typeparam>
    /// <returns>True if a behavior of the specified type is registered; otherwise, false.</returns>
    public bool Has<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => TryGet<T>(out _, propertyName, scope);

    /// <summary>
    /// Gets a behavior of the specified type from the registry, or returns null if no behavior of that type is found. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, it is returned; otherwise, null is returned. This provides a convenient way to access behaviors when their presence in the registry is optional, allowing for more flexible code that can handle cases where certain behaviors may not be registered without throwing exceptions.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>The behavior instance of the specified type, or null if not found.</returns>
    public T? GetOrDefault<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        TryGet<T>(out var behavior, propertyName, scope);

        return behavior;
    }

    /// <summary>
    /// Tries to execute an action on a behavior of the specified type if it is registered in the registry. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided action is executed with the behavior as its parameter, and the method returns true. If no behavior of the specified type is found, the method returns false and the action is not executed. This allows for conditional execution of actions based on the presence of specific behaviors in the registry, enabling more dynamic and flexible code that can adapt to different configurations of registered behaviors.
    /// </summary>
    /// <param name="action">The action to execute on the behavior if it is found.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to execute the action on.</typeparam>
    /// <returns>True if the action was executed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    public bool TryExecute<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!TryGet<T>(out var behavior, propertyName, scope))
            return false;

        action(behavior);
        return true;
    }

    /// <summary>
    /// Executes an action on a behavior of the specified type. This method calls Get to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided action is executed with the behavior as its parameter. If no behavior of the specified type is found, an InvalidOperationException is thrown with a message indicating that the behavior was not found. This method provides a convenient way to execute actions on behaviors when you expect them to be present in the registry, while also providing clear error handling when they are not found.
    /// </summary>
    /// <param name="action">The action to execute on the behavior.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to execute the action on.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the behavior is not found.</exception>
    public void Execute<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(action);

        action(Get<T>(propertyName, scope));
    }

    /// <summary>
    /// Tries to evaluate a selector function on a behavior of the specified type if it is registered in the registry. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided selector function is executed with the behavior as its parameter, and the result is returned through the out parameter. The method returns true if the selector was executed; otherwise, false if no behavior of the specified type was found. This allows for conditional evaluation of functions based on the presence of specific behaviors in the registry, enabling more dynamic and flexible code that can adapt to different configurations of registered behaviors.
    /// </summary>
    /// <param name="selector">The selector function to evaluate on the behavior.</param>
    /// <param name="result">The result of the selector function if the behavior is found; otherwise, the default value of TResult.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="TBehavior">The type of the behavior to evaluate the selector on.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the selector function.</typeparam>
    /// <returns>True if the selector was executed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    public bool TryEvaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, [MaybeNullWhen(false)] out TResult result, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!TryGet<TBehavior>(out var behavior, propertyName, scope))
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
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="TBehavior">The type of the behavior to evaluate the selector on.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the selector function.</typeparam>
    /// <returns>The result of the selector function if the behavior is found; otherwise, the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    public TResult Evaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, TResult defaultValue = default!, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(selector);

        return TryGet<TBehavior>(out var behavior, propertyName, scope) ? selector(behavior) : defaultValue;
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
            foreach (var b in _behaviors.Values.OfType<IDisposable>())
                b.Dispose();

            _behaviors.Clear();
            _changing.Clear();
            _changed.Clear();

            Rebuild();
        }
    }

    /// <summary>
    /// Throws an ObjectDisposedException if the registry has been disposed. This method is called at the beginning of each public method to ensure that operations are not performed on a disposed registry, which could lead to undefined behavior or exceptions. By throwing an ObjectDisposedException, it provides a clear indication that the registry is no longer available for use and helps prevent potential issues caused by accessing disposed resources.
    /// </summary>
    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, nameof(BehaviorRegistry));
}

/// <summary>
/// Represents a key for identifying a behavior in the registry. This struct is used to uniquely identify a behavior based on its scope and property name. Scope can be used to differentiate behaviors that are registered for different scopes (e.g., different observable objects or contexts), and PropertyName can be used to differentiate behaviors that are registered for different properties. By using a BehaviorKey, the registry can efficiently manage and retrieve behaviors based on their unique identifiers, allowing for more flexible and dynamic behavior management in observable objects.
/// </summary>dynamic behavior /summary>
/// <param name="BehaviorTye">The type of the behavior.</param>
/// <param name="Scope">The scope of the behavior.</param>
/// <param name="PropertyName">The name of the property associated with the behavior.</param>
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Properties are used for equality and hashing, not for direct access.")]
public readonly record struct BehaviorKey(Type BehaviorTye, string? Scope = null, string? PropertyName = null);
