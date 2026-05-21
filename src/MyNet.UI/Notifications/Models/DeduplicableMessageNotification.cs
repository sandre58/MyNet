// -----------------------------------------------------------------------
// <copyright file="DeduplicableMessageNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Represents a message notification that can be deduplicated based on its content and severity.
/// </summary>
/// <param name="message">The message content.</param>
/// <param name="title">The title of the notification.</param>
/// <param name="severity">The severity of the notification.</param>
public sealed class DeduplicableMessageNotification(string message, string title = "", NotificationSeverity severity = NotificationSeverity.Information)
    : MessageNotification(message, title, severity), IDeduplicableNotification
{
    /// <inheritdoc/>
    public bool IsDuplicateOf(INotification other) =>
        other is MessageNotification m &&
        string.Equals(Message, m.Message, StringComparison.OrdinalIgnoreCase) &&
        Severity == m.Severity;
}
