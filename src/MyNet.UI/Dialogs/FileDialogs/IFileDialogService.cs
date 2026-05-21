// -----------------------------------------------------------------------
// <copyright file="IFileDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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
    /// <returns>
    /// If the user clicks the OK button of the dialog that is displayed, true is returned;
    /// otherwise false.
    /// </returns>
    Task<FileDialogResult> ShowOpenFileDialogAsync(OpenFileDialogSettings settings);

    /// <summary>
    /// Displays the SaveFileDialog.
    /// </summary>
    /// <param name="settings">The settings for the save file dialog.</param>
    /// <returns>
    /// If the user clicks the OK button of the dialog that is displayed, true is returned;
    /// otherwise false.
    /// </returns>
    Task<FileDialogResult> ShowSaveFileDialogAsync(SaveFileDialogSettings settings);

    /// <summary>
    /// Displays the OpenFolderDialog.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <returns>
    /// If the user clicks the OK button of the dialog that is displayed, true is returned;
    /// otherwise false.
    /// </returns>
    Task<FileDialogResult> ShowFolderDialogAsync(OpenFolderDialogSettings settings);
}
