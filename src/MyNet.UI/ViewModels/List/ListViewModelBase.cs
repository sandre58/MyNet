// -----------------------------------------------------------------------
// <copyright file="ListViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Collections.Sorting;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.Utilities.Deferring;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Provides a lightweight list pipeline implementation for ViewModels.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class ListViewModelBase<T> : ViewModelBase, IListViewModel<T>
    where T : notnull
{
    private readonly ObservableCollection<IGroup<T>> _groups = [];
    private readonly DeferredAction _pipelineRefreshDeferrer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModelBase{T}"/> class.
    /// </summary>
    protected ListViewModelBase(IListDataProvider<T> dataProvider, ListViewModelOptions<T>? options)
        : this(dataProvider, options?.Filters, options?.Sorting, options?.Grouping, options?.Paging, options?.Scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModelBase{T}"/> class.
    /// </summary>
    /// <param name="dataProvider">The pipeline data provider.</param>
    /// <param name="filters">Optional filtering configuration.</param>
    /// <param name="sorting">Optional sorting configuration.</param>
    /// <param name="grouping">Optional grouping configuration.</param>
    /// <param name="paging">Optional paging configuration.</param>
    /// <param name="scheduler">Optional scheduler for managing asynchronous operations.</param>
    protected ListViewModelBase(
        IListDataProvider<T> dataProvider,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IScheduler? scheduler = null)
    {
        ArgumentNullException.ThrowIfNull(dataProvider);

        var scheduler1 = scheduler ?? Scheduler.Default;
        DataProvider = dataProvider;
        _pipelineRefreshDeferrer = new(RefreshPipeline);

        Source = DataProvider.Source;
        FilteredItems = DataProvider.FilteredItems;
        Items = DataProvider.Items;
        Groups = new(_groups);

        Filters = filters;
        Sorting = sorting;
        Grouping = grouping;
        Paging = paging;

        CurrentFilter = filters?.CurrentFilter;
        CurrentSorting = sorting?.CurrentSorting ?? [];
        CurrentGrouping = grouping?.CurrentGrouping ?? [];

        RefreshPipeline();
        SubscribeToConfigurationEvents();

        Disposables.Add(
            DataProvider.ConnectGroups()
                .ObserveOn(scheduler1)
                .Subscribe(groups =>
                {
                    _groups.Clear();
                    foreach (var g in groups)
                        _groups.Add(new Group<T>(g.Key, [.. g.Items]));
                }));

        if (Paging is not null)
        {
            Disposables.Add(
                DataProvider.ConnectFiltered()
                    .ObserveOn(scheduler1)
                    .Subscribe(_ => SyncPagingMetadata()));
        }
    }

    /// <summary>
    /// Gets the underlying list data provider.
    /// </summary>
    protected IListDataProvider<T> DataProvider { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> Source { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> FilteredItems { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> Items { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<IGroup<T>>? Groups { get; }

    /// <inheritdoc />
    public int TotalCount => Source.Count;

    /// <inheritdoc />
    public int FilteredCount => DataProvider.FilteredCount;

    /// <inheritdoc />
    public IFiltersViewModel<T>? Filters { get; }

    /// <inheritdoc />
    public IFilter<T>? CurrentFilter { get; private set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public ISortingViewModel<T>? Sorting { get; }

    /// <inheritdoc />
    public IReadOnlyList<ISortingProperty<T>> CurrentSorting { get; private set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public IGroupingViewModel<T>? Grouping { get; }

    /// <inheritdoc />
    public IReadOnlyList<IGroupingProperty<T>> CurrentGrouping { get; private set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public IPagingViewModel? Paging { get; }

    /// <summary>
    /// Creates a deferral scope that suspends pipeline refreshes until the scope is disposed.
    /// Multiple configuration changes (filter, sorting, grouping) occurring within a
    /// single scope will trigger exactly ONE pipeline refresh when the scope ends.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> scope; dispose it to flush the deferred refresh.</returns>
    /// <example>
    /// <code>
    /// using (vm.DeferRefresh())
    /// {
    ///     vm.Filters.Apply(...);
    ///     vm.Sorting.Apply(...);
    ///     vm.Grouping.Apply(...);
    /// } // ← single pipeline refresh here
    /// </code>
    /// </example>
    public IDisposable DeferRefresh() => _pipelineRefreshDeferrer.Defer();

    /// <summary>
    /// Refreshes the collection pipeline by applying the current filter, sorting, grouping, and paging window.
    /// </summary>
    private void RefreshPipeline()
    {
        ApplyFilter();
        ApplySorting();
        ApplyGrouping();
        ApplyPaging();
        SyncPagingMetadata();

        if (Paging is not null)
            ApplyPaging();
    }

    /// <summary>
    /// Requests a pipeline refresh using the same debounce/deferral mechanism as configuration changes.
    /// Derived classes can use this hook when external actions (for example CRUD operations)
    /// require re-applying the pipeline.
    /// </summary>
    protected void RequestPipelineRefresh() => _pipelineRefreshDeferrer.Request();

    /// <summary>
    /// Applies the current filter from the Filters view model to the collection.
    /// When no <see cref="Filters"/> view model is configured, the data provider filter is left unchanged
    /// so derived classes can manage ad-hoc filters through <see cref="IListDataProvider{T}.SetFilter"/>.
    /// </summary>
    private void ApplyFilter()
    {
        if (Filters is null)
            return;

        CurrentFilter = Filters.CurrentFilter;

        if (CurrentFilter is null)
            DataProvider.ClearFilter();
        else
            DataProvider.SetFilter(CurrentFilter);
    }

    /// <summary>
    /// Applies the current sorting from the Sorting view model to the collection.
    /// </summary>
    private void ApplySorting()
    {
        CurrentSorting = Sorting?.CurrentSorting ?? [];

        if (CurrentSorting.Count == 0)
            DataProvider.ClearSorting();
        else
            DataProvider.SetSorting([.. CurrentSorting]);
    }

    /// <summary>
    /// Applies the current grouping from the Grouping view model to the collection.
    /// </summary>
    private void ApplyGrouping()
    {
        CurrentGrouping = Grouping?.CurrentGrouping ?? [];

        if (CurrentGrouping.Count == 0)
            DataProvider.ClearGrouping();
        else
            DataProvider.SetGrouping([.. CurrentGrouping]);
    }

    /// <summary>
    /// Applies the current paging window to the collection.
    /// </summary>
    private void ApplyPaging()
    {
        if (Paging is null)
        {
            DataProvider.ClearPaging();
            return;
        }

        DataProvider.SetPaging(Paging.CurrentPage, Paging.PageSize);
    }

    /// <summary>
    /// Updates paging metadata based on the current filtered count.
    /// </summary>
    private void SyncPagingMetadata()
    {
        if (Paging is null)
            return;

        var previousPage = Paging.CurrentPage;
        var currentPage = Math.Max(1, Paging.CurrentPage);
        Paging.Update(DataProvider.FilteredCount, currentPage);

        if (Paging.CurrentPage != previousPage)
            ApplyPaging();
    }

    /// <summary>
    /// Handles changes in the filtering configuration.
    /// </summary>
    private void HandleFiltersChanged(object? sender, FiltersChangedEventArgs<T> e)
    {
        CurrentFilter = e.Filter;
        _pipelineRefreshDeferrer.Request();
    }

    /// <summary>
    /// Handles changes in the sorting configuration.
    /// </summary>
    private void HandleSortingChanged(object? sender, SortingChangedEventArgs<T> e)
    {
        CurrentSorting = e.Sorting;
        _pipelineRefreshDeferrer.Request();
    }

    /// <summary>
    /// Handles changes in the grouping configuration.
    /// </summary>
    private void HandleGroupingChanged(object? sender, GroupingChangedEventArgs<T> e)
    {
        CurrentGrouping = e.Grouping;
        _pipelineRefreshDeferrer.Request();
    }

    /// <summary>
    /// Handles changes in the paging configuration.
    /// </summary>
    private void HandlePagingChanged(object? sender, PagingChangedEventArgs e) => _pipelineRefreshDeferrer.Request();

    /// <summary>
    /// Subscribes to configuration events.
    /// </summary>
    private void SubscribeToConfigurationEvents()
    {
        Filters?.FiltersChanged += HandleFiltersChanged;
        Sorting?.SortingChanged += HandleSortingChanged;
        Grouping?.GroupingChanged += HandleGroupingChanged;
        Paging?.PagingChanged += HandlePagingChanged;
    }

    /// <summary>
    /// Unsubscribes from configuration events.
    /// </summary>
    private void UnsubscribeFromConfigurationEvents()
    {
        Filters?.FiltersChanged -= HandleFiltersChanged;
        Sorting?.SortingChanged -= HandleSortingChanged;
        Grouping?.GroupingChanged -= HandleGroupingChanged;
        Paging?.PagingChanged -= HandlePagingChanged;
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        UnsubscribeFromConfigurationEvents();
        DataProvider.Dispose();
        base.DisposeManagedResources();
    }
}

/// <summary>
/// List view model base that exposes the underlying <see cref="ExtendedCollection{T}"/> for advanced scenarios (e.g. selection).
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <typeparam name="TCollection">The collection type.</typeparam>
public class ListViewModelBase<T, TCollection> : ListViewModelBase<T>
    where TCollection : ExtendedCollection<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModelBase{T, TCollection}"/> class.
    /// </summary>
    protected ListViewModelBase(TCollection collection, ListViewModelOptions<T>? options)
        : this(collection, options?.Filters, options?.Sorting, options?.Grouping, options?.Paging, options?.Scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModelBase{T, TCollection}"/> class.
    /// </summary>
    protected ListViewModelBase(
        TCollection collection,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IScheduler? scheduler = null)
        : base(new ExtendedCollectionDataProvider<T>(collection), filters, sorting, grouping, paging, scheduler)
        => Collection = collection;

    /// <summary>
    /// Gets the underlying extended collection used by the list pipeline.
    /// </summary>
    [field: SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by the shared data provider in the base class.")]
    protected TCollection Collection { get; }
}
