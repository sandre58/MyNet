// -----------------------------------------------------------------------
// <copyright file="CollectionTrackingBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using MyNet.Reflection;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Tracks changes on collections of an ObservableObject. This behavior allows you to track changes on collections that implement INotifyCollectionChanged, such as <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>. It automatically tracks all observable collections on the owner object, and raises a CollectionChanged event whenever any of the tracked collections change. You can also manually track or untrack specific collections using the Track and Untrack methods. This behavior is useful for scenarios where you want to react to changes in collections, such as updating the UI or performing additional logic when items are added, removed, or modified in a collection. By implementing this behavior, you can easily track changes on collections without having to manually subscribe to each collection's events, making it easier to manage and maintain your code.
/// </summary>
public sealed class CollectionTrackingBehavior : SuspendableBehavior<ObservableObject>
{
    private readonly HashSet<string> _trackedProperties = new(StringComparer.Ordinal);
    private readonly HashSet<INotifyCollectionChanged> _trackedCollections = [];
    private readonly Lock _gate = new();
    private int _initialTrackScheduled;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionTrackingBehavior"/> class, which tracks changes on collections of the specified ObservableObject. This constructor takes an ObservableObject as a parameter and initializes the behavior to track changes on collections of that object. It also sets up tracking for any collections that implement INotifyCollectionChanged, allowing it to track changes to those collections as well. If the owner parameter is null, an ArgumentNullException will be thrown. The optional changed parameter allows you to specify a callback that will be invoked whenever a tracked collection changes, providing you with the ability to react to collection changes in a custom way.
    /// </summary>
    /// <param name="owner">The ObservableObject whose collections will be tracked.</param>
    /// <exception cref="ArgumentNullException">Thrown if the owner parameter is null.</exception>
    public CollectionTrackingBehavior(ObservableObject owner)
        : base(owner)
    {
        TrackExistingCollections();
        ScheduleDeferredInitialTrack();
    }

    /// <summary>
    /// Occurs when a tracked collection changes.
    /// </summary>
    public event EventHandler<NotifyCollectionChangedEventArgs>? CollectionChanged;

    #region Tracking

    /// <summary>
    /// Registers a collection manually.
    /// </summary>
    public void Track(INotifyCollectionChanged collection)
    {
        if (IsDisposed)
            return;

        if (IsSuspended)
            return;

        lock (_gate)
        {
            if (!_trackedCollections.Add(collection))
                return;

            collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    /// Unregisters a collection manually.
    /// </summary>
    public void Untrack(INotifyCollectionChanged collection)
    {
        lock (_gate)
        {
            if (!_trackedCollections.Remove(collection))
                return;

            collection.CollectionChanged -= OnCollectionChanged;
        }
    }

    /// <summary>
    /// Tracks observable collections exposed by readable owner properties that are already initialized.
    /// </summary>
    internal void TrackExistingCollections()
    {
        if (IsDisposed)
            return;

        foreach (var property in Owner.GetType().GetPublicProperties())
            TryTrackProperty(property);
    }

    private void TryTrackProperty(PropertyInfo property)
    {
        if (!typeof(INotifyCollectionChanged).IsAssignableFrom(property.PropertyType))
            return;

        if (_trackedProperties.Contains(property.Name))
            return;

        if (!PropertyValueAccess.TryGetValue(Owner, property, out var value))
            return;

        if (value is not INotifyCollectionChanged collection)
            return;

        _trackedProperties.Add(property.Name);
        Track(collection);
    }

    private void ScheduleDeferredInitialTrack()
    {
        if (Interlocked.Exchange(ref _initialTrackScheduled, 1) != 0)
            return;

        var context = SynchronizationContext.Current;
        if (context is not null)
        {
            context.Post(static state => ((CollectionTrackingBehavior)state!).DeferredInitialTrack(), this);
            return;
        }

        ThreadPool.QueueUserWorkItem(static state => ((CollectionTrackingBehavior)state!).DeferredInitialTrack(), this);
    }

    private void DeferredInitialTrack()
    {
        if (IsDisposed)
            return;

        TrackExistingCollections();
    }

    #endregion

    #region Event handling

    /// <summary>
    /// Handles the CollectionChanged event for tracked collections. This method is called when a collection that implements INotifyCollectionChanged changes, and it raises the CollectionChanged event of the behavior, as well as invoking the optional _changed callback if it is provided. This allows you to react to collection changes in a custom way by subscribing to the CollectionChanged event or by providing a callback when creating the behavior. If the behavior is disposed, this method will return without doing anything.
    /// </summary>
    /// <param name="sender">The collection that changed.</param>
    /// <param name="e">The event arguments describing the change.</param>
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (IsDisposed || IsSuspended)
            return;

        TrackExistingCollections();
        CollectionChanged?.Invoke(sender, e);
    }

    #endregion

    #region IDisposable

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        lock (_gate)
        {
            foreach (var collection in _trackedCollections)
                collection.CollectionChanged -= OnCollectionChanged;

            _trackedCollections.Clear();
            _trackedProperties.Clear();
        }

        base.DisposeManagedResources();
    }

    #endregion
}
