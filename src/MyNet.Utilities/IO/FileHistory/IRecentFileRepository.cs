// -----------------------------------------------------------------------
// <copyright file="IRecentFileRepository.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyNet.Utilities.IO.FileHistory;

/// <summary>
/// Defines an interface for managing a repository of recent files, allowing for asynchronous operations to retrieve, add, update, and remove recent file entries, as well as clearing the entire history. This interface is designed to be implemented by classes that handle the storage and retrieval of recent file information in a way that is abstracted from the underlying data storage mechanism.
/// </summary>
public interface IRecentFileRepository
{
    /// <summary>
    /// Asynchronously retrieves a read-only list of all recent files in the repository. The returned list should reflect the current state of the repository, and any modifications to the repository after this method is called will not affect the returned list.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of recent files.</returns>
    Task<IReadOnlyList<RecentFile>> GetAllAsync();

    /// <summary>
    /// Asynchronously adds a new recent file to the repository. If a file with the same path already exists, it will be updated.
    /// </summary>
    /// <param name="file">The recent file to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added or updated recent file.</returns>
    Task<RecentFile?> AddAsync(RecentFile file);

    /// <summary>
    /// Asynchronously updates an existing recent file in the repository. If the file does not exist, it will not be added.
    /// </summary>
    /// <param name="file">The recent file to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated recent file, or null if the file does not exist.</returns>
    Task<RecentFile?> UpdateAsync(RecentFile file);

    /// <summary>
    /// Asynchronously removes a recent file from the repository by its path.
    /// </summary>
    /// <param name="path">The path of the recent file to remove.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the file was successfully removed.</returns>
    Task<bool> RemoveAsync(string path);

    /// <summary>
    /// Asynchronously clears all recent files from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ClearAsync();
}
