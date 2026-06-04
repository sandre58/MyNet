// -----------------------------------------------------------------------
// <copyright file="OpenFileDialogBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Fluent builder for open file dialogs.
/// </summary>
public sealed class OpenFileDialogBuilder(IDialogService dialogService)
    : FileDialogBuilderBase<OpenFileDialogBuilder, OpenFileDialogSettings>
{
    /// <summary>
    /// Sets whether multiple files can be selected.
    /// </summary>
    public OpenFileDialogBuilder WithMultiselect(bool multiselect = true)
    {
        Settings.Multiselect = multiselect;
        return this;
    }

    /// <inheritdoc />
    public override Task<FileDialogResult> PickAsync(CancellationToken cancellationToken = default) =>
        dialogService.ShowOpenFileDialogAsync(Settings, cancellationToken);
}
