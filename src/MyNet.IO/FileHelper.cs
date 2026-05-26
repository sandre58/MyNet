// -----------------------------------------------------------------------
// <copyright file="FileHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;

namespace MyNet.IO;

/// <summary>
/// Provides utility methods for common file-system operations.
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Ensures that the specified directory exists.
    /// Creates it if it does not already exist.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="path"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown when a file already exists at the specified path.
    /// </exception>
    public static void EnsureDirectoryExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (File.Exists(path))
        {
            throw new IOException($"Cannot create directory '{path}' because a file already exists at this location.");
        }

        _ = Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Ensures that the configured filename is valid and accessible.
    /// </summary>
    /// <param name="filename">The filename to validate.</param>
    /// <returns>The validated filename.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no filename has been configured.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the configured file does not exist.
    /// </exception>
    public static string EnsureFileExists(string filename)
        => string.IsNullOrWhiteSpace(filename)
            ? throw new InvalidOperationException("No source filename has been configured.")
            : !File.Exists(filename)
                ? throw new FileNotFoundException("The specified items file was not found.", filename)
                : filename;

    /// <summary>
    /// Deletes the specified file if it exists.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>
    /// <c>true</c> if the file existed and was deleted;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool TryDeleteFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            return false;
        }

        File.Delete(filePath);

        return true;
    }

    /// <summary>
    /// Attempts to create a new empty file at the specified path with exclusive access. Returns <c>true</c> if the file was created successfully, or <c>false</c> if a file already exists at that path.
    /// </summary>
    /// <param name="path">The path of the file to create.</param>
    /// <returns><c>true</c> if the file was created successfully; otherwise, <c>false</c>.</returns>
    public static bool TryCreateFile(string path)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.CreateNew);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
