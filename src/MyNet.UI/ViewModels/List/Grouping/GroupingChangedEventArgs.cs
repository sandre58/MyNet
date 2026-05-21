// -----------------------------------------------------------------------
// <copyright file="GroupingChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Observable.Collections.Grouping;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Provides data for the GroupingChanged event, containing the grouping property view models that define the current grouping configuration.
/// </summary>
/// <param name="properties">The collection of grouping property view models representing the current configuration.</param>
public class GroupingChangedEventArgs<T>(IReadOnlyList<IGroupingProperty<T>> properties) : EventArgs
{
    /// <summary>
    /// Gets the collection of grouping property view models that define the current grouping configuration.
    /// These view models contain UI-related information such as display names, enabled state, and order.
    /// </summary>
    public IReadOnlyList<IGroupingProperty<T>> Grouping { get; } = properties;
}
