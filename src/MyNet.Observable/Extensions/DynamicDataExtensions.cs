// -----------------------------------------------------------------------
// <copyright file="DynamicDataExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using MyNet.Utilities;

namespace MyNet.Observable.Extensions;

public static class DynamicDataExtensions
{
    /// <summary>
    /// Compared to MergeMany, MergeManyEx will forward all items belonged to the outer Observable which removed.
    /// </summary>
    public static IObservable<IChangeSet<TDestination>> MergeMany<T, TDestination>(
        this IObservable<IChangeSet<T>> source,
        Func<T, IObservable<IChangeSet<TDestination>>> observableSelector)
        where T : notnull
        where TDestination : notnull
        => source == null
        ? throw new ArgumentNullException(nameof(source))
        : observableSelector == null
            ? throw new ArgumentNullException(nameof(observableSelector))
            : new MergeManyEx<T, TDestination>(source, observableSelector).Run();

    /// <summary>
    /// Compared to MergeMany, MergeManyEx will forward all items belonged to the outer Observable which removed.
    /// </summary>
    public static IObservable<IChangeSet<TDestination, TDestinationKey>> MergeMany<T, TKey, TDestination, TDestinationKey>(
        this IObservable<IChangeSet<T, TKey>> source,
        Func<T, IObservable<IChangeSet<TDestination, TDestinationKey>>> observableSelector,
        Func<TDestination, TDestinationKey> observableKeySelector)
        where T : notnull
        where TDestination : notnull
        where TKey : notnull
        where TDestinationKey : notnull
        => source == null
            ? throw new ArgumentNullException(nameof(source))
            : observableSelector == null
                ? throw new ArgumentNullException(nameof(observableSelector))
                : new MergeManyEx<T, TKey, TDestination, TDestinationKey>(source, observableSelector, observableKeySelector).Run();

    public static void Set<TObject, TKey>(this ISourceCache<TObject, TKey> source, IEnumerable<TObject> items)
        where TObject : notnull
        where TKey : notnull
        => source.Edit(x =>
    {
        x.Clear();
        x.AddOrUpdate(items);
    });

    public static void RemoveMany<T>(this ICollection<T> source, IEnumerable<T> itemsToRemove)
    {
        ArgumentNullException.ThrowIfNull(source);

        ArgumentNullException.ThrowIfNull(itemsToRemove);

        var toRemoveArray = itemsToRemove.ToList();

        // match all indices and remove in reverse as it is more efficient
        var toRemove = source.IndexOfMany(toRemoveArray)
            .OrderByDescending(x => x.Index)
            .ToList();

        // if there are duplicates, it could be that an item exists in the
        // source collection more than once - in that case the fast remove
        // would remove each instance
        var hasDuplicates = toRemove.Duplicates(t => t.Item).Any();

        if (!hasDuplicates && source is IList list)
        {
            // Fast remove because we know the index of all, and we remove in order
            toRemove.ForEach(t => list.RemoveAt(t.Index));
        }
        else
        {
            // Slow remove but safe
            toRemoveArray.ForEach(t => source.Remove(t));
        }
    }

    public static IEnumerable<T> GetAddedItems<T>(this IChangeSet<T> changes)
        where T : notnull
        => changes.Where(y => y.Reason == ListChangeReason.Add).Select(z => z.Item.Current).Concat(changes.Where(y => y.Reason == ListChangeReason.AddRange).SelectMany(z => z.Range));

    public static IEnumerable<T> GetRemovedItems<T>(this IChangeSet<T> changes)
        where T : notnull
        => changes.Where(y => y.Reason == ListChangeReason.Remove).Select(z => z.Item.Current).Concat(changes.Where(y => y.Reason == ListChangeReason.RemoveRange).SelectMany(z => z.Range));

    public static IDisposable SubscribeAll<T>(this IObservable<IChangeSet<T>> source, Action action)
        where T : INotifyPropertyChanged
        => source.SubscribeMany(x => x.WhenAnyPropertyChanged().Subscribe(_ => action()))
            .Subscribe(_ => action());

    public static IDisposable SubscribeAll<T, TKey>(this IObservable<IChangeSet<T, TKey>> source, Action action)
        where T : INotifyPropertyChanged
        where TKey : notnull
        => source.SubscribeMany(x => x.WhenAnyPropertyChanged().Subscribe(_ => action()))
            .Subscribe(_ => action());

    public static IObservable<TObject?> WhenExceptPropertyChanged<TObject>(this TObject source, params string[] propertiesToExclude)
        where TObject : INotifyPropertyChanged => source is null
        ? throw new ArgumentNullException(nameof(source))
        : System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler?, PropertyChangedEventArgs>(handler => source.PropertyChanged += handler, handler => source.PropertyChanged -= handler).Where(x => !propertiesToExclude.Contains(x.EventArgs.PropertyName.OrEmpty())).Select(_ => source);

    public static IObservable<IChangeSet<T, TKey>> BindItems<T, TKey>(this IObservable<IChangeSet<T, TKey>> observable, ObservableCollection<T> source)
        where TKey : notnull
        where T : notnull
        => observable
            .ForEachChange(change =>
            {
                switch (change.Reason)
                {
                    case ChangeReason.Add:
                        if (change.CurrentIndex >= 0)
                            source.Insert(change.CurrentIndex, change.Current);
                        else
                            source.Add(change.Current);
                        break;
                    case ChangeReason.Update:
                        source.RemoveAt(change.PreviousIndex);
                        source.Insert(change.CurrentIndex, change.Current);
                        break;
                    case ChangeReason.Remove:
                        _ = source.Remove(change.Current);
                        break;
                    case ChangeReason.Moved:
                        source.Move(change.PreviousIndex, change.CurrentIndex);
                        break;
                }
            });

    public static IObservable<TSource> ObserveOnOptional<TSource>(this IObservable<TSource> source, IScheduler? scheduler)
        => source == null ? throw new ArgumentNullException(nameof(source)) : scheduler is null ? source : source.ObserveOn(scheduler);
}
