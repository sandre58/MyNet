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
    /// Gets the current items after the pipeline has been applied.
    /// </summary>
    ReadOnlyObservableCollection<T> Items { get; }

    /// <summary>
    /// Observes item changes after filter/sort operations.
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
}
