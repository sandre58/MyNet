// -----------------------------------------------------------------------
// <copyright file="INotificationPublisher.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Notifications;

/// <summary>
/// Defines the contract for a notification publisher, responsible for publishing notifications to subscribers.
/// </summary>
public interface INotificationPublisher
{
    /// <summary>
    /// Publishes the specified notification to all subscribers.
    /// </summary>
    /// <param name="notification">The notification to publish.</param>
    void Publish(INotification notification);
}
