// -----------------------------------------------------------------------
// <copyright file="NotificationPublisherExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.UI.Messages;
using MyNet.UI.Notifications.Models;
using MyNet.Utilities;
using MyNet.Utilities.Exceptions;
using MyNet.Utilities.Logging;

namespace MyNet.UI.Notifications;

public static class NotificationPublisherExtensions
{
    /// <summary>
    /// Logs an exception and publishes it as a UI notification.
    /// </summary>
    /// <param name="notificationPublisher">Notification publisher used to emit the error notification.</param>
    /// <param name="exception">Exception to surface.</param>
    /// <param name="showInTaskBar">Whether to send a taskbar error state message.</param>
    /// <param name="reportTaskBar">Optional callback to publish taskbar messages from host application.</param>
    /// <param name="onClick">Optional action executed when notification is activated.</param>
    public static void PublishException(
        this INotificationPublisher notificationPublisher,
        Exception exception,
        bool showInTaskBar = false,
        Action<UpdateTaskBarInfoMessage>? reportTaskBar = null,
        Action<INotification>? onClick = null)
    {
        var innerException = exception.InnerException ?? exception;
        LogManager.Error(innerException);

        if (showInTaskBar)
            reportTaskBar?.Invoke(new(TaskbarProgressState.Error, 1));

        var message = innerException is TranslatableException translatableException
            ? translatableException.Parameters is not null
                ? translatableException.ResourceKey.Translate().FormatWith(CultureInfo.CurrentCulture, translatableException.Parameters)
                : translatableException.ResourceKey.Translate()
            : innerException.Message;

        INotification notification = onClick is null
            ? new MessageNotification(message, severity: NotificationSeverity.Error)
            : new ActionNotification(message, string.Empty, NotificationSeverity.Error, action: onClick);

        notificationPublisher.Publish(notification);
    }
}
