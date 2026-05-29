// -----------------------------------------------------------------------
// <copyright file="ShellService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell;

/// <inheritdoc />
public sealed class ShellService(IShellHostProvider hostProvider) : IShellService
{
    private readonly IShellHostProvider _hostProvider = hostProvider ?? throw new ArgumentNullException(nameof(hostProvider));

    /// <inheritdoc />
    public bool IsAvailable => _hostProvider.Current is not null;

    /// <inheritdoc />
    public void CloseDrawers() => RequireHost().CloseDrawers();

    /// <inheritdoc />
    public void SetNotificationsDrawer(ShellDrawerAction action) => RequireHost().SetNotificationsDrawer(action);

    /// <inheritdoc />
    public void SetFileMenuDrawer(ShellDrawerAction action) => RequireHost().SetFileMenuDrawer(action);

    /// <inheritdoc />
    public void OpenFileMenuContent<T>()
        where T : class, IWorkspaceViewModel
        => RequireFileMenuHost().OpenFileMenuContent<T>();

    /// <inheritdoc />
    public void OpenFileMenuContent(Type contentType) => RequireFileMenuHost().OpenFileMenuContent(contentType);

    /// <inheritdoc />
    public void CloseFileMenuContent() => RequireFileMenuHost().CloseFileMenuContent();

    /// <inheritdoc />
    public void SetFileMenuContentVisibility<T>(ShellDrawerAction action)
        where T : class, IWorkspaceViewModel
        => RequireFileMenuHost().SetFileMenuContentVisibility<T>(action);

    /// <inheritdoc />
    public void SetFileMenuContentVisibility(Type contentType, ShellDrawerAction action)
        => RequireFileMenuHost().SetFileMenuContentVisibility(contentType, action);

    private IShellHost RequireHost()
        => _hostProvider.Current
           ?? throw new InvalidOperationException("No shell host is attached. Register MainWindowViewModel with IShellHostProvider.");

    private IShellHostWithFileMenu RequireFileMenuHost()
        => _hostProvider.Current is MainWindowViewModel { HasFileMenu: true } host
            ? host
            : throw new InvalidOperationException(
                "The attached shell host does not have a file menu. Pass FileMenuViewModel to MainWindowViewModel.");
}
