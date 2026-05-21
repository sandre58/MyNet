// -----------------------------------------------------------------------
// <copyright file="MessageNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Represents a simple notification message with a title and severity.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageNotification"/> class.
/// </remarks>
/// <param name="message">The message content.</param>
/// <param name="title">The title of the notification.</param>
/// <param name="severity">The severity of the notification.</param>
public class MessageNotification(string message, string title = "", NotificationSeverity severity = NotificationSeverity.Information) : NotificationBase(message, title, severity);
