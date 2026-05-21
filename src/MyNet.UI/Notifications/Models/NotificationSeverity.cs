// -----------------------------------------------------------------------
// <copyright file="NotificationSeverity.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Specifies the severity level of a notification.
/// </summary>
public enum NotificationSeverity
{
    /// <summary>
    /// No severity specified.
    /// </summary>
    None,

    /// <summary>
    /// Informational notification.
    /// </summary>
    Information,

    /// <summary>
    /// Success notification.
    /// </summary>
    Success,

    /// <summary>
    /// Warning notification.
    /// </summary>
    Warning,

    /// <summary>
    /// Error notification.
    /// </summary>
    Error
}
