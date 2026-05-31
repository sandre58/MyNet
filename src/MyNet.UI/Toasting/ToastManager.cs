// -----------------------------------------------------------------------
// <copyright file="ToastManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Threading;
using MyNet.UI.Toasting.Filters;
using MyNet.UI.Toasting.Models;
using MyNet.UI.Toasting.Settings;

namespace MyNet.UI.Toasting;

/// <summary>
/// Manages the display and lifecycle of toasts based on incoming notifications, applying filtering, prioritization, and automatic dismissal according to specified settings.
/// </summary>
public sealed class ToastManager : IToastManager
{
    private readonly ObservableCollection<IToast> _visible = [];
    private readonly PriorityQueue<INotification, int> _queue = new();
    private readonly Dictionary<Guid, IDisposable> _lifetimeSubscriptions = [];
    private readonly Dictionary<Guid, EventHandler<CloseRequestedEventArgs>> _closeHandlers = [];
    private readonly ISchedulerProvider _scheduler;
    private readonly IToastFactory _factory;
    private readonly IToastFilter _filter;
    private readonly ToastManagerOptions _options;
    private readonly IDisposable _subscription;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastManager"/> class, subscribing to the notification service to receive notifications, using the provided factory to create toasts, applying the specified filter to determine which notifications should be displayed as toasts, and ensuring that all operations are observed on the UI scheduler for thread safety, while also allowing for configuration of toast display options through the provided <see cref="ToastManagerOptions"/>.
    /// </summary>
    /// <param name="service">The notification service to subscribe to for receiving notifications.</param>
    /// <param name="scheduler">The scheduler provider for managing UI and background thread operations.</param>
    /// <param name="factory">The factory used to create toast instances.</param>
    /// <param name="filter">The filter used to determine which notifications should be displayed as toasts.</param>
    /// <param name="options">The options for configuring the behavior of the toast manager.</param>
    public ToastManager(
        INotificationService service,
        ISchedulerProvider scheduler,
        IToastFactory factory,
        IToastFilter filter,
        ToastManagerOptions? options = null)
    {
        _scheduler = scheduler;
        _factory = factory;
        _filter = filter;
        _options = options ?? new ToastManagerOptions();

        Toasts = new(_visible);

        _subscription = service.Notifications
            .ObserveOn(_scheduler.Ui)
            .Subscribe(OnNotification);
    }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<IToast> Toasts { get; }

    /// <summary>
    /// Handles incoming notifications from the service, applying the filter to determine if they should be displayed as toasts, and either showing them immediately if there is capacity or enqueuing them based on their priority for later display when space becomes available. This method ensures that only notifications that pass the filter criteria are processed for display as toasts, and manages the lifecycle of visible toasts according to the configured options.
    /// </summary>
    /// <param name="notification">The notification to process for potential display as a toast.</param>
    private void OnNotification(INotification notification)
    {
        if (_isDisposed)
            return;

        if (!_filter.ShouldDisplay(notification))
            return;

        if (_visible.Count < _options.MaxVisibleToasts)
        {
            Add(notification);
        }
        else
        {
            Enqueue(notification);
        }
    }

    /// <summary>
    /// Creates and displays a toast for the given notification, adding it to the collection of visible toasts and starting its lifetime management if it is set to auto-close. This method is responsible for instantiating the toast using the factory, ensuring it is added to the visible collection, and initiating any necessary timers or subscriptions to manage its automatic dismissal based on the configured duration and closing strategy.
    /// </summary>
    /// <param name="notification">The notification to display as a toast.</param>
    private void Add(INotification notification)
    {
        var toast = _factory.Create(notification);

        _visible.Add(toast);

        HookLifecycle(toast);
        StartLifetime(toast);
    }

    /// <summary>
    /// Enqueues a notification that cannot be displayed immediately due to the maximum visible toasts limit, using the priority selector from the options to determine its position in the queue. This method ensures that notifications are stored in a priority queue based on their assigned priority, allowing for higher-priority notifications to be displayed before lower-priority ones when space becomes available in the visible collection of toasts.
    /// </summary>
    /// <param name="notification">The notification to enqueue for later display as a toast.</param>
    private void Enqueue(INotification notification)
    {
        if (_options.MaxQueueSize <= 0 || _queue.Count >= _options.MaxQueueSize)
            return;

        var priority = _options.PrioritySelector(notification);
        _queue.Enqueue(notification, -priority);
    }

    /// <summary>
    /// Starts the lifetime management for a toast that is set to auto-close, using a timer to automatically remove the toast after the specified duration. This method checks the closing strategy of the toast's settings and, if it is set to auto-close, it creates an observable timer that will trigger the removal of the toast after the configured duration, ensuring that toasts are dismissed automatically according to their settings without requiring manual intervention.
    /// </summary>
    /// <param name="toast">The toast for which to start lifetime management.</param>
    private void StartLifetime(IToast toast)
    {
        if (toast.Settings.ClosingStrategy is not ToastClosingStrategy.AutoClose and not ToastClosingStrategy.Both)
            return;

        if (toast.Settings.FreezeOnMouseEnter)
            return;

        var duration = toast.Settings.Duration ?? _options.DefaultDuration;

        var subscription = System.Reactive.Linq.Observable.Timer(duration, _scheduler.Background)
            .ObserveOn(_scheduler.Ui)
            .Subscribe(_ => Remove(toast));

        _lifetimeSubscriptions[toast.Notification.Id] = subscription;
    }

    private void HookLifecycle(IToast toast)
    {
        if (toast.Notification is not IClosableNotification closableNotification)
            return;

        _closeHandlers[toast.Notification.Id] = handler;
        closableNotification.CloseRequested += handler;
        return;

        void handler(object? sender, CloseRequestedEventArgs e) => Remove(toast);
    }

    private void UnhookLifecycle(IToast toast)
    {
        if (toast.Notification is IClosableNotification closableNotification
            && _closeHandlers.TryGetValue(toast.Notification.Id, out var handler))
        {
            closableNotification.CloseRequested -= handler;
            _closeHandlers.Remove(toast.Notification.Id);
        }

        if (_lifetimeSubscriptions.TryGetValue(toast.Notification.Id, out var subscription))
        {
            subscription.Dispose();
            _lifetimeSubscriptions.Remove(toast.Notification.Id);
        }
    }

    /// <inheritdoc />
    public void Remove(IToast toast) => _scheduler.Ui.Schedule(() => RemoveCore(toast));

    private void RemoveCore(IToast toast)
    {
        if (_isDisposed)
            return;

        if (!_visible.Remove(toast))
            return;

        UnhookLifecycle(toast);
        TryDequeue();
    }

    /// <summary>
    /// Attempts to dequeue the next notification from the priority queue and display it as a toast if there is capacity in the visible collection. This method is called after a toast is removed to ensure that any pending notifications in the queue are displayed as soon as space becomes available, maintaining the flow of notifications according to their priority and the configured maximum number of visible toasts.
    /// </summary>
    private void TryDequeue()
    {
        while (_visible.Count < _options.MaxVisibleToasts && _queue.Count > 0)
        {
            var next = _queue.Dequeue();
            Add(next);
        }
    }

    /// <inheritdoc />
    public void Clear() =>
        _scheduler.Ui.Schedule(() =>
        {
            if (_isDisposed)
                return;

            foreach (var toast in _visible.ToList())
                UnhookLifecycle(toast);

            _visible.Clear();
            _queue.Clear();
        });

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _subscription.Dispose();

        foreach (var toast in _visible.ToList())
            UnhookLifecycle(toast);

        _visible.Clear();
        _queue.Clear();
    }
}
