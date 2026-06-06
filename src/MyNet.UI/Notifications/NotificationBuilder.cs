// -----------------------------------------------------------------------
// <copyright file="NotificationBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Settings;

namespace MyNet.UI.Notifications;

/// <summary>
/// Fluent builder for constructing and publishing notifications.
/// </summary>
public sealed class NotificationBuilder : INotificationBuilder
{
    private readonly INotificationPublisher? _publisher;

    private string _message = string.Empty;
    private string _title = string.Empty;
    private NotificationSeverity _severity = NotificationSeverity.Information;
    private bool _deduplicable;
    private bool _useClosableNotification;
    private bool _isClosable = true;
    private Action<INotification>? _onClick;
    private ToastSettingsOverrides? _toastSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationBuilder"/> class without an associated publisher.
    /// </summary>
    public NotificationBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationBuilder"/> class bound to the given publisher.
    /// </summary>
    /// <param name="publisher">Publisher used by <see cref="Publish"/>.</param>
    public NotificationBuilder(INotificationPublisher publisher) => _publisher = publisher;

    /// <inheritdoc />
    public INotificationBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder WithSeverity(NotificationSeverity severity)
    {
        _severity = severity;
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder AsSuccess() => WithSeverity(NotificationSeverity.Success);

    /// <inheritdoc />
    public INotificationBuilder AsError() => WithSeverity(NotificationSeverity.Error);

    /// <inheritdoc />
    public INotificationBuilder AsWarning() => WithSeverity(NotificationSeverity.Warning);

    /// <inheritdoc />
    public INotificationBuilder AsInformation() => WithSeverity(NotificationSeverity.Information);

    /// <inheritdoc />
    public INotificationBuilder Deduplicable()
    {
        _deduplicable = true;
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder Closable(bool isClosable = true)
    {
        _useClosableNotification = true;
        _isClosable = isClosable;
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder OnClick(Action<INotification> action)
    {
        _onClick = action;
        _useClosableNotification = true;
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder WithClosingStrategy(ToastClosingStrategy closingStrategy)
    {
        _toastSettings = MergeToastSettings(_toastSettings, new() { ClosingStrategy = closingStrategy });
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder WithFreezeOnMouseEnter(bool freezeOnMouseEnter)
    {
        _toastSettings = MergeToastSettings(_toastSettings, new() { FreezeOnMouseEnter = freezeOnMouseEnter });
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder WithDuration(TimeSpan duration)
    {
        _toastSettings = MergeToastSettings(_toastSettings, new() { Duration = duration });
        return this;
    }

    /// <inheritdoc />
    public INotificationBuilder WithToastSettings(ToastSettingsOverrides toastSettings)
    {
        ArgumentNullException.ThrowIfNull(toastSettings);
        _toastSettings = MergeToastSettings(_toastSettings, toastSettings);
        return this;
    }

    /// <inheritdoc />
    public INotification Build()
    {
        if (_useClosableNotification || _onClick is not null)
        {
            return new ActionNotification(_message, _title, _severity, _isClosable, _onClick)
            {
                ToastSettings = _toastSettings
            };
        }

        if (_deduplicable)
        {
            return new DeduplicableMessageNotification(_message, _title, _severity)
            {
                ToastSettings = _toastSettings
            };
        }

        return new MessageNotification(_message, _title, _severity)
        {
            ToastSettings = _toastSettings
        };
    }

    /// <inheritdoc />
    public void Publish()
    {
        if (_publisher is null)
            throw new InvalidOperationException("Cannot publish: create the builder via INotificationPublisher.Notify() or pass a publisher to NotificationBuilder.");

        _publisher.Publish(Build());
    }

    private static ToastSettingsOverrides MergeToastSettings(
        ToastSettingsOverrides? current,
        ToastSettingsOverrides update)
    {
        if (current is null)
            return update;

        return new()
        {
            ClosingStrategy = update.ClosingStrategy ?? current.ClosingStrategy,
            FreezeOnMouseEnter = update.FreezeOnMouseEnter ?? current.FreezeOnMouseEnter,
            Duration = update.Duration ?? current.Duration
        };
    }
}
