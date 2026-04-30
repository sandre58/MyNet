// -----------------------------------------------------------------------
// <copyright file="FilterEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Subjects;

namespace MyNet.Observable.Collections.Filters;

/// <summary>
/// Represents an engine that compiles a collection of filters into a single predicate function. The engine listens for changes in the filters collection and updates the compiled predicate accordingly. It also provides an observable that emits a notification whenever the filters are refreshed. This allows consumers to react to changes in the filter criteria and update their views or data accordingly.
/// </summary>
/// <typeparam name="T">The type of objects to be filtered.</typeparam>
internal sealed class FilterEngine<T> : IDisposable
{
    private readonly BehaviorSubject<Func<T, bool>> _predicate = new(static _ => true);

    /// <summary>
    /// Gets the current root filter node that represents the combined logic of all filters in the collection. This property is updated whenever the filters are refreshed, and it allows consumers to access the current filter configuration directly if needed. The root filter node can be used to inspect the structure of the filters or to perform additional operations based on the current filtering criteria.
    /// </summary>
    public IFilter<T>? Current { get; private set; }

    /// <summary>
    /// Gets an observable that emits the compiled predicate function whenever the filters are updated. Subscribers can use this observable to receive the latest predicate function that represents the combined logic of all filters in the collection, allowing them to apply the filtering criteria to their data as needed.
    /// </summary>
    public IObservable<Func<T, bool>> Predicate => _predicate;

    /// <summary>
    /// Updates the filter engine with a new root filter node. This method checks if the new root is different from the current one, and if so, it compiles the new filter node into a predicate function and emits it through the _predicate subject. If the new root is null, it defaults to a predicate that always returns true, effectively removing all filters. This allows the filter engine to dynamically update its filtering logic based on changes in the filter collection.
    /// </summary>
    /// <param name="root">The new root filter node to be used by the filter engine.</param>
    public void Set(IFilter<T> root) => OnNext(root);

    /// <summary>
    /// Clears the filter engine by setting the root filter node to null. This method effectively removes all filters from the engine, resulting in a predicate that always returns true. This allows consumers to reset the filtering criteria and include all items without any restrictions when the filters are cleared.
    /// </summary>
    public void Clear() => OnNext(null);

    /// <summary>
    /// Invalidates the filter engine by re-emitting the current root filter node. This method can be used to trigger a refresh of the filters without changing the actual filter configuration, allowing consumers to react to changes in the filter criteria or to force a re-evaluation of the current filters when necessary.
    /// </summary>
    public void Invalidate() => OnNext(Current, true);

    /// <summary>
    /// Updates the filter engine with a new root filter node. This method checks if the new root is different from the current one, and if so, it compiles the new filter node into a predicate function and emits it through the _predicate subject. If the new root is null, it defaults to a predicate that always returns true, effectively removing all filters. This allows the filter engine to dynamically update its filtering logic based on changes in the filter collection.
    /// </summary>
    /// <param name="root">The new root filter node to be used by the filter engine.</param>
    /// <param name="force">If set to true, the filter engine will re-emit the current root filter node even if it hasn't changed.</param>
    private void OnNext(IFilter<T>? root, bool force = false)
    {
        if (!force && ReferenceEquals(Current, root))
            return;

        Current = root;

        var expr = root?.ProvideExpression() ?? (_ => true);
        var compiled = expr.Compile();

        _predicate.OnNext(compiled);
    }

    /// <summary>
    /// Disposes the filter engine by completing the _predicate subject and releasing any resources associated with it. This method should be called when the filter engine is no longer needed to ensure that all subscribers are properly notified of the completion and that any resources are cleaned up to prevent memory leaks.
    /// </summary>
    public void Dispose() => _predicate.Dispose();
}
