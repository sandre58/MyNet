// -----------------------------------------------------------------------
// <copyright file="NotificationsManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Threading;

namespace MyNet.UI.Notifications;

/// <summary>
/// Implements the <see cref="INotificationsManager"/> interface, managing a collection of notifications received from a notification service, allowing for their lifecycle management including addition, removal, and clearing, while ensuring thread safety by observing on the UI scheduler and preventing duplicates when necessary.
/// </summary>
public sealed class NotificationsManager : INotificationsManager, IDisposable
{
    private readonly ObservableCollection<INotification> _items = [];
    private readonly Dictionary<Guid, EventHandler<CloseRequestedEventArgs>> _closeHandlers = [];
    private readonly ISchedulerProvider _schedulerProvider;
    private readonly IDisposable _subscription;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsManager"/> class, subscribing to the notification stream and observing updates on the UI scheduler.
    /// </summary>
    /// <param name="service">The notification service to subscribe to for receiving notifications.</param>
    /// <param name="schedulerProvider">The scheduler provider to ensure UI thread safety.</param>
    public NotificationsManager(
        INotificationService service,
        ISchedulerProvider schedulerProvider)
    {
        _schedulerProvider = schedulerProvider;
        Notifications = new(_items);

        _subscription = service.Notifications
            .ObserveOn(schedulerProvider.Ui)
            .Subscribe(OnNotificationReceived);
    }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<INotification> Notifications { get; }

    /// <summary>
    /// Handles incoming notifications, checks for duplicates, then tracks lifecycle hooks for closable notifications.
    /// </summary>
    /// <param name="notification">The notification received from the service.</param>
    private void OnNotificationReceived(INotification notification)
    {
        if (IsDuplicate(notification))
            return;

        _items.Add(notification);

        HookLifecycle(notification);
    }

    /// <summary>
    /// Determines whether the given notification is a duplicate of any existing notification in the collection, based on the logic defined in the <see cref="IDeduplicableNotification"/> interface if implemented, allowing for prevention of duplicate notifications from being added to the collection.
    /// </summary>
    /// <param name="notification">The notification to check for duplication.</param>
    /// <returns>True if the notification is a duplicate; otherwise, false.</returns>
    private bool IsDuplicate(INotification notification) =>
        notification is IDeduplicableNotification dedup && _items.Any(x =>
            x is IDeduplicableNotification existing &&
            dedup.IsDuplicateOf(existing));

    /// <summary>
    /// Hooks into the lifecycle of a closable notification by subscribing to its CloseRequested event, allowing for automatic removal of the notification from the collection when it is requested to be closed by the user or programmatically, ensuring proper cleanup and management of notifications in the UI.
    /// </summary>
    /// <param name="notification">The closable notification to hook into.</param>
    private void HookLifecycle(INotification notification)
    {
        if (notification is not IClosableNotification closableNotification)
            return;

        EventHandler<CloseRequestedEventArgs> handler = (_, _) => Remove(notification);
        _closeHandlers[notification.Id] = handler;
        closableNotification.CloseRequested += handler;
    }

    private void UnhookLifecycle(INotification notification)
    {
        if (notification is not IClosableNotification closableNotification)
            return;

        if (!_closeHandlers.TryGetValue(notification.Id, out var handler))
            return;

        closableNotification.CloseRequested -= handler;
        _closeHandlers.Remove(notification.Id);
    }

    /// <inheritdoc />
    public void Remove(INotification notification) => _schedulerProvider.Ui.Schedule(() => RemoveCore(notification));

    private void RemoveCore(INotification notification)
    {
        if (_isDisposed)
            return;

        var item = _items.FirstOrDefault(x => x.Id == notification.Id);
        if (item is null)
            return;

        UnhookLifecycle(item);
        _items.Remove(item);
    }

    /// <inheritdoc />
    public void Clear() => _schedulerProvider.Ui.Schedule(ClearCore);

    private void ClearCore()
    {
        if (_isDisposed)
            return;

        foreach (var item in _items.ToList())
            UnhookLifecycle(item);

        _items.Clear();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _subscription.Dispose();

        foreach (var item in _items.ToList())
            UnhookLifecycle(item);

        _items.Clear();
    }
}
