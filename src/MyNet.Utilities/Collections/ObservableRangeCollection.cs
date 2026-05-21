// -----------------------------------------------------------------------
// <copyright file="ObservableRangeCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyNet.Utilities.Collections;

/// <summary>
/// An optimized observable collection with batch operations, notification suspension, and improved performance.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
public class ObservableRangeCollection<T> : ObservableCollection<T>, IObservableRangeCollection<T>
{
    private bool _suspendCount;
    private bool _suspendNotifications;

    // Track if we need to send a reset after batch operations
    private bool _deferredResetPending;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class.
    /// </summary>
    public ObservableRangeCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class with initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity to pre-allocate.</param>
    public ObservableRangeCollection(int capacity)
        : base(new(capacity))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that contains elements copied from the specified list.
    /// </summary>
    /// <param name="list">The list from which the elements are copied.</param>
    public ObservableRangeCollection(Collection<T> list)
        : base(list)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection from which the elements are copied.</param>
    public ObservableRangeCollection(IEnumerable<T> collection)
        : base(collection)
    {
    }

    /// <summary>
    /// Gets the current capacity of the underlying list, if supported.
    /// </summary>
    public int Capacity => Items is List<T> list ? list.Capacity : Count;

    /// <summary>
    /// Adds the elements of the specified collection to the end of the collection.
    /// Optimized to send a single notification for the entire operation.
    /// </summary>
    /// <param name="items">The collection whose elements should be added.</param>
    public virtual void AddRange(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        // Fast path for ICollection<T>
        var col = items as ICollection<T> ?? [.. items];

        if (col.Count == 0) return;

        CheckReentrancy();

        // Pre-allocate if possible (internal List<T>)
        if (Items is List<T> list)
        {
            list.Capacity = Math.Max(list.Capacity, list.Count + col.Count);
        }

        foreach (var item in col)
            Items.Add(item);

        // Send batch notification if not suspended
        // NotifyCollectionChangedAction.Add with multiple items is supported in WPF
        // but can cause issues in some bindings, so we use Reset for safety
        OnCountPropertyChanged();
        if (!_suspendNotifications)
        {
            // NotifyCollectionChangedAction.Add with multiple items is supported in WPF
            // but can cause issues in some bindings, so we use Reset for safety
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }
    }

    /// <summary>
    /// Inserts the elements of a collection into the collection at the specified index.
    /// </summary>
    /// <param name="items">The collection whose elements should be inserted.</param>
    /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
    public virtual void InsertRange(IEnumerable<T> items, int index)
    {
        ArgumentNullException.ThrowIfNull(items);

        // Materialize to avoid multiple enumerations
        var collection = items as IList<T> ?? [.. items];
        if (collection.Count == 0) return;

        CheckReentrancy();

        // Pre-allocate
        if (Items is List<T> list)
        {
            list.Capacity = Math.Max(list.Capacity, list.Count + collection.Count);
        }

        foreach (var item in collection)
            Items.Insert(index++, item);

        OnCountPropertyChanged();

        if (!_suspendNotifications)
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }
    }

    /// <summary>
    /// Clears the collection and loads the specified items in one operation.
    /// Most efficient way to replace all items.
    /// </summary>
    /// <param name="items">The items to load.</param>
    public virtual void Load(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        CheckReentrancy();

        var source = items as ICollection<T> ?? [.. items];

        var oldCount = Count;

        Items.Clear();

        // Pre-allocate if we know the size
        if (Items is List<T> list)
        {
            list.Capacity = source.Count;
        }

        foreach (var item in source)
            Items.Add(item);

        // Only notify if count actually changed or if we have listeners
        if (Count != oldCount)
            OnCountPropertyChanged();

        if (!_suspendNotifications)
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }
    }

    /// <summary>
    /// Removes a range of elements from the collection.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    public virtual void RemoveRange(int index, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (index + count > Count)
            throw new ArgumentException("Index and count do not denote a valid range of elements.");

        if (count == 0) return;

        CheckReentrancy();

        // Remove in reverse order to maintain indices
        for (var i = count - 1; i >= 0; i--)
            Items.RemoveAt(index + i);

        OnCountPropertyChanged();

        if (!_suspendNotifications)
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }
    }

    /// <summary>
    /// Removes all items matching the predicate.
    /// </summary>
    /// <param name="predicate">The predicate to test items.</param>
    /// <returns>The number of items removed.</returns>
    public virtual int RemoveAll(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        CheckReentrancy();

        var itemsToRemove = Items.Where(predicate).ToList();
        if (itemsToRemove.Count == 0) return 0;

        foreach (var item in itemsToRemove)
            Items.Remove(item);

        OnCountPropertyChanged();

        if (!_suspendNotifications)
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }

        return itemsToRemove.Count;
    }

    /// <summary>
    /// Suspends count notifications.
    /// </summary>
    /// <returns>A disposable that will resume notifications when disposed.</returns>
    public IDisposable SuspendCount()
    {
        var count = Count;
        _suspendCount = true;

        return new CountSuspendScope(this, count);
    }

    /// <summary>
    /// Suspends all notifications. When disposed, a reset notification is fired.
    /// </summary>
    /// <returns>A disposable that will resume notifications when disposed.</returns>
    public IDisposable SuspendNotifications()
    {
        _suspendCount = true;
        _suspendNotifications = true;
        _deferredResetPending = false;

        return new NotificationSuspendScope(this);
    }

    /// <summary>
    /// Raises the CollectionChanged event.
    /// </summary>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_suspendNotifications)
        {
            _deferredResetPending = true;
            return;
        }

        base.OnCollectionChanged(e);
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (_suspendCount && e.PropertyName == nameof(Count))
        {
            _deferredResetPending = true;
            return;
        }

        base.OnPropertyChanged(e);
    }

    /// <summary>
    /// Raises the Count property changed event.
    /// </summary>
    protected virtual void OnCountPropertyChanged(bool sendCollectionReset = false)
    {
        OnPropertyChanged(new(nameof(Count)));

        if (sendCollectionReset && !_suspendNotifications)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Sets the capacity of the underlying list if supported.
    /// Useful to pre-allocate before adding many items.
    /// </summary>
    /// <param name="capacity">The desired capacity.</param>
    public virtual void SetCapacity(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);

        if (Items is List<T> list)
        {
            list.Capacity = capacity;
        }
    }

    /// <summary>
    /// Internal scope for suspending count notifications.
    /// </summary>
    private sealed class CountSuspendScope(ObservableRangeCollection<T> owner, int savedCount) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            owner._suspendCount = false;

            if (owner.Count != savedCount)
                owner.OnCountPropertyChanged();
        }
    }

    /// <summary>
    /// Internal scope for suspending all notifications.
    /// </summary>
    private sealed class NotificationSuspendScope(ObservableRangeCollection<T> owner) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            owner._suspendCount = false;
            owner._suspendNotifications = false;

            // Send reset notification if there were any changes
            if (owner._deferredResetPending)
            {
                owner.OnCountPropertyChanged();
                owner.OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
                owner._deferredResetPending = false;
            }
        }
    }
}
