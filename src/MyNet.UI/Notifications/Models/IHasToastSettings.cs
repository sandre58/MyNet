// -----------------------------------------------------------------------
// <copyright file="IHasToastSettings.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Toasting.Settings;

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Marks a notification that optionally overrides toast behavior settings.
/// </summary>
public interface IHasToastSettings
{
    /// <summary>
    /// Gets optional toast setting overrides applied when the notification is displayed as a toast.
    /// </summary>
    ToastSettingsOverrides? ToastSettings { get; }
}
