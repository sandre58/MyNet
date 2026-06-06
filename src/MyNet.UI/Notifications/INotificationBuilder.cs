// -----------------------------------------------------------------------
// <copyright file="INotificationBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Settings;

namespace MyNet.UI.Notifications;

/// <summary>
/// Fluent builder for constructing and publishing <see cref="INotification"/> instances.
/// </summary>
public interface INotificationBuilder
{
    /// <summary>
    /// Sets the notification message.
    /// </summary>
    INotificationBuilder WithMessage(string message);

    /// <summary>
    /// Sets the notification title.
    /// </summary>
    INotificationBuilder WithTitle(string title);

    /// <summary>
    /// Sets the notification severity.
    /// </summary>
    INotificationBuilder WithSeverity(NotificationSeverity severity);

    /// <summary>
    /// Sets severity to <see cref="NotificationSeverity.Success"/>.
    /// </summary>
    INotificationBuilder AsSuccess();

    /// <summary>
    /// Sets severity to <see cref="NotificationSeverity.Error"/>.
    /// </summary>
    INotificationBuilder AsError();

    /// <summary>
    /// Sets severity to <see cref="NotificationSeverity.Warning"/>.
    /// </summary>
    INotificationBuilder AsWarning();

    /// <summary>
    /// Sets severity to <see cref="NotificationSeverity.Information"/>.
    /// </summary>
    INotificationBuilder AsInformation();

    /// <summary>
    /// Uses a deduplicable message notification (same message and severity are collapsed in the notification center).
    /// </summary>
    INotificationBuilder Deduplicable();

    /// <summary>
    /// Builds a closable notification (optionally with a click action).
    /// </summary>
    INotificationBuilder Closable(bool isClosable = true);

    /// <summary>
    /// Sets the action executed when the notification is activated.
    /// </summary>
    INotificationBuilder OnClick(Action<INotification> action);

    /// <summary>
    /// Overrides the toast closing strategy for this notification.
    /// </summary>
    INotificationBuilder WithClosingStrategy(ToastClosingStrategy closingStrategy);

    /// <summary>
    /// Overrides whether the toast pauses auto-close while the pointer is over it.
    /// </summary>
    INotificationBuilder WithFreezeOnMouseEnter(bool freezeOnMouseEnter);

    /// <summary>
    /// Overrides the toast auto-close duration for this notification.
    /// </summary>
    INotificationBuilder WithDuration(TimeSpan duration);

    /// <summary>
    /// Applies optional toast setting overrides for this notification.
    /// </summary>
    INotificationBuilder WithToastSettings(ToastSettingsOverrides toastSettings);

    /// <summary>
    /// Builds the notification without publishing it.
    /// </summary>
    INotification Build();

    /// <summary>
    /// Builds and publishes the notification using the publisher provided when the builder was created.
    /// </summary>
    /// <exception cref="InvalidOperationException">No publisher was associated with this builder.</exception>
    void Publish();
}
