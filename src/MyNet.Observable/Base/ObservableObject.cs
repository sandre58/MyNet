// -----------------------------------------------------------------------
// <copyright file="ObservableObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.CompilerServices;
using MyNet.Observable.Behaviors;
using MyNet.Utilities.Suspending;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A base class for objects of which the properties must be observable.
/// </summary>
public abstract class ObservableObject : IObservableObject
{
    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> PropertyChangedEventArgsCache = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, PropertyChangingEventArgs> PropertyChangingEventArgsCache = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<(Type Type, string PropertyName), Func<object, object?>> PropertyGetterCache = new();

    private readonly Suspender _propertyNotificationsSuspender = new();

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in DisposeManagedResources")]
    private readonly BehaviorRegistry _behaviors = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableObject"/> class and applies metadata-driven behaviors.
    /// </summary>
    protected ObservableObject() => MetadataBehaviorApplicator.Apply(this);

    /// <summary>
    /// Gets the collection of disposables that will be disposed when the object is disposed.
    /// </summary>
    protected CompositeDisposable Disposables { get; } = [];

    /// <summary>
    /// Gets a value indicating whether property notifications are currently suspended.
    /// </summary>
    protected bool AreNotificationsSuspended => _propertyNotificationsSuspender.IsSuspended;

    /// <summary>
    /// Gets a value indicating whether the object has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    #region INotifyPropertyChanging

    /// <summary>
    /// Occurs when a property value is changing. This event is raised before the property value changes, allowing subscribers to react to the change before it happens. The event handlers receive the sender (the ObservableObject that raised the event) and a PropertyChangingEventArgs instance containing the name of the property that is changing. By subscribing to this event, you can execute custom logic before a property changes, such as validation, modification tracking, or collection tracking, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Handles the property changing event by executing custom logic before a property changes. This method is called before a property changes, and receives the name of the property that is changing, the value of the property before the change, and the value of the property after the change. Implementing this method allows you to define custom behavior that should be executed before a property changes, such as validation, modification tracking, or collection tracking, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="propertyName">The name of the property that is changing.</param>
    /// <param name="before">The value of the property before the change.</param>
    /// <param name="after">The value of the property after the change.</param>
    protected virtual void ProcessPropertyChanging(string propertyName, object? before, object? after)
    {
        if (ShouldSkipNotification(propertyName))
            return;

        if (AreNotificationsSuspended)
            return;

        var context = new PropertyMutationContext
        {
            Sender = this,
            PropertyName = propertyName,
            OldValue = before,
            NewValue = after
        };

        foreach (var behavior in _behaviors.Changing)
        {
            behavior.OnPropertyChanging(context);
        }

        OnPropertyChangingCore(context);

        RaisePropertyChanging(propertyName);
    }

    /// <summary>
    /// Raises the PropertyChanging event for the specified property name, with the provided before and after values. This method calls the ProcessPropertyChanging method to raise the event with the specified before and after values, allowing subscribers to react to the impending change with knowledge of both the old and new values. By calling this method, you can ensure that subscribers are properly notified before a property changes, enabling them to execute any necessary logic in response to the change, such as validation, modification tracking, or collection tracking, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="propertyName">The name of the property that is changing.</param>
    /// <param name="before">The value of the property before the change.</param>
    /// <param name="after">The value of the property after the change.</param>
    protected virtual void OnPropertyChanging(string propertyName, object? before, object? after) => ProcessPropertyChanging(propertyName, before, after);

    /// <summary>
    /// Raises the PropertyChanging event for the specified property name. This method is called to notify subscribers that a property is about to change. It creates a PropertyChangingEventArgs instance for the specified property name and invokes the PropertyChanging event handlers, allowing them to react to the impending change. By calling this method, you can ensure that subscribers are properly notified before a property changes, enabling them to execute any necessary logic in response to the change.
    /// </summary>
    /// <param name="propertyName">The name of the property that is changing.</param>
    private void RaisePropertyChanging(string propertyName) => PropertyChanging?.Invoke(this, GetPropertyChangingEventArgs(propertyName));

    /// <summary>
    /// Provides a core implementation for handling the property changing event. This method is called after all behaviors have been executed in response to a property changing event, allowing derived classes to implement additional logic that should be executed before a property changes. By overriding this method, you can define custom behavior that should be executed in response to a property changing event, such as additional validation, logging, or other side effects, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="context">The context of the property changing event, containing information about the sender, property name, old value, and new value.</param>
    protected virtual void OnPropertyChangingCore(PropertyMutationContext context)
    {
    }

    #endregion

    #region INotifyPropertyChanged

    /// <summary>
    /// Occurs when a property value has changed. This event is raised after the property value has changed, allowing subscribers to react to the change after it happens. The event handlers receive the sender (the ObservableObject that raised the event) and a PropertyChangedEventArgs instance containing the name of the property that has changed. By subscribing to this event, you can execute custom logic after a property changes, such as updating the UI, triggering other actions, or performing additional processing in response to the change, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Handles the property changed event by executing custom logic after a property has changed. This method is called after a property has changed, and receives the name of the property that has changed, the value of the property before the change, and the value of the property after the change. Implementing this method allows you to define custom behavior that should be executed after a property changes, such as updating the UI, triggering other actions, or performing additional processing in response to the change, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    /// <param name="before">The value of the property before the change.</param>
    /// <param name="after">The value of the property after the change.</param>
    protected virtual void ProcessPropertyChanged(string propertyName, object? before, object? after)
    {
        if (ShouldSkipNotification(propertyName))
            return;

        if (AreNotificationsSuspended)
            return;

        var context = new PropertyMutationContext
        {
            Sender = this,
            PropertyName = propertyName,
            OldValue = before,
            NewValue = after
        };

        foreach (var behavior in _behaviors.Changed)
        {
            behavior.OnPropertyChanged(context);
        }

        OnPropertyChangedCore(context);

        RaisePropertyChanged(propertyName);
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property name, with the provided before and after values. This method calls the ProcessPropertyChanged method to raise the event with the specified before and after values, allowing subscribers to react to the change with knowledge of both the old and new values. By calling this method, you can ensure that subscribers are properly notified after a property changes, enabling them to execute any necessary logic in response to the change, such as updating the UI, triggering other actions, or performing additional processing in response to the change, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    /// <param name="before">The value of the property before the change.</param>
    /// <param name="after">The value of the property after the change.</param>
    protected virtual void OnPropertyChanged(string propertyName, object? before, object? after) => ProcessPropertyChanged(propertyName, before, after);

    /// <summary>
    /// Raises the PropertyChanged event for the specified property name. This method is called to notify subscribers that a property has changed. It creates a PropertyChangedEventArgs instance for the specified property name and invokes the PropertyChanged event handlers, allowing them to react to the change. By calling this method, you can ensure that subscribers are properly notified after a property changes, enabling them to execute any necessary logic in response to the change.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    private void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, GetPropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Provides a core implementation for handling the property changed event. This method is called after all behaviors have been executed in response to a property changed event, allowing derived classes to implement additional logic that should be executed after a property changes. By overriding this method, you can define custom behavior that should be executed in response to a property changed event, such as additional processing, logging, or other side effects, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="context">The context of the property changed event, containing information about the sender, property name, old value, and new value.</param>
    protected virtual void OnPropertyChangedCore(PropertyMutationContext context)
    {
    }

    /// <summary>
    /// Notifies subscribers that a property has changed by raising the PropertyChanged event for the specified property name. This method retrieves the current value of the property using the GetPropertyValue method and then calls the ProcessPropertyChanged method to raise the event with the current value as both the old and new values. By calling this method, you can ensure that subscribers are properly notified after a property changes, allowing them to execute any necessary logic in response to the change.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    protected internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var currentValue = GetPropertyValue(propertyName);

        ProcessPropertyChanged(propertyName, UnknownValue.Instance, currentValue);
    }

    /// <summary>
    /// Notifies subscribers that a property has changed by raising the PropertyChanged event for the specified property name, with the provided before and after values. This method calls the ProcessPropertyChanged method to raise the event with the specified before and after values, allowing subscribers to react to the change with knowledge of both the old and new values. By calling this method, you can ensure that subscribers are properly notified after a property changes, enabling them to execute any necessary logic in response to the change, such as updating the UI, triggering other actions, or performing additional processing in response to the change, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    /// <param name="before">The value of the property before the change.</param>
    /// <param name="after">The value of the property after the change.</param>
    protected internal void NotifyPropertyChanged(string propertyName, object? before, object? after) => ProcessPropertyChanged(propertyName, before, after);

    #endregion

    #region Notification Suspension

    /// <summary>
    /// Suspends property notifications until the returned scope is disposed.
    /// Deferred PropertyChanged notifications are replayed when suspension ends.
    /// </summary>
    /// <returns>A disposable suspension scope.</returns>
    protected IDisposable SuspendNotifications() => IsDisposed ? Disposable.Empty : _propertyNotificationsSuspender.Suspend();

    /// <summary>
    /// Determines whether property change notifications should be skipped based on the current state of the object and the provided property name. This method checks if the object has been disposed or if the property name is null, empty, or consists only of whitespace. If either of these conditions is true, it returns true, indicating that notifications should be skipped; otherwise, it returns false, allowing notifications to proceed.
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <returns>True if notifications should be skipped; otherwise, false.</returns>
    private bool ShouldSkipNotification(string? propertyName) => IsDisposed || string.IsNullOrWhiteSpace(propertyName);

    #endregion

    #region Behaviors

    /// <summary>
    /// Adds a behavior to the observable object. This method allows you to add any behavior that implements the <see cref="IObservableBehavior"/> interface to the observable object, enabling you to enhance its functionality in a modular and reusable way. By calling this method with a specific behavior, you can easily add new features or capabilities to your observable objects without modifying their original implementation or creating new derived classes.
    /// </summary>
    /// <param name="behavior">The behavior to add.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to register.</typeparam>
    public void RegisterBehavior<T>(T behavior, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior => _behaviors.Register(behavior, propertyName, scope);

    /// <summary>
    /// Tries to get a behavior of the specified type that is attached to the observable object. This method can be called by derived classes to retrieve a specific behavior that is currently attached to the observable object using the Attach method. By calling this method with a specific behavior type, you can attempt to retrieve that behavior if it is currently attached, allowing you to interact with its functionality or properties in a modular and reusable way. The method returns true if the behavior of the specified type is found and assigned to the out parameter; otherwise, it returns false and assigns null to the out parameter.
    /// </summary>
    /// <param name="behavior">When this method returns, contains the behavior of the specified type if it is found; otherwise, null.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>True if the behavior of the specified type is found; otherwise, false.</returns>
    public bool TryGetBehavior<T>([NotNullWhen(true)] out T? behavior, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior => _behaviors.TryGet(out behavior, propertyName, scope);

    /// <summary>
    /// Gets a behavior of the specified type that is attached to the observable object. This method can be called by derived classes to retrieve a specific behavior that is currently attached to the observable object using the Attach method. By calling this method with a specific behavior type, you can retrieve that behavior if it is currently attached, allowing you to interact with its functionality or properties in a modular and reusable way. If the behavior of the specified type is not found, this method throws an InvalidOperationException, indicating that the requested behavior is not currently attached to the observable object.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>The behavior of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the behavior of the specified type is not found.</exception>
    public T GetBehavior<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior => _behaviors.Get<T>(propertyName, scope);

    /// <summary>
    /// Checks if a behavior of the specified type is registered in the registry. This method calls TryGet to determine if a behavior of the specified type exists in the registry. It returns true if a behavior of the specified type is found, and false otherwise. This allows for quick checks to see if a particular behavior is available in the registry without needing to retrieve the behavior instance itself.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to check.</typeparam>
    /// <returns>True if a behavior of the specified type is registered; otherwise, false.</returns>
    public bool HasBehavior<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior => _behaviors.Has<T>(propertyName, scope);

    /// <summary>
    /// Gets a behavior of the specified type from the registry, or returns null if no behavior of that type is found. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, it is returned; otherwise, null is returned. This provides a convenient way to access behaviors when their presence in the registry is optional, allowing for more flexible code that can handle cases where certain behaviors may not be registered without throwing exceptions.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to retrieve.</typeparam>
    /// <returns>The behavior instance of the specified type, or null if not found.</returns>
    public T? GetBehaviorOrDefault<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior => _behaviors.GetOrDefault<T>(propertyName, scope);

    /// <summary>
    /// Tries to execute an action on a behavior of the specified type if it is registered in the registry. This method calls TryGet to attempt to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided action is executed with the behavior as its parameter, and the method returns true. If no behavior of the specified type is found, the method returns false and the action is not executed. This allows for conditional execution of actions based on the presence of specific behaviors in the registry, enabling more dynamic and flexible code that can adapt to different configurations of registered behaviors.
    /// </summary>
    /// <param name="action">The action to execute on the behavior if it is found.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to execute the action on.</typeparam>
    /// <returns>True if the action was executed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    public bool TryExecuteBehavior<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior => _behaviors.TryExecute(action, propertyName, scope);

    /// <summary>
    /// Executes an action on a behavior of the specified type. This method calls Get to retrieve a behavior of the specified type. If a behavior of the specified type is found, the provided action is executed with the behavior as its parameter. If no behavior of the specified type is found, an InvalidOperationException is thrown with a message indicating that the behavior was not found. This method provides a convenient way to execute actions on behaviors when you expect them to be present in the registry, while also providing clear error handling when they are not found.
    /// </summary>
    /// <param name="action">The action to execute on the behavior.</param>
    /// <param name="propertyName">The name of the property associated with the behavior, if any.</param>
    /// <param name="scope">The scope associated with the behavior, if any.</param>
    /// <typeparam name="T">The type of the behavior to execute the action on.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the behavior is not found.</exception>
    public void ExecuteBehavior<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior => _behaviors.Execute(action, propertyName, scope);

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
    public bool TryEvaluateBehavior<TBehavior, TResult>(Func<TBehavior, TResult> selector, [MaybeNullWhen(false)] out TResult result, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior => _behaviors.TryEvaluate(selector, out result, propertyName, scope);

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
    public TResult EvaluateBehavior<TBehavior, TResult>(Func<TBehavior, TResult> selector, TResult defaultValue = default!, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior
        => _behaviors.Evaluate(selector, defaultValue, propertyName, scope);

    #endregion;

    #region IDisposable Support

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    protected virtual void DisposeManagedResources()
    {
        _behaviors.Dispose();

        Disposables.Dispose();
    }

    /// <summary>
    /// Disposes the object and releases all resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose (true) or from a finalizer (false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;

        if (disposing)
            DisposeManagedResources();

        IsDisposed = true;
    }

    /// <summary>
    /// Disposes the object and releases all resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support

    #region Helpers

    /// <summary>
    /// Creates a getter function for the specified property of the given type using expression trees. This method generates a compiled lambda expression that retrieves the value of the specified property from an instance of the given type. The generated getter function takes an object as input, casts it to the specified type, accesses the property, and returns its value as an object. By using expression trees and caching the generated getters, this method provides an efficient way to access property values by name without incurring the overhead of reflection for subsequent accesses to the same property.
    /// </summary>
    /// <param name="type">The type of the object containing the property.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A function that gets the value of the specified property from an object.</returns>
    private static Func<object, object?> CreateGetter(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        if (property?.CanRead != true || property.GetIndexParameters().Length != 0)
            return static _ => UnknownValue.Instance;

        var parameter = Expression.Parameter(typeof(object));

        var cast = Expression.Convert(parameter, type);

        var propertyAccess = Expression.Property(cast, property);

        var convert = Expression.Convert(propertyAccess, typeof(object));

        return Expression.Lambda<Func<object, object?>>(convert, parameter).Compile();
    }

    /// <summary>
    /// Gets a cached PropertyChangedEventArgs instance for the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A cached PropertyChangedEventArgs instance.</returns>
    private static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName) => PropertyChangedEventArgsCache.GetOrAdd(propertyName, static x => new(x));

    /// <summary>
    /// Gets a cached PropertyChangingEventArgs instance for the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A cached PropertyChangingEventArgs instance.</returns>
    private static PropertyChangingEventArgs GetPropertyChangingEventArgs(string propertyName) => PropertyChangingEventArgsCache.GetOrAdd(propertyName, static x => new(x));

    /// <summary>
    /// Gets the value of a property by name using a cached getter function. This method retrieves the value of a property from the current instance based on the provided property name. It uses a cache to store compiled getter functions for each property, improving performance by avoiding reflection for subsequent accesses to the same property. If the getter function for the specified property name does not exist in the cache, it is created using expression trees and added to the cache before being invoked to retrieve the property value.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The value of the property.</returns>
    private object? GetPropertyValue(string propertyName)
    {
        var key = (GetType(), propertyName);

        var getter = PropertyGetterCache.GetOrAdd(key, static x => CreateGetter(x.Type, x.PropertyName));

        return getter(this);
    }

    #endregion
}
