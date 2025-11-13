// -----------------------------------------------------------------------
// <copyright file="FilteredEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Observable.Collections.Filters;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Provides data for the Filtered event, containing the composite filters that were applied to a collection.
/// </summary>
/// <param name="filters">The collection of composite filters that were applied.</param>
public class FilteredEventArgs(IEnumerable<CompositeFilter> filters) : EventArgs
{
    /// <summary>
    /// Gets the collection of composite filters that were applied to the collection.
    /// These are the actual filter implementations (not view models) used for filtering.
    /// </summary>
    public IEnumerable<CompositeFilter> Filters { get; } = filters;
}
