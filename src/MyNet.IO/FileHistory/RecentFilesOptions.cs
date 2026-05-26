// -----------------------------------------------------------------------
// <copyright file="RecentFilesOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.IO.FileHistory;

/// <summary>
/// Represents configuration options for managing a list of recently accessed files, including the maximum number of entries to keep, the base path for storing recent file information, and the supported file extensions to track. This class is designed to be immutable and can be easily instantiated with object initializer syntax.
/// </summary>
public sealed class RecentFilesOptions
{
    /// <summary>
    /// Gets the base path where recent file information will be stored. This can be a directory path or a registry key path, depending on the implementation of the recent files manager. The base path serves as the root location for all recent file entries and can be used to organize them in a specific location.
    /// </summary>
    public string BasePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the maximum number of recent file entries to maintain. When the number of entries exceeds this limit, the oldest entries will be removed to make room for new ones.
    /// </summary>
    public int MaxEntries { get; init; }

    /// <summary>
    /// Gets the set of supported file extensions for recent file entries. Only files with these extensions will be tracked.
    /// </summary>
    public IReadOnlySet<string> SupportedExtensions { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
