// -----------------------------------------------------------------------
// <copyright file="FiltersChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable.Collections.Filters;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Provides data for the FiltersChanged event, containing the composite filter view models that define the current filter configuration.
/// </summary>
/// <param name="filter">The composite filter view model representing the current filter configuration.</param>
public class FiltersChangedEventArgs<T>(IFilter<T>? filter) : EventArgs
{
    /// <summary>
    /// Gets the composite filter view model that defines the current filter configuration.
    /// Each composite filter wraps an inner filter with UI state (enabled/disabled, logical operator).
    /// </summary>
    public IFilter<T>? Filter { get; } = filter;
}
