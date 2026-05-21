// -----------------------------------------------------------------------
// <copyright file="IListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Collections.Sorting;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;

namespace MyNet.UI.ViewModels.List;

public interface IListViewModel<T> : IDisposable
{
    /// <summary>
    /// Gets the original source items before any transformation.
    /// </summary>
    ReadOnlyObservableCollection<T> Source { get; }

    /// <summary>
    /// Gets the resulting items after applying the pipeline (filter, sort, page).
    /// </summary>
    ReadOnlyObservableCollection<T> Items { get; }

    /// <summary>
    /// Gets the grouped representation of the items if grouping is active.
    /// Null if no grouping is applied.
    /// </summary>
    ReadOnlyObservableCollection<IGroup<T>>? Groups { get; }

    /// <summary>
    /// Gets the total number of items in the source.
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// Gets the filtering configuration ViewModel.
    /// </summary>
    IFiltersViewModel<T>? Filters { get; }

    /// <summary>
    /// Gets the current filter tree applied to the list.
    /// This is the core representation used by the pipeline.
    /// </summary>
    IFilter<T>? CurrentFilter { get; }

    /// <summary>
    /// Gets the sorting configuration ViewModel.
    /// </summary>
    ISortingViewModel<T>? Sorting { get; }

    /// <summary>
    /// Gets the effective sorting descriptors applied to the list.
    /// </summary>
    IReadOnlyList<ISortingProperty<T>> CurrentSorting { get; }

    /// <summary>
    /// Gets the grouping configuration ViewModel.
    /// </summary>
    IGroupingViewModel<T>? Grouping { get; }

    /// <summary>
    /// Gets the effective grouping descriptors applied to the list.
    /// </summary>
    IReadOnlyList<IGroupingProperty<T>> CurrentGrouping { get; }

    /// <summary>
    /// Gets the paging configuration ViewModel.
    /// </summary>
    IPagingViewModel? Paging { get; }
}
