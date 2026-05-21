// -----------------------------------------------------------------------
// <copyright file="IRecentFilesService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyNet.Utilities.IO.FileHistory;

/// <summary>
/// Defines a service for managing a list of recently accessed files, allowing retrieval, addition, removal, and state updates of recent-file entries.
/// </summary>
public interface IRecentFilesService
{
    /// <summary>
    /// Gets all recent files currently tracked by the service.
    /// </summary>
    /// <returns>A read-only snapshot of all recent files.</returns>
    IReadOnlyList<RecentFile> GetAll();

    /// <summary>
    /// Asynchronously gets all recent files currently tracked by the service.
    /// </summary>
    /// <returns>A read-only snapshot of all recent files.</returns>
    Task<IReadOnlyList<RecentFile>> GetAllAsync();

    /// <summary>
    /// Gets the most recently accessed non-recovered file.
    /// </summary>
    /// <returns>
    /// The most recently accessed file, or <c>null</c> when no matching file exists.
    /// </returns>
    RecentFile? GetLast();

    /// <summary>
    /// Asynchronously gets the most recently accessed non-recovered file.
    /// </summary>
    /// <returns>
    /// The most recently accessed file, or <c>null</c> when no matching file exists.
    /// </returns>
    Task<RecentFile?> GetLastAsync();

    /// <summary>
    /// Adds a file to the recent-files list.
    /// If the file already exists, its access timestamp is refreshed.
    /// </summary>
    /// <param name="name">Display name of the file.</param>
    /// <param name="path">Full path to the file.</param>
    /// <param name="isRecovered">
    /// Indicates whether the file represents a recovered document.
    /// </param>
    /// <returns>
    /// The persisted recent-file entry, or <c>null</c> when the operation failed.
    /// </returns>
    RecentFile? Add(string name, string path, bool isRecovered = false);

    /// <summary>
    /// Asynchronously adds a file to the recent-files list.
    /// If the file already exists, its access timestamp is refreshed.
    /// </summary>
    /// <param name="name">Display name of the file.</param>
    /// <param name="path">Full path to the file.</param>
    /// <param name="isRecovered">
    /// Indicates whether the file represents a recovered document.
    /// </param>
    /// <returns>
    /// The persisted recent-file entry, or <c>null</c> when the operation failed.
    /// </returns>
    Task<RecentFile?> AddAsync(
        string name,
        string path,
        bool isRecovered = false);

    /// <summary>
    /// Removes a file from the recent-files list.
    /// </summary>
    /// <param name="path">Full path of the file to remove.</param>
    /// <returns>
    /// <c>true</c> if the file was removed; otherwise <c>false</c>.
    /// </returns>
    bool Remove(string path);

    /// <summary>
    /// Asynchronously removes a file from the recent-files list.
    /// </summary>
    /// <param name="path">Full path of the file to remove.</param>
    /// <returns>
    /// <c>true</c> if the file was removed; otherwise <c>false</c>.
    /// </returns>
    Task<bool> RemoveAsync(string path);

    /// <summary>
    /// Updates the pinned state of a recent file.
    /// </summary>
    /// <param name="path">Full path of the file to update.</param>
    /// <param name="isPinned">New pinned state.</param>
    /// <returns>
    /// The updated recent-file entry, or <c>null</c> if the file was not found.
    /// </returns>
    RecentFile? Pin(string path, bool isPinned);

    /// <summary>
    /// Asynchronously updates the pinned state of a recent file.
    /// </summary>
    /// <param name="path">Full path of the file to update.</param>
    /// <param name="isPinned">New pinned state.</param>
    /// <returns>
    /// The updated recent-file entry, or <c>null</c> if the file was not found.
    /// </returns>
    Task<RecentFile?> PinAsync(string path, bool isPinned);

    /// <summary>
    /// Determines whether a file exists in the recent-files list.
    /// </summary>
    /// <param name="path">Full path of the file.</param>
    /// <returns>
    /// <c>true</c> if the file exists in the list; otherwise <c>false</c>.
    /// </returns>
    bool Contains(string path);

    /// <summary>
    /// Asynchronously determines whether a file exists in the recent-files list.
    /// </summary>
    /// <param name="path">Full path of the file.</param>
    /// <returns>
    /// <c>true</c> if the file exists in the list; otherwise <c>false</c>.
    /// </returns>
    Task<bool> ContainsAsync(string path);

    /// <summary>
    /// Clears all recent-file entries.
    /// </summary>
    void Clear();

    /// <summary>
    /// Asynchronously clears all recent-file entries.
    /// </summary>
    Task ClearAsync();
}
