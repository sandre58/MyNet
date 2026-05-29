// -----------------------------------------------------------------------
// <copyright file="FileMenuViewModelFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell.FileMenu;

/// <inheritdoc />
public sealed class FileMenuViewModelFactory : IFileMenuViewModelFactory
{
    /// <inheritdoc />
    public FileMenuViewModel Create(
        IEnumerable<IWorkspaceViewModel> contentViewModels,
        ICommandFactory? commandFactory = null)
        => new(contentViewModels, commandFactory);
}
