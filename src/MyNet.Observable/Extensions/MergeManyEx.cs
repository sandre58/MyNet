// -----------------------------------------------------------------------
// <copyright file="MergeManyEx.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

namespace MyNet.Observable.Extensions;

internal sealed class MergeManyEx<T, TDestination>(IObservable<IChangeSet<T>> source,
    Func<T, IObservable<IChangeSet<TDestination>>> observableSelector)
    where T : notnull
    where TDestination : notnull
{
    private readonly IObservable<IChangeSet<T>> _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly Func<T, IObservable<IChangeSet<TDestination>>> _observableSelector = observableSelector ?? throw new ArgumentNullException(nameof(observableSelector));

    public IObservable<IChangeSet<TDestination>> Run() => System.Reactive.Linq.Observable.Create<IChangeSet<TDestination>>(
        observer =>
        {
            var locker = new object();
            return _source

                // SubscribeMany will forward initial cascaded inner items when it observers new item,
                // but if one item was removed, any item belonged to it won't be forwarded to observer.
                .SubscribeMany(t => _observableSelector(t).Synchronize(locker).Subscribe(observer.OnNext))

                // So I add an observer here to subscribe all items belonged to the removed item, buildup a
                // ChangeSet and forward it to the original observer.
                .Subscribe(t =>
                {
                    foreach (var x in t)
                    {
                        switch (x.Reason)
                        {
                            case ListChangeReason.RemoveRange:
                                {
                                    foreach (var item in x.Range) ForwardWhenRemove(observer, item);
                                    break;
                                }

                            case ListChangeReason.Remove:
                                {
                                    ForwardWhenRemove(observer, x.Item.Current);
                                    break;
                                }
                        }
                    }
                },
                observer.OnError);
        });

    private void ForwardWhenRemove(IObserver<IChangeSet<TDestination>> observer, T sourceItem)
    {
        var observableList = _observableSelector(sourceItem).AsObservableList();
        var changeset = new ChangeSet<TDestination>(
        [
            new Change<TDestination>(ListChangeReason.RemoveRange, observableList.Items)
        ]);
        observableList.Dispose();
        observer.OnNext(changeset);
    }
}

internal sealed class MergeManyEx<T, TKey, TDestination, TDestinationKey>(IObservable<IChangeSet<T, TKey>> source,
    Func<T, IObservable<IChangeSet<TDestination, TDestinationKey>>> observableSelector,
    Func<TDestination, TDestinationKey> observableKeySelector)
    where T : notnull
    where TDestination : notnull
    where TKey : notnull
    where TDestinationKey : notnull
{
    private readonly IObservable<IChangeSet<T, TKey>> _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly Func<T, IObservable<IChangeSet<TDestination, TDestinationKey>>> _observableSelector = observableSelector ?? throw new ArgumentNullException(nameof(observableSelector));
    private readonly Func<TDestination, TDestinationKey> _observableKeySelector = observableKeySelector ?? throw new ArgumentNullException(nameof(observableKeySelector));

    public IObservable<IChangeSet<TDestination, TDestinationKey>> Run() => System.Reactive.Linq.Observable.Create<IChangeSet<TDestination, TDestinationKey>>(
        observer =>
        {
            var locker = new object();
            return _source

                // SubscribeMany will forward initial cascaded inner items when it observers new item,
                // but if one item was removed, any item belonged to it won't be forwarded to observer.
                .SubscribeMany(t => _observableSelector(t).Synchronize(locker).Subscribe(observer.OnNext))

                // So I add an observer here to subscribe all items belonged to the removed item, buildup a
                // ChangeSet and forward it to the original observer.
                .Subscribe(t =>
                {
                    foreach (var x in t)
                    {
                        switch (x.Reason)
                        {
                            case ChangeReason.Remove:
                                {
                                    ForwardWhenRemove(observer, x.Current);
                                    break;
                                }
                        }
                    }
                },
                observer.OnError);
        });

    private void ForwardWhenRemove(IObserver<IChangeSet<TDestination, TDestinationKey>> observer, T sourceItem)
    {
        var observableList = _observableSelector(sourceItem).AsObservableCache();
        var changeset = new ChangeSet<TDestination, TDestinationKey>();
        changeset.AddRange(observableList.Items.Select(x => new Change<TDestination, TDestinationKey>(ChangeReason.Remove, _observableKeySelector.Invoke(x), x)));
        observableList.Dispose();
        observer.OnNext(changeset);
    }
}
