// -----------------------------------------------------------------------
// <copyright file="IFiltersViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable.Collections.Filters;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents a view model for managing a collection of filters.
/// Provides methods and events to configure, apply, and reset filters.
/// </summary>
public interface IFiltersViewModel<T>
{
    /// <summary>
    /// Gets the root filter node view model.
    /// Represents the full filter tree configured in the UI.
    /// </summary>
    IFilterGroupViewModel<T> Root { get; }

    /// <summary>
    /// Gets a value indicating whether the current state differs from the last applied filter.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Gets or sets a value indicating whether filters are automatically applied.
    /// </summary>
    bool AutoApply { get; set; }

    /// <summary>
    /// Gets the current filter built from the UI configuration.
    /// Returns null if no filter is active.
    /// </summary>
    IFilter<T>? CurrentFilter { get; }

    /// <summary>
    /// Gets a value indicating whether there are any active filters in the current configuration.
    /// </summary>
    bool HasActiveFilters { get; }

    /// <summary>
    /// Applies the current filter configuration and raises FiltersChanged.
    /// </summary>
    void Apply();

    /// <summary>
    /// Clears all filters (sets them to empty state).
    /// </summary>
    void Clear();

    /// <summary>
    /// Resets filters to their default state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Occurs when the filter configuration changes and a new filter is produced.
    /// </summary>
    event EventHandler<FiltersChangedEventArgs<T>>? FiltersChanged;
}
