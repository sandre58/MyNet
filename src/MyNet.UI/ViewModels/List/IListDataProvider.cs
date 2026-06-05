// -----------------------------------------------------------------------
// <copyright file="IListDataProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Collections.Sorting;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Provides an abstraction over the collection pipeline used by list view models.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IListDataProvider<T> : IDisposable
    where T : notnull
{
    /// <summary>
    /// Gets the original source items before filtering.
    /// </summary>
    ReadOnlyObservableCollection<T> Source { get; }

    /// <summary>
    /// Gets the items after filter and sort, before paging.
    /// </summary>
    ReadOnlyObservableCollection<T> FilteredItems { get; }

    /// <summary>
    /// Gets the current page items after filter, sort, and paging.
    /// When paging is disabled, this matches <see cref="FilteredItems"/>.
    /// </summary>
    ReadOnlyObservableCollection<T> Items { get; }

    /// <summary>
    /// Gets the number of items after filter and sort, before paging.
    /// </summary>
    int FilteredCount { get; }

    /// <summary>
    /// Observes item changes after filter/sort operations, before paging is applied.
    /// </summary>
    IObservable<IChangeSet<T>> ConnectFiltered();

    /// <summary>
    /// Observes item changes after filter/sort/paging operations.
    /// </summary>
    IObservable<IChangeSet<T>> Connect();

    /// <summary>
    /// Observes group changes produced by the grouping pipeline.
    /// </summary>
    IObservable<IReadOnlyList<CollectionGroup<T>>> ConnectGroups();

    /// <summary>
    /// Sets the active filter.
    /// </summary>
    void SetFilter(IFilter<T> filter);

    /// <summary>
    /// Clears the active filter.
    /// </summary>
    void ClearFilter();

    /// <summary>
    /// Sets the active sorting.
    /// </summary>
    void SetSorting(params ISortingProperty<T>[] sorting);

    /// <summary>
    /// Clears the active sorting.
    /// </summary>
    void ClearSorting();

    /// <summary>
    /// Sets the active grouping.
    /// </summary>
    void SetGrouping(params IGroupingProperty<T>[] grouping);

    /// <summary>
    /// Clears the active grouping.
    /// </summary>
    void ClearGrouping();

    /// <summary>
    /// Sets the active paging window.
    /// </summary>
    void SetPaging(int page, int pageSize);

    /// <summary>
    /// Clears the active paging window.
    /// </summary>
    void ClearPaging();
}
