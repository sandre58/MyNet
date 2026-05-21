// -----------------------------------------------------------------------
// <copyright file="CommonFileFilters.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.IO.FileExtensions;

/// <summary>
/// Provides common file extension groups for filtering files in file dialogs or other file selection interfaces. This class includes predefined groups for common image file formats, allowing developers to easily filter for specific types of files when implementing file selection functionality in their applications.
/// </summary>
public static class CommonFileFilters
{
    /// <summary>
    /// Gets a file extension group representing common image file formats, including JPG, JPEG, PNG, GIF, BMP, and TIF. This group can be used to filter image files in file dialogs or other file selection interfaces.
    /// </summary>
    public static FileExtensionGroup Images { get; } =
        new(nameof(Images), [
            new(".jpg"),
            new(".jpeg"),
            new(".png"),
            new(".gif"),
            new(".bmp"),
            new(".tif")
        ]);
}
