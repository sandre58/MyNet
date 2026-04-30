// -----------------------------------------------------------------------
// <copyright file="ExtendedCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using DynamicData;
using DynamicData.Binding;

namespace MyNet.Observable.Collections.Extensions;

/// <summary>
/// Provides extension methods for the ExtendedCollection class, allowing it to be converted into an observable change set that can be subscribed to for real-time updates on changes to the collection.
/// </summary>
public static class ExtendedCollectionExtensions
{
    /// <summary>
    /// Converts an ExtendedCollection of type T into an observable change set that emits updates whenever the collection changes. This allows subscribers to react to additions, removals, and modifications of items in the collection in real time.
    /// </summary>
    /// <param name="source">The ExtendedCollection to convert.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <returns>An observable change set that emits updates whenever the collection changes.</returns>
    public static IObservable<IChangeSet<T>> ToObservableChangeSet<T>(this ExtendedCollection<T> source)
        where T : notnull
        => source.ToObservableChangeSet<ExtendedCollection<T>, T>();
}
