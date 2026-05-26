// -----------------------------------------------------------------------
// <copyright file="FileFilters.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.IO.FileExtensions;

/// <summary>
/// Provides predefined file extension filters for common file types, such as images and Excel files. These filters can be used to restrict file selection dialogs to specific types of files, improving user experience and preventing invalid file selections.
/// </summary>
public static class FileFilters
{
    /// <summary>
    /// Returns a <see cref="FileExtensionFilter"/> that includes common image file extensions. This filter can be used in file dialogs to allow users to select only image files, such as JPEG, PNG, GIF, and BMP formats.
    /// </summary>
    /// <returns>A <see cref="FileExtensionFilter"/> for common image file extensions.</returns>
    public static FileExtensionFilter Images()
        => new([
            CommonFileFilters.Images
        ]);

    /// <summary>
    /// Returns a <see cref="FileExtensionFilter"/> that includes common Excel file extensions. This filter can be used in file dialogs to allow users to select only Excel files, such as XLSX and XLS formats.
    /// </summary>
    /// <returns>A <see cref="FileExtensionFilter"/> for common Excel file extensions.</returns>
    public static FileExtensionFilter Excel()
        => new([
            FileExtensionGroups.Excel
        ]);

    /// <summary>
    /// Returns a <see cref="FileExtensionFilter"/> that includes all file extensions. This filter can be used in file dialogs to allow users to select any type of file.
    /// </summary>
    /// <returns>A <see cref="FileExtensionFilter"/> for all file extensions.</returns>
    public static FileExtensionFilter All()
        => new([
            FileExtensionGroups.AllFiles
        ]);
}
