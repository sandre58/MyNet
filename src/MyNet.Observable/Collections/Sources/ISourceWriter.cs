// -----------------------------------------------------------------------
// <copyright file="ISourceWriter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DynamicData;

namespace MyNet.Observable.Collections.Sources;

/// <summary>
/// Defines an interface for a source writer that allows adding, removing, clearing, setting, and editing items in a data source. This interface provides methods for manipulating the collection of items, including adding single items or ranges of items, removing single items or ranges of items, clearing the entire collection, setting the collection to a new set of items, and editing the collection using an action that operates on an extended list of items.
/// </summary>
/// <typeparam name="T">The type of items managed by the source writer.</typeparam>
public interface ISourceWriter<T>
    where T : notnull
{
    /// <summary>
    /// Adds an item to the data source. This method allows you to add a single item to the collection managed by the source writer. The item will be added to the underlying data structure, and any necessary notifications or updates will be triggered to reflect the change in the collection.
    /// </summary>
    /// <param name="item">The item to add to the collection.</param>
    void Add(T item);

    /// <summary>
    /// Adds a range of items to the data source. This method allows you to add multiple items to the collection managed by the source writer. The items will be added to the underlying data structure, and any necessary notifications or updates will be triggered to reflect the changes in the collection.
    /// </summary>
    /// <param name="items">The items to add to the collection.</param>
    void AddRange(IEnumerable<T> items);

    /// <summary>
    /// Removes an item from the data source. This method allows you to remove a single item from the collection managed by the source writer. The item will be removed from the underlying data structure, and any necessary notifications or updates will be triggered to reflect the change in the collection.
    /// </summary>
    /// <param name="item">The item to remove from the collection.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    bool Remove(T item);

    /// <summary>
    /// Removes a range of items from the data source. This method allows you to remove multiple items from the collection managed by the source writer. The items will be removed from the underlying data structure, and any necessary notifications or updates will be triggered to reflect the changes in the collection.
    /// </summary>
    /// <param name="items">The items to remove from the collection.</param>
    void RemoveMany(IEnumerable<T> items);

    /// <summary>
    /// Clears all items from the data source. This method removes all items from the collection managed by the source writer, and any necessary notifications or updates will be triggered to reflect the change in the collection.
    /// </summary>
    void Clear();

    /// <summary>
    /// Sets the data source to a new set of items. This method replaces the current collection with the specified items, and any necessary notifications or updates will be triggered to reflect the change in the collection.
    /// </summary>
    /// <param name="items">The new set of items to set in the collection.</param>
    void Set(IEnumerable<T> items);

    /// <summary>
    /// Edits the collection using the specified update action. This method allows you to perform complex updates on the collection by providing an action that operates on an extended list of items. Any necessary notifications or updates will be triggered to reflect the changes in the collection.
    /// </summary>
    /// <param name="update">The action to perform on the extended list of items.</param>
    void Edit(Action<IExtendedList<T>> update);
}
