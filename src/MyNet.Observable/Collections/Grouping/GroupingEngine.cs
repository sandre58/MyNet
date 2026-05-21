// -----------------------------------------------------------------------
// <copyright file="GroupingEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace MyNet.Observable.Collections.Grouping;

/// <summary>
/// Manages the grouping configuration for a reactive collection pipeline.
/// Exposes an observable that emits the current grouping properties whenever they change,
/// allowing downstream consumers (e.g. <see cref="MyNet.Observable.Collections.ExtendedCollection{T}"/>)
/// to reactively recompute grouped representations.
/// </summary>
/// <typeparam name="T">The type of items to be grouped.</typeparam>
public sealed class GroupingEngine<T> : IDisposable
    where T : notnull
{
    private readonly BehaviorSubject<IGroupingProperty<T>[]> _subject = new([]);

    /// <summary>
    /// Computes a flat list of groups from a snapshot of items and a set of grouping properties.
    /// Groups are identified by a composite key built by concatenating the values of all grouping properties.
    /// </summary>
    /// <param name="items">The items to group.</param>
    /// <param name="groupingProperties">The active grouping properties.</param>
    /// <returns>A read-only list of <see cref="CollectionGroup{T}"/>. Returns an empty list if no grouping properties are provided.</returns>
    public static IReadOnlyList<CollectionGroup<T>> ComputeGroups(IReadOnlyList<T> items, IGroupingProperty<T>[] groupingProperties)
    {
        if (groupingProperties.Length == 0)
            return [];

        var keySelectors = groupingProperties
            .Select(g => g.ProvideExpression().Compile())
            .ToArray();

        return
        [
            .. items
                .GroupBy(buildKey)
                .Select(g => new CollectionGroup<T>(g.Key, [.. g]))
        ];

        string buildKey(T item) => string.Join("|", keySelectors.Select(s => s(item)?.ToString() ?? "<null>"));
    }

    /// <summary>
    /// Gets the currently active grouping properties.
    /// </summary>
    public IGroupingProperty<T>[] Current { get; private set; } = [];

    /// <summary>
    /// Gets an observable that emits the current grouping properties whenever they change.
    /// </summary>
    public IObservable<IGroupingProperty<T>[]> Grouping => _subject;

    /// <summary>
    /// Sets the active grouping properties and notifies subscribers.
    /// </summary>
    /// <param name="grouping">The new set of grouping properties to apply.</param>
    public void Set(IEnumerable<IGroupingProperty<T>> grouping)
    {
        Current = [.. grouping];
        _subject.OnNext(Current);
    }

    /// <summary>
    /// Clears all active grouping properties and notifies subscribers.
    /// </summary>
    public void Clear()
    {
        Current = [];
        _subject.OnNext(Current);
    }

    /// <inheritdoc/>
    public void Dispose() => _subject.Dispose();
}
