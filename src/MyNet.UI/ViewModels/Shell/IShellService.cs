// -----------------------------------------------------------------------
// <copyright file="IShellService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Facade for shell drawer and file menu operations from other view models.
/// Requires <see cref="MainWindowViewModel"/> to be attached via <see cref="IShellHostProvider"/>.
/// </summary>
public interface IShellService
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
    /// Applies an action to the file menu drawer.
    /// </summary>
    void SetFileMenuDrawer(ShellDrawerAction action);

    /// <summary>
    /// Opens the file menu and shows content for <typeparamref name="T"/>.
    /// </summary>
    void OpenFileMenuContent<T>()
        where T : class, IWorkspaceViewModel;

    /// <summary>
    /// Opens the file menu and shows content for <paramref name="contentType"/>.
    /// </summary>
    void OpenFileMenuContent(Type contentType);

    /// <summary>
    /// Hides file menu content and closes the drawer.
    /// </summary>
    void CloseFileMenuContent();

    /// <summary>
    /// Applies an action to file menu drawer and content for <typeparamref name="T"/>.
    /// </summary>
    void SetFileMenuContentVisibility<T>(ShellDrawerAction action)
        where T : class, IWorkspaceViewModel;

    /// <summary>
    /// Applies an action to file menu drawer and content.
    /// </summary>
    void SetFileMenuContentVisibility(Type contentType, ShellDrawerAction action);
}
