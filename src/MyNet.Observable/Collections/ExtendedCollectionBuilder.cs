// -----------------------------------------------------------------------
// <copyright file="ExtendedCollectionBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using DynamicData;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Collections.Selection;
using MyNet.Observable.Collections.Sorting;
using MyNet.Observable.Collections.Sources;
using MyNet.Utilities.Providers;

namespace MyNet.Observable.Collections;

/// <summary>
/// Provides a fluent API for constructing an <see cref="ExtendedCollection{T}"/> instance with specified source, filter, and sorting configurations. This builder allows for the definition of the collection's data source, as well as the application of complex filtering and sorting criteria through a fluent interface. The resulting collection will be initialized with the provided configurations at construction time, allowing for a flexible and customizable collection setup.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public sealed class ExtendedCollectionBuilder<T>
    where T : notnull
{
    private readonly FilterBuilder<T> _filterBuilder = FilterBuilder<T>.Create();
    private readonly SortingBuilder<T> _sortingBuilder = SortingBuilder<T>.Create();
    private readonly GroupingBuilder<T> _groupingBuilder = GroupingBuilder<T>.Create();
    private Func<SourceEngine<T>> _sourceFactory = SourceEngine<T>.Empty;

    /// <summary>
    /// Defines the source of items for the collection. The provided items will be loaded into the collection at construction time.
    /// </summary>
    /// <param name="items">The initial set of items for the collection.</param>
    /// <param name="isReadOnly">Determines whether the collection operates in read-only mode.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> From(IEnumerable<T> items, bool isReadOnly = false)
    {
        _sourceFactory = () => SourceEngine<T>.From(items, isReadOnly);
        return this;
    }

    /// <summary>
    /// Defines the source of items for the collection using a custom provider. The provided items will be loaded into the collection at construction time.
    /// </summary>
    /// <param name="source">The observable source of change sets for the collection.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> FromObservable(IObservable<IChangeSet<T>> source)
    {
        _sourceFactory = () => SourceEngine<T>.FromObservable(source);
        return this;
    }

    /// <summary>
    /// Defines the source of items for the collection using a snapshot provider. The provided factory function will be invoked at construction time to create the snapshot of items for the collection, allowing for dynamic retrieval of the initial data set when the collection is initialized.
    /// </summary>
    /// <param name="factory">A function that provides a snapshot of the collection's state.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> FromSnapshot(Func<IEnumerable<T>> factory)
    {
        _sourceFactory = () => SourceEngine<T>.FromSnapshot(factory);
        return this;
    }

    /// <summary>
    /// Defines the source of items for the collection using a custom provider. The provided provider will be used to retrieve the initial set of items for the collection at construction time, allowing for dynamic retrieval of the data set when the collection is initialized.
    /// </summary>
    /// <param name="provider">The custom provider that will be used to retrieve the initial set of items for the collection.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> FromProvider(IItemsProvider<T> provider)
    {
        _sourceFactory = () => SourceEngine<T>.FromProvider(provider);
        return this;
    }

    /// <summary>
    /// Defines the source of items for the collection using a custom provider. The provided factory function will be invoked at construction time to create the source engine for the collection.
    /// </summary>
    /// <param name="predicate">A lambda expression that defines the filter criteria.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        _filterBuilder.Where(predicate);
        return this;
    }

    /// <summary>
    /// Defines a logical AND grouping of filter criteria for the collection. The provided predicate will be combined with the existing filter criteria using a logical AND operation, meaning that items must satisfy all of the specified criteria to be included in the filtered collection.
    /// </summary>
    /// <param name="predicate">A lambda expression that defines the filter criteria for the AND operation.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> And(Expression<Func<T, bool>> predicate)
    {
        _filterBuilder.And(predicate);
        return this;
    }

    /// <summary>
    /// Defines a logical OR grouping of filter criteria for the collection. The provided predicate will be combined with the existing filter criteria using a logical OR operation, meaning that items must satisfy at least one of the specified criteria to be included in the filtered collection.
    /// </summary>
    /// <param name="predicate">A lambda expression that defines the filter criteria for the OR operation.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> Or(Expression<Func<T, bool>> predicate)
    {
        _filterBuilder.Or(predicate);
        return this;
    }

    /// <summary>
    /// Defines a logical AND grouping of filter criteria for the collection. The provided group function allows for the definition of a nested set of filter criteria that will be combined with the existing filter criteria using a logical AND operation. This method can be used to create complex filter expressions by grouping multiple criteria together, where items must match all of the grouped criteria to be included in the filtered collection.
    /// </summary>
    /// <param name="group">A function that defines the nested set of filter criteria.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> And(Func<FilterBuilder<T>, FilterBuilder<T>> group)
    {
        _filterBuilder.And(group);
        return this;
    }

    /// <summary>
    /// Defines a logical OR grouping of filter criteria for the collection. The provided group function allows for the definition of a nested set of filter criteria that will be combined with the existing filter criteria using a logical OR operation. This method can be used to create complex filter expressions by grouping multiple criteria together, where items that match any of the grouped criteria will be included in the filtered collection.
    /// </summary>
    /// <param name="group">A function that defines the nested set of filter criteria.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> Or(Func<FilterBuilder<T>, FilterBuilder<T>> group)
    {
        _filterBuilder.Or(group);
        return this;
    }

    /// <summary>
    /// Defines the primary sorting key for the collection. The provided key expression will be used to sort the items in ascending order. This method clears any existing sorting criteria and sets the new sorting key as the primary sorting criterion for the collection, with the sorting direction set to ascending.
    /// </summary>
    /// <param name="key">A lambda expression that defines the key for sorting.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> OrderBy(Expression<Func<T, object?>> key)
    {
        _sortingBuilder.Add(key, ListSortDirection.Ascending);
        return this;
    }

    /// <summary>
    /// Defines the primary sorting criterion for the collection, specifying that the items should be sorted in descending order based on the provided key expression. This method clears any existing sorting criteria and sets the new sorting key as the primary sorting criterion for the collection, with the sorting direction set to descending.
    /// </summary>
    /// <param name="key">A lambda expression that defines the key for sorting.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> OrderByDescending(Expression<Func<T, object?>> key)
    {
        _sortingBuilder.Add(key, ListSortDirection.Descending);
        return this;
    }

    /// <summary>
    /// Adds a secondary sorting key to the collection. The provided key will be used for sorting items that are considered equal based on the previous sorting keys. The sorting direction can be specified as ascending (default) or descending.
    /// </summary>
    /// <param name="key">A lambda expression that defines the key for sorting.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> ThenBy(Expression<Func<T, object?>> key)
    {
        _sortingBuilder.ThenBy(key);
        return this;
    }

    /// <summary>
    /// Adds a secondary sorting criterion to the collection, allowing for multi-level sorting. The provided key expression will be used to sort the items in descending order after applying any previous sorting criteria defined by OrderBy or ThenBy methods. This method can be called multiple times to add additional sorting levels, with each subsequent call adding another level of sorting based on the specified key expression.
    /// </summary>
    /// <param name="key">A lambda expression that defines the key for sorting.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> ThenByDescending(Expression<Func<T, object?>> key)
    {
        _sortingBuilder.ThenByDescending(key);
        return this;
    }

    /// <summary>
    /// Adds a grouping key to the collection. Items will be grouped by the provided expression.
    /// </summary>
    /// <param name="key">A lambda expression that defines the grouping key.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> GroupBy(Expression<Func<T, object?>> key)
    {
        _groupingBuilder.Add(key);
        return this;
    }

    /// <summary>
    /// Adds a secondary grouping key using a fluent ThenBy style.
    /// </summary>
    /// <param name="key">A lambda expression that defines the secondary grouping key.</param>
    /// <returns>The current instance of the <see cref="ExtendedCollectionBuilder{T}"/> class.</returns>
    public ExtendedCollectionBuilder<T> ThenGroupBy(Expression<Func<T, object?>> key)
    {
        _groupingBuilder.ThenBy(key);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="ExtendedCollection{T}"/> instance based on the defined source, filter, and sorting configurations. The collection will be initialized with the provided source of items, and the specified filter and sorting will be applied to the collection at construction time. An optional scheduler can be provided to control the execution context for collection updates.
    /// </summary>
    /// <param name="scheduler">The scheduler to control the execution context for collection updates.</param>
    /// <returns>The constructed <see cref="ExtendedCollection{T}"/> instance.</returns>
    public ExtendedCollection<T> Build(IScheduler? scheduler = null)
    {
        var source = _sourceFactory.Invoke();

        var collection = new ExtendedCollection<T>(source, scheduler);

        // Apply filter
        var filter = _filterBuilder.Build();
        if (filter is not null)
            collection.SetFilter(filter);

        // Apply sorting
        var sorting = _sortingBuilder.Build();
        collection.SetSorting([.. sorting]);

        // Apply grouping
        var grouping = _groupingBuilder.Build();
        if (grouping.Count > 0)
            collection.SetGrouping([.. grouping]);

        return collection;
    }

    /// <summary>
    /// Builds a <see cref="SelectableCollection{T}"/> instance based on the defined source, filter, and sorting configurations. The collection will be initialized with the provided source of items, and the specified filter and sorting will be applied to the collection at construction time. An optional scheduler can be provided to control the execution context for collection updates. The resulting collection will support item selection based on the specified selection mode.
    /// </summary>
    /// <param name="selectionMode">The selection mode for the collection.</param>
    /// <param name="scheduler">The scheduler to control the execution context for collection updates.</param>
    /// <returns>The constructed <see cref="SelectableCollection{T}"/> instance.</returns>
    public SelectableCollection<T> BuildSelectable(SelectionMode selectionMode = SelectionMode.Multiple, IScheduler? scheduler = null)
    {
        var source = _sourceFactory.Invoke();

        var collection = new ExtendedCollection<T>(source, scheduler);

        // Apply filter
        var filter = _filterBuilder.Build();
        if (filter is not null)
            collection.SetFilter(filter);

        // Apply sorting
        var sorting = _sortingBuilder.Build();
        collection.SetSorting([.. sorting]);

        // Apply grouping
        var grouping = _groupingBuilder.Build();
        if (grouping.Count > 0)
            collection.SetGrouping([.. grouping]);

        return new(collection, selectionMode);
    }
}
