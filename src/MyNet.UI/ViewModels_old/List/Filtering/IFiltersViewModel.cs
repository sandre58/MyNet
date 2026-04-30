// -----------------------------------------------------------------------
// <copyright file="IFiltersViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents a view model for managing a collection of filters.
/// Provides methods and events to configure, apply, and reset filters.
/// </summary>
public interface IFiltersViewModel
{
    /// <summary>
    /// Resets all filters to their default state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Clears all active filters.
    /// </summary>
    void Clear();

    /// <summary>
    /// Refreshes the filters and triggers a re-evaluation.
    /// </summary>
    void Refresh();

    /// <summary>
    /// Occurs when the filter configuration has changed.
    /// Subscribers can react to apply the new filters to their collections.
    /// </summary>
    event EventHandler<FiltersChangedEventArgs>? FiltersChanged;
}
