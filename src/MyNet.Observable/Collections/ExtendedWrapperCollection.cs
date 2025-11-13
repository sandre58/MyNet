// -----------------------------------------------------------------------
// <copyright file="ExtendedWrapperCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using DynamicData;
using DynamicData.Binding;
using MyNet.Observable.Collections.Providers;
using MyNet.Observable.Extensions;
using MyNet.Utilities;
using MyNet.Utilities.Providers;

namespace MyNet.Observable.Collections;

public class ExtendedWrapperCollection<T, TWrapper> : ExtendedCollection<T>
    where TWrapper : IWrapper<T>
    where T : notnull
{
    // Optimize: Cache compiled expression tree for better performance
#if NET9_0_OR_GREATER
    private static readonly System.Threading.Lock FactoryLock = new();
#else
    private static readonly object FactoryLock = new();
#endif
    private static Func<T, TWrapper>? _cachedWrapperFactory;

    private readonly Func<T, TWrapper> _createWrapper;
    private readonly ReadOnlyObservableCollection<TWrapper> _wrappersSource;
    private readonly ReadOnlyObservableCollection<TWrapper> _wrappers;
    private readonly IObservable<IChangeSet<TWrapper>> _observableWrapperSource;
    private readonly IObservable<IChangeSet<TWrapper>> _observableWrappers;
    private readonly Dictionary<T, TWrapper> _cache;
#if NET9_0_OR_GREATER
    private readonly System.Threading.Lock _cacheLock = new();
#else
    private readonly object _cacheLock = new();
#endif

    public ReadOnlyObservableCollection<TWrapper> Wrappers => _wrappers;

    public ReadOnlyObservableCollection<TWrapper> WrappersSource => _wrappersSource;

    public ExtendedWrapperCollection(ICollection<T> source, IScheduler? scheduler = null, Func<T, TWrapper>? createWrapper = null)
        : this(new SourceList<T>(), source.IsReadOnly, scheduler, createWrapper) => AddRange(source);

    public ExtendedWrapperCollection(IItemsProvider<T> source, bool loadItems = true, IScheduler? scheduler = null, Func<T, TWrapper>? createWrapper = null)
        : this(new ItemsSourceProvider<T>(source, loadItems), scheduler, createWrapper) { }

    public ExtendedWrapperCollection(ISourceProvider<T> source, IScheduler? scheduler = null, Func<T, TWrapper>? createWrapper = null)
        : this(source.Connect(), scheduler, createWrapper) { }

    public ExtendedWrapperCollection(IObservable<IChangeSet<T>> source, IScheduler? scheduler = null, Func<T, TWrapper>? createWrapper = null)
   : this(new SourceList<T>(source), true, scheduler, createWrapper) { }

    public ExtendedWrapperCollection(IScheduler? scheduler = null, Func<T, TWrapper>? createWrapper = null)
          : this(new SourceList<T>(), false, scheduler, createWrapper) { }

    protected ExtendedWrapperCollection(
        SourceList<T> sourceList,
        bool isReadOnly,
        IScheduler? scheduler = null,
        Func<T, TWrapper>? createWrapper = null)
        : base(sourceList, isReadOnly, scheduler)
    {
        // Optimize: Pre-allocate cache with reasonable initial capacity
        _cache = new Dictionary<T, TWrapper>(capacity: 16);
        _createWrapper = createWrapper ?? CreateWrapperFactory();

        var observable = ConnectSortedSource();

        // Optimize: Use single Transform call with proper disposal
        var transformedObservable = observable.Transform(GetOrCreate)
           .ObserveOnOptional(scheduler)
          .DisposeMany();

        Disposables.AddRange(
   [
            transformedObservable.Bind(out _wrappersSource)
   .Subscribe(),
            observable.OnItemRemoved(RemoveFromCache).Subscribe(),
            ConnectSortedAndFilteredSource().Transform(GetOrCreate)
    .ObserveOnOptional(scheduler)
       .Bind(out _wrappers)
                  .Subscribe()
        ]);

        _observableWrappers = _wrappers.ToObservableChangeSet();
        _observableWrapperSource = _wrappersSource.ToObservableChangeSet();

        Refresh();
    }

    // Optimize: Thread-safe cache access with lock-free read path for better concurrency
    protected TWrapper GetOrCreate(T item)
    {
        // Fast path: try to get without lock first
        if (_cache.TryGetValue(item, out var wrapper))
            return wrapper;

        // Slow path: create new wrapper with lock
        lock (_cacheLock)
        {
            // Double-check pattern to avoid race conditions
            if (_cache.TryGetValue(item, out wrapper))
                return wrapper;

            wrapper = _createWrapper(item);
            _cache[item] = wrapper;

            return wrapper;
        }
    }

    // Optimize: Extract cache removal to separate method for clarity and reusability
    private void RemoveFromCache(T item)
    {
        lock (_cacheLock)
        {
            _cache.Remove(item);
        }
    }

    public IObservable<IChangeSet<TWrapper>> ConnectWrappers() => _observableWrappers;

    public IObservable<IChangeSet<TWrapper>> ConnectWrappersSource() => _observableWrapperSource;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "Double-check pattern")]
    private static Func<T, TWrapper> CreateWrapperFactory()
    {
        // Check if we already have a cached factory
        if (_cachedWrapperFactory is not null)
            return _cachedWrapperFactory;

        lock (FactoryLock)
        {
            // Double-check pattern
            if (_cachedWrapperFactory is not null)
                return _cachedWrapperFactory;

            var ctor = typeof(TWrapper).GetConstructor([typeof(T)])
             ?? throw new InvalidOperationException($"Type {typeof(TWrapper).Name} must have a constructor with a single parameter of type {typeof(T).Name}.");

            var param = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            var body = System.Linq.Expressions.Expression.New(ctor, param);
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, TWrapper>>(body, param);

            _cachedWrapperFactory = lambda.Compile();
            return _cachedWrapperFactory;
        }
    }

    protected override void Cleanup()
    {
        lock (_cacheLock)
        {
            // Optimize: Dispose wrappers that implement IDisposable
            foreach (var wrapper in _cache.Values)
            {
                if (wrapper is IDisposable disposable)
                    disposable.Dispose();
            }

            _cache.Clear();
        }

        base.Cleanup();
    }
}
