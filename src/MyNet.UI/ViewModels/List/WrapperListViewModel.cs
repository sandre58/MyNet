// -----------------------------------------------------------------------
// <copyright file="WrapperListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Observable.Collections;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Provides a list view model that exposes wrappers for each item.
/// </summary>
/// <typeparam name="T">The source item type.</typeparam>
/// <typeparam name="TWrapper">The wrapper type.</typeparam>
/// <typeparam name="TCollection">The collection type.</typeparam>
public class WrapperListViewModel<T, TWrapper, TCollection> : ListViewModelBase<T, TCollection>, IWrapperListViewModel<T, TWrapper>
    where TCollection : ExtendedWrapperCollection<T, TWrapper>
    where TWrapper : class, IWrapper<T>
    where T : notnull
{
    private readonly ObservableCollection<TWrapper> _wrappers = [];
    private readonly ObservableCollection<IGroup<TWrapper>> _wrapperGroups = [];
    private readonly Dictionary<T, TWrapper> _wrapperByItem = [];
    private readonly Func<T, TWrapper> _wrapperFactory;
    private readonly Lock _wrapperSync = new();
    private readonly IScheduler _scheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T,TWrapper, TCollection}"/> class.
    /// </summary>
    /// <param name="collection">The source items.</param>
    /// <param name="wrapperFactory">The factory used to create wrappers.</param>
    /// <param name="filters">Optional filtering configuration.</param>
    /// <param name="sorting">Optional sorting configuration.</param>
    /// <param name="grouping">Optional grouping configuration.</param>
    /// <param name="paging">Optional paging configuration.</param>
    /// <param name="busyService">Optional busy service.</param>
    /// <param name="scheduler">Optional scheduler for managing asynchronous operations.</param>
    protected WrapperListViewModel(
        TCollection collection,
        Func<T, TWrapper> wrapperFactory,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
        : base(collection, filters, sorting, grouping, paging, busyService)
    {
        _scheduler = scheduler ?? Scheduler.Default;
        _wrapperFactory = wrapperFactory ?? throw new ArgumentNullException(nameof(wrapperFactory));

        Wrappers = new(_wrappers);
        WrapperGroups = new(_wrapperGroups);

        SubscribeToItems();
        RebuildWrappers();
    }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<TWrapper> Wrappers { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<IGroup<TWrapper>>? WrapperGroups { get; }

    /// <inheritdoc />
    public TWrapper? GetWrapper(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _wrapperByItem.GetValueOrDefault(item);
    }

    /// <inheritdoc />
    public TWrapper GetOrCreateWrapper(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_wrapperByItem.TryGetValue(item, out var wrapper))
            return wrapper;

        wrapper = _wrapperFactory(item);
        _wrapperByItem[item] = wrapper;
        return wrapper;
    }

    /// <summary>
    /// Subscribes to collection changes of the source items to keep wrappers in sync. When the source collection changes, it schedules a refresh of the wrappers after a short delay to ensure that the UI remains responsive and that multiple rapid changes are coalesced into a single update.
    /// </summary>
    private void SubscribeToItems()
    {
        if (Items is INotifyCollectionChanged notify)
            notify.CollectionChanged += (_, _) => ScheduleWrapperRefresh();
    }

    /// <summary>
    /// Schedules a refresh of the wrappers after a short delay. This is used to coalesce multiple rapid changes to the source collection into a single update, improving performance and responsiveness of the UI.
    /// </summary>
    private void ScheduleWrapperRefresh() =>
        _ = _scheduler.Schedule(async _ =>
        {
            await Task.Delay(50).ConfigureAwait(false);
            RebuildWrappers();
        });

    /// <summary>
    /// Rebuilds the wrappers and wrapper groups based on the current source items and groups. This method is called when the source collection changes to ensure that the wrappers remain in sync with the underlying items. It first synchronizes the wrappers for all items, then synchronizes the wrapper groups if grouping is active. The method is thread-safe, using a lock to ensure that only one rebuild operation occurs at a time.
    /// </summary>
    private void RebuildWrappers()
    {
        lock (_wrapperSync)
        {
            SyncWrappers();
            SyncWrapperGroups();
        }
    }

    /// <summary>
    /// Synchronizes the wrappers with the current source items. It clears the existing wrappers and creates new wrappers for each item in the source collection using the GetOrCreateWrapper method. This ensures that the wrappers collection accurately reflects the current state of the source items.
    /// </summary>
    private void SyncWrappers()
    {
        _wrappers.Clear();

        foreach (var item in Items)
            _wrappers.Add(GetOrCreateWrapper(item));
    }

    /// <summary>
    /// Synchronizes the wrapper groups with the current source groups. It clears the existing wrapper groups and creates new groups based on the current grouping configuration. For each group in the source collection, it creates a corresponding group of wrappers by mapping each item in the group to its wrapper using the GetOrCreateWrapper method. This ensures that the wrapper groups accurately reflect the current state of the source groups when grouping is active.
    /// </summary>
    private void SyncWrapperGroups()
    {
        _wrapperGroups.Clear();

        if (Groups is null)
            return;

        foreach (var group in Groups)
        {
            var wrappers = group.Items.Select(GetOrCreateWrapper).ToList();
            _wrapperGroups.Add(new Group<TWrapper>(group.Key, wrappers));
        }
    }
}
