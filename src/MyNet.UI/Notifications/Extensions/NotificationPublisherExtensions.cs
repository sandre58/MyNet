// -----------------------------------------------------------------------
// <copyright file="NotificationPublisherExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Facade;
using MyNet.Primitives;
using MyNet.Primitives.Exceptions;
using MyNet.UI.Notifications.Models;
using MyNet.UI.ViewModels.Shell;

#pragma warning disable IDE0130
namespace MyNet.UI.Notifications;
#pragma warning restore IDE0130

public static class NotificationPublisherExtensions
{
    /// <summary>
    /// Logs an exception and publishes it as a UI notification.
    /// </summary>
    /// <param name="notificationPublisher">Notification publisher used to emit the error notification.</param>
    /// <param name="exception">Exception to surface.</param>
    /// <param name="showInTaskBar">Whether to set the taskbar to an error state.</param>
    /// <param name="taskbarProgress">Optional taskbar progress source (from <c>AddShell()</c>).</param>
    /// <param name="reportTaskBar">Optional callback when <paramref name="taskbarProgress"/> is not provided.</param>
    /// <param name="onClick">Optional action executed when notification is activated.</param>
    public static void PublishException(
        this INotificationPublisher notificationPublisher,
        Exception exception,
        bool showInTaskBar = false,
        ITaskbarProgressSource? taskbarProgress = null,
        Action<TaskbarProgressState, double?>? reportTaskBar = null,
        Action<INotification>? onClick = null)
    {
        var innerException = exception.InnerException ?? exception;

        if (showInTaskBar)
        {
            if (taskbarProgress is not null)
                taskbarProgress.SetError();
            else
                reportTaskBar?.Invoke(TaskbarProgressState.Error, 1);
        }

        var message = innerException is TranslatableException translatableException
            ? translatableException.Parameters.Count > 0
                ? translatableException.ResourceKey.Translate().FormatWith(CultureInfo.CurrentCulture, translatableException.Parameters)
                : translatableException.ResourceKey.Translate()
            : innerException.Message;

        INotification notification = onClick is null
            ? new MessageNotification(message, severity: NotificationSeverity.Error)
            : new ActionNotification(message, string.Empty, NotificationSeverity.Error, action: onClick);

        notificationPublisher.Publish(notification);
    }
}
