// -----------------------------------------------------------------------
// <copyright file="IRecentFilesOperations.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyNet.IO.FileHistory;

namespace MyNet.UI.Services.FileHistory;

/// <summary>
/// UI-facing operations for the recent-files list.
/// </summary>
public interface IRecentFilesOperations
{
    /// <summary>
    /// Occurs when the recent-files store has changed.
    /// </summary>
    event EventHandler? Changed;

    /// <summary>
    /// Gets all recent files.
    /// </summary>
    Task<IReadOnlyList<RecentFile>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or refreshes a recent file entry.
    /// </summary>
    Task<RecentFile?> AddAsync(string name, string path, bool isRecovered = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a file from the recent-files list.
    /// </summary>
    Task<bool> RemoveAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the pinned state of a recent file.
    /// </summary>
    Task<RecentFile?> SetPinnedAsync(string path, bool isPinned, CancellationToken cancellationToken = default);
}
