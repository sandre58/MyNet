// -----------------------------------------------------------------------
// <copyright file="NotificationBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Notifications.Models;

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
    public INotification Build() => _useClosableNotification || _onClick is not null
        ? new ActionNotification(_message, _title, _severity, _isClosable, _onClick)
        : _deduplicable
            ? new DeduplicableMessageNotification(_message, _title, _severity)
            : (INotification)new MessageNotification(_message, _title, _severity);

    /// <inheritdoc />
    public void Publish()
    {
        if (_publisher is null)
            throw new InvalidOperationException("Cannot publish: create the builder via INotificationPublisher.Notify() or pass a publisher to NotificationBuilder.");

        _publisher.Publish(Build());
    }
}
