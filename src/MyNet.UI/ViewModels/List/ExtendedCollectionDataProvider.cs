// -----------------------------------------------------------------------
// <copyright file="ExtendedCollectionDataProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Collections.Sorting;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Adapts an <see cref="ExtendedCollection{T}"/> instance to the list view model pipeline abstraction.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed class ExtendedCollectionDataProvider<T>(ExtendedCollection<T> collection) : IListDataProvider<T>
    where T : notnull
{
    /// <summary>
    /// Gets the adapted collection.
    /// </summary>
    public ExtendedCollection<T> Collection { get; } = collection ?? throw new ArgumentNullException(nameof(collection));

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> Source => Collection.Source;

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> FilteredItems => Collection.FilteredItems;

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> Items => Collection.Items;

    /// <inheritdoc />
    public int FilteredCount => Collection.Count;

    /// <inheritdoc />
    public IObservable<IChangeSet<T>> ConnectFiltered() => Collection.ConnectFiltered();

    /// <inheritdoc />
    public IObservable<IChangeSet<T>> Connect() => Collection.Connect();

    /// <inheritdoc />
    public IObservable<IReadOnlyList<CollectionGroup<T>>> ConnectGroups() => Collection.ConnectGroups();

    /// <inheritdoc />
    public void SetFilter(IFilter<T> filter) => Collection.SetFilter(filter);

    /// <inheritdoc />
    public void ClearFilter() => Collection.ClearFilter();

    /// <inheritdoc />
    public void SetSorting(params ISortingProperty<T>[] sorting) => Collection.SetSorting(sorting);

    /// <inheritdoc />
    public void ClearSorting() => Collection.ClearSorting();

    /// <inheritdoc />
    public void SetGrouping(params IGroupingProperty<T>[] grouping) => Collection.SetGrouping(grouping);

    /// <inheritdoc />
    public void ClearGrouping() => Collection.ClearGrouping();

    /// <inheritdoc />
    public void SetPaging(int page, int pageSize) => Collection.SetPaging(page, pageSize);

    /// <inheritdoc />
    public void ClearPaging() => Collection.ClearPaging();

    /// <inheritdoc />
    public void Dispose() => Collection.Dispose();
}
