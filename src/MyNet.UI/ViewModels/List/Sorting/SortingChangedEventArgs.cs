// -----------------------------------------------------------------------
// <copyright file="SortingChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Observable.Collections.Sorting;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Provides data for the SortingChanged event, containing the sorting property view models that define the current sorting configuration.
/// </summary>
/// <param name="properties">The collection of sorting property view models representing the current configuration.</param>
public class SortingChangedEventArgs<T>(IReadOnlyList<ISortingProperty<T>> properties) : EventArgs
{
    /// <summary>
    /// Gets the collection of sorting property view models that define the current sorting configuration.
    /// These view models contain UI-related information such as display names, enabled state, and order.
    /// </summary>
    public IReadOnlyList<ISortingProperty<T>> Sorting { get; } = properties;
}
