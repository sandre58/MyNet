// -----------------------------------------------------------------------
// <copyright file="CompositeToastFilter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Toasting.Filters;

/// <summary>
/// Implements a toast filter that combines multiple filters, allowing a notification to be displayed only if it passes all the specified filters.
/// </summary>
/// <param name="filters">The collection of filters to be applied.</param>
public sealed class CompositeToastFilter(IEnumerable<IToastFilter> filters) : IToastFilter
{
    /// <summary>
    /// Determines whether the specified notification should be displayed based on the combined results of all filters. A notification will only be displayed if it passes all filters in the collection.
    /// </summary>
    /// <param name="notification">The notification to be evaluated.</param>
    /// <returns><c>true</c> if the notification should be displayed; otherwise, <c>false</c>.</returns>
    public bool ShouldDisplay(INotification notification) => filters.All(f => f.ShouldDisplay(notification));
}
