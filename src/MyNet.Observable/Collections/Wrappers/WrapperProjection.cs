// -----------------------------------------------------------------------
// <copyright file="WrapperProjection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using MyNet.Observable.Extensions;
using MyNet.Utilities;

namespace MyNet.Observable.Collections.Wrappers;

/// <summary>
/// Represents a wrapper projection that transforms a source observable of change sets into a read-only observable collection of wrapper instances. The wrapper projection creates wrapper instances for each item in the source collection using a provided factory function, and exposes the resulting collection as an observable that emits updates whenever the source collection changes. This allows for efficient management of wrapper instances while keeping the underlying data synchronized with the source collection.
/// </summary>
/// <typeparam name="T">The type of items in the source collection.</typeparam>
/// <typeparam name="TWrapper">The type of wrapper instances.</typeparam>
public sealed class WrapperProjection<T, TWrapper> : ObservableObject
    where T : notnull
    where TWrapper : class, IWrapper<T>
{
    private readonly Func<T, TWrapper> _factory;
    private readonly ReadOnlyObservableCollection<TWrapper> _items;
    private readonly IObservable<IChangeSet<TWrapper>> _connect;

    /// <summary>
    /// Gets the read-only observable collection of wrapper instances that represents the current state of the wrapper projection. This collection is updated in real time as changes occur in the source collection, allowing subscribers to react to additions, removals, and modifications of wrapper instances as they happen.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> Items => _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperProjection{T, TWrapper}"/> class with the specified source observable of change sets, factory function for creating wrapper instances, and optional scheduler for observing changes. The constructor sets up the necessary transformations and subscriptions to ensure that the wrapper projection remains synchronized with the source collection and emits updates whenever changes occur.
    /// </summary>
    /// <param name="source">The source observable of change sets.</param>
    /// <param name="factory">The factory function for creating wrapper instances.</param>
    /// <param name="scheduler">The optional scheduler for observing changes.</param>
    public WrapperProjection(IObservable<IChangeSet<T>> source, Func<T, TWrapper> factory, IScheduler? scheduler = null)
    {
        _factory = factory;

        var shared = source.Publish().RefCount();

        _connect = shared
            .Transform(CreateWrapper)
            .DisposeMany();

        Disposables.Add(_connect
            .ObserveOnOptional(scheduler)
            .Bind(out _items)
            .Subscribe());
    }

    /// <summary>
    /// Creates a wrapper instance for the given item using the factory function. This method is called for each item in the source collection to create the corresponding wrapper instance that will be included in the wrapper projection. The factory function is responsible for defining how the wrapper instances are created based on the source items, allowing for customization of the wrapper projection as needed.
    /// </summary>
    /// <param name="item">The source item for which to create a wrapper.</param>
    /// <returns>The wrapper instance created for the specified source item.</returns>
    private TWrapper CreateWrapper(T item) => _factory(item);

    /// <summary>
    /// Returns an observable stream of wrapper changes for current items.
    /// </summary>
    /// <returns>An observable stream of wrapper changes for current items.</returns>
    public IObservable<IChangeSet<TWrapper>> Connect() => _connect;
}
