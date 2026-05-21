// -----------------------------------------------------------------------
// <copyright file="AllToastsFilter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Toasting.Filters;

/// <summary>
/// Implements a toast filter that allows all notifications to be displayed as toasts, without any filtering or exclusion.
/// </summary>
public sealed class AllToastsFilter : IToastFilter
{
    /// <inheritdoc />
    public bool ShouldDisplay(INotification notification) => true;
}
