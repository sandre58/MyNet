// -----------------------------------------------------------------------
// <copyright file="INotificationProcessor.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Notifications.Processors;

/// <summary>
/// Defines the contract for a notification processor, responsible for processing notifications before they are published or displayed.
/// </summary>
public interface INotificationProcessor
{
    /// <summary>
    /// Processes the specified notification.
    /// </summary>
    /// <param name="notification">The notification to process.</param>
    /// <returns>The processed notification.</returns>
    INotification? Process(INotification notification);
}
