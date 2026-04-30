// -----------------------------------------------------------------------
// <copyright file="PipelineEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using DynamicData;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Sorting;

namespace MyNet.Observable.Collections;

/// <summary>
/// Provides a pipeline engine that builds sorted and filtered observables from a source observable of change sets. The pipeline engine applies sorting and filtering operations based on the provided filter and sort engines, allowing for dynamic updates to the sorted and filtered collections as the source changes.
/// </summary>
internal static class PipelineEngine
{
    /// <summary>
    /// Builds sorted and filtered observables from a source observable of change sets, applying sorting and filtering operations based on the provided filter and sort engines. The sorted observable is created by applying the sorting engine to the source, while the filtered observable is created by applying both the filter and sort engines to the source, with automatic refreshes triggered by changes in the filter engine.
    /// </summary>
    /// <param name="source">The source observable of change sets.</param>
    /// <param name="filterEngine">The filter engine to apply filtering operations.</param>
    /// <param name="sortEngine">The sort engine to apply sorting operations.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <returns>A tuple containing the sorted and filtered observables.</returns>
    public static (IObservable<IChangeSet<T>> Sorted, IObservable<IChangeSet<T>> Filtered) Build<T>(
        IObservable<IChangeSet<T>> source,
        FilterEngine<T> filterEngine,
        SortEngine<T> sortEngine)
    where T : notnull
    {
        var sorted = source
            .Sort(sortEngine.Comparer, resort: sortEngine.Resort);

        var filtered = source
            .AutoRefreshOnObservable(_ => filterEngine.Predicate)
            .Filter(filterEngine.Predicate)
            .Sort(sortEngine.Comparer, resort: sortEngine.Resort);

        return (sorted, filtered);
    }
}
