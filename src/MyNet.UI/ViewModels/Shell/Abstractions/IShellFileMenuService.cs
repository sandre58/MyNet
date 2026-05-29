// -----------------------------------------------------------------------
// <copyright file="IShellFileMenuService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.ViewModels.Workspace;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Facade for file menu navigation from other view models.
/// </summary>
public interface IShellFileMenuService
{
    /// <summary>
    /// Gets a value indicating whether the attached host has a file menu.
    /// </summary>
    bool IsFileMenuAvailable { get; }

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
