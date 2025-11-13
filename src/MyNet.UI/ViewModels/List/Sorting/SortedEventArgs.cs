// -----------------------------------------------------------------------
// <copyright file="SortedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Observable.Collections.Sorting;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Provides data for the Sorted event, containing the sorting properties that were applied to a collection.
/// </summary>
/// <param name="properties">The collection of sorting properties that were applied.</param>
public class SortedEventArgs(IEnumerable<SortingProperty> properties) : EventArgs
{
    /// <summary>
    /// Gets the collection of sorting properties that were applied.
    /// These represent the final, active sorting configuration.
    /// </summary>
    public IEnumerable<SortingProperty> SortProperties { get; } = properties;
}
