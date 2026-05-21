// -----------------------------------------------------------------------
// <copyright file="FileExtensionGroup.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNet.Utilities.IO.FileExtensions;

public sealed class FileExtensionGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileExtensionGroup"/> class with the specified key and extensions. The key is used to represent the extension group, and the extensions are used to filter files in a file dialog.
    /// </summary>
    /// <param name="key">The key representing the extension group.</param>
    /// <param name="extensions">The file extensions in the group.</param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="key"/> is null, empty, or consists only of whitespace.</exception>
    public FileExtensionGroup(string key, IEnumerable<FileExtension> extensions)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        Key = key;
        Extensions = [.. extensions];
    }

    /// <summary>
    /// Gets the key representing the extension group. This key can be used to translate the name of the extension group when generating file filters for file dialogs.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Creates a new <see cref="FileExtensionGroup"/> with the specified key and extensions.
    /// </summary>
    /// <param name="key">The key representing the extension group.</param>
    /// <param name="extensions">The file extensions in the group.</param>
    /// <returns>A new <see cref="FileExtensionGroup"/> instance.</returns>
    public static FileExtensionGroup Create(string key, params string[] extensions) => new(key, extensions.Select(x => new FileExtension(x)));

    /// <summary>
    /// Gets the list of file extensions associated with the extension group. These extensions are used to filter files in a file dialog, allowing users to select files that match the specified extensions.
    /// </summary>
    public IReadOnlyList<FileExtension> Extensions { get; }

    /// <summary>
    /// Gets the default file extension for the extension group. This is typically the first extension in the list.
    /// </summary>
    /// <returns>The default file extension, or <c>null</c> if the group has no extensions.</returns>
    public FileExtension? GetDefaultExtension() => Extensions.FirstOrDefault();
}
