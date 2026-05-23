// -----------------------------------------------------------------------
// <copyright file="ObservableObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
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

    private readonly PropertyNotificationSuspension _notificationSuspension = new();

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in DisposeManagedResources")]
    private readonly BehaviorRegistry _behaviors = new();

    private readonly Suspender _deliveringDeferredNotificationsSuspender = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableObject"/> class and applies metadata-driven behaviors.
    /// </summary>
    protected ObservableObject()
    {
        _notificationSuspension.PendingFlush = DeliverDeferredPropertyChanged;
        Behaviors = new(_behaviors);
        MetadataBehaviorApplicator.Apply(this);
    }

    /// <summary>
    /// Gets the behavior registry facade for this instance.
    /// </summary>
    public ObservableBehaviors Behaviors { get; }

    /// <summary>
    /// Gets the collection of disposables that will be disposed when the object is disposed.
    /// </summary>
    protected CompositeDisposable Disposables { get; } = [];

    /// <summary>
    /// Gets a value indicating whether property notifications are currently suspended.
    /// </summary>
    protected bool AreNotificationsSuspended => _notificationSuspension.IsSuspended;

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
    /// <returns><c>false</c> when the mutation was vetoed via <see cref="PropertyMutationContext.Cancel"/>; otherwise <c>true</c>.</returns>
    protected virtual bool ProcessPropertyChanging(string propertyName, object? before, object? after)
    {
        if (ShouldSkipNotification(propertyName))
            return true;

        if (!_deliveringDeferredNotificationsSuspender.IsSuspended && _notificationSuspension.IsSuspended)
            return true;

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

            if (context.Cancel)
                return false;
        }

        OnPropertyChangingCore(context);

        if (context.Cancel)
            return false;

        RaisePropertyChanging(propertyName);

        return true;
    }

    /// <summary>
    /// Raises the PropertyChanging event for the specified property name, with the provided before and after values. This method calls the ProcessPropertyChanging method to raise the event with the specified before and after values, allowing subscribers to react to the impending change with knowledge of both the old and new values. By calling this method, you can ensure that subscribers are properly notified before a property changes, enabling them to execute any necessary logic in response to the change, such as validation, modification tracking, or collection tracking, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="propertyName">The name of the property that is changing.</param>
    /// <param name="before">The value of the property before the change.</param>
    /// <param name="after">The value of the property after the change.</param>
    /// <returns><c>false</c> when the mutation was vetoed; otherwise <c>true</c>.</returns>
    protected virtual bool OnPropertyChanging(string propertyName, object? before, object? after) => ProcessPropertyChanging(propertyName, before, after);

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

        if (!_deliveringDeferredNotificationsSuspender.IsSuspended && _notificationSuspension.TryHandlePropertyChanged(propertyName, before, after))
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
    /// Notifies subscribers that a property has changed when old and new values are not available (for example relayed or synthetic property names).
    /// Prefer <see cref="NotifyPropertyChanged(string, object?, object?)"/> from property setters when mutation values are known.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    protected internal void NotifyPropertyChanged(string propertyName) => ProcessPropertyChanged(propertyName, UnknownValue.Instance, UnknownValue.Instance);

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
    /// By default, changes are coalesced per property and replayed when the outermost scope ends.
    /// </summary>
    /// <param name="mode">How notifications are handled while suspended.</param>
    /// <returns>A disposable suspension scope.</returns>
    protected IDisposable SuspendNotifications(NotificationSuspensionMode mode = NotificationSuspensionMode.CoalesceOnResume) => IsDisposed ? Disposable.Empty : _notificationSuspension.Enter(mode);

    private void DeliverDeferredPropertyChanged(string propertyName, object? before, object? after)
    {
        if (IsDisposed)
            return;

        using (_deliveringDeferredNotificationsSuspender.Suspend())
            ProcessPropertyChanged(propertyName, before, after);
    }

    /// <summary>
    /// Determines whether property change notifications should be skipped based on the current state of the object and the provided property name. This method checks if the object has been disposed or if the property name is null, empty, or consists only of whitespace. If either of these conditions is true, it returns true, indicating that notifications should be skipped; otherwise, it returns false, allowing notifications to proceed.
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <returns>True if notifications should be skipped; otherwise, false.</returns>
    private bool ShouldSkipNotification(string? propertyName) => IsDisposed || string.IsNullOrWhiteSpace(propertyName);

    #endregion

    #region IDisposable Support

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    protected virtual void DisposeManagedResources()
    {
        _notificationSuspension.Dispose();
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

    #region SetProperty

    /// <summary>
    /// Assigns the field and raises changing/changed notifications through the behavior pipeline when the value differs.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">Reference to the backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The name of the property (inferred from the caller by default).</param>
    /// <returns><c>true</c> if the value changed and notifications were raised; otherwise, <c>false</c> (unchanged value or vetoed via <see cref="PropertyMutationContext.Cancel"/>).</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var before = field;

        if (!OnPropertyChanging(propertyName, before, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName, before, value);
        return true;
    }

    #endregion

    #region Helpers

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

    #endregion
}

/// <summary>
/// Defines how <see cref="ObservableObject"/> handles property notifications while a suspension scope is active.
/// </summary>
public enum NotificationSuspensionMode
{
    /// <summary>
    /// Notifications are suppressed and not replayed when the scope ends.
    /// </summary>
    Drop,

    /// <summary>
    /// Notifications are coalesced per property name and replayed once when the outermost scope ends.
    /// </summary>
    CoalesceOnResume
}

/// <summary>
/// Tracks nested notification suspension scopes and coalesced property mutations.
/// </summary>
internal sealed class PropertyNotificationSuspension
{
    private readonly List<NotificationSuspensionMode> _modes = [];
    private readonly List<string> _pendingOrder = [];
    private readonly Dictionary<string, PendingPropertyMutation> _pending = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets a value indicating whether at least one suspension scope is active.
    /// </summary>
    public bool IsSuspended => _modes.Count > 0;

    /// <summary>
    /// Gets the mode of the innermost active scope.
    /// </summary>
    public NotificationSuspensionMode CurrentMode => _modes[^1];

    /// <summary>
    /// Begins a suspension scope with the specified mode.
    /// </summary>
    public IDisposable Enter(NotificationSuspensionMode mode)
    {
        _modes.Add(mode);
        return new Scope(this);
    }

    /// <summary>
    /// Clears pending mutations without replaying them.
    /// </summary>
    public void Dispose()
    {
        _modes.Clear();
        ClearPending();
    }

    /// <summary>
    /// Handles a property-changed notification while suspended.
    /// </summary>
    /// <returns><c>true</c> if the notification was handled and must not be delivered now.</returns>
    public bool TryHandlePropertyChanged(string propertyName, object? before, object? after)
    {
        if (!IsSuspended)
            return false;

        if (CurrentMode == NotificationSuspensionMode.Drop)
            return true;

        if (_pending.TryGetValue(propertyName, out var pending))
        {
            pending.NewValue = after;
            return true;
        }

        _pendingOrder.Add(propertyName);
        _pending[propertyName] = new(before, after);
        return true;
    }

    private void Exit()
    {
        if (_modes.Count == 0)
            return;

        _modes.RemoveAt(_modes.Count - 1);

        if (IsSuspended)
            return;

        FlushPending();
    }

    private void FlushPending()
    {
        if (_pendingOrder.Count == 0)
            return;

        var snapshot = new List<(string Name, object? Before, object? After)>(_pendingOrder.Count);
        snapshot.AddRange(from propertyName in _pendingOrder
            let pending = _pending[propertyName]
            where !Equals(pending.OldValue, pending.NewValue)
            select (propertyName, pending.OldValue, pending.NewValue));

        ClearPending();

        foreach (var (name, before, after) in snapshot)
            PendingFlush?.Invoke(name, before, after);
    }

    private void ClearPending()
    {
        _pendingOrder.Clear();
        _pending.Clear();
    }

    /// <summary>
    /// Gets or sets the callback invoked for each coalesced property when the outermost scope ends.
    /// Must deliver without re-entering suspension.
    /// </summary>
    public Action<string, object?, object?>? PendingFlush { get; set; }

    private sealed class PendingPropertyMutation(object? oldValue, object? newValue)
    {
        /// <summary>
        /// Gets the first observed value before coalescing.
        /// </summary>
        public object? OldValue { get; } = oldValue;

        /// <summary>
        /// Gets or sets the last observed value after coalescing.
        /// </summary>
        public object? NewValue { get; set; } = newValue;
    }

    private sealed class Scope(PropertyNotificationSuspension owner) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            owner.Exit();
        }
    }
}
