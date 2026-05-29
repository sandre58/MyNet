// -----------------------------------------------------------------------
// <copyright file="ShellFileMenuHost.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Coordinates file menu content navigation and drawer visibility for the shell host.
/// </summary>
public sealed class ShellFileMenuHost
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellFileMenuHost"/> class.
    /// </summary>
    public ShellFileMenuHost(FileMenuViewModel fileMenuViewModel, IShellFileMenuDrawer drawer)
    {
        FileMenuViewModel = fileMenuViewModel ?? throw new ArgumentNullException(nameof(fileMenuViewModel));
        Drawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
    }

    /// <summary>
    /// Gets the file menu content view model.
    /// </summary>
    public FileMenuViewModel FileMenuViewModel { get; }

    /// <summary>
    /// Gets the shell drawer state surface.
    /// </summary>
    public IShellFileMenuDrawer Drawer { get; }

    /// <summary>
    /// Hides file menu content when the shell closes all drawers.
    /// </summary>
    public void OnCloseAllDrawers() => FileMenuViewModel.HideContent();

    /// <summary>
    /// Opens the file menu drawer and shows the workspace registered for <typeparamref name="T"/>.
    /// </summary>
    public void OpenFileMenuContent<T>()
        where T : class, IWorkspaceViewModel
        => OpenFileMenuContent(typeof(T));

    /// <summary>
    /// Opens the file menu drawer and shows the workspace registered for <paramref name="contentType"/>.
    /// </summary>
    public void OpenFileMenuContent(Type contentType)
    {
        ArgumentNullException.ThrowIfNull(contentType);
        Drawer.IsFileMenuOpen = true;
        FileMenuViewModel.ShowContent(contentType);
    }

    /// <summary>
    /// Hides file menu content and closes the file menu drawer.
    /// </summary>
    public void CloseFileMenuContent()
    {
        FileMenuViewModel.HideContent();
        Drawer.IsFileMenuOpen = false;
    }

    /// <summary>
    /// Applies an action to the file menu drawer and its content for the given workspace type.
    /// </summary>
    public void SetFileMenuContentVisibility<T>(ShellDrawerAction action)
        where T : class, IWorkspaceViewModel
        => SetFileMenuContentVisibility(typeof(T), action);

    /// <summary>
    /// Applies an action to the file menu drawer and its content.
    /// </summary>
    public void SetFileMenuContentVisibility(Type contentType, ShellDrawerAction action)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        switch (action)
        {
            case ShellDrawerAction.Show:
                Drawer.IsFileMenuOpen = true;
                FileMenuViewModel.ShowContent(contentType);
                break;

            case ShellDrawerAction.Hide:
                FileMenuViewModel.HideContent();
                Drawer.IsFileMenuOpen = false;
                break;

            case ShellDrawerAction.Toggle:
                if (Drawer.IsFileMenuOpen)
                {
                    if (!FileMenuViewModel.ContentIsVisible(contentType))
                        FileMenuViewModel.ShowContent(contentType);
                    else
                        CloseFileMenuContent();
                }
                else
                {
                    OpenFileMenuContent(contentType);
                }

                break;
        }
    }

    /// <summary>
    /// Applies a drawer visibility action without changing content.
    /// </summary>
    public void SetDrawer(ShellDrawerAction action)
        => Drawer.IsFileMenuOpen = action.ApplyToOpenState(Drawer.IsFileMenuOpen);
}
