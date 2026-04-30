// -----------------------------------------------------------------------
// <copyright file="SortEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;

namespace MyNet.Observable.Collections.Sorting;

/// <summary>
/// Provides sorting capabilities for a collection of items of type T. It maintains the current sorting properties and exposes an observable comparer that can be used to sort the collection based on the specified properties.
/// </summary>
/// <typeparam name="T">The type of items to be sorted.</typeparam>
internal sealed class SortEngine<T> : IDisposable
{
    private readonly BehaviorSubject<IComparer<T>> _comparer = new(Comparer<T>.Default);
    private readonly Subject<Unit> _resort = new();

    /// <summary>
    /// Gets the current sorting properties applied to the collection. This property holds an array of sorting properties that define how the items in the collection should be sorted. Each sorting property specifies a key selector and a sort direction (ascending or descending). The current sorting properties can be updated using the Set method, which will also update the comparer accordingly. If no sorting properties are set, the default comparer will be used for sorting.
    /// </summary>
    public ISortingProperty<T>[] Current { get; private set; } = [];

    /// <summary>
    /// Gets an observable that emits the current comparer based on the sorting properties. Whenever the sorting properties are updated using the Set method, the comparer is recalculated and emitted through this observable. Subscribers can use this observable to receive the latest comparer that reflects the current sorting configuration, allowing them to sort their collections accordingly. If no sorting properties are set, the default comparer will be emitted.
    /// </summary>
    public IObservable<IComparer<T>> Comparer => _comparer;

    /// <summary>
    /// Gets an observable that emits a notification whenever the sorting engine is invalidated. This observable can be used to trigger a refresh of the sorted collection whenever the sorting properties are updated or when the Invalidate method is called. Subscribers can react to this observable to re-apply the sorting logic to their collections, ensuring that they are always sorted according to the latest configuration.
    /// </summary>
    public IObservable<Unit> Resort => _resort;

    /// <summary>
    /// Updates the sorting engine with a new set of sorting properties. This method takes an enumerable of sorting properties, converts it to an array, and updates the Current property. It then creates a new comparer based on the provided sorting properties and emits it through the _comparer subject. If the provided sorting properties are empty, it defaults to the default comparer for type T. This allows the sort engine to dynamically update its sorting logic based on changes in the sorting configuration.
    /// </summary>
    /// <param name="sorting">The new set of sorting properties to apply.</param>
    public void Set(IEnumerable<ISortingProperty<T>> sorting) => OnNext(sorting);

    /// <summary>
    /// Clears the sorting engine by setting the sorting properties to an empty array. This method effectively removes all sorting properties from the engine, resulting in the default comparer being emitted through the _comparer subject. This allows consumers to reset the sorting configuration and use the default sorting behavior when no specific sorting properties are defined.
    /// </summary>
    public void Clear() => OnNext([]);

    /// <summary>
    /// Invalidates the sorting engine by re-emitting the current sorting properties. This method can be used to trigger a refresh of the sorting without changing the actual sorting configuration, allowing consumers to react to changes in the sorting criteria or to force a re-evaluation of the current sorting properties when necessary.
    /// </summary>
    public void Invalidate() => _resort.OnNext(Unit.Default);

    /// <summary>
    /// Updates the sorting engine with a new set of sorting properties. This method takes an enumerable of sorting properties, converts it to an array, and updates the Current property. It then creates a new comparer based on the provided sorting properties and emits it through the _comparer subject. If the provided sorting properties are empty, it defaults to the default comparer for type T. This allows the sort engine to dynamically update its sorting logic based on changes in the sorting configuration.
    /// </summary>
    /// <param name="sorting">The new set of sorting properties to apply.</param>
    private void OnNext(IEnumerable<ISortingProperty<T>> sorting)
    {
        var newSorting = sorting.ToArray();
        Current = newSorting;

        if (newSorting.Length == 0)
        {
            _comparer.OnNext(Comparer<T>.Default);
            return;
        }

        _comparer.OnNext(new SortingComparer<T>(newSorting));
        _resort.OnNext(Unit.Default);
    }

    /// <summary>
    /// Disposes the sort engine by disposing the underlying comparer subject. This method is called when the sort engine is no longer needed, allowing it to release any resources it holds and clean up any subscriptions to the comparer observable. After calling this method, the sort engine should not be used, as it will no longer emit any comparers or respond to updates in the sorting properties.
    /// </summary>
    public void Dispose()
    {
        _comparer.Dispose();
        _resort.Dispose();
    }
}
