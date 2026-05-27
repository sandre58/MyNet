// -----------------------------------------------------------------------
// <copyright file="ListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Reactive.Concurrency;
using MyNet.Observable.Collections.Sources;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Represents a view model for a list of items of type T, providing functionalities for filtering, sorting, grouping, and paging the items in the list. This class serves as a base for creating specific list view models that can be used in various UI scenarios to display and manage collections of data. It encapsulates the logic for handling user interactions related to list manipulation and provides properties and commands to facilitate these operations in the UI.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ListViewModel{T}"/> class with the specified collection and optional view models for filtering, sorting, grouping, paging, and busy state management. This constructor allows you to create a list view model that is ready to be used in the UI with the provided collection and configurations for various list operations.
/// </remarks>
/// <param name="dataProvider">The collection of items to be managed by the view model.</param>
/// <param name="filters">Optional view model for managing filters.</param>
/// <param name="sorting">Optional view model for managing sorting.</param>
/// <param name="grouping">Optional view model for managing grouping.</param>
/// <param name="paging">Optional view model for managing paging.</param>
/// <param name="busyService">Optional service for managing busy state.</param>
/// <param name="scheduler">Optional scheduler for managing concurrency.</param>
public class ListViewModel<T>(IListDataProvider<T> dataProvider,
    IFiltersViewModel<T>? filters = null,
    ISortingViewModel<T>? sorting = null,
    IGroupingViewModel<T>? grouping = null,
    IPagingViewModel? paging = null,
    IBusyService? busyService = null,
    IScheduler? scheduler = null) : ListViewModelBase<T>(dataProvider, filters, sorting, grouping, paging, busyService, scheduler)
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModel{T}"/> class with optional options.
    /// </summary>
    /// <param name="source">The collection of items to be managed by the view model.</param>
    /// <param name="options">Optional list view model options.</param>
    public ListViewModel(SourceEngine<T> source, ListViewModelOptions<T>? options)
        : this(new ExtendedCollectionDataProvider<T>(new(source, options?.Scheduler)), options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModel{T}"/> class from a source engine.
    /// </summary>
    public ListViewModel(SourceEngine<T> source,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
        : this(new ExtendedCollectionDataProvider<T>(new(source, scheduler)), filters, sorting, grouping, paging, busyService, scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModel{T}"/> class with optional options.
    /// </summary>
    public ListViewModel(IListDataProvider<T> dataProvider, ListViewModelOptions<T>? options)
        : this(dataProvider, options?.Filters, options?.Sorting, options?.Grouping, options?.Paging, options?.BusyService, options?.Scheduler)
    {
    }
}
