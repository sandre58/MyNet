// -----------------------------------------------------------------------
// <copyright file="OpenFileDialogSettings.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Settings for configuring the OpenFileDialog.
/// </summary>
public class OpenFileDialogSettings : FileDialogSettings
{
    /// <summary>
    /// Gets the default settings for the OpenFileDialog.
    /// </summary>
    public static OpenFileDialogSettings Default => new();

    /// <summary>
    /// Gets or sets a value indicating whether multiple files can be selected.
    /// </summary>
    public bool Multiselect { get; set; }
}
