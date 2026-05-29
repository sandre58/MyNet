// -----------------------------------------------------------------------
// <copyright file="IFileMenuViewModelFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell.FileMenu;

/// <summary>
/// Creates <see cref="FileMenuViewModel"/> instances with workspace content supplied by the host application.
/// </summary>
public interface IFileMenuViewModelFactory
{
    /// <summary>
    /// Creates a file menu view model for the given workspace panels.
    /// </summary>
    FileMenuViewModel Create(
        IEnumerable<IWorkspaceViewModel> contentViewModels,
        ICommandFactory? commandFactory = null);
}
