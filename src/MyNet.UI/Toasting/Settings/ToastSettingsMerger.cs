// -----------------------------------------------------------------------
// <copyright file="ToastSettingsMerger.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Toasting.Settings;

/// <summary>
/// Merges global toast defaults, optional notification overrides, and notification-type rules.
/// </summary>
public static class ToastSettingsMerger
{
    /// <summary>
    /// Builds effective toast settings for the given notification.
    /// </summary>
    /// <param name="notification">The notification being displayed as a toast.</param>
    /// <param name="options">Global toast manager defaults.</param>
    /// <returns>Resolved toast settings for the toast instance.</returns>
    public static ToastSettings Merge(INotification notification, ToastManagerOptions? options = null)
    {
        options ??= new();

        var settings = new ToastSettings
        {
            ClosingStrategy = options.DefaultClosingStrategy,
            FreezeOnMouseEnter = options.DefaultFreezeOnMouseEnter,
            Duration = null
        };

        if (notification is IHasToastSettings { ToastSettings: { } overrides })
            ApplyOverrides(settings, overrides);

        ApplyNotificationTypeDefaults(settings, notification);

        return settings;
    }

    private static void ApplyOverrides(ToastSettings settings, ToastSettingsOverrides overrides)
    {
        if (overrides.ClosingStrategy is { } closingStrategy)
            settings.ClosingStrategy = closingStrategy;

        if (overrides.FreezeOnMouseEnter is { } freezeOnMouseEnter)
            settings.FreezeOnMouseEnter = freezeOnMouseEnter;

        if (overrides.Duration is { } duration)
            settings.Duration = duration;
    }

    private static void ApplyNotificationTypeDefaults(ToastSettings settings, INotification notification)
    {
        if (notification is not IClosableNotification { IsClosable: true })
            return;

        settings.ClosingStrategy = settings.ClosingStrategy switch
        {
            ToastClosingStrategy.AutoClose => ToastClosingStrategy.Both,
            ToastClosingStrategy.None => ToastClosingStrategy.CloseButton,
            _ => settings.ClosingStrategy
        };
    }
}
