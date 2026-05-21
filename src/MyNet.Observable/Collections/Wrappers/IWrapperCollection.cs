// -----------------------------------------------------------------------
// <copyright file="IWrapperCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;

namespace MyNet.Observable.Collections.Wrappers;

/// <summary>
/// Defines a collection of wrapper objects that are created from a source collection of items. The wrapper collection provides a way to manage and observe the wrappers while maintaining a connection to the underlying items. It allows for efficient retrieval of wrappers based on their corresponding items and supports change tracking through observables.
/// </summary>
/// <typeparam name="T">The type of the items in the source collection.</typeparam>
/// <typeparam name="TWrapper">The type of the wrapper objects.</typeparam>
public interface IWrapperCollection<T, TWrapper> : ICollection<T>
where T : notnull
where TWrapper : notnull
{
    /// <summary>
    /// Gets a read-only collection of the wrapper objects in this wrapper collection. This collection reflects the current state of the wrappers and can be observed for changes. Each wrapper corresponds to an item in the source collection, and the collection provides efficient access to the wrappers based on their associated items.
    /// </summary>
    ReadOnlyObservableCollection<TWrapper> Wrappers { get; }

    /// <summary>
    /// Gets the wrapper object associated with the specified item. If a wrapper for the item does not exist, it creates a new wrapper using the provided factory method and adds it to the collection. This method ensures that each item has a corresponding wrapper and allows for efficient retrieval of wrappers based on their associated items.
    /// </summary>
    /// <param name="item">The item for which to get or create a wrapper.</param>
    /// <returns>The wrapper object associated with the specified item.</returns>
    TWrapper GetOrCreate(T item);

    /// <summary>
    /// Connects to the wrapper collection and returns an observable sequence of change sets for the wrappers. This allows for monitoring changes to the wrappers in the collection, such as additions, removals, and updates.
    /// </summary>
    /// <returns>An observable sequence of change sets for the wrappers.</returns>
    IObservable<IChangeSet<TWrapper>> ConnectWrappers();
}
