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

namespace MyNet.Utilities.Collections;

/// <summary>
/// An observable collection that supports sorting using a selector.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class SortableObservableCollection<T> : ThreadSafeObservableCollection<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class.
    /// </summary>
    public SortableObservableCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class that contains elements copied from the specified list.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new collection.</param>
    public SortableObservableCollection(Collection<T> list)
        : base(list) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new collection.</param>
    public SortableObservableCollection(IEnumerable<T> collection)
        : base(collection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableObservableCollection{T}"/> class with a sort selector and direction.
    /// </summary>
    /// <param name="sortSelector">A function used to select the value to sort by.</param>
    /// <param name="direction">The sort direction.</param>
    public SortableObservableCollection(Func<T, object> sortSelector, ListSortDirection direction = ListSortDirection.Ascending) => (SortSelector, SortDirection) = (sortSelector, direction);

    /// <summary>
    /// Gets or sets the selector used to determine the sorting key for items.
    /// </summary>
    public Func<T, object>? SortSelector { get; set; }

    /// <summary>
    /// Gets or sets the direction of the sort.
    /// </summary>
    public ListSortDirection SortDirection { get; set; }

    /// <summary>
    /// Sorts the collection using the configured selector and direction.
    /// </summary>
    public void Sort()
    {
        if (SortSelector is null || Count < 2) return;

        ExecuteThreadSafe(() =>
        {
            var query = this.Select((x, index) => (Item: x, Index: index));

            query = SortDirection == ListSortDirection.Ascending ? query.OrderBy(x => SortSelector.Invoke(x.Item)) : query.OrderByDescending(x => SortSelector.Invoke(x.Item));

            var map = query.Select((x, index) => (OldIndex: x.Index, NewIndex: index)).Where(o => o.OldIndex != o.NewIndex);

            using var enumerator = map.GetEnumerator();
            if (enumerator.MoveNext())
                Move(enumerator.Current.OldIndex, enumerator.Current.NewIndex);
        });
    }

    protected override void InvokeNotifyCollectionChanged(NotifyCollectionChangedEventHandler notifyEventHandler, NotifyCollectionChangedEventArgs e)
    {
        base.InvokeNotifyCollectionChanged(notifyEventHandler, e);

        if (SortSelector is null || e.Action == NotifyCollectionChangedAction.Remove)
            return;

        Sort();
    }
}
