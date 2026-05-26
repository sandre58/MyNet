// -----------------------------------------------------------------------
// <copyright file="SynchronizedObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNet.Collections;

/// <summary>
/// External synchronization adapter around an observable range collection.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}"/> class with the specified inner collection and synchronizer.
/// </remarks>
/// <param name="inner">The inner observable range collection.</param>
/// <param name="synchronizer">The collection synchronizer.</param>
public sealed class SynchronizedObservableCollection<T>(ObservableRangeCollection<T> inner, ICollectionSynchronizer? synchronizer = null) : ObservableCollectionDecorator<T>(inner)
{
    private readonly ICollectionSynchronizer _synchronizer = synchronizer ?? new LockCollectionSynchronizer();

    /// <summary>
    /// Gets the number of elements contained in the collection, using the synchronizer to ensure thread-safe access.
    /// </summary>
    public override int Count => _synchronizer.Read(() => Inner.Count);

    /// <summary>
    /// Gets a value indicating whether the collection is read-only. This implementation always returns false, as the collection is mutable.
    /// </summary>
    public override bool IsReadOnly => false;

    /// <summary>
    /// Gets or sets the element at the specified index, using the synchronizer to ensure thread-safe access for both reading and writing.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    public override T this[int index]
    {
        get => _synchronizer.Read(() => Inner[index]);
        set => _synchronizer.Write(() => Inner[index] = value);
    }

    /// <summary>
    /// Adds an item to the collection, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="item">The item to add to the collection.</param>
    public override void Add(T item) => _synchronizer.Write(() => Inner.Add(item));

    /// <summary>
    /// Adds a range of items to the collection, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="items">The items to add to the collection.</param>
    public override void AddRange(IEnumerable<T> items) => _synchronizer.Write(() => Inner.AddRange(items));

    /// <summary>
    /// Removes all items from the collection, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    public override void Clear() => _synchronizer.Write(Inner.Clear);

    /// <summary>
    /// Determines whether the collection contains a specific value, using the synchronizer to ensure thread-safe access.
    /// </summary>
    /// <param name="item">The item to locate in the collection.</param>
    /// <returns>true if item is found in the collection; otherwise, false.</returns>
    public override bool Contains(T item) => _synchronizer.Read(() => Inner.Contains(item));

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index, using the synchronizer to ensure thread-safe access.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public override void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        _synchronizer.Read(() => Inner.CopyTo(array, arrayIndex));
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection. This implementation creates a snapshot of the collection to allow for safe enumeration without holding a read lock for the entire duration of the iteration.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public override IEnumerator<T> GetEnumerator()
    {
        // Enumerate over a snapshot to avoid holding a read lock for the whole iteration.
        var snapshot = _synchronizer.Read(() => Inner.ToList());
        return snapshot.GetEnumerator();
    }

    /// <summary>
    /// Determines the index of a specific item in the collection, using the synchronizer to ensure thread-safe access.
    /// </summary>
    /// <param name="item">The item to locate in the collection.</param>
    /// <returns>The index of the item if found; otherwise, -1.</returns>
    public override int IndexOf(T item) => _synchronizer.Read(() => Inner.IndexOf(item));

    /// <summary>
    /// Inserts an item into the collection at the specified index, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="index">The zero-based index at which the item should be inserted.</param>
    /// <param name="item">The item to insert.</param>
    public override void Insert(int index, T item) => _synchronizer.Write(() => Inner.Insert(index, item));

    /// <summary>
    /// Inserts a range of items into the collection at the specified index, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="items">The items to insert.</param>
    /// <param name="index">The zero-based index at which the items should be inserted.</param>
    public override void InsertRange(IEnumerable<T> items, int index) => _synchronizer.Write(() => Inner.InsertRange(items, index));

    /// <summary>
    /// Removes the first occurrence of a specific object from the collection, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>true if item was successfully removed; otherwise, false.</returns>
    public override bool Remove(T item) => _synchronizer.Write(() => Inner.Remove(item));

    /// <summary>
    /// Removes all the elements that match the conditions defined by the specified predicate, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="predicate">The predicate that defines the conditions of the elements to remove.</param>
    /// <returns>The number of elements removed.</returns>
    public override int RemoveAll(Func<T, bool> predicate) => _synchronizer.Write(() => Inner.RemoveAll(predicate));

    /// <summary>
    /// Removes the element at the specified index, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    public override void RemoveAt(int index) => _synchronizer.Write(() => Inner.RemoveAt(index));

    /// <summary>
    /// Removes a range of elements from the collection, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    public override void RemoveRange(int index, int count) => _synchronizer.Write(() => Inner.RemoveRange(index, count));

    /// <summary>
    /// Loads a collection of items into the collection, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="items">The items to load into the collection.</param>
    public override void Load(IEnumerable<T> items) => _synchronizer.Write(() => Inner.Load(items));

    /// <summary>
    /// Sets the capacity of the collection, using the synchronizer to ensure thread-safe access for modifications.
    /// </summary>
    /// <param name="capacity">The new capacity of the collection.</param>
    public override void SetCapacity(int capacity) => _synchronizer.Write(() => Inner.SetCapacity(capacity));
}
