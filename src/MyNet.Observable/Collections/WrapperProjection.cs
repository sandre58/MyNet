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

namespace MyNet.Observable.Collections;

public sealed class WrapperProjection<T, TWrapper> : ObservableObject
    where T : notnull
    where TWrapper : class, IWrapper<T>
{
    private readonly Func<T, TWrapper> _factory;
    private readonly ReadOnlyObservableCollection<TWrapper> _items;
    private readonly IObservable<IChangeSet<TWrapper>> _connect;

    public ReadOnlyObservableCollection<TWrapper> Items => _items;

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

    private TWrapper CreateWrapper(T item) => _factory(item);

    public IObservable<IChangeSet<TWrapper>> Connect() => _connect;
}
