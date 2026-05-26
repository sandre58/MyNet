// -----------------------------------------------------------------------
// <copyright file="IObservableRangeCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyNet.Collections;

/// <summary>
/// Defines an observable collection that supports batch operations for adding and removing items, while providing notifications for collection changes and property changes.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public interface IObservableRangeCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    /// <summary>
    /// Adds a range of items to the collection in a single operation, providing a single notification for the entire batch addition.
    /// </summary>
    /// <param name="items">The items to add to the collection.</param>
    void AddRange(IEnumerable<T> items);

    /// <summary>
    /// Loads a range of items into the collection, replacing the existing items, and providing a single notification for the entire batch load.
    /// </summary>
    /// <param name="items">The items to load into the collection.</param>
    void Load(IEnumerable<T> items);

    /// <summary>
    /// Removes a range of items from the collection, providing a single notification for the entire batch removal.
    /// </summary>
    /// <param name="index">The zero-based index at which to start removing items.</param>
    /// <param name="count">The number of items to remove.</param>
    void RemoveRange(int index, int count);

    /// <summary>
    /// Moves an item from one index to another within the collection, providing a single notification for the move operation.
    /// </summary>
    /// <param name="oldIndex">The zero-based index of the item to move.</param>
    /// <param name="newIndex">The zero-based index to move the item to.</param>
    void Move(int oldIndex, int newIndex);

    /// <summary>
    /// Removes all items that match the specified predicate, providing a single notification for the entire batch removal.
    /// </summary>
    /// <param name="predicate">The predicate to determine which items to remove.</param>
    /// <returns>The number of items removed.</returns>
    int RemoveAll(Func<T, bool> predicate);

    /// <summary>
    /// Inserts a range of items into the collection at the specified index, providing a single notification for the entire batch insertion.
    /// </summary>
    /// <param name="items">The collection of items to insert.</param>
    /// <param name="index">The zero-based index at which to insert the items.</param>
    void InsertRange(IEnumerable<T> items, int index);

    /// <summary>
    /// Sets the capacity of the collection.
    /// </summary>
    /// <param name="capacity">The new capacity of the collection.</param>
    void SetCapacity(int capacity);

    /// <summary>
    /// Gets the current capacity of the collection.
    /// </summary>
    int Capacity { get; }

    /// <summary>
    /// Suspends count notifications, allowing batch updates without triggering count change events until the returned disposable is disposed.
    /// </summary>
    /// <returns>A disposable that, when disposed, resumes count notifications.</returns>
    IDisposable SuspendCount();

    /// <summary>
    /// Suspends collection and property change notifications, allowing batch updates without triggering events until the returned disposable is disposed.
    /// </summary>
    /// <returns>A disposable that, when disposed, resumes collection and property change notifications.</returns>
    IDisposable SuspendNotifications();
}
