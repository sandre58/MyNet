// -----------------------------------------------------------------------
// <copyright file="ShellFileMenuHost.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell.FileMenu;

/// <summary>
/// Coordinates file menu content navigation and drawer visibility for the shell host.
/// </summary>
public sealed class ShellFileMenuHost
{
    private readonly Action? _onDrawerOpening;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellFileMenuHost"/> class.
    /// </summary>
    public ShellFileMenuHost(
        FileMenuViewModel fileMenuViewModel,
        IShellFileMenuDrawer drawer,
        Action? onDrawerOpening = null)
    {
        FileMenuViewModel = fileMenuViewModel ?? throw new ArgumentNullException(nameof(fileMenuViewModel));
        Drawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
        _onDrawerOpening = onDrawerOpening;
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
        NotifyDrawerOpening();
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
                NotifyDrawerOpening();
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
                    {
                        NotifyDrawerOpening();
                        FileMenuViewModel.ShowContent(contentType);
                    }
                    else
                    {
                        CloseFileMenuContent();
                    }
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
    {
        if (action.WillOpen(Drawer.IsFileMenuOpen))
            NotifyDrawerOpening();

        Drawer.IsFileMenuOpen = action.ApplyToOpenState(Drawer.IsFileMenuOpen);
    }

    private void NotifyDrawerOpening()
    {
        if (!Drawer.IsFileMenuOpen)
            _onDrawerOpening?.Invoke();
    }
}
