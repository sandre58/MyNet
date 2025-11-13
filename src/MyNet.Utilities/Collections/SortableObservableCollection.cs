// -----------------------------------------------------------------------
// <copyright file="SortableObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyNet.Utilities.Suspending;

namespace MyNet.Utilities.Collections;

/// <summary>
/// An observable collection that supports automatic sorting using a selector.
/// Optimized to minimize allocations and correctly sort all items.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class SortableObservableCollection<T> : ThreadSafeObservableCollection<T>
{
    private readonly Suspender _isSortingSuspender = new();
    private IComparer<T>? _cachedComparer;
    private Func<T, object>? _sortSelector;
    private ListSortDirection _sortDirection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class.
    /// </summary>
    public SortableObservableCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class with initial capacity.
    /// </summary>
    public SortableObservableCollection(int capacity, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : base(capacity, notifyOnUi, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class with elements from a list.
    /// </summary>
    public SortableObservableCollection(Collection<T> list, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : base(list, notifyOnUi, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class with elements from a collection.
    /// </summary>
    public SortableObservableCollection(IEnumerable<T> collection, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : base(collection, notifyOnUi, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class with a sort selector and direction.
    /// </summary>
    /// <param name="sortSelector">A function used to select the value to sort by.</param>
    /// <param name="direction">The sort direction.</param>
    /// <param name="notifyOnUi">Optional action used to marshal notifications on the UI thread.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously to avoid blocking.</param>
    public SortableObservableCollection(Func<T, object> sortSelector, ListSortDirection direction = ListSortDirection.Ascending, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : base(notifyOnUi, useAsyncNotifications)
    {
        SortSelector = sortSelector;
        SortDirection = direction;
        UpdateCachedComparer();
    }

    /// <summary>
    /// Gets or sets the selector used to determine the sorting key for items.
    /// </summary>
    public Func<T, object>? SortSelector
    {
        get => _sortSelector;
        set
        {
            if (ReferenceEquals(_sortSelector, value)) return;
            _sortSelector = value;
            UpdateCachedComparer();
        }
    }

    /// <summary>
    /// Gets or sets the direction of the sort.
    /// </summary>
    public ListSortDirection SortDirection
    {
        get => _sortDirection;
        set
        {
            if (_sortDirection == value) return;
            _sortDirection = value;
            UpdateCachedComparer();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically sort on insertions.
    /// Default is true. Set to false if you want to control sorting manually.
    /// </summary>
    public bool AutoSort { get; set; } = true;

    /// <summary>
    /// Updates the cached comparer when selector or direction changes.
    /// </summary>
    private void UpdateCachedComparer()
    {
        if (SortSelector is null)
        {
            _cachedComparer = null;
            return;
        }

        _cachedComparer = SortDirection == ListSortDirection.Ascending
            ? Comparer<T>.Create((x, y) => Comparer<object>.Default.Compare(SortSelector(x), SortSelector(y)))
            : Comparer<T>.Create((x, y) => Comparer<object>.Default.Compare(SortSelector(y), SortSelector(x)));
    }

    /// <summary>
    /// Sorts the collection using the configured selector and direction.
    /// Optimized to use List.Sort() for O(n log n) performance.
    /// </summary>
    public void Sort()
    {
        if (SortSelector is null || Count < 2 || _isSortingSuspender.IsSuspended) return;

        ExecuteWriteLocked(() =>
        {
            if (_isSortingSuspender.IsSuspended) return; // Double-check inside lock

            using (_isSortingSuspender.Suspend())
            {
                // Suspend notifications during sort
                using (SuspendNotifications())
                {
                    // Convert to list and sort in-place
                    if (Items is List<T> list)
                    {
                        // Direct sort on underlying list - O(n log n)
                        list.Sort(_cachedComparer ?? Comparer<T>.Create((x, y) => Comparer<object>.Default.Compare(SortSelector(x), SortSelector(y))));
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
                    else
                    {
                        // Fallback: Create sorted list and reload
                        var sorted = SortDirection == ListSortDirection.Ascending
                            ? this.OrderBy(SortSelector).ToList()
                            : [.. this.OrderByDescending(SortSelector)];

                        ClearItems();
                        foreach (var item in sorted)
                            Add(item);
                    }
                }
            }
        });
    }

    /// <summary>
    /// Inserts an item at the correct sorted position.
    /// Much more efficient than inserting then sorting entire collection.
    /// </summary>
    protected override void InsertItem(int index, T item)
    {
        if (!AutoSort || SortSelector is null || _isSortingSuspender.IsSuspended)
        {
            base.InsertItem(index, item);
            return;
        }

        ExecuteWriteLocked(() =>
        {
            // Binary search for correct position
            var sortedIndex = FindSortedInsertIndex(item);
            base.InsertItem(sortedIndex, item);
        });
    }

    /// <summary>
    /// Finds the correct index to insert an item to maintain sort order.
    /// Uses binary search for O(log n) performance.
    /// </summary>
    private int FindSortedInsertIndex(T item)
    {
        if (Count == 0 || SortSelector is null)
            return 0;

        var comparer = _cachedComparer ?? Comparer<T>.Default;

        // Binary search
        var left = 0;
        var right = Count;

        while (left < right)
        {
            var mid = (left + right) / 2;
            var comparison = comparer.Compare(Items[mid], item);

            if (comparison < 0)
                left = mid + 1;
            else
                right = mid;
        }

        return left;
    }

    /// <summary>
    /// Override to prevent automatic sort during notifications.
    /// </summary>
    protected override void InvokeNotifyCollectionChanged(
        NotifyCollectionChangedEventHandler notifyEventHandler,
        NotifyCollectionChangedEventArgs e) => base.InvokeNotifyCollectionChanged(notifyEventHandler, e);

    /// <summary>
    /// Adds items in batch and sorts once at the end.
    /// Much more efficient than individual insertions.
    /// </summary>
    public new void AddRange(IEnumerable<T> collection)
        => ExecuteWriteLocked(() =>
        {
            using (SuspendNotifications())
            {
                var oldAutoSort = AutoSort;
                try
                {
                    // Disable auto-sort during batch add
                    AutoSort = false;
                    base.AddRange(collection);
                }
                finally
                {
                    AutoSort = oldAutoSort;
                }

                // Sort once at the end if needed
                if (AutoSort && SortSelector is not null)
                    Sort();
            }
        });

    /// <summary>
    /// Loads items and sorts once at the end.
    /// </summary>
    public new void Load(IEnumerable<T> items)
        => ExecuteWriteLocked(() =>
        {
            base.Load(items);

            // Sort after load if needed
            if (AutoSort && SortSelector is not null)
                Sort();
        });
}
