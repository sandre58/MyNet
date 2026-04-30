// -----------------------------------------------------------------------
// <copyright file="FiltersChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Provides data for the FiltersChanged event, containing the composite filter view models that define the current filter configuration.
/// </summary>
/// <param name="filters">The collection of composite filter view models representing the current filter configuration.</param>
public class FiltersChangedEventArgs(IEnumerable<ICompositeFilterViewModel> filters) : EventArgs
{
    /// <summary>
    /// Gets the collection of composite filter view models that define the current filter configuration.
    /// Each composite filter wraps an inner filter with UI state (enabled/disabled, logical operator).
    /// </summary>
    public IEnumerable<ICompositeFilterViewModel> Filters { get; } = filters;
}
