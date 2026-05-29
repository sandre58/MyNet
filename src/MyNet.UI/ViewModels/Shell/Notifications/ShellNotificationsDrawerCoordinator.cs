// -----------------------------------------------------------------------
// <copyright file="ShellNotificationsDrawerCoordinator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Commands;

namespace MyNet.UI.ViewModels.Shell.Notifications;

/// <summary>
/// Closes the notifications drawer when the panel becomes empty and refreshes toggle command availability.
/// </summary>
public sealed class ShellNotificationsDrawerCoordinator
{
    private IShellNotificationsDrawerHost? _host;
    private ShellNotificationsViewModel? _notifications;

    /// <summary>
    /// Subscribes to <paramref name="notifications"/> and drives <paramref name="host"/> drawer state.
    /// </summary>
    public void Bind(IShellNotificationsDrawerHost host, ShellNotificationsViewModel notifications)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(notifications);

        Unbind();

        _host = host;
        _notifications = notifications;
        _notifications.NotificationsChanged += OnNotificationsChanged;
        OnNotificationsChanged(notifications, EventArgs.Empty);
    }

    /// <summary>
    /// Removes subscriptions created by <see cref="Bind"/>.
    /// </summary>
    public void Unbind()
    {
        _notifications?.NotificationsChanged -= OnNotificationsChanged;

        _host = null;
        _notifications = null;
    }

    private void OnNotificationsChanged(object? sender, EventArgs e)
    {
        if (_host is null || _notifications is null)
            return;

        if (!_notifications.HasNotifications)
            _host.IsNotificationsOpen = false;

        if (_host.ToggleNotificationsCommand is IRaiseCanExecuteChanged raise)
            raise.RaiseCanExecuteChanged();
    }
}
