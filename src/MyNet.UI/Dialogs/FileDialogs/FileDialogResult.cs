// -----------------------------------------------------------------------
// <copyright file="FileDialogResult.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Represents the result of a file dialog operation, including whether the operation was canceled and the list of selected files.
/// </summary>
public sealed class FileDialogResult
{
    /// <summary>
    /// Gets a value indicating whether the file dialog operation was canceled by the user. If true, no files were selected and the operation was aborted.
    /// </summary>
    public bool IsCancelled { get; init; }

    /// <summary>
    /// Gets the list of file paths selected by the user in the file dialog. This list is empty if the operation was canceled or if no files were selected.
    /// </summary>
    public IReadOnlyList<string> Files { get; init; } = [];

    /// <summary>
    /// Gets the first file path from the list of selected files, or null if no files were selected. This is a convenient property for single-file selection scenarios, where only one file is expected to be chosen.
    /// </summary>
    public string? Filename => Files.Count > 0 ? Files[0] : null;
}
