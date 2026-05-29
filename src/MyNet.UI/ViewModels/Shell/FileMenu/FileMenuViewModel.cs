// -----------------------------------------------------------------------
// <copyright file="FileMenuViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell.FileMenu;

/// <summary>
/// View model for the file menu drawer content.
/// Drawer visibility is controlled by the shell host via <see cref="IShellFileMenuDrawer"/>.
/// </summary>
public sealed class FileMenuViewModel : ViewModelBase
{
    private readonly HashSet<IWorkspaceViewModel> _contentViewModels = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="FileMenuViewModel"/> class.
    /// </summary>
    public FileMenuViewModel(
        IEnumerable<IWorkspaceViewModel> contentViewModels,
        ICommandFactory? commandFactory = null)
    {
        ArgumentNullException.ThrowIfNull(contentViewModels);

        foreach (var workspace in contentViewModels)
            _contentViewModels.Add(workspace);

        var commands = commandFactory ?? RelayCommandFactory.Default;
        ToggleFileMenuContentCommand = commands.CreateRequired<Type>(ToggleContent);
    }

    /// <summary>
    /// Gets or sets the workspace currently displayed in the file menu.
    /// </summary>
    public IWorkspaceViewModel? Content { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the command that toggles file menu content by workspace type.
    /// </summary>
    public ICommand ToggleFileMenuContentCommand { get; }

    /// <summary>
    /// Shows the workspace registered for <typeparamref name="T"/>.
    /// </summary>
    public void ShowContent<T>()
        where T : IWorkspaceViewModel
        => ShowContent(typeof(T));

    /// <summary>
    /// Shows the workspace registered for <paramref name="contentType"/>.
    /// </summary>
    public void ShowContent(Type contentType)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        if (!ContentIsVisible(contentType))
            Content = _contentViewModels.FirstOrDefault(contentType.IsInstanceOfType);
    }

    /// <summary>
    /// Hides the current file menu content.
    /// </summary>
    public void HideContent()
    {
        if (Content is not null)
            Content = null;
    }

    /// <summary>
    /// Toggles visibility of the workspace registered for <paramref name="contentType"/>.
    /// </summary>
    public void ToggleContent(Type contentType)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        if (!ContentIsVisible(contentType))
            ShowContent(contentType);
        else
            HideContent();
    }

    /// <summary>
    /// Returns whether <paramref name="contentType"/> matches the current <see cref="Content"/>.
    /// </summary>
    public bool ContentIsVisible(Type contentType) => contentType.IsInstanceOfType(Content);

    /// <summary>
    /// Returns whether <typeparamref name="T"/> matches the current <see cref="Content"/>.
    /// </summary>
    public bool ContentIsVisible<T>()
        where T : IWorkspaceViewModel
        => ContentIsVisible(typeof(T));
}
