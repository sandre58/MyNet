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
using MyNet.Observable;
using MyNet.Observable.Collections.Wrappers;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.List.Wrappers;

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
    private readonly ObservableCollection<IGroup<TWrapper>> _wrapperGroups = [];

    /// <summary>
    /// Determines if a wrapper group matches a source group (same key and same items in the same order).
    /// </summary>
    /// <param name="wrapperGroup">The wrapper group.</param>
    /// <param name="sourceGroup">The source group.</param>
    /// <returns>True if the groups match; otherwise, false.</returns>
    private static bool GroupContentMatches(IGroup<TWrapper> wrapperGroup, IGroup<T> sourceGroup) =>
        Equals(wrapperGroup.Key, sourceGroup.Key) && (wrapperGroup.Items.Count == sourceGroup.Items.Count && !sourceGroup.Items.Where((t, i) => !EqualityComparer<T>.Default.Equals(wrapperGroup.Items[i].Item, t)).Any());

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T,TWrapper, TCollection}"/> class.
    /// </summary>
    /// <param name="collection">The source items.</param>
    /// <param name="filters">Optional filtering configuration.</param>
    /// <param name="sorting">Optional sorting configuration.</param>
    /// <param name="grouping">Optional grouping configuration.</param>
    /// <param name="paging">Optional paging configuration.</param>
    /// <param name="busyService">Optional busy service.</param>
    /// <param name="scheduler">Optional scheduler for managing asynchronous operations.</param>
    public WrapperListViewModel(
        TCollection collection,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
        : base(collection, filters, sorting, grouping, paging, busyService, scheduler)
    {
        Wrappers = collection.Wrappers;
        WrapperGroups = new(_wrapperGroups);

        SubscribeToCollectionChanges();
        RebuildWrapperGroups();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T,TWrapper, TCollection}"/> class.
    /// </summary>
    /// <param name="collection">The source items.</param>
    /// <param name="options">Optional list view model options.</param>
    public WrapperListViewModel(
        TCollection collection,
        ListViewModelOptions<T>? options)
        : this(collection, options?.Filters, options?.Sorting, options?.Grouping, options?.Paging, options?.BusyService, options?.Scheduler)
    {
    }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<TWrapper> Wrappers { get; }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<IGroup<TWrapper>>? WrapperGroups { get; }

    /// <summary>
    /// Gets a wrapper for a given item (delegated to the collection).
    /// </summary>
    public TWrapper GetWrapper(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return Collection.GetOrCreate(item);
    }

    /// <summary>
    /// Subscribes to collection changes to keep grouping in sync.
    /// </summary>
    private void SubscribeToCollectionChanges()
    {
        if (Wrappers is INotifyCollectionChanged wrappersNotify)
        {
            Disposables.Add(
                System.Reactive.Linq.Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => wrappersNotify.CollectionChanged += h,
                        h => wrappersNotify.CollectionChanged -= h)
                    .Subscribe(_ => RebuildWrapperGroups()));
        }

        if (Groups is INotifyCollectionChanged groupsNotify)
        {
            Disposables.Add(
                System.Reactive.Linq.Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => groupsNotify.CollectionChanged += h,
                        h => groupsNotify.CollectionChanged -= h)
                    .Subscribe(_ => RebuildWrapperGroups()));
        }
    }

    /// <summary>
    /// Rebuilds wrapper groups based on ListViewModel grouping.
    /// </summary>
    private void RebuildWrapperGroups()
    {
        if (Groups is null)
        {
            if (_wrapperGroups.Count > 0)
                _wrapperGroups.Clear();

            return;
        }

        for (var i = 0; i < Groups.Count; i++)
        {
            var sourceGroup = Groups[i];

            if (i < _wrapperGroups.Count && GroupContentMatches(_wrapperGroups[i], sourceGroup))
                continue;

            var wrappers = sourceGroup.Items
                .Select(Collection.GetOrCreate)
                .ToList();

            var updatedGroup = new Group<TWrapper>(sourceGroup.Key, wrappers);

            if (i < _wrapperGroups.Count)
                _wrapperGroups[i] = updatedGroup;
            else
                _wrapperGroups.Add(updatedGroup);
        }

        while (_wrapperGroups.Count > Groups.Count)
            _wrapperGroups.RemoveAt(_wrapperGroups.Count - 1);
    }
}

/// <summary>
/// Convenience wrapper list view model using <see cref="ExtendedWrapperCollection{T, TWrapper}"/> as backing collection.
/// </summary>
/// <typeparam name="T">The source item type.</typeparam>
/// <typeparam name="TWrapper">The wrapper type.</typeparam>
public class WrapperListViewModel<T, TWrapper> : WrapperListViewModel<T, TWrapper, ExtendedWrapperCollection<T, TWrapper>>
    where T : notnull
    where TWrapper : class, IWrapper<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper}"/> class.
    /// </summary>
    public WrapperListViewModel(
        ExtendedWrapperCollection<T, TWrapper> collection,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
        : base(collection, filters, sorting, grouping, paging, busyService, scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper}"/> class.
    /// </summary>
    public WrapperListViewModel(
        ExtendedWrapperCollection<T, TWrapper> collection,
        ListViewModelOptions<T>? options)
        : base(collection, options)
    {
    }
}
