// -----------------------------------------------------------------------
// <copyright file="EmptyNotificationFilter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Notifications.Processors;

/// <summary>
/// Implements a notification processor that filters out notifications with empty or whitespace-only messages, returning null for such notifications to prevent them from being published or displayed.
/// </summary>
public sealed class EmptyNotificationFilter : INotificationProcessor
{
    /// <inheritdoc />
    public INotification? Process(INotification notification) =>
        string.IsNullOrWhiteSpace(notification.Message)
            ? null
            : notification;
}
