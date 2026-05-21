// -----------------------------------------------------------------------
// <copyright file="IDirectoryService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.IO;

/// <summary>
/// Defines a service that manages a working directory and provides helpers to create files and sub-directories
/// under it.
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Gets the absolute root path managed by this service.
    /// </summary>
    string RootDirectory { get; }

    /// <summary>
    /// Creates a new sub-directory under <see cref="RootDirectory"/> and returns its full path.
    /// </summary>
    /// <param name="name">Name of the sub-directory to create.</param>
    /// <returns>The full path of the created sub-directory.</returns>
    string CreateSubDirectory(string name);

    /// <summary>
    /// Creates a new empty file under <see cref="RootDirectory"/> with an exclusive (unique) name and returns
    /// its full path.
    /// </summary>
    /// <param name="fileExtension">
    /// Optional file extension (e.g. <c>.tmp</c>, <c>*.csv</c>). When omitted, the preferred-file-name extension
    /// or <c>.tmp</c> is used.
    /// </param>
    /// <param name="preferredFileName">
    /// Optional preferred base file name. When the name is already taken, a random name is generated instead.
    /// </param>
    /// <returns>Full path of the newly created file.</returns>
    /// <exception cref="System.IO.IOException">
    /// Thrown when no unique name could be found after the maximum number of attempts.
    /// </exception>
    string CreateFile(string? fileExtension = null, string? preferredFileName = null);

    /// <summary>
    /// Resolves a file name under <see cref="RootDirectory"/> without creating the file.
    /// </summary>
    /// <param name="fileExtension">Optional file extension.</param>
    /// <param name="preferredFileName">Optional preferred base file name.</param>
    /// <returns>Full path of the resolved file name.</returns>
    string GetFileName(string? fileExtension = null, string? preferredFileName = null);

    /// <summary>
    /// Deletes all files and sub-directories inside <see cref="RootDirectory"/> without removing the root itself.
    /// Errors are logged but do not propagate.
    /// </summary>
    void Clean();

    /// <summary>
    /// Deletes the <see cref="RootDirectory"/> and all its contents recursively.
    /// Errors are logged but do not propagate.
    /// </summary>
    void Delete();
}
