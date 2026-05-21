// -----------------------------------------------------------------------
// <copyright file="ToastManagerOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Notifications.Models;
using MyNet.Utilities;

namespace MyNet.UI.Toasting.Settings;

/// <summary>
/// Represents the configuration options for the ToastManager, allowing customization of toast display behavior such as maximum visible toasts, queue size, and priority selection based on notification severity.
/// </summary>
public class ToastManagerOptions
{
    /// <summary>
    /// Gets or sets the maximum number of toasts that can be displayed simultaneously. If the number of active toasts exceeds this limit, additional toasts will be queued until space is available.
    /// </summary>
    public int MaxVisibleToasts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum number of toasts that can be queued. If the queue exceeds this limit, additional toasts will be discarded.
    /// </summary>
    public int MaxQueueSize { get; set; } = 50;

    /// <summary>
    /// Gets or sets the function used to determine the priority of a notification. Higher values indicate higher priority.
    /// </summary>
    public Func<INotification, int> PrioritySelector { get; set; } =
        n => n.Severity switch
        {
            NotificationSeverity.Error => 100,
            NotificationSeverity.Warning => 80,
            NotificationSeverity.Success => 50,
            _ => 10
        };

    /// <summary>
    /// Gets or sets the default duration for which a toast is displayed before it is automatically dismissed. This can be overridden on a per-toast basis if needed.
    /// </summary>
    public TimeSpan DefaultDuration { get; set; } = 5.Seconds();
}
