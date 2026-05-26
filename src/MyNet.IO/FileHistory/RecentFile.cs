// -----------------------------------------------------------------------
// <copyright file="RecentFile.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.IO.FileHistory;

/// <summary>
/// Represents a recently accessed file, including its name, path, access and modification timestamps, and status flags for pinning and recovery.
/// </summary>
public sealed record RecentFile
{
    /// <summary>
    /// Gets the name of the recently accessed file, which is typically the file name without the path. This property is required and must be provided when creating an instance of <see cref="RecentFile"/>.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the full path of the recently accessed file, which includes the directory and file name. This property is required and must be provided when creating an instance of <see cref="RecentFile"/>.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Gets the timestamp of the last access to the file. This property is optional and may be <c>null</c> if the access time is not available.
    /// </summary>
    public DateTimeOffset? LastAccessedAt { get; init; }

    /// <summary>
    /// Gets the timestamp of the last modification to the file. This property is optional and may be <c>null</c> if the modification time is not available.
    /// </summary>
    public DateTimeOffset? LastModifiedAt { get; init; }

    /// <summary>
    /// Gets a value indicating whether the file is pinned. Pinned files are typically given special treatment, such as being displayed at the top of a list.
    /// </summary>
    public bool IsPinned { get; init; }

    /// <summary>
    /// Gets a value indicating whether the file is a recovered document. Recovered files are typically those that were restored after an unexpected shutdown or crash.
    /// </summary>
    public bool IsRecovered { get; init; }
}
