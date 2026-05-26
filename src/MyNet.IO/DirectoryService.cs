// -----------------------------------------------------------------------
// <copyright file="DirectoryService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MyNet.IO;

/// <summary>
/// Manages a directory root and provides helpers to create uniquely named files and sub-directories.
/// The root path is created on demand, supports environment-variable expansion, and falls back to
/// the system temporary directory when no path is provided.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DirectoryService"/> class.
/// Initializes a new instance of <see cref="DirectoryService"/>.
/// </remarks>
/// <param name="root">
/// The root directory path. Environment variables (e.g. <c>%TEMP%</c>) are expanded automatically.
/// Pass an empty string or <c>null</c> to use the system temporary directory.
/// </param>
/// <param name="logger">Optional logger. Defaults to the application-wide logger for this class.</param>
[SuppressMessage("Performance", "CA1823:Avoid unused private fields", Justification = "Used by LoggerMessage source generator")]
public partial class DirectoryService(string root, ILogger<DirectoryService>? logger = null) : IDirectoryService
{
    private readonly ILogger<DirectoryService> _logger = logger ?? NullLogger<DirectoryService>.Instance;

    /// <summary>
    /// Gets the absolute root path managed by this instance. The directory is created on demand if it does not exist.
    /// </summary>
    public string RootDirectory { get; } = ResolveRoot(root);

    /// <summary>
    /// Resolves the root directory from the supplied raw path. Falls back to the system temporary directory when <paramref name="root"/> is empty.
    /// </summary>
    /// <param name="root">The raw root path.</param>
    /// <returns>The resolved absolute root path.</returns>
    private static string ResolveRoot(string? root)
    {
        var path = string.IsNullOrWhiteSpace(root)
            ? Path.GetTempPath()
            : Environment.ExpandEnvironmentVariables(root);

        FileHelper.EnsureDirectoryExists(path);

        return path;
    }

    /// <inheritdoc/>
    public string CreateSubDirectory(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Subdirectory name cannot be empty.", nameof(name));

        var fullPath = Path.Combine(RootDirectory, name);
        Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    /// <inheritdoc/>
    public string CreateFile(string? fileExtension = null, string? preferredFileName = null)
    {
        const int maxAttempts = 10;

        for (var i = 0; i < maxAttempts; i++)
        {
            var path = GetFileName(fileExtension, preferredFileName);

            if (FileHelper.TryCreateFile(path))
                return path;

            preferredFileName = null;
        }

        throw new IOException("Unable to create a unique file after multiple attempts.");
    }

    /// <inheritdoc/>
    public string GetFileName(string? fileExtension = null, string? preferredFileName = null)
    {
        var extension = NormalizeExtension(fileExtension);

        var baseName = string.IsNullOrWhiteSpace(preferredFileName) ? Path.GetRandomFileName() : Path.GetFileNameWithoutExtension(preferredFileName);

        return Path.Combine(RootDirectory, Path.ChangeExtension(baseName, extension));
    }

    /// <summary>
    /// Normalizes a raw file-extension token into a canonical form with a leading dot (e.g. <c>.csv</c>).
    /// Falls back to <c>.tmp</c> when both parameters are empty.
    /// </summary>
    private static string NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return ".tmp";

        extension = extension.Trim();

        if (extension.StartsWith('*'))
            extension = extension[1..];

        if (!extension.StartsWith('.'))
            extension = "." + extension;

        return extension;
    }

    /// <inheritdoc/>
    public void Delete()
    {
        try
        {
            Directory.Delete(RootDirectory, true);
        }
        catch (Exception ex)
        {
            LogFailedToDeleteRoot(ex);
        }
    }

    /// <inheritdoc/>
    public void Clean()
    {
        try
        {
            var dir = new DirectoryInfo(RootDirectory);

            foreach (var file in dir.GetFiles())
                TryDelete(file);

            foreach (var subDir in dir.GetDirectories())
                TryDelete(subDir);
        }
        catch (Exception ex)
        {
            LogFailedToClean(ex);
        }
    }

    /// <summary>
    /// Attempts to delete a directory recursively, logging a warning on failure.
    /// </summary>
    /// <param name="info">The directory to delete.</param>
    private void TryDelete(FileSystemInfo info)
    {
        try
        {
            if (info is DirectoryInfo dir)
                dir.Delete(true);
            else
                info.Delete();
        }
        catch (Exception ex)
        {
            LogFailedToDeleteItem(ex);
        }
    }

    /// <summary>
    /// Attempts to delete a single file, logging a warning on failure.
    /// </summary>
    private void TryDelete(FileInfo info)
    {
        try
        {
            info.Delete();
        }
        catch (Exception ex)
        {
            LogFailedToDeleteAFile(ex);
        }
    }

    [LoggerMessage(LogLevel.Warning, "Failed to clean directory.")]
    private partial void LogFailedToClean(Exception exception);

    [LoggerMessage(LogLevel.Warning, "Failed to delete root directory.")]
    private partial void LogFailedToDeleteRoot(Exception exception);

    [LoggerMessage(LogLevel.Warning, "Failed to delete file system item.")]
    private partial void LogFailedToDeleteItem(Exception exception);

    [LoggerMessage(LogLevel.Warning, "Failed to delete a file.")]
    private partial void LogFailedToDeleteAFile(Exception exception);
}
