// -----------------------------------------------------------------------
// <copyright file="OpenFolderDialogBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Fluent builder for folder picker dialogs.
/// </summary>
public sealed class OpenFolderDialogBuilder(IDialogService dialogService)
    : FileDialogBuilderBase<OpenFolderDialogBuilder, OpenFolderDialogSettings>
{
    /// <inheritdoc />
    public override Task<FileDialogResult> PickAsync(CancellationToken cancellationToken = default) =>
        dialogService.ShowFolderDialogAsync(Settings, cancellationToken);
}
