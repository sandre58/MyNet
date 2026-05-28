// -----------------------------------------------------------------------
// <copyright file="OpenFolderDialogSettings.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Settings for configuring the OpenFolderDialog.
/// </summary>
public class OpenFolderDialogSettings : FileDialogSettings
{
    /// <summary>
    /// Gets the default settings for the OpenFolderDialog.
    /// </summary>
    public static OpenFolderDialogSettings Default => new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFolderDialogSettings"/> class.
    /// </summary>
    public OpenFolderDialogSettings()
    {
        CheckFileExists = false;
    }

    /// <summary>
    /// Gets or sets the folder to select or open. Maps to <see cref="FileDialogSettings.InitialDirectory"/>.
    /// </summary>
    public string Folder
    {
        get => InitialDirectory;
        set => InitialDirectory = value;
    }
}
