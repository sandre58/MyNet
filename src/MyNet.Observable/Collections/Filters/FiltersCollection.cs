// -----------------------------------------------------------------------
// <copyright file="FiltersCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MyNet.Utilities.Collections;
using MyNet.Utilities.Comparison;
using MyNet.Utilities.Deferring;
using PropertyChanged;

namespace MyNet.Observable.Collections.Filters;

/// <summary>
/// An optimized observable collection for managing composite filters with efficient batch operations.
/// </summary>
public class FiltersCollection : OptimizedObservableCollection<CompositeFilter>
{
    private readonly Deferrer _filtersChangedDeferrer;

    /// <summary>
    /// Occurs when filters in the collection have changed.
    /// </summary>
    public event EventHandler? FiltersChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersCollection"/> class.
    /// </summary>
    public FiltersCollection()
        => _filtersChangedDeferrer = new Deferrer(OnFiltersChanged);

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersCollection"/> class with initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity to pre-allocate.</param>
    public FiltersCollection(int capacity)
        : base(capacity)
   => _filtersChangedDeferrer = new Deferrer(OnFiltersChanged);

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersCollection"/> class with existing filters.
    /// </summary>
    /// <param name="filters">The initial filters to add.</param>
    public FiltersCollection(IEnumerable<CompositeFilter> filters)
        : base(filters)
        => _filtersChangedDeferrer = new Deferrer(OnFiltersChanged);

    [SuppressPropertyChangedWarnings]
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        _filtersChangedDeferrer.DeferOrExecute();
    }

    /// <summary>
    /// Adds a filter with AND logic.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public FiltersCollection And(IFilter filter)
    {
        Add(new CompositeFilter(filter));
        return this;
    }

    /// <summary>
    /// Adds a filter with OR logic.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public FiltersCollection Or(IFilter filter)
    {
        Add(new CompositeFilter(filter, LogicalOperator.Or));
        return this;
    }

    /// <summary>
    /// Replaces all filters with the specified collection.
    /// Optimized to send a single notification for the entire operation.
    /// </summary>
    /// <param name="filters">The filters to set.</param>
    public void Set(IEnumerable<CompositeFilter> filters)
    {
        // Optimize: Use Load for better performance when replacing all items
        using (_filtersChangedDeferrer.Defer())
        {
            Load(filters);
        }
    }

    /// <summary>
    /// Adds multiple filters at once.
    /// </summary>
    /// <param name="filters">The filters to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public new FiltersCollection AddRange(IEnumerable<CompositeFilter> filters)
    {
        using (_filtersChangedDeferrer.Defer())
            base.AddRange(filters);

        return this;
    }

    /// <summary>
    /// Adds multiple filters with AND logic.
    /// Optimized for batch operations.
    /// </summary>
    /// <param name="filters">The filters to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public FiltersCollection AndRange(IEnumerable<IFilter> filters)
    {
        // Optimize: Convert to list once and use AddRange for better performance
        var compositeFilters = filters.Select(x => new CompositeFilter(x));

        using (_filtersChangedDeferrer.Defer())
            base.AddRange(compositeFilters);

        return this;
    }

    /// <summary>
    /// Adds multiple filters with OR logic.
    /// Optimized for batch operations.
    /// </summary>
    /// <param name="filters">The filters to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public FiltersCollection OrRange(IEnumerable<IFilter> filters)
    {
        // Optimize: Convert to list once and use AddRange for better performance
        var compositeFilters = filters.Select(x => new CompositeFilter(x, LogicalOperator.Or));

        using (_filtersChangedDeferrer.Defer())
            base.AddRange(compositeFilters);

        return this;
    }

    /// <summary>
    /// Removes all filters matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to test filters.</param>
    /// <returns>The number of filters removed.</returns>
    public int RemoveFilters(Func<CompositeFilter, bool> predicate)
    {
        using (_filtersChangedDeferrer.Defer())
            return RemoveAll(predicate);
    }

    [SuppressPropertyChangedWarnings]
    private void OnFiltersChanged() => FiltersChanged?.Invoke(this, EventArgs.Empty);
}
