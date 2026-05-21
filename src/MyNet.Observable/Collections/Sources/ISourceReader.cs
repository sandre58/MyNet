// -----------------------------------------------------------------------
// <copyright file="ISourceReader.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using DynamicData;

namespace MyNet.Observable.Collections.Sources;

/// <summary>
/// Defines an interface for a source reader that provides read-only access to a collection of items, allowing subscribers to observe changes in the collection through an observable stream of change sets. The source reader also provides properties to determine the mode of operation (read-only snapshot or writable) and the current count of items in the collection, enabling subscribers to react accordingly based on the capabilities of the source engine.
/// </summary>
/// <typeparam name="T">The type of items managed by the source reader.</typeparam>
public interface ISourceReader<T>
    where T : notnull
{
    /// <summary>
    /// Gets a value indicating whether the source engine is in read-only snapshot mode. If this property is true, it means that the source engine operates in a mode where the collection cannot be modified directly, and subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state. If this property is false, it means that the source engine allows modifications to the collection, and subscribers can add, remove, or edit items directly through the provided methods.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Gets the number of items currently in the collection managed by the source engine. This property allows subscribers to quickly determine the size of the collection without needing to subscribe to changes or access the underlying list directly.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns an observable stream of change sets that represent the changes occurring in the collection managed by the source engine. Subscribers can subscribe to this observable to receive real-time updates whenever items are added, removed, or modified in the collection, allowing them to react to changes and keep their own state synchronized with the source engine's collection.
    /// </summary>
    /// <returns>An observable stream of change sets representing the changes in the collection.</returns>
    IObservable<IChangeSet<T>> Connect();
}
