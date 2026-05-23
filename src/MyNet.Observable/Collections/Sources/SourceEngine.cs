// -----------------------------------------------------------------------
// <copyright file="SourceEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DynamicData;
using MyNet.Utilities.Providers;

namespace MyNet.Observable.Collections.Sources;

/// <summary>
/// Represents a source engine that manages a collection of items of type T and provides an observable stream of changes to the collection. The source engine allows adding, removing, and editing items in the collection, and notifies subscribers of any changes through the Connect method.
/// </summary>
/// <typeparam name="T">The type of items managed by the source engine.</typeparam>
public sealed class SourceEngine<T> : ISourceEngine<T>, ISourceWriter<T>, IRefreshableSource
    where T : notnull
{
    private readonly SourceList<T>? _source;
    private readonly Func<IEnumerable<T>>? _snapshotFactory;
    private readonly IObservable<IChangeSet<T>> _external;
    private readonly IDisposable? _externalCountSubscription;
    private int _externalCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceEngine{T}"/> class with the specified source list and collection source mode. This constructor is used when the source engine is operating in owned mutable mode, where the collection is managed internally by the source engine and subscribers can modify the collection directly through the provided methods.
    /// </summary>
    /// <param name="source">The source list managed by the source engine.</param>
    /// <param name="isReadOnly">Indicates whether the source engine is read-only.</param>
    private SourceEngine(SourceList<T> source, bool isReadOnly)
    {
        _source = source;
        _external = source.Connect();
        IsReadOnly = isReadOnly;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceEngine{T}"/> class with the specified snapshot factory. This constructor is used when the source engine is operating in read-only snapshot mode, where the collection cannot be modified directly and subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state. The snapshot factory is a function that provides a snapshot of the collection's state, allowing subscribers to get the current state of the collection when they subscribe to changes.
    /// </summary>
    /// <param name="snapshotFactory">The function that provides a snapshot of the collection's state.</param>
    private SourceEngine(Func<IEnumerable<T>> snapshotFactory)
        : this(new SourceList<T>(), true)
    {
        _snapshotFactory = snapshotFactory;
        ReloadSnapshot();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceEngine{T}"/> class with the specified external observable stream of changes and collection source mode. This constructor is used when the source engine is operating in external live mode, where the collection is managed externally and the source engine only provides an observable stream of changes to subscribers.
    /// </summary>
    /// <param name="external">The external observable stream of changes.</param>
    /// <param name="isReadOnly">Indicates whether the source engine is read-only.</param>
    private SourceEngine(IObservable<IChangeSet<T>> external, bool isReadOnly)
    {
        _external = external;
        IsReadOnly = isReadOnly;
        _externalCountSubscription = external.Subscribe(TrackExternalCount);
    }

    /// <summary>
    /// Gets a value indicating whether the source engine is in read-only snapshot mode. If this property is true, it means that the source engine operates in a mode where the collection cannot be modified directly, and subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state. If this property is false, it means that the source engine allows modifications to the collection, and subscribers can add, remove, or edit items directly through the provided methods.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets the number of items currently in the collection managed by the source engine. This property allows subscribers to quickly determine the size of the collection without needing to subscribe to changes or access the underlying list directly.
    /// </summary>
    public int Count => _source?.Count ?? _externalCount;

    #region Factories

    /// <summary>
    /// Creates a new instance of the <see cref="SourceEngine{T}"/> class in owned mutable mode, where the collection is managed internally by the source engine and subscribers can modify the collection directly through the provided methods. This method allows subscribers to easily create a source engine with an empty collection that they can populate and modify as needed, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <returns>A new instance of the <see cref="SourceEngine{T}"/> class.</returns>
    public static SourceEngine<T> Empty() => new(new SourceList<T>(), false);

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

        return new(list, readOnly);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SourceEngine{T}"/> class from an external observable stream of changes. This method allows subscribers to create a source engine that operates in external live mode, where the collection is managed externally and the source engine only provides an observable stream of changes to subscribers. Subscribers can use this method to connect to an existing collection and receive real-time updates about changes to the collection without needing to manage the collection directly through the source engine's methods.
    /// </summary>
    /// <param name="source">The external observable stream of changes.</param>
    /// <returns>A new instance of the <see cref="SourceEngine{T}"/> class.</returns>
    public static SourceEngine<T> FromObservable(IObservable<IChangeSet<T>> source) => new(source, true);

    /// <summary>
    /// Creates a new instance of the <see cref="SourceEngine{T}"/> class from a snapshot factory function. This method allows subscribers to create a source engine that operates in read-only snapshot mode, where the collection cannot be modified directly and subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state. The snapshot factory is a function that provides a snapshot of the collection's state, allowing subscribers to get the current state of the collection when they subscribe to changes. Subscribers can use this method to create a source engine that reflects the current state of an external collection without needing to manage the collection directly through the source engine's methods, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="factory">The snapshot factory function that provides the current state of the collection.</param>
    /// <returns>A new instance of the <see cref="SourceEngine{T}"/> class.</returns>
    public static SourceEngine<T> FromSnapshot(Func<IEnumerable<T>> factory) => new(factory);

    /// <summary>
    /// Creates a new instance of the <see cref="SourceEngine{T}"/> class from an items provider. This method allows subscribers to create a source engine that operates in read-only snapshot mode, where the collection cannot be modified directly and subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state. The items provider is an implementation of the <see cref="IItemsProvider{T}"/> interface that provides a snapshot of the collection's state through its GetItems method, allowing subscribers to get the current state of the collection when they subscribe to changes. Subscribers can use this method to create a source engine that reflects the current state of an external collection provided by the items provider without needing to manage the collection directly through the source engine's methods, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="provider">The items provider that provides the current state of the collection.</param>
    /// <returns>A new instance of the <see cref="SourceEngine{T}"/> class.</returns>
    public static SourceEngine<T> FromProvider(IItemsProvider<T> provider) =>
        FromSnapshot(() => Materialize(provider.GetItemsAsync()));

    #endregion

    private static List<T> Materialize(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        var enumerator = source.GetAsyncEnumerator();

        try
        {
            while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
                list.Add(enumerator.Current);
        }
        finally
        {
            enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        return list;
    }

    /// <summary>
    /// Reloads the snapshot of the collection by calling the snapshot factory function to get the current state of the collection and updating the internal source list with the new items. This method is used when the source engine is operating in read-only snapshot mode, allowing subscribers to refresh their view of the collection whenever they need to get the latest state of the collection from the snapshot factory. Any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    private void ReloadSnapshot()
    {
        var items = _snapshotFactory?.Invoke() ?? throw new InvalidOperationException("Snapshot factory is not defined.");

        _source?.Edit(list =>
        {
            list.Clear();
            list.AddRange(items);
        });
    }

    /// <summary>
    /// Refreshes the snapshot of the collection by calling the ReloadSnapshot method to update the internal source list with the latest state of the collection from the snapshot factory. This method is used when the source engine is operating in read-only snapshot mode, allowing subscribers to manually trigger a refresh of their view of the collection whenever they need to get the latest state of the collection from the snapshot factory. Any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the source engine is not operating in read-only snapshot mode.</exception>
    public void Refresh()
    {
        if (_snapshotFactory is null)
            throw new InvalidOperationException("Refresh is only supported for snapshot sources.");

        ReloadSnapshot();
    }

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
        _source?.Add(item);
    }

    /// <summary>
    /// Adds a range of items to the collection managed by the source engine. This method allows subscribers to easily add multiple items to the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="items">The items to add to the collection.</param>
    public void AddRange(IEnumerable<T> items)
    {
        EnsureWritable();
        _source?.AddRange(items);
    }

    /// <summary>
    /// Removes an item from the collection managed by the source engine. This method allows subscribers to easily remove items from the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="item">The item to remove from the collection.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(T item)
    {
        EnsureWritable();
        return _source?.Remove(item) ?? false;
    }

    /// <summary>
    /// Removes a range of items from the collection managed by the source engine. This method allows subscribers to easily remove multiple items from the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="items">The items to remove from the collection.</param>
    public void RemoveMany(IEnumerable<T> items)
    {
        EnsureWritable();
        _source?.RemoveMany(items);
    }

    /// <summary>
    /// Clears all items from the collection managed by the source engine. This method allows subscribers to easily remove all items from the collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    public void Clear()
    {
        EnsureWritable();
        _source?.Clear();
    }

    /// <summary>
    /// Sets the collection managed by the source engine to the specified items. This method allows subscribers to easily replace the entire collection, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="items">The items to set in the collection.</param>
    public void Set(IEnumerable<T> items)
    {
        EnsureWritable();
        _source?.Edit(x =>
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
        _source?.Edit(update);
    }

    /// <summary>
    /// Ensures that the source engine is in a writable mode before allowing modifications to the collection. If the source engine is in read-only snapshot mode, this method will throw an InvalidOperationException, indicating that the collection cannot be modified directly and that subscribers will need to subscribe to changes and react accordingly to receive updates about the collection's state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the source engine is in read-only snapshot mode and modifications are attempted.</exception>
    private void EnsureWritable()
    {
        if (IsReadOnly)
            throw new InvalidOperationException("Collection is read only and cannot be modified.");
    }

    /// <summary>
    /// Tracks the count of items in the collection when the source engine is operating in external live mode, where the collection is managed externally and the source engine only provides an observable stream of changes to subscribers. This method subscribes to the external observable stream of changes and updates the internal count of items based on the type of change (addition, removal, etc.) that occurs in the collection. This allows subscribers to get an accurate count of items in the collection even when it is managed externally, and any changes will be automatically propagated to subscribers through the Connect method, ensuring that they stay up-to-date with the latest state of the collection.
    /// </summary>
    /// <param name="changes">The set of changes to the collection.</param>
    private void TrackExternalCount(IChangeSet<T> changes)
    {
        foreach (var change in changes)
        {
            switch (change.Reason)
            {
                case ListChangeReason.Add:
                    _externalCount++;
                    break;
                case ListChangeReason.AddRange:
                    _externalCount += change.Range.Count;
                    break;
                case ListChangeReason.Remove:
                    _externalCount = Math.Max(0, _externalCount - 1);
                    break;
                case ListChangeReason.RemoveRange:
                    _externalCount = Math.Max(0, _externalCount - change.Range.Count);
                    break;
                case ListChangeReason.Clear:
                    _externalCount = 0;
                    break;
                case ListChangeReason.Replace:
                case ListChangeReason.Moved:
                case ListChangeReason.Refresh:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Disposes the source engine and releases all resources. This method ensures that any subscriptions and resources associated with the source engine are properly cleaned up.
    /// </summary>
    public void Dispose()
    {
        _externalCountSubscription?.Dispose();
        _source?.Dispose();
    }
}
