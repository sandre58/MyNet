// -----------------------------------------------------------------------
// <copyright file="ExtendedWrapperCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using MyNet.Observable.Collections.Sources;
using MyNet.Observable.Extensions;
using MyNet.Utilities;

namespace MyNet.Observable.Collections.Wrappers;

/// <summary>
/// Represents an extended collection that exposes wrapper instances for each source item, allowing for additional functionalities and optimizations when managing collections of items with associated wrapper projections. This class extends the capabilities of a standard extended collection by providing a convenient way to create and manage wrapper instances for each item in the source collection, enabling more efficient data manipulation and change tracking while maintaining encapsulation and integrity of the underlying data. The ExtendedWrapperCollection class provides methods for creating instances from various sources, including empty collections, enumerable collections, and observable sequences of change sets, allowing for flexible initialization based on different data sources and requirements.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "This class serves as a factory for ExtendedCollection<T> and the name reflects its purpose.")]
public static class ExtendedWrapperCollection
{
    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class with an optional scheduler for observing collection changes. This method provides a convenient way to create an extended collection without needing to specify an initial source list, allowing for dynamic population of the collection after creation. The Create method initializes the collection with an empty source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="createWrapper">A function to create a wrapper instance for each item.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
    /// <typeparam name="TWrapper">The type of wrapper instances managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedWrapperCollection{T, TWrapper}"/> instance.</returns>
    public static ExtendedWrapperCollection<T, TWrapper> Create<T, TWrapper>(Func<T, TWrapper> createWrapper, IScheduler? scheduler = null)
        where T : notnull
        where TWrapper : class, IWrapper<T>
        => new(SourceEngine<T>.Empty(), createWrapper, scheduler);

    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class from an existing enumerable collection of items, with an optional scheduler for observing collection changes. This method allows you to initialize the extended collection with a predefined set of items, providing a convenient way to create a collection that is already populated with data. The From method initializes the collection with the provided enumerable as the source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="items">The enumerable collection of items to initialize the extended collection with.</param>
    /// <param name="createWrapper">A function to create a wrapper instance for each item.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
    /// <typeparam name="TWrapper">The type of wrapper instances managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedWrapperCollection{T, TWrapper}"/> instance.</returns>
    public static ExtendedWrapperCollection<T, TWrapper> From<T, TWrapper>(IEnumerable<T> items, Func<T, TWrapper> createWrapper, IScheduler? scheduler = null)
        where T : notnull
        where TWrapper : class, IWrapper<T>
        => new(SourceEngine<T>.From(items, readOnly: false), createWrapper, scheduler);

    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class from an existing enumerable collection of items as a read-only snapshot, with an optional scheduler for observing collection changes. This method allows you to initialize the extended collection with a predefined set of items while ensuring that the collection is read-only, providing a convenient way to create an immutable collection that is already populated with data. The FromReadOnly method initializes the collection with the provided enumerable as the source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="items">The enumerable collection of items to initialize the extended collection with.</param>
    /// <param name="createWrapper">A function to create a wrapper instance for each item.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
       /// <typeparam name="TWrapper">The type of wrapper instances managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedWrapperCollection{T, TWrapper}"/> instance.</returns>
    public static ExtendedWrapperCollection<T, TWrapper> FromReadOnly<T, TWrapper>(IEnumerable<T> items, Func<T, TWrapper> createWrapper, IScheduler? scheduler = null)
        where TWrapper : class, IWrapper<T>
        where T : notnull
        => new(SourceEngine<T>.From(items, readOnly: true), createWrapper, scheduler);

    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedCollection{T}"/> class from an observable sequence of change sets, with an optional scheduler for observing collection changes. This method allows you to initialize the extended collection with a dynamic data source that emits change sets, providing a convenient way to create a collection that automatically updates based on changes in the underlying data. The FromObservable method initializes the collection with the provided observable as the source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="source">The observable sequence of change sets to initialize the extended collection with.</param>
    /// <param name="createWrapper">A function to create a wrapper instance for each item.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
    /// <typeparam name="TWrapper">The type of wrapper instances managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedWrapperCollection{T, TWrapper}"/> instance.</returns>
    public static ExtendedWrapperCollection<T, TWrapper> FromObservable<T, TWrapper>(IObservable<IChangeSet<T>> source, Func<T, TWrapper> createWrapper, IScheduler? scheduler = null)
        where T : notnull
        where TWrapper : class, IWrapper<T>
        => new(SourceEngine<T>.FromObservable(source), createWrapper, scheduler);
}

/// <summary>
/// Represents an extended collection exposing wrapper instances for each source item.
/// </summary>
/// <typeparam name="T">The source item type.</typeparam>
/// <typeparam name="TWrapper">The wrapper type.</typeparam>
public class ExtendedWrapperCollection<T, TWrapper> : ExtendedCollection<T>, IWrapperCollection<T, TWrapper>
    where T : notnull
    where TWrapper : class, IWrapper<T>
{
    private readonly Func<T, TWrapper> _createWrapper;
    private readonly Dictionary<T, TWrapper> _cache = [];

    private readonly IObservable<IChangeSet<TWrapper>> _wrappersObservable;
    private readonly IObservable<IChangeSet<TWrapper>> _wrappersSourceObservable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class with the specified source engine, wrapper creation function, and optional scheduler for observing collection changes. The constructor sets up the necessary transformations and subscriptions to ensure that the wrapper collections remain synchronized with the source collection and emit updates whenever changes occur. The wrapper creation function is used to create wrapper instances for each item in the source collection, allowing for customization of the wrapper projection as needed. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="sourceEngine">The source engine managing the underlying collection.</param>
    /// <param name="createWrapper">The function used to create wrapper instances for each source item.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <exception cref="ArgumentNullException">Thrown if the createWrapper function is null.</exception>
    public ExtendedWrapperCollection(SourceEngine<T> sourceEngine, Func<T, TWrapper> createWrapper, IScheduler? scheduler = null)
        : base(sourceEngine, scheduler)
    {
        _createWrapper = createWrapper ?? throw new ArgumentNullException(nameof(createWrapper));

        _wrappersObservable = Connect()
            .Transform(GetOrCreate)
            .Publish()
            .RefCount();

        _wrappersSourceObservable = ConnectSource()
            .Transform(GetOrCreate)
            .Publish()
            .RefCount();

        Disposables.AddRange([
            _wrappersObservable
                .ObserveOnOptional(scheduler)
                .Bind(out var wrappers)
                .Subscribe(),
            _wrappersSourceObservable
                .ObserveOnOptional(scheduler)
                .Bind(out var wrappersSource)
                .Subscribe(),
            ConnectSource().Subscribe(PruneCache)
        ]);

        Wrappers = wrappers;
        WrappersSource = wrappersSource;
    }

    /// <summary>
    /// Gets wrappers corresponding to filtered/sorted items.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> Wrappers { get; }

    /// <summary>
    /// Gets wrappers corresponding to source items.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> WrappersSource { get; }

    /// <summary>
    /// Returns an observable stream of wrapper changes for current items.
    /// </summary>
    public IObservable<IChangeSet<TWrapper>> ConnectWrappers() => _wrappersObservable;

    /// <summary>
    /// Returns an observable stream of wrapper changes for source items.
    /// </summary>
    public IObservable<IChangeSet<TWrapper>> ConnectWrappersSource() => _wrappersSourceObservable;

    /// <summary>
    /// Gets the wrapper for an item, creating it if necessary.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The wrapper associated with the specified item.</returns>
    public TWrapper GetOrCreate(T item)
    {
        if (_cache.TryGetValue(item, out var wrapper))
            return wrapper;

        wrapper = _createWrapper(item);
        _cache[item] = wrapper;
        return wrapper;
    }

    /// <summary>
    /// Rebuilds the wrapper collections based on the current items and source items, ensuring that the wrapper instances are up to date and synchronized with the underlying data. This method is called whenever there are changes to the items or source items, allowing for efficient management of wrapper instances while keeping the collections in sync with the source data. The wrapper collections are updated by creating or retrieving wrapper instances for each item in the respective collections, ensuring that the wrapper projections remain accurate and reflect the current state of the underlying data.
    /// </summary>
    private void PruneCache(IChangeSet<T> changes)
    {
        foreach (var change in changes)
        {
            switch (change.Reason)
            {
                case ListChangeReason.Remove:
                    Evict(change.Item.Current);
                    break;
                case ListChangeReason.RemoveRange:
                    foreach (var item in change.Range)
                        Evict(item);

                    break;
                case ListChangeReason.Replace:
                    if (change.Item.Previous.HasValue)
                        Evict(change.Item.Previous.Value);

                    break;
                case ListChangeReason.Clear:
                    ClearCache();
                    break;
                case ListChangeReason.Add:
                case ListChangeReason.AddRange:
                case ListChangeReason.Moved:
                case ListChangeReason.Refresh:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Evicts the wrapper associated with the specified item from the cache and disposes of it if it implements IDisposable. This method is called when an item is removed from the collection, ensuring that the corresponding wrapper instance is properly disposed of and removed from the cache to free up resources and maintain the integrity of the collection. By evicting the wrapper instance, any resources associated with it can be released, preventing memory leaks and ensuring that the collection remains efficient and responsive even after significant changes to the underlying data.
    /// </summary>
    /// <param name="item">The item whose wrapper should be evicted.</param>
    private void Evict(T item)
    {
        if (!_cache.Remove(item, out var wrapper))
            return;

        if (wrapper is IDisposable disposable)
            disposable.Dispose();
    }

    /// <summary>
    /// Clears the wrapper cache and disposes all wrapper instances. This method is called when the collection is cleared, ensuring that all wrapper instances are properly disposed of and the cache is emptied to free up resources and maintain the integrity of the collection. By disposing of the wrapper instances, any resources associated with them can be released, preventing memory leaks and ensuring that the collection remains efficient and responsive even after significant changes to the underlying data.
    /// </summary>
    private void ClearCache()
    {
        foreach (var wrapper in _cache.Values)
        {
            if (wrapper is IDisposable disposable)
                disposable.Dispose();
        }

        _cache.Clear();
    }
}
