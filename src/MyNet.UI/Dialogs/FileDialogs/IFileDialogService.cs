// -----------------------------------------------------------------------
// <copyright file="IFileDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Service for displaying file dialogs.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Displays the OpenFileDialog.
    /// </summary>
    /// <param name="settings">The settings for the open file dialog.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A <see cref="FileDialogResult"/> describing the user selection.</returns>
    Task<FileDialogResult> ShowOpenFileDialogAsync(
        OpenFileDialogSettings settings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Displays the SaveFileDialog.
    /// </summary>
    /// <param name="settings">The settings for the save file dialog.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A <see cref="FileDialogResult"/> describing the user selection.</returns>
    Task<FileDialogResult> ShowSaveFileDialogAsync(
        SaveFileDialogSettings settings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Displays the OpenFolderDialog.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A <see cref="FileDialogResult"/> describing the user selection.</returns>
    Task<FileDialogResult> ShowFolderDialogAsync(
        OpenFolderDialogSettings settings,
        CancellationToken cancellationToken = default);
}
