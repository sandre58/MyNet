// -----------------------------------------------------------------------
// <copyright file="ActionNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Represents a closable notification with an associated action to execute.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ActionNotification"/> class.
/// </remarks>
/// <param name="message">The message content.</param>
/// <param name="title">The title of the notification.</param>
/// <param name="severity">The severity of the notification.</param>
/// <param name="isClosable">Indicates whether the notification can be closed.</param>
/// <param name="action">The action to execute when the notification is triggered.</param>
public class ActionNotification(string message, string title, NotificationSeverity severity, bool isClosable = true, Action<INotification>? action = null) : ClosableNotification(message, title, severity, isClosable)
{
    /// <summary>
    /// Gets the action to execute when the notification is triggered.
    /// </summary>
    public Action<INotification>? Action { get; } = action;
}
