// -----------------------------------------------------------------------
// <copyright file="IShellHost.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Shell state surface implemented by the shell host view model.
/// </summary>
public interface IShellHost
{
    /// <summary>
    /// Closes all shell drawers.
    /// </summary>
    void CloseDrawers();

    /// <summary>
    /// Applies an action to the notifications drawer.
    /// </summary>
    void SetNotificationsDrawer(ShellDrawerAction action);

    /// <summary>
    /// Applies an action to the file menu drawer.
    /// </summary>
    void SetFileMenuDrawer(ShellDrawerAction action);
}
