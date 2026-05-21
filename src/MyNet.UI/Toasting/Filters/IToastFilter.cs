// -----------------------------------------------------------------------
// <copyright file="IToastFilter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Toasting.Filters;

/// <summary>
/// Defines the contract for a toast filter, responsible for determining whether a given notification should be displayed as a toast notification.
/// </summary>
public interface IToastFilter
{
    /// <summary>
    /// Determines whether the specified notification should be displayed as a toast.
    /// </summary>
    /// <param name="notification">The notification model to evaluate.</param>
    /// <returns><c>true</c> if the notification should be displayed as a toast; otherwise, <c>false</c>.</returns>
    bool ShouldDisplay(INotification notification);
}
