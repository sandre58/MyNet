// -----------------------------------------------------------------------
// <copyright file="RegistryRecentFileEntry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Platform.Windows.Registry.FileHistory;

/// <summary>
/// Represents a recent file entry stored in the registry.
/// This is the persistence model (not the domain model).
/// </summary>
public sealed class RegistryRecentFileEntry
{
    /// <summary>
    /// Gets or sets the display name of the file.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full file path.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last access time stored as a Windows file time (UTC).
    /// A value of 0 means "unknown".
    /// </summary>
    public long LastAccessUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the file is pinned in the recent list.
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this file comes from an auto-recovery process.
    /// </summary>
    public bool IsRecovered { get; set; }
}
