// -----------------------------------------------------------------------
// <copyright file="IShellNotificationsDrawerHost.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Windows.Input;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Shell surface used by <see cref="ShellNotificationsDrawerCoordinator"/> to sync the notifications drawer.
/// </summary>
public interface IShellNotificationsDrawerHost
{
    /// <summary>
    /// Gets or sets whether the notifications drawer is open.
    /// </summary>
    bool IsNotificationsOpen { get; set; }

    /// <summary>
    /// Gets the command that toggles the notifications drawer.
    /// </summary>
    ICommand ToggleNotificationsCommand { get; }
}
