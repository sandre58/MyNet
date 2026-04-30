// -----------------------------------------------------------------------
// <copyright file="ExtendedCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using DynamicData;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Sorting;
using MyNet.Observable.Collections.Sources;
using MyNet.Observable.Extensions;
using MyNet.Utilities;

namespace MyNet.Observable.Collections;

/// <summary>
/// Provides factory methods for creating instances of the <see cref="ExtendedCollection{T}"/> class. The <see cref="ExtendedCollection"/> class offers a flexible and powerful way to manage collections of items with support for filtering, sorting, and change notifications. The static methods in this class allow you to create extended collections from various sources, including empty collections, existing enumerables, and observable sequences of change sets, with optional schedulers for observing collection changes. These factory methods provide convenient ways to initialize extended collections based on different scenarios and data sources, while still maintaining encapsulation and integrity of the data.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "This class serves as a factory for ExtendedCollection<T> and the name reflects its purpose.")]
public static class ExtendedCollection
{
    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedCollection{T}"/> class with an optional scheduler for observing collection changes. This method provides a convenient way to create an extended collection without needing to specify an initial source list, allowing for dynamic population of the collection after creation. The Create method initializes the collection with an empty source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedCollection{T}"/> instance.</returns>
    public static ExtendedCollection<T> Create<T>(IScheduler? scheduler = null)
        where T : notnull
        => new(SourceEngine<T>.Empty(), scheduler);

    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedCollection{T}"/> class from an existing enumerable collection of items, with an optional scheduler for observing collection changes. This method allows you to initialize the extended collection with a predefined set of items, providing a convenient way to create a collection that is already populated with data. The From method initializes the collection with the provided enumerable as the source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="items">The enumerable collection of items to initialize the extended collection with.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedCollection{T}"/> instance.</returns>
    public static ExtendedCollection<T> From<T>(IEnumerable<T> items, IScheduler? scheduler = null)
        where T : notnull
        => new(SourceEngine<T>.From(items, readOnly: false), scheduler);

    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedCollection{T}"/> class from an existing enumerable collection of items as a read-only snapshot, with an optional scheduler for observing collection changes. This method allows you to initialize the extended collection with a predefined set of items while ensuring that the collection is read-only, providing a convenient way to create an immutable collection that is already populated with data. The FromReadOnly method initializes the collection with the provided enumerable as the source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="items">The enumerable collection of items to initialize the extended collection with.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedCollection{T}"/> instance.</returns>
    public static ExtendedCollection<T> FromReadOnly<T>(IEnumerable<T> items, IScheduler? scheduler = null)
        where T : notnull
        => new(SourceEngine<T>.From(items, readOnly: true), scheduler);

    /// <summary>
    /// Creates a new instance of the <see cref="ExtendedCollection{T}"/> class from an observable sequence of change sets, with an optional scheduler for observing collection changes. This method allows you to initialize the extended collection with a dynamic data source that emits change sets, providing a convenient way to create a collection that automatically updates based on changes in the underlying data. The FromObservable method initializes the collection with the provided observable as the source and sets up the necessary engines for filtering and sorting, while still maintaining encapsulation and integrity of the data. If a scheduler is provided, it will be used to observe collection changes; otherwise, the current thread scheduler will be used by default.
    /// </summary>
    /// <param name="source">The observable sequence of change sets to initialize the extended collection with.</param>
    /// <param name="scheduler">The scheduler on which the collection's operations will be performed.</param>
    /// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
    /// <returns>The created <see cref="ExtendedCollection{T}"/> instance.</returns>
    public static ExtendedCollection<T> FromObservable<T>(IObservable<IChangeSet<T>> source, IScheduler? scheduler = null)
        where T : notnull
        => new(SourceEngine<T>.FromObservable(source), scheduler);
}

/// <summary>
/// Represents an extended collection that supports filtering, sorting, and change notifications. This collection is designed to work with dynamic data sources and provides a flexible way to manage and manipulate collections of items of type T. The collection automatically updates its contents based on the defined filters and sorting properties, and it raises collection changed events when the underlying data changes.
/// </summary>
/// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
[ComVisible(false)]
[DebuggerDisplay("Count = {Count}/{SourceCount}")]
[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
public class ExtendedCollection<T> : ObservableObject, ICollection<T>, IReadOnlyList<T>, INotifyCollectionChanged
    where T : notnull
{
    private readonly SourceEngine<T> _source;
    private readonly FilterEngine<T> _filterEngine = new();
    private readonly SortEngine<T> _sortEngine = new();
    private readonly PropertyDependencyExtractor<T> _dependencyExtractor = new();

    private readonly IObservable<IChangeSet<T>> _itemsObservable;
    private readonly IObservable<IChangeSet<T>> _sourceObservable;
    private readonly ReadOnlyObservableCollection<T> _items;
    private readonly ReadOnlyObservableCollection<T> _sortedSource;
    private readonly Dictionary<string, int> _filterPropertyUsage = [];
    private readonly Dictionary<string, int> _sortPropertyUsage = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedCollection{T}"/> class with a specified source list, read-only flag, and optional scheduler for observing collection changes. This constructor allows for more control over the initial state of the collection, including whether it should be read-only and which source list to use. The constructor sets up the necessary engines for filtering and sorting, and it subscribes to changes in the filters and sorting properties to ensure that the collection updates correctly when these configurations change.
    /// </summary>
    /// <param name="source">The source list to initialize the collection with.</param>
    /// <param name="scheduler">The scheduler to use for observing collection changes. If null, the current thread scheduler is used.</param>
    public ExtendedCollection(SourceEngine<T> source, IScheduler? scheduler = null)
    {
        _source = source;

        (_sourceObservable, _itemsObservable) = PipelineEngine.Build(_source.Connect(), _filterEngine, _sortEngine);

        Disposables.AddRange(
        [
            _sourceObservable.ObserveOnOptional(scheduler).Bind(out _sortedSource).Subscribe(_ => OnPropertyChanged(nameof(SourceCount))),
            _itemsObservable.ObserveOnOptional(scheduler).Bind(out _items).Subscribe(),
            _source.Connect().SubscribeMany(SubscribeToItemChanges).Subscribe(),
            ObserveCollectionChanges(_items).Subscribe(HandleCollectionChanged)
        ]);
    }

    /// <summary>
    /// Gets a read-only observable collection of items that represents the current state of the collection after applying filters and sorting. This collection automatically updates when the underlying data changes, and it raises collection changed events to notify subscribers of any changes to the items in the collection. The Items property provides a way to access the filtered and sorted items while ensuring that external code cannot modify the collection directly, maintaining encapsulation and integrity of the data.
    /// </summary>
    public ReadOnlyObservableCollection<T> Items => _items;

    /// <summary>
    /// Gets a read-only observable collection of items that represents the current state of the source collection after applying sorting but before filtering. This collection automatically updates when the underlying data changes, and it raises collection changed events to notify subscribers of any changes to the items in the source collection. The Source property provides a way to access the sorted items before filtering is applied, allowing for scenarios where you may want to work with the sorted data without the influence of filters, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public ReadOnlyObservableCollection<T> Source => _sortedSource;

    /// <summary>
    /// Gets the count of items in the source collection after sorting but before filtering. This count reflects the number of items that are currently in the source collection, regardless of any filters that may be applied to the collection. The SourceCount property provides a way to determine how many items are in the source collection, which can be useful for scenarios where you want to know the total number of items available before any filtering is applied, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public int SourceCount => _source.Count;

    /// <summary>
    /// Gets the collection of sorting properties that are applied to the items in the collection. The Sorting property allows you to define and manage a set of sorting properties that determine the order of items in the collection based on specific criteria. When sorting properties are added, removed, or modified, the collection automatically updates to reflect the changes, and it raises collection changed events to notify subscribers of any changes to the items in the collection. The Sorting property provides a flexible way to control the order of items in the collection based on dynamic conditions, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public ISortingProperty<T>[] CurrentSort => _sortEngine.Current;

    /// <summary>
    /// Gets the collection of filters that are applied to the items in the collection. The Filters property allows you to define and manage a set of filters that determine which items are included in the collection based on specific criteria. When filters are added, removed, or modified, the collection automatically updates to reflect the changes, and it raises collection changed events to notify subscribers of any changes to the items in the collection. The Filters property provides a flexible way to control which items are included in the collection based on dynamic conditions, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public IFilter<T>? CurrentFilter => _filterEngine.Current;

    /// <summary>
    /// Connects to the collection and returns an observable sequence of change sets representing the changes to the items in the collection after filtering and sorting are applied. This method allows subscribers to receive notifications of changes to the collection's contents, including additions, removals, and updates to items, based on the current filters and sorting properties. The Connect method provides a way to react to changes in the collection's items, enabling dynamic updates to user interfaces or other dependent components based on the specific changes that occur in the collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public IObservable<IChangeSet<T>> Connect() => _itemsObservable;

    /// <summary>
    /// Connects to the source collection and returns an observable sequence of change sets representing the changes to the source items in the collection before filtering and sorting are applied. This method allows subscribers to receive notifications of changes to the source collection's contents, including additions, removals, and updates to items, based on the original data. The ConnectSource method provides a way to react to changes in the source collection's items, enabling dynamic updates to user interfaces or other dependent components based on the specific changes that occur in the source collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public IObservable<IChangeSet<T>> ConnectSource() => _sourceObservable;

    /// <summary>
    /// Performs cleanup of resources used by the collection, including disposing of the filter engine, sort engine, and source engine. This method is called when the collection is being disposed of, ensuring that all resources are properly released and that there are no memory leaks or lingering subscriptions. The Cleanup method overrides the base class implementation to provide specific cleanup logic for the extended collection, maintaining encapsulation and integrity of the data while ensuring efficient resource management.
    /// </summary>
    protected override void Cleanup()
    {
        _filterEngine.Dispose();
        _sortEngine.Dispose();
        _source.Dispose();

        base.Cleanup();
    }

    #region Filter

    /// <summary>
    /// Sets the filter for the collection using the specified filter node. This method updates the filter engine with the new filter configuration, which will automatically update the collection's contents based on the defined filtering criteria. When a new filter is set, the collection will re-evaluate its items against the new filter and raise collection changed events to notify subscribers of any changes to the items in the collection. The SetFilter method provides a way to dynamically change the filtering logic of the collection, allowing for flexible control over which items are included based on specific conditions, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="filter">The filter to apply to the collection.</param>
    public void SetFilter(IFilter<T> filter)
    {
        _filterEngine.Set(filter);
        RebuildFilterDependencies(filter);
    }

    /// <summary>
    /// Clears the filter from the collection, effectively removing all filtering criteria and including all items in the collection. This method updates the filter engine to remove any existing filters, which will automatically update the collection's contents to include all items without any restrictions. When the filter is cleared, the collection will re-evaluate its items and raise collection changed events to notify subscribers of any changes to the items in the collection. The ClearFilter method provides a way to reset the filtering logic of the collection, allowing for a fresh start when defining new filters or when you want to include all items without any filtering, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public void ClearFilter()
    {
        _filterEngine.Clear();
        _filterPropertyUsage.Clear();
    }

    #endregion

    #region Sorting

    /// <summary>
    /// Sets the sorting properties for the collection using the specified array of sorting properties. This method updates the sort engine with the new sorting configuration, which will automatically update the order of items in the collection based on the defined sorting criteria. When new sorting properties are set, the collection will re-evaluate its items against the new sorting configuration and raise collection changed events to notify subscribers of any changes to the order of items in the collection. The SetSorting method provides a way to dynamically change the sorting logic of the collection, allowing for flexible control over the order of items based on specific conditions, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="sorting">The array of sorting properties to apply to the collection.</param>
    public void SetSorting(params ISortingProperty<T>[] sorting)
    {
        _sortEngine.Set(sorting);
        RebuildSortDependencies(sorting);
    }

    /// <summary>
    /// Clears the sorting properties from the collection, effectively removing all sorting criteria and returning to the default order of items. This method updates the sort engine to remove any existing sorting properties, which will automatically update the order of items in the collection to reflect the default sorting behavior. When the sorting is cleared, the collection will re-evaluate its items and raise collection changed events to notify subscribers of any changes to the order of items in the collection. The ClearSorting method provides a way to reset the sorting logic of the collection, allowing for a fresh start when defining new sorting properties or when you want to return to the default order of items without any specific sorting, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public void ClearSorting()
    {
        _sortEngine.Clear();
        _sortPropertyUsage.Clear();
    }

    #endregion

    #region INotifyCollectionChanged

    /// <summary>
    /// Occurs when the collection changes. This event is raised whenever items are added, removed, or replaced in the collection, allowing subscribers to react to changes in the collection's contents. The CollectionChanged event provides a way to notify external code of changes to the collection, enabling dynamic updates to user interfaces or other dependent components based on the specific changes that occur in the collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Use OnCollectionChanged")]
    event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
    {
        add => CollectionChanged += value;
        remove => CollectionChanged -= value;
    }

    /// <summary>
    /// Raises the CollectionChanged event with the specified event arguments. This method is called internally to notify subscribers of changes to the collection, allowing external code to react to specific changes in the collection's contents. The HandleCollectionChanged method provides a way to trigger the CollectionChanged event with the appropriate event arguments, ensuring that subscribers receive accurate and timely notifications of changes to the collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    [SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Use OnCollectionChanged")]
    private event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Handles the collection changed event by invoking the CollectionChanged event with the provided event arguments. This method is called whenever a change occurs in the collection, allowing subscribers to react to specific changes in the collection's contents. The HandleCollectionChanged method ensures that subscribers receive accurate and timely notifications of changes to the collection, enabling dynamic updates to user interfaces or other dependent components based on the specific changes that occur in the collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="e">The event arguments describing the change that occurred in the collection.</param>
    private void HandleCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

    #endregion INotifyCollectionChanged

    #region ICollection

    /// <summary>
    /// Gets the number of items in the collection after filtering is applied. This count reflects the number of items that are currently visible in the collection based on the defined filters, and it may differ from the total number of items in the source collection. The Count property provides a way to determine how many items are currently included in the collection after filtering, which can be useful for scenarios where you want to know the number of items that meet specific criteria defined by the filters, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only. If true, the collection does not allow modifications such as adding, removing, or clearing items. The IsReadOnly property provides a way to determine if the collection can be modified, which can be useful for scenarios where you want to enforce immutability or restrict changes to the collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets the item at the specified index in the collection after filtering and sorting are applied. This index is based on the current state of the collection, which may differ from the original source collection due to the applied filters and sorting properties. The indexer provides a way to access individual items in the collection based on their position in the filtered and sorted view, allowing for dynamic access to items while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="index">The zero-based index of the item to get.</param>
    /// <returns>The item at the specified index.</returns>
    public T this[int index] => _items[index];

    /// <summary>
    /// Adds an item to the collection if it is not read-only. This method checks the IsReadOnly property before attempting to add the item, ensuring that modifications are only allowed when the collection is not read-only. If the collection is read-only, the method does nothing, maintaining encapsulation and integrity of the data while enforcing immutability when necessary.
    /// </summary>
    /// <param name="item">The item to add to the collection.</param>
    public void Add(T item) => IsReadOnly.IfFalse(() => _source.Add(item));

    /// <summary>
    /// Removes all items from the collection if it is not read-only. This method checks the IsReadOnly property before attempting to clear the collection, ensuring that modifications are only allowed when the collection is not read-only. If the collection is read-only, the method does nothing, maintaining encapsulation and integrity of the data while enforcing immutability when necessary.
    /// </summary>
    public void Clear() => IsReadOnly.IfFalse(() => _source.Clear());

    /// <summary>
    /// Removes the first occurrence of a specific item from the collection if it is not read-only. This method checks the IsReadOnly property before attempting to remove the item, ensuring that modifications are only allowed when the collection is not read-only. If the collection is read-only, the method does nothing and returns false, maintaining encapsulation and integrity of the data while enforcing immutability when necessary. If the item is successfully removed, the method returns true; otherwise, it returns false if the item was not found in the collection.
    /// </summary>
    /// <param name="item">The item to remove from the collection.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(T item) => !IsReadOnly && _source.Remove(item);

    /// <summary>
    /// Determines the index of a specific item in the collection after filtering and sorting are applied. This method searches for the specified item in the current state of the collection, which may differ from the original source collection due to the applied filters and sorting properties. The IndexOf method provides a way to find the position of an item in the filtered and sorted view of the collection, allowing for dynamic access to items based on their position while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="item">The item to locate in the collection.</param>
    /// <returns>The index of the item if found; otherwise, -1.</returns>
    public int IndexOf(T item) => _items.IndexOf(item);

    /// <summary>
    /// Determines whether the collection contains a specific item after filtering is applied. This method checks for the presence of the specified item in the current state of the collection, which may differ from the original source collection due to the applied filters. The Contains method provides a way to check if an item is currently included in the collection based on the defined filters, allowing for dynamic checks of item presence while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="item">The item to locate in the collection.</param>
    /// <returns>True if the item is found in the collection; otherwise, false.</returns>
    public bool Contains(T item) => _items.Contains(item);

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index. This method allows you to create a copy of the items in the collection in a standard array format, which can be useful for interoperability with APIs that require arrays or for performing operations that are more efficient with arrays. The CopyTo method ensures that the items are copied in the correct order based on the current state of the collection after filtering and sorting, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    /// <summary>
    /// Returns an enumerator that iterates through the collection. This method allows you to use foreach loops or LINQ queries to iterate over the items in the collection, providing a convenient way to access each item in the filtered and sorted view of the collection. The GetEnumerator method returns an enumerator that reflects the current state of the collection after filtering and sorting, allowing for dynamic iteration while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection. This method is an explicit implementation of the non-generic IEnumerable interface, allowing for compatibility with non-generic collection interfaces and enabling iteration over the collection using non-generic enumerators. The GetEnumerator method returns an enumerator that reflects the current state of the collection after filtering and sorting, allowing for dynamic iteration while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    #endregion

    #region ICollection Extensions

    /// <summary>
    /// Adds multiple items to the collection if it is not read-only. This method checks the IsReadOnly property before attempting to add the items, ensuring that modifications are only allowed when the collection is not read-only. If the collection is read-only, the method does nothing, maintaining encapsulation and integrity of the data while enforcing immutability when necessary. If the collection is not read-only, the method adds all items from the provided enumerable to the collection, allowing for efficient batch additions while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="items">The items to add to the collection.</param>
    public void AddRange(IEnumerable<T> items) => IsReadOnly.IfFalse(() => _source.AddRange(items));

    /// <summary>
    /// Removes multiple items from the collection if it is not read-only. This method checks the IsReadOnly property before attempting to remove the items, ensuring that modifications are only allowed when the collection is not read-only. If the collection is read-only, the method does nothing, maintaining encapsulation and integrity of the data while enforcing immutability when necessary. If the collection is not read-only, the method removes all items from the provided enumerable from the collection, allowing for efficient batch removals while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="itemsToRemove">The items to remove from the collection.</param>
    public void RemoveMany(IEnumerable<T> itemsToRemove) => IsReadOnly.IfFalse(() => _source.RemoveMany(itemsToRemove));

    /// <summary>
    /// Replaces the current items in the collection with the specified items if the collection is not read-only. This method checks the IsReadOnly property before attempting to set the items, ensuring that modifications are only allowed when the collection is not read-only. If the collection is read-only, the method does nothing, maintaining encapsulation and integrity of the data while enforcing immutability when necessary. If the collection is not read-only, the method clears the current items and adds all items from the provided enumerable, allowing for efficient batch updates while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="items">The items to set in the collection.</param>
    public void Set(IEnumerable<T> items) => IsReadOnly.IfFalse(() => _source.Edit(x =>
    {
        x.Clear();
        x.AddRange(items);
    }));

    #endregion

    #region Wrappers

    /// <summary>
    /// Creates a wrapper projection of the collection using the specified factory function to create wrapper instances for each item in the collection. This method allows you to create a new collection that contains wrapper objects instead of the original items, providing a way to add additional functionality or properties to the items while still maintaining a connection to the original data. The CreateWrapperProjectionFromItems method returns a new instance of the WrapperProjection class, which automatically updates its contents based on changes to the underlying collection and applies the factory function to create wrapper instances for each item, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="factory">The factory function to create wrapper instances for each item in the collection.</param>
    /// <param name="scheduler">The scheduler to use for updating the wrapper projection.</param>
    /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
    /// <returns>A new instance of the WrapperProjection class.</returns>
    public WrapperProjection<T, TWrapper> CreateWrapperProjectionFromItems<TWrapper>(
        Func<T, TWrapper> factory,
        IScheduler? scheduler = null)
        where TWrapper : class, IWrapper<T> => new(Connect(), factory, scheduler);

    /// <summary>
    /// Creates a wrapper projection of the source collection using the specified factory function to create wrapper instances for each item in the source collection. This method allows you to create a new collection that contains wrapper objects instead of the original items from the source collection, providing a way to add additional functionality or properties to the items while still maintaining a connection to the original data. The CreateWrapperProjectionFromSource method returns a new instance of the WrapperProjection class, which automatically updates its contents based on changes to the underlying source collection and applies the factory function to create wrapper instances for each item, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="factory">The factory function to create wrapper instances for each item in the source collection.</param>
    /// <param name="scheduler">The scheduler to use for updating the wrapper projection.</param>
    /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
    /// <returns>A new instance of the WrapperProjection class.</returns>
    public WrapperProjection<T, TWrapper> CreateWrapperProjectionFromSource<TWrapper>(
        Func<T, TWrapper> factory,
        IScheduler? scheduler = null)
        where TWrapper : class, IWrapper<T> => new(ConnectSource(), factory, scheduler);

    #endregion Wrappers

    #region Observable

    /// <summary>
    /// Observes collection changes on the specified source collection and returns an observable sequence of collection changed event arguments. This method uses Reactive Extensions to create an observable that listens for changes to the collection and emits the corresponding event arguments whenever a change occurs. The ObserveCollectionChanges method provides a way to react to changes in the collection, allowing subscribers to update their state or perform actions based on the specific changes that occur in the collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="source">The source collection to observe for changes.</param>
    /// <returns>An observable sequence of collection changed event arguments.</returns>
    private static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanges(
        INotifyCollectionChanged source) =>
        System.Reactive.Linq.Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h)
            .Select(x => x.EventArgs);

    /// <summary>
    /// Subscribes to property changes of an item in the collection if it implements INotifyPropertyChanged. This method creates an observable that listens for property change events on the item and triggers a refresh of the filter and sort engines when relevant properties change. The SubscribeToItemChanges method ensures that the collection remains up-to-date with changes to the properties of individual items, allowing for dynamic updates to the collection based on changes to the underlying data, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="item">The item to observe for property changes.</param>
    /// <returns>A disposable that can be used to unsubscribe from property change notifications.</returns>
    private IDisposable SubscribeToItemChanges(T item) =>
        item is not INotifyPropertyChanged npc
            ? Disposable.Empty
            : System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => npc.PropertyChanged += h,
                    h => npc.PropertyChanged -= h)
                .Subscribe(e =>
                {
                    var prop = e.EventArgs.PropertyName;
                    if (prop is null) return;

                    var affectsFilter = _filterPropertyUsage.ContainsKey(prop);
                    var affectsSort = _sortPropertyUsage.ContainsKey(prop);

                    if (affectsFilter)
                        _filterEngine.Invalidate();

                    if (affectsSort)
                        _sortEngine.Invalidate();
                });

    #endregion

    #region Dependencies

    /// <summary>
    /// Rebuilds the filter dependencies based on the provided filter. This method extracts the properties used in the filter's expression and updates the _filterPropertyUsage dictionary to track which properties are relevant for filtering. This allows the collection to efficiently determine when to invalidate the filter engine based on changes to item properties, ensuring that the collection remains up-to-date with changes to the underlying data while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="filter">The filter for which to rebuild dependencies.</param>
    private void RebuildFilterDependencies(IFilter<T> filter)
    {
        _filterPropertyUsage.Clear();

        var expr = filter.ProvideExpression();
        var deps = _dependencyExtractor.ExtractFilter(expr);

        foreach (var d in deps)
            _filterPropertyUsage[d] = 1;
    }

    /// <summary>
    /// Rebuilds the sort dependencies based on the provided sorting properties. This method extracts the properties used in the sorting key selectors and updates the _sortPropertyUsage dictionary to track which properties are relevant for sorting. This allows the collection to efficiently determine when to invalidate the sort engine based on changes to item properties, ensuring that the collection remains up-to-date with changes to the underlying data while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="sorting">The sorting properties for which to rebuild dependencies.</param>
    private void RebuildSortDependencies(ISortingProperty<T>[] sorting)
    {
        _sortPropertyUsage.Clear();

        foreach (var s in sorting)
        {
            var deps = _dependencyExtractor.ExtractSort(s.ProvideExpression());

            foreach (var d in deps)
                _sortPropertyUsage[d] = 1;
        }
    }

    #endregion
}
