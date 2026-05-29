// -----------------------------------------------------------------------
// <copyright file="ShellNotificationsHost.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Shell.Notifications;

namespace MyNet.UI.ViewModels.Shell.Drawers;

/// <summary>
/// Coordinates notifications drawer state and toggle command availability for the shell host.
/// </summary>
public sealed class ShellNotificationsHost : IDisposable
{
    private readonly IShellNotificationsDrawerHost _drawer;
    private readonly ShellNotificationsViewModel _notifications;
    private readonly ShellNotificationsDrawerCoordinator _coordinator;
    private readonly Func<bool> _canUseShellChrome;
    private readonly Action? _onDrawerOpening;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellNotificationsHost"/> class.
    /// </summary>
    public ShellNotificationsHost(
        IShellNotificationsDrawerHost drawer,
        ShellNotificationsViewModel notifications,
        ShellNotificationsDrawerCoordinator coordinator,
        Func<bool> canUseShellChrome,
        Action? onDrawerOpening = null)
    {
        _drawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
        _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));
        _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        _canUseShellChrome = canUseShellChrome ?? throw new ArgumentNullException(nameof(canUseShellChrome));
        _onDrawerOpening = onDrawerOpening;

        _coordinator.Bind(drawer, notifications);
    }

    /// <summary>
    /// Creates the toggle notifications command.
    /// </summary>
    public ICommand CreateToggleCommand(ICommandFactory commands)
        => commands.Create(() => SetDrawer(ShellDrawerAction.Toggle), CanToggle);

    /// <summary>
    /// Applies an action to the notifications drawer.
    /// </summary>
    public void SetDrawer(ShellDrawerAction action)
    {
        if (action.WillOpen(_drawer.IsNotificationsOpen))
            _onDrawerOpening?.Invoke();

        _drawer.IsNotificationsOpen = action.ApplyToOpenState(_drawer.IsNotificationsOpen);
    }

    /// <summary>
    /// Closes the notifications drawer.
    /// </summary>
    public void Close() => _drawer.IsNotificationsOpen = false;

    /// <summary>
    /// Returns whether the toggle command can execute.
    /// </summary>
    public bool CanToggle() => _canUseShellChrome() && _notifications.HasNotifications;

    /// <inheritdoc />
    public void Dispose() => _coordinator.Unbind();
}
