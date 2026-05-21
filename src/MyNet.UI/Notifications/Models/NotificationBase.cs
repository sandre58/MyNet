// -----------------------------------------------------------------------
// <copyright file="NotificationBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable;

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Represents the base class for notifications, providing common properties and functionality for all types of notifications.
/// </summary>
/// <param name="message">The message content of the notification.</param>
/// <param name="title">The title of the notification.</param>
/// <param name="severity">The severity level of the notification.</param>
public abstract class NotificationBase(string message, string title = "", NotificationSeverity severity = NotificationSeverity.Information)
    : ObservableObject, INotification
{
    /// <inheritdoc/>
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Title { get; } = title;

    /// <inheritdoc/>
    public string Message { get; } = message;

    /// <inheritdoc/>
    public NotificationSeverity Severity { get; } = severity;

    /// <inheritdoc/>
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public override string ToString() => Message;
}
