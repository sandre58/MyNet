// -----------------------------------------------------------------------
// <copyright file="SaveFileDialogBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Fluent builder for save file dialogs.
/// </summary>
public sealed class SaveFileDialogBuilder(IDialogService dialogService)
    : FileDialogBuilderBase<SaveFileDialogBuilder, SaveFileDialogSettings>
{
    /// <summary>
    /// Sets whether the dialog prompts before overwriting an existing file.
    /// </summary>
    public SaveFileDialogBuilder WithOverwritePrompt(bool overwritePrompt = true)
    {
        Settings.OverwritePrompt = overwritePrompt;
        return this;
    }

    /// <summary>
    /// Sets whether the dialog prompts to create a file when it does not exist.
    /// </summary>
    public SaveFileDialogBuilder WithCreatePrompt(bool createPrompt = true)
    {
        Settings.CreatePrompt = createPrompt;
        return this;
    }

    /// <inheritdoc />
    public override Task<FileDialogResult> PickAsync(CancellationToken cancellationToken = default) =>
        dialogService.ShowSaveFileDialogAsync(Settings, cancellationToken);
}
