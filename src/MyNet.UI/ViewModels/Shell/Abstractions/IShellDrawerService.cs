// -----------------------------------------------------------------------
// <copyright file="IShellDrawerService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Facade for shell drawer operations from other view models.
/// </summary>
public interface IShellDrawerService
{
    /// <summary>
    /// Gets a value indicating whether a shell host is attached.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Closes all shell drawers when a host is attached.
    /// </summary>
    void CloseDrawers();

    /// <summary>
    /// Applies an action to the notifications drawer.
    /// </summary>
    void SetNotificationsDrawer(ShellDrawerAction action);

    /// <summary>
    /// Applies an action to the file menu drawer when the host supports it.
    /// </summary>
    void SetFileMenuDrawer(ShellDrawerAction action);
}
