// -----------------------------------------------------------------------
// <copyright file="SourceEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DynamicData;

namespace MyNet.Observable.Collections.Sources;

/// <summary>
/// Represents a source engine that manages a collection of items of type T and provides an observable stream of changes to the collection. The source engine allows adding, removing, and editing items in the collection, and notifies subscribers of any changes through the Connect method.
/// </summary>
/// <typeparam name="T">The type of items managed by the source engine.</typeparam>
public sealed class SourceEngine<T> : IDisposable
    where T : notnull
{
    private readonly SourceList<T>? _source;
    private readonly IObservable<IChangeSet<T>> _external;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceEngine{T}"/> class with the specified source list and collection source mode. This constructor is used when the source engine is operating in owned mutable mode, where the collection is managed internally by the source engine and subscribers can modify the collection directly through the provided methods.
    /// </summary>
    /// <param name="source">The source list managed by the source engine.</param>
    /// <param name="mode">The collection source mode.</param>
    private SourceEngine(SourceList<T> source, CollectionSourceMode mode)
        : this(source.Connect(), mode) => _source = source;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceEngine{T}"/> class with the specified external observable stream of changes and collection source mode. This constructor is used when the source engine is operating in external live mode, where the collection is managed externally and the source engine only provides an observable stream of changes to subscribers.
    /// </summary>
    /// <param name="external">The external observable stream of changes.</param>
    /// <param name="mode">The collection source mode.</param>
    private SourceEngine(IObservable<IChangeSet<T>> external, CollectionSourceMode mode)
    {
        _external = external;
        Mode = mode;
    }

    /// <summary>
    /// Gets the collection source mode that indicates whether the source engine operates in read-only snapshot mode or in a writable mode. This property allows subscribers to determine the capabilities of the source engine and whether they can modify the collection directly or if they need to subscribe to changes and react accordingly.
    /// </summary>
    public CollectionSourceMode Mode { get; }

    /// <summary>
    /// Gets a value indicating whether the source engine is in read-only snapshot mode. If this property is true, it means that the source engine operates in a mode where the collection cannot be modified directly, and subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state. If this property is false, it means that the source engine allows modifications to the collection, and subscribers can add, remove, or edit items directly through the provided methods.
    /// </summary>
    public bool IsReadOnly => Mode == CollectionSourceMode.ReadOnlySnapshot;

    /// <summary>
    /// Gets the number of items currently in the collection managed by the source engine. This property allows subscribers to quickly determine the size of the collection without needing to subscribe to changes or access the underlying list directly.
    /// </summary>
    public int Count => _source?.Count ?? 0;

    #region Factories

    /// <summary>
    /// Creates a new instance of the <see cref="SourceEngine{T}"/> class in owned mutable mode, where the collection is managed internally by the source engine and subscribers can modify the collection directly through the provided methods. This method allows subscribers to easily create a source engine with an empty collection that they can populate and modify as needed, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <returns>A new instance of the <see cref="SourceEngine{T}"/> class.</returns>
    public static SourceEngine<T> Empty() => new(new SourceList<T>(), CollectionSourceMode.OwnedMutable);

    /// <summary>
    /// Creates a new instance of the <see cref="SourceEngine{T}"/> class from an enumerable collection of items. This method allows subscribers to easily create a source engine with an initial set of items, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection. The readOnly parameter determines whether the source engine operates in read-only snapshot mode or in a writable mode, allowing subscribers to choose the appropriate mode based on their needs and how they intend to interact with the collection. If readOnly is true, the source engine will operate in read-only snapshot mode, and subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state. If readOnly is false, the source engine will allow modifications to the collection, and subscribers can add, remove, or edit items directly through the provided methods.
    /// </summary>
    /// <param name="items">The initial set of items for the source engine.</param>
    /// <param name="readOnly">Determines whether the source engine operates in read-only snapshot mode.</param>
    /// <returns>A new instance of the <see cref="SourceEngine{T}"/> class.</returns>
    public static SourceEngine<T> From(IEnumerable<T> items, bool readOnly)
    {
        var list = new SourceList<T>();
        list.AddRange(items);

        return readOnly ? new(list.Connect(), CollectionSourceMode.ReadOnlySnapshot) : new(list, CollectionSourceMode.OwnedMutable);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SourceEngine{T}"/> class from an external observable stream of changes. This method allows subscribers to create a source engine that operates in external live mode, where the collection is managed externally and the source engine only provides an observable stream of changes to subscribers. Subscribers can use this method to connect to an existing collection and receive real-time updates about changes to the collection without needing to manage the collection directly through the source engine's methods.
    /// </summary>
    /// <param name="source">The external observable stream of changes.</param>
    /// <returns>A new instance of the <see cref="SourceEngine{T}"/> class.</returns>
    public static SourceEngine<T> FromObservable(IObservable<IChangeSet<T>> source) => new(source, CollectionSourceMode.ExternalLive);

    #endregion

    /// <summary>
    /// Connects to the source engine and returns an observable stream of changes to the collection. Subscribers can use this method to receive notifications whenever items are added, removed, or edited in the collection, allowing them to react to changes in real-time.
    /// </summary>
    /// <returns>An observable stream of changes to the collection.</returns>
    public IObservable<IChangeSet<T>> Connect() => _external;

    /// <summary>
    /// Adds an item to the collection managed by the source engine. This method allows subscribers to easily add new items to the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="item">The item to add to the collection.</param>
    public void Add(T item)
    {
        EnsureWritable();
        _source!.Add(item);
    }

    /// <summary>
    /// Adds a range of items to the collection managed by the source engine. This method allows subscribers to easily add multiple items to the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="items">The items to add to the collection.</param>
    public void AddRange(IEnumerable<T> items)
    {
        EnsureWritable();
        _source!.AddRange(items);
    }

    /// <summary>
    /// Removes an item from the collection managed by the source engine. This method allows subscribers to easily remove items from the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="item">The item to remove from the collection.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(T item)
    {
        EnsureWritable();
        return _source!.Remove(item);
    }

    /// <summary>
    /// Removes a range of items from the collection managed by the source engine. This method allows subscribers to easily remove multiple items from the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="items">The items to remove from the collection.</param>
    public void RemoveMany(IEnumerable<T> items)
    {
        EnsureWritable();
        _source!.RemoveMany(items);
    }

    /// <summary>
    /// Clears all items from the collection managed by the source engine. This method allows subscribers to easily remove all items from the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    public void Clear()
    {
        EnsureWritable();
        _source!.Clear();
    }

    /// <summary>
    /// Sets the collection managed by the source engine to the specified items. This method allows subscribers to easily replace the entire collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="items">The items to set in the collection.</param>
    public void Set(IEnumerable<T> items)
    {
        EnsureWritable();
        _source!.Edit(x =>
        {
            x.Clear();
            x.AddRange(items);
        });
    }

    /// <summary>
    /// Edits the collection managed by the source engine using the specified update action. This method allows subscribers to easily perform custom updates on the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="update">The action to perform on the collection.</param>
    public void Edit(Action<IExtendedList<T>> update)
    {
        EnsureWritable();
        _source!.Edit(update);
    }

    /// <summary>
    /// Ensures that the source engine is in a writable mode before allowing modifications to the collection. If the source engine is in read-only snapshot mode, this method will throw an InvalidOperationException, indicating that the collection cannot be modified directly and that subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the source engine is in read-only snapshot mode and modifications are attempted.</exception>
    private void EnsureWritable()
    {
        if (_source is null)
            throw new InvalidOperationException($"Collection is {Mode} and cannot be modified.");
    }

    /// <summary>
    /// Disposes the source engine and releases all resources. This method ensures that any subscriptions and resources associated with the source engine are properly cleaned up.
    /// </summary>
    public void Dispose() => _source?.Dispose();
}
