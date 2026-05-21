// -----------------------------------------------------------------------
// <copyright file="INotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.Utilities;

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Defines the contract for a notification, including severity and identification.
/// </summary>
public interface INotification : INotifyPropertyChanged, IIdentifiable<Guid>
{
    /// <summary>
    /// Gets the title of the notification.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the message content of the notification.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the severity of the notification.
    /// </summary>
    NotificationSeverity Severity { get; }

    /// <summary>
    /// Gets the timestamp indicating when the notification was created or published.
    /// </summary>
    DateTime Timestamp { get; }
}
