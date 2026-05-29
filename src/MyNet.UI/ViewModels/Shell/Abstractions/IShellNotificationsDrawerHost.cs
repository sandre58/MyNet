// -----------------------------------------------------------------------
// <copyright file="IShellNotificationsDrawerHost.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Windows.Input;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Shell surface used by <see cref="Notifications.ShellNotificationsDrawerCoordinator"/> to sync the notifications drawer.
/// </summary>
public interface IShellNotificationsDrawerHost
{
    /// <summary>
    /// Gets or sets a value indicating whether the notifications drawer is open.
    /// </summary>
    bool IsNotificationsOpen { get; set; }

    /// <summary>
    /// Gets the command that toggles the notifications drawer.
    /// </summary>
    ICommand ToggleNotificationsCommand { get; }
}
