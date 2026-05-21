// -----------------------------------------------------------------------
// <copyright file="FileExtensionGroups.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.IO.FileExtensions;

/// <summary>
/// Provides predefined groups of file extensions for common file types, such as images, documents, and spreadsheets. These groups can be used to easily filter files based on their extensions in various file handling operations.
/// </summary>
public static class FileExtensionGroups
{
    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents all files, using a wildcard extension. This group can be used to allow users to select any file type in file dialogs, without restricting the selection to specific extensions.
    /// </summary>
    public static FileExtensionGroup AllFiles { get; } = new(nameof(AllFiles), [new("*")]);

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents CSV files, using the ".csv" extension. This group can be used to filter files in file dialogs to only show CSV files, which are commonly used for storing tabular data in a plain text format.
    /// </summary>
    public static FileExtensionGroup Csv { get; } = FileExtensionGroup.Create(nameof(Csv), ".csv");

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents Excel files, using the ".xlsx" and ".xls" extensions. This group can be used to filter files in file dialogs to only show Excel files, which are commonly used for storing and analyzing data in spreadsheet format.
    /// </summary>
    public static FileExtensionGroup Excel { get; } = FileExtensionGroup.Create(nameof(Excel), ".xlsx", ".xls");

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents PNG image files, using the ".png" extension. This group can be used to filter files in file dialogs to only show PNG images, which are commonly used for lossless image compression.
    /// </summary>
    public static FileExtensionGroup Png { get; } = FileExtensionGroup.Create(nameof(Png), ".png");

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents JPEG image files, using the ".jpg" extension. This group can be used to filter files in file dialogs to only show JPEG images, which are commonly used for lossy image compression.
    /// </summary>
    public static FileExtensionGroup Jpg { get; } = FileExtensionGroup.Create(nameof(Jpg), ".jpg");

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents JPEG image files, using the ".jpeg" extension. This group can be used to filter files in file dialogs to only show JPEG images, which are commonly used for lossy image compression.
    /// </summary>
    public static FileExtensionGroup Jpeg { get; } = FileExtensionGroup.Create(nameof(Jpeg), ".jpeg");

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents GIF image files, using the ".gif" extension. This group can be used to filter files in file dialogs to only show GIF images, which are commonly used for simple animations and graphics.
    /// </summary>
    public static FileExtensionGroup Gif { get; } = FileExtensionGroup.Create(nameof(Gif), ".gif");

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents BMP image files, using the ".bmp" extension. This group can be used to filter files in file dialogs to only show BMP images, which are commonly used for uncompressed raster graphics.
    /// </summary>
    public static FileExtensionGroup Bmp { get; } = FileExtensionGroup.Create(nameof(Bmp), ".bmp");

    /// <summary>
    /// Gets a <see cref="FileExtensionGroup"/> that represents TIFF image files, using the ".tif" extension. This group can be used to filter files in file dialogs to only show TIFF images, which are commonly used for high-quality graphics and images.
    /// </summary>
    public static FileExtensionGroup Tif { get; } = FileExtensionGroup.Create(nameof(Tif), ".tif");
}
