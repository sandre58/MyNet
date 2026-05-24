// -----------------------------------------------------------------------
// <copyright file="DynamicDataExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DynamicDataExtensions
{
    /// <summary>
    /// Like DynamicData <c>MergeMany</c>, but forwards a remove change set for inner items when an outer item is removed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <paramref name="observableSelector"/> is invoked again when an outer item is removed to build the forwarded
    /// remove change set. For the same outer item it must return an observable that reflects the same inner items
    /// (no side effects, no new disposable resources that leak).
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Ex distinguishes this operator from DynamicData MergeMany.")]
    public static IObservable<IChangeSet<TDestination>> MergeManyEx<T, TDestination>(
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
    /// Keyed variant of <see cref="MergeManyEx{T, TDestination}"/>.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="MergeManyEx{T, TDestination}(IObservable{IChangeSet{T}}, Func{T, IObservable{IChangeSet{TDestination}}})"/>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Ex distinguishes this operator from DynamicData MergeMany.")]
    public static IObservable<IChangeSet<TDestination, TDestinationKey>> MergeManyEx<T, TKey, TDestination, TDestinationKey>(
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

    extension<T>(IChangeSet<T> changes)
        where T : notnull
    {
        /// <summary>Gets items added in this change set (single add and add range).</summary>
        public IEnumerable<T> GetAddedItems() => changes.Where(y => y.Reason == ListChangeReason.Add).Select(z => z.Item.Current).Concat(changes.Where(y => y.Reason == ListChangeReason.AddRange).SelectMany(z => z.Range));

        /// <summary>Gets items removed in this change set (single remove and remove range).</summary>
        public IEnumerable<T> GetRemovedItems() => changes.Where(y => y.Reason == ListChangeReason.Remove).Select(z => z.Item.Current).Concat(changes.Where(y => y.Reason == ListChangeReason.RemoveRange).SelectMany(z => z.Range));
    }

    extension<T, TKey>(IChangeSet<T, TKey> changes)
        where T : notnull
        where TKey : notnull
    {
        /// <summary>Gets outer items removed in this keyed change set (each <see cref="ChangeReason.Remove"/> entry).</summary>
        public IEnumerable<T> GetRemovedItems() => changes.Where(c => c.Reason == ChangeReason.Remove).Select(c => c.Current);
    }

    /// <summary>
    /// Invokes <paramref name="action"/> when the change set updates and when any item raises
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/>.
    /// </summary>
    /// <remarks>
    /// <paramref name="action"/> runs once immediately when the outer subscription receives a change set, and again
    /// for each item property change. After the initial bind, expect at least one call per batch of changes.
    /// </remarks>
    public static IDisposable SubscribeAll<T>(this IObservable<IChangeSet<T>> source, Action action)
        where T : INotifyPropertyChanged
        => source.SubscribeMany(x => x.WhenAnyPropertyChanged().Subscribe(_ => action()))
            .Subscribe(_ => action());

    /// <inheritdoc cref="SubscribeAll{T}(IObservable{IChangeSet{T}}, Action)"/>
    public static IDisposable SubscribeAll<T, TKey>(this IObservable<IChangeSet<T, TKey>> source, Action action)
        where T : INotifyPropertyChanged
        where TKey : notnull
        => source.SubscribeMany(x => x.WhenAnyPropertyChanged().Subscribe(_ => action()))
            .Subscribe(_ => action());

    /// <summary>
    /// Like ObserveOn(IScheduler), but if <paramref name="scheduler"/> is null, returns the source observable without observing on any scheduler..
    /// </summary>
    /// <param name="source">The source observable sequence.</param>
    /// <param name="scheduler">The scheduler to observe on, or null to observe on the current thread.</param>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <returns>An observable sequence that observes on the specified scheduler, or the source sequence if the scheduler is null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
    public static IObservable<TSource> ObserveOnOptional<TSource>(this IObservable<TSource> source, IScheduler? scheduler) => source == null ? throw new ArgumentNullException(nameof(source)) : scheduler is null ? source : source.ObserveOn(scheduler);
}
