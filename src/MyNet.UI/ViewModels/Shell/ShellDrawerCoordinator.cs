// -----------------------------------------------------------------------
// <copyright file="ShellDrawerCoordinator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Dialogs.ContentDialogs;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Closes shell drawers when a content dialog opens (file dialogs are unaffected).
/// </summary>
public sealed class ShellDrawerCoordinator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellDrawerCoordinator"/> class.
    /// </summary>
    public ShellDrawerCoordinator(IContentDialogService contentDialogService, IShellHostProvider hostProvider)
    {
        ArgumentNullException.ThrowIfNull(contentDialogService);
        ArgumentNullException.ThrowIfNull(hostProvider);

        contentDialogService.DialogOpened += (_, _) => hostProvider.Current?.CloseDrawers();
    }
}
