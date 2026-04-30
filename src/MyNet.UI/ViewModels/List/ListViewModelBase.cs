// -----------------------------------------------------------------------
// <copyright file="ListViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Collections.Sorting;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Provides a lightweight list pipeline implementation for ViewModels2.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <typeparam name="TCollection">The collection type.</typeparam>
public class ListViewModelBase<T, TCollection> : ViewModelBase, IListViewModel<T>
    where TCollection : ExtendedCollection<T>
    where T : notnull
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed through the shared disposables collection.")]
    private readonly TCollection _collection;

    private readonly ObservableCollection<IGroup<T>> _groups = [];
    private readonly Lock _sync = new();
    private readonly IScheduler _scheduler;
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed through the shared disposables collection.")]
    private CancellationTokenSource? _refreshCts;

    private static string BuildCompositeKey(Func<T, object?>[] keySelectors, T item)
        => string.Join("|", keySelectors.Select(selector => selector(item)?.ToString() ?? "<null>"));

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModelBase{T, TCollection}"/> class.
    /// </summary>
    /// <param name="collection">The initial source items.</param>
    /// <param name="filters">Optional filtering configuration.</param>
    /// <param name="sorting">Optional sorting configuration.</param>
    /// <param name="grouping">Optional grouping configuration.</param>
    /// <param name="paging">Optional paging configuration.</param>
    /// <param name="busyService">Optional busy service.</param>
    /// <param name="scheduler">Optional scheduler for managing asynchronous operations.</param>
    protected ListViewModelBase(
        TCollection collection,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
    : base(busyService)
    {
        _scheduler = scheduler ?? Scheduler.Default;
        _collection = collection;

        Source = _collection.Source;
        Items = _collection.Items;
        Groups = new(_groups);

        Filters = filters;
        Sorting = sorting;
        Grouping = grouping;
        Paging = paging;

        CurrentFilter = filters?.CurrentFilter;
        CurrentSorting = sorting?.CurrentSorting ?? [];
        CurrentGrouping = grouping?.CurrentGrouping ?? [];

        Subscribe();
        ScheduleRefresh();
    }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> Source { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<T> Items { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<IGroup<T>>? Groups { get; }

    /// <inheritdoc />
    public int TotalCount => Source.Count;

    /// <inheritdoc />
    public IFiltersViewModel<T>? Filters { get; }

    /// <inheritdoc />
    public IFilter<T>? CurrentFilter { get; private set; }

    /// <inheritdoc />
    public ISortingViewModel<T>? Sorting { get; }

    /// <inheritdoc />
    public IReadOnlyList<ISortingProperty<T>> CurrentSorting { get; private set; }

    /// <inheritdoc />
    public IGroupingViewModel<T>? Grouping { get; }

    /// <inheritdoc />
    public IReadOnlyList<IGroupingProperty<T>> CurrentGrouping { get; private set; }

    /// <inheritdoc />
    public IPagingViewModel? Paging { get; }

    /// <summary>
    /// Schedules a refresh of the list pipeline by applying the current filter, sorting, grouping, and paging configurations.
    /// </summary>
    private void ScheduleRefresh()
    {
        _refreshCts?.Cancel();
        _refreshCts = new();

        var token = _refreshCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(50, token).ConfigureAwait(false);

                await RebuildPipelineAsync(token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
        },
            token);
    }

    /// <summary>
    /// Rebuilds the list pipeline by applying the current filter, sorting, grouping, and paging configurations to the collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async Task RebuildPipelineAsync(CancellationToken cancellationToken) =>
        await ExecuteAsync(async _ =>
            {
                lock (_sync)
                {
                    ApplyFilter();
                    ApplySorting();
                    ApplyGrouping();
                    UpdatePaging();
                }

                await Task.CompletedTask.ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Handles the FiltersChanged event from the Filters view model. Updates the current filter and schedules a refresh of the list pipeline to apply the new filter configuration.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HandleFiltersChanged(object? sender, FiltersChangedEventArgs<T> e)
    {
        CurrentFilter = e.Filter;
        ScheduleRefresh();
    }

    /// <summary>
    /// Handles the SortingChanged event from the Sorting view model. Updates the current sorting configuration and schedules a refresh of the list pipeline to apply the new sorting configuration.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HandleSortingChanged(object? sender, SortingChangedEventArgs<T> e)
    {
        CurrentSorting = e.Sorting;
        ScheduleRefresh();
    }

    /// <summary>
    /// Handles the GroupingChanged event from the Grouping view model. Updates the current grouping configuration and schedules a refresh of the list pipeline to apply the new grouping configuration.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HandleGroupingChanged(object? sender, GroupingChangedEventArgs<T> e)
    {
        CurrentGrouping = e.Grouping;
        ScheduleRefresh();
    }

    /// <summary>
    /// Handles the PagingChanged event from the Paging view model. Updates the paging configuration and refreshes the paging information based on the current total count and page number.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HandlePagingChanged(object? sender, PagingChangedEventArgs e) => UpdatePaging();

    /// <summary>
    /// Applies the current filter configuration from the Filters view model to the collection. If there is no active filter, it clears any existing filters from the collection. Otherwise, it sets the new filter to the collection to update the filtered items accordingly.
    /// </summary>
    private void ApplyFilter()
    {
        CurrentFilter = Filters?.CurrentFilter;

        if (CurrentFilter is null)
            _collection.ClearFilter();
        else
            _collection.SetFilter(CurrentFilter);
    }

    /// <summary>
    /// Applies the current sorting configuration from the Sorting view model to the collection. If there are no active sorting properties, it clears any existing sorting from the collection. Otherwise, it sets the new sorting properties to the collection to update the sorted items accordingly.
    /// </summary>
    private void ApplySorting()
    {
        CurrentSorting = Sorting?.CurrentSorting ?? [];

        if (CurrentSorting.Count == 0)
            _collection.ClearSorting();
        else
            _collection.SetSorting(CurrentSorting.ToArray());
    }

    /// <summary>
    /// Applies the current grouping configuration from the Grouping view model to the collection. If there are no active grouping properties, it clears any existing groups from the collection. Otherwise, it builds composite keys for each item based on the active grouping properties and creates new groups accordingly to update the grouped items in the collection.
    /// </summary>
    private void ApplyGrouping() =>
        _scheduler.Schedule(() =>
        {
            _groups.Clear();

            CurrentGrouping = Grouping?.CurrentGrouping ?? [];

            if (CurrentGrouping.Count == 0)
                return;

            var keySelectors = CurrentGrouping
                .Select(grouping => grouping.ProvideExpression().Compile())
                .ToArray();

            foreach (var grouping in Items.GroupBy(item => BuildCompositeKey(keySelectors, item)))
                _groups.Add(new Group<T>(grouping.Key, [.. grouping]));
        });

    /// <summary>
    /// Updates the paging information in the Paging view model based on the current total count of items and the current page number. If there is no Paging view model, it does nothing. Otherwise, it ensures that the current page number is valid and updates the paging information accordingly to reflect the new total count and page number.
    /// </summary>
    private void UpdatePaging()
    {
        if (Paging is null)
            return;

        var currentPage = Paging.CurrentPage;
        if (currentPage < 1)
            currentPage = 1;

        Paging.Update(TotalCount, currentPage);
    }

    /// <summary>
    /// Subscribes to the FiltersChanged, SortingChanged, GroupingChanged, and PagingChanged events from the respective view models to handle changes in filter, sorting, grouping, and paging configurations. When any of these events are raised, the corresponding event handler is invoked to update the current configuration and schedule a refresh of the list pipeline to apply the new configuration.
    /// </summary>
    private void Subscribe()
    {
        Filters?.FiltersChanged += HandleFiltersChanged;
        Sorting?.SortingChanged += HandleSortingChanged;
        Grouping?.GroupingChanged += HandleGroupingChanged;
        Paging?.PagingChanged += HandlePagingChanged;
    }

    /// <inheritdoc />
    protected override void Cleanup()
    {
        _collection.Dispose();
        _refreshCts?.Cancel();
        _refreshCts?.Dispose();

        Filters?.FiltersChanged -= HandleFiltersChanged;
        Sorting?.SortingChanged -= HandleSortingChanged;
        Grouping?.GroupingChanged -= HandleGroupingChanged;
        Paging?.PagingChanged -= HandlePagingChanged;

        base.Cleanup();
    }
}
