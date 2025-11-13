// -----------------------------------------------------------------------
// <copyright file="GroupingChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Provides data for the GroupingChanged event, containing the grouping property view models that define the current configuration.
/// </summary>
/// <param name="groupProperties">The collection of grouping property view models representing the current grouping configuration.</param>
public class GroupingChangedEventArgs(IEnumerable<IGroupingPropertyViewModel> groupProperties) : EventArgs
{
    /// <summary>
    /// Gets the collection of grouping property view models that define the current grouping configuration.
    /// These view models contain UI-related information such as display names, enabled state, order, and sorting properties.
    /// </summary>
    public IEnumerable<IGroupingPropertyViewModel> GroupProperties { get; } = groupProperties;
}
