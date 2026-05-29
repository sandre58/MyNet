// -----------------------------------------------------------------------
// <copyright file="IShellHostWithFileMenu.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.ViewModels.Shell.FileMenu;
using MyNet.UI.ViewModels.Workspace;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// <see cref="IShellHost"/> extended with file menu content navigation.
/// </summary>
public interface IShellHostWithFileMenu : IShellHost
{
    /// <summary>
    /// Gets the file menu view model.
    /// </summary>
    FileMenuViewModel FileMenuViewModel { get; }

    /// <summary>
    /// Opens the file menu drawer and shows the workspace registered for <typeparamref name="T"/>.
    /// </summary>
    void OpenFileMenuContent<T>()
        where T : class, IWorkspaceViewModel;

    /// <summary>
    /// Opens the file menu drawer and shows the workspace registered for <paramref name="contentType"/>.
    /// </summary>
    void OpenFileMenuContent(Type contentType);

    /// <summary>
    /// Hides file menu content and closes the file menu drawer.
    /// </summary>
    void CloseFileMenuContent();

    /// <summary>
    /// Applies an action to the file menu drawer and its content for the given workspace type.
    /// </summary>
    void SetFileMenuContentVisibility<T>(ShellDrawerAction action)
        where T : class, IWorkspaceViewModel;

    /// <summary>
    /// Applies an action to the file menu drawer and its content.
    /// </summary>
    void SetFileMenuContentVisibility(Type contentType, ShellDrawerAction action);
}
