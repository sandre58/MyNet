// -----------------------------------------------------------------------
// <copyright file="ExtendedCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using MyNet.Observable.Collections.Extensions;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Providers;
using MyNet.Observable.Collections.Sorting;
using MyNet.Observable.Extensions;
using MyNet.Utilities;
using MyNet.Utilities.Deferring;
using MyNet.Utilities.Providers;

namespace MyNet.Observable.Collections;

[System.Runtime.InteropServices.ComVisible(false)]
[DebuggerDisplay("Count = {Count}/{SourceCount}")]
public class ExtendedCollection<T> : ObservableObject, ICollection<T>, IReadOnlyList<T>, INotifyCollectionChanged
    where T : notnull
{
    private readonly Deferrer _applyFilterDeferrer;
    private readonly Deferrer _applySortDeferrer;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup method")]
    private readonly Subject<IList<CompositeFilter>> _filterSubject = new();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup method")]
    private readonly Subject<Unit> _resortSubject = new();
    private readonly IObservable<IChangeSet<T>> _observableSource;
    private readonly IObservable<IChangeSet<T>> _observable;
    private readonly ReadOnlyObservableCollection<T> _items;
    private readonly ReadOnlyObservableCollection<T> _sortedSource;
    private readonly SourceList<T> _source;
    private readonly SortingComparer<T> _sortComparer;
    private ImmutableHashSet<string> _filterProperties = [];
    private ImmutableHashSet<string> _sortProperties = [];
    private Func<T, bool>? _cachedFilterFunc;
    private IList<CompositeFilter>? _lastFiltersList;

    public FiltersCollection Filters { get; } = [];

    public SortingPropertiesCollection SortingProperties { get; } = [];

    public ReadOnlyObservableCollection<T> Source => _sortedSource;

    public ReadOnlyObservableCollection<T> Items => _items;

    public int SourceCount => _source.Count;

    public ExtendedCollection(IScheduler? scheduler = null)
        : this(new SourceList<T>(), false, scheduler) { }

    public ExtendedCollection(ICollection<T> source, IScheduler? scheduler = null)
        : this(new SourceList<T>(), source.IsReadOnly, scheduler)
        => AddRange(source);

    public ExtendedCollection(IItemsProvider<T> source, bool loadItems = true, IScheduler? scheduler = null)
        : this(new ItemsSourceProvider<T>(source, loadItems), scheduler) { }

    public ExtendedCollection(ISourceProvider<T> source, IScheduler? scheduler = null)
        : this(source.Connect(), scheduler) { }

    public ExtendedCollection(IObservable<IChangeSet<T>> source, IScheduler? scheduler = null)
      : this(new SourceList<T>(source), true, scheduler) { }

    protected ExtendedCollection(SourceList<T> sourceList, bool isReadOnly, IScheduler? scheduler = null)
    {
        _source = sourceList;
        IsReadOnly = isReadOnly;
        _applyFilterDeferrer = new Deferrer(InvalidateFilterCache);
        _applySortDeferrer = new Deferrer(() => _resortSubject.OnNext(Unit.Default));
        _observableSource = _source.Connect();
        _sortComparer = new SortingComparer<T>(SortingProperties);

        // Optimize: Combine filter and sort change events into single observable
        var filterChanges = System.Reactive.Linq.Observable.FromEventPattern(
            x => Filters.FiltersChanged += x,
            x => Filters.FiltersChanged -= x).Do(_ => UpdateWatchedProperties());

        var sortChanges = System.Reactive.Linq.Observable.FromEventPattern(
            x => SortingProperties.SortChanged += x,
            x => SortingProperties.SortChanged -= x).Do(_ => UpdateWatchedProperties());

        Disposables.AddRange(
        [
            filterChanges.Subscribe(_ => _applyFilterDeferrer.DeferOrExecute()),
            sortChanges.Subscribe(_ => _applySortDeferrer.DeferOrExecute()),

            ConnectSortedSource().ObserveOnOptional(scheduler).Bind(out _sortedSource).Subscribe(OnSourceRefreshed),
            ConnectSortedAndFilteredSource().ObserveOnOptional(scheduler).Bind(out _items).Subscribe(OnItemsRefreshed),

            // Optimize: Use SubscribeMany with property change observation
            _observableSource.SubscribeMany(SubscribeToItemPropertyChanges).Subscribe(),

            System.Reactive.Linq.Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                x => ((INotifyCollectionChanged)_items).CollectionChanged += x,
                x => ((INotifyCollectionChanged)_items).CollectionChanged -= x).Subscribe(x => HandleCollectionChanged(x.EventArgs)),
            System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => ((INotifyPropertyChanged)_items).PropertyChanged += x,
                x => ((INotifyPropertyChanged)_items).PropertyChanged -= x).Subscribe(x => OnPropertyChanged(x.EventArgs.PropertyName)),

            _filterSubject,
            _resortSubject,
            _source
        ]);

        _observable = _items.ToObservableChangeSet();

        RefreshFilter();
    }

    // Optimize: Extract property change subscription to separate method for better readability and performance
    private IDisposable SubscribeToItemPropertyChanges(T item) => item is not INotifyPropertyChanged notifyPropertyChanged
            ? Disposable.Empty
            : System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => notifyPropertyChanged.PropertyChanged += handler,
                handler => notifyPropertyChanged.PropertyChanged -= handler)
                .Where(y =>
                {
                    var propertyName = y.EventArgs.PropertyName.OrEmpty();
                    return _sortProperties.Contains(propertyName) || _filterProperties.Contains(propertyName);
                })
                .Subscribe(y =>
                {
                    var propertyName = y.EventArgs.PropertyName.OrEmpty();
                    if (_sortProperties.Contains(propertyName))
                        _applySortDeferrer.DeferOrExecute();

                    if (_filterProperties.Contains(propertyName))
                        _applyFilterDeferrer.DeferOrExecute();
                });

    protected IObservable<IChangeSet<T>> ConnectSortedSource() => _observableSource.Sort(_sortComparer, resort: _resortSubject);

    protected IObservable<IChangeSet<T>> ConnectSortedAndFilteredSource() =>
        _observableSource.Filter(_filterSubject.Select(GetOrCreateFilterFunc))
           .Sort(_sortComparer, resort: _resortSubject);

    public IObservable<IChangeSet<T>> Connect() => _observable;

    public IObservable<IChangeSet<T>> ConnectSource() => _observableSource;

    protected virtual void OnItemsRefreshed(IChangeSet<T> changeSet) => OnPropertyChanged(nameof(Count));

    protected virtual void OnSourceRefreshed(IChangeSet<T> changeSet) => OnPropertyChanged(nameof(SourceCount));

    public IDisposable DeferRefresh() => new CompositeDisposable(_applySortDeferrer.Defer(), _applyFilterDeferrer.Defer());

    public IDisposable DeferSort() => _applySortDeferrer.Defer();

    public IDisposable DeferFilter() => _applyFilterDeferrer.Defer();

    public void Refresh()
    {
        RefreshFilter();
        RefreshSorting();
    }

    public void RefreshSorting() => _applySortDeferrer.Execute();

    public void RefreshFilter() => _applyFilterDeferrer.Execute();

    private static Func<T, bool> CreateFilterFunc(IList<CompositeFilter> filters)
    {
        // Optimize: For common cases, avoid closure allocation
        if (filters.Count == 0)
            return static _ => true;

        if (filters.Count == 1)
        {
            var singleFilter = filters[0];
            return x => singleFilter.Filter.IsMatch(x);
        }

        // For multiple filters, use the general case
        return x => filters.Match(x);
    }

    // Optimize: Cache filter function to avoid repeated allocations
    private void InvalidateFilterCache()
    {
        _cachedFilterFunc = null;
        _filterSubject.OnNext([.. Filters]);
    }

    // Optimize: Use cached filter function when filters haven't changed
    private Func<T, bool> GetOrCreateFilterFunc(IList<CompositeFilter> filters)
    {
        // Check if we can reuse the cached filter
        if (_cachedFilterFunc is not null && ReferenceEquals(_lastFiltersList, filters))
            return _cachedFilterFunc;

        _lastFiltersList = filters;
        _cachedFilterFunc = CreateFilterFunc(filters);
        return _cachedFilterFunc;
    }

    // Optimize: Use ImmutableHashSet to avoid repeated allocations and improve lookup performance
    private void UpdateWatchedProperties()
    {
        var filterPropsBuilder = ImmutableHashSet.CreateBuilder<string>();
        var sortPropsBuilder = ImmutableHashSet.CreateBuilder<string>();

        foreach (var filter in Filters)
        {
            var propName = filter.Filter.PropertyName;
            if (!string.IsNullOrEmpty(propName))
                filterPropsBuilder.Add(propName);
        }

        foreach (var sortProp in SortingProperties)
        {
            var propName = sortProp.PropertyName;
            if (!string.IsNullOrEmpty(propName))
                sortPropsBuilder.Add(propName);
        }

        _filterProperties = filterPropsBuilder.ToImmutable();
        _sortProperties = sortPropsBuilder.ToImmutable();
    }

    protected override void Cleanup()
    {
        base.Cleanup();

        _resortSubject.Dispose();
        _filterSubject.Dispose();
        _cachedFilterFunc = null;
        _lastFiltersList = null;
    }

    #region INotifyCollectionChanged

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Use OnCollectionChanged")]
    event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
    {
        add => CollectionChanged += value;
        remove => CollectionChanged -= value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Use OnCollectionChanged")]
    private event NotifyCollectionChangedEventHandler? CollectionChanged;

    private void HandleCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

    #endregion INotifyCollectionChanged

    #region ICollection

    public int Count => _items.Count;

    public bool IsReadOnly { get; }

    public T this[int index] => _items[index];

    public void Add(T item) => IsReadOnly.IfFalse(() => _source.Add(item));

    public void Clear() => IsReadOnly.IfFalse(() => _source.Clear());

    public bool Remove(T item) => !IsReadOnly && _source.Remove(item);

    public int IndexOf(T item) => _items.IndexOf(item);

    public bool Contains(T item) => _items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    #endregion

    #region ICollection Extensions

    public void AddRange(IEnumerable<T> items) => IsReadOnly.IfFalse(() => _source.AddRange(items));

    public void RemoveMany(IEnumerable<T> itemsToRemove) => IsReadOnly.IfFalse(() => _source.RemoveMany(itemsToRemove));

    public void Set(IEnumerable<T> items) => IsReadOnly.IfFalse(() => _source.Edit(x =>
    {
        x.Clear();
        x.AddRange(items);
    }));

    #endregion
}
