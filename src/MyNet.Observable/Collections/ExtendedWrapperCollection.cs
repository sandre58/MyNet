// -----------------------------------------------------------------------
// <copyright file="ExtendedWrapperCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using DynamicData;
using DynamicData.Binding;
using MyNet.Observable.Collections.Providers;
using MyNet.Observable.Collections.Sources;
using MyNet.Observable.Extensions;
using MyNet.Utilities;
using MyNet.Utilities.Providers;

namespace MyNet.Observable.Collections;

/// <summary>
/// Represents an extended collection exposing wrapper instances for each source item.
/// </summary>
/// <typeparam name="T">The source item type.</typeparam>
/// <typeparam name="TWrapper">The wrapper type.</typeparam>
public class ExtendedWrapperCollection<T, TWrapper> : ExtendedCollection<T>
    where T : notnull
    where TWrapper : class, IWrapper<T>
{
    private readonly Func<T, TWrapper> _createWrapper;
    private readonly Dictionary<T, TWrapper> _cache = new();
    private readonly ObservableCollection<TWrapper> _wrappers = [];
    private readonly ObservableCollection<TWrapper> _wrappersSource = [];

    private readonly IObservable<IChangeSet<TWrapper>> _wrappersObservable;
    private readonly IObservable<IChangeSet<TWrapper>> _wrappersSourceObservable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class with an empty mutable source.
    /// </summary>
    public ExtendedWrapperCollection(IScheduler? scheduler, Func<T, TWrapper> createWrapper)
        : this(SourceEngine<T>.Empty(), scheduler, createWrapper) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class from a static list.
    /// </summary>
    public ExtendedWrapperCollection(ICollection<T> source, IScheduler? scheduler, Func<T, TWrapper> createWrapper)
        : this(SourceEngine<T>.From(source, readOnly: false), scheduler, createWrapper) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class from an items provider.
    /// </summary>
    public ExtendedWrapperCollection(IItemsProvider<T> source, bool loadItems, IScheduler? scheduler, Func<T, TWrapper> createWrapper)
        : this(SourceEngine<T>.From(loadItems ? source.ProvideItems() : [], readOnly: false), scheduler, createWrapper) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class from an observable source provider.
    /// </summary>
    public ExtendedWrapperCollection(ISourceProvider<T> source, IScheduler? scheduler, Func<T, TWrapper> createWrapper)
        : this(SourceEngine<T>.FromObservable(source.Connect()), scheduler, createWrapper) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class from an observable change stream.
    /// </summary>
    public ExtendedWrapperCollection(IObservable<IChangeSet<T>> source, IScheduler? scheduler, Func<T, TWrapper> createWrapper)
        : this(SourceEngine<T>.FromObservable(source), scheduler, createWrapper) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class from an external source list.
    /// </summary>
    public ExtendedWrapperCollection(SourceList<T> sourceList, bool isReadOnly, IScheduler? scheduler, Func<T, TWrapper> createWrapper)
        : this(isReadOnly
            ? SourceEngine<T>.From(sourceList.Items, readOnly: true)
            : SourceEngine<T>.FromObservable(sourceList.Connect()),
            scheduler,
            createWrapper) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedWrapperCollection{T, TWrapper}"/> class.
    /// </summary>
    protected ExtendedWrapperCollection(SourceEngine<T> sourceEngine, IScheduler? scheduler, Func<T, TWrapper> createWrapper)
        : base(sourceEngine, scheduler)
    {
        _createWrapper = createWrapper ?? throw new ArgumentNullException(nameof(createWrapper));

        Wrappers = new(_wrappers);
        WrappersSource = new(_wrappersSource);

        _wrappersObservable = Wrappers.ToObservableChangeSet();
        _wrappersSourceObservable = WrappersSource.ToObservableChangeSet();

        RebuildWrappers();

        Disposables.AddRange([
            Connect().Subscribe(_ => RebuildWrappers()),
            ConnectSource().Subscribe(_ => RebuildSourceWrappers())
        ]);
    }

    /// <summary>
    /// Gets wrappers corresponding to filtered/sorted items.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> Wrappers { get; }

    /// <summary>
    /// Gets wrappers corresponding to source items.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> WrappersSource { get; }

    /// <summary>
    /// Returns an observable stream of wrapper changes for current items.
    /// </summary>
    public IObservable<IChangeSet<TWrapper>> ConnectWrappers() => _wrappersObservable;

    /// <summary>
    /// Returns an observable stream of wrapper changes for source items.
    /// </summary>
    public IObservable<IChangeSet<TWrapper>> ConnectWrappersSource() => _wrappersSourceObservable;

    /// <summary>
    /// Gets the wrapper for an item, creating it if necessary.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The wrapper associated with the specified item.</returns>
    public TWrapper GetOrCreate(T item)
    {
        if (_cache.TryGetValue(item, out var wrapper))
            return wrapper;

        wrapper = _createWrapper(item);
        _cache[item] = wrapper;
        return wrapper;
    }

    private void RebuildWrappers() => _wrappers.Set(Items.Select(GetOrCreate));

    private void RebuildSourceWrappers() => _wrappersSource.Set(Source.Select(GetOrCreate));
}
