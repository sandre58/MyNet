// -----------------------------------------------------------------------
// <copyright file="NotificationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Notifications.Processors;

namespace MyNet.UI.Notifications;

/// <summary>
/// Implements a notification service that allows publishing notifications and processing them through a chain of processors before they are emitted to subscribers.
/// </summary>
public sealed class NotificationService : INotificationService, IDisposable
{
    private readonly Subject<INotification> _subject = new();
    private readonly ISubject<INotification> _synchronizedSubject;
    private readonly IReadOnlyList<INotificationProcessor> _processors;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationService"/> class.
    /// </summary>
    /// <param name="processors">An optional collection of notification processors to apply before publication.</param>
    public NotificationService(IEnumerable<INotificationProcessor>? processors = null)
    {
        _synchronizedSubject = Subject.Synchronize(_subject);
        _processors = processors?.ToList() ?? [];
    }

    /// <inheritdoc/>
    public IObservable<INotification> Notifications => _subject.AsObservable();

    /// <inheritdoc/>
    public void Publish(INotification notification)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var current = notification;

        foreach (var processor in _processors)
        {
            current = processor.Process(current);
            if (current is null)
                return;
        }

        _synchronizedSubject.OnNext(current);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _subject.OnCompleted();
        _subject.Dispose();
    }
}
