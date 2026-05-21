// -----------------------------------------------------------------------
// <copyright file="RecentFilesService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.Utilities.IO.FileHistory;

/// <summary>
/// Service responsible for managing recent files, including adding, retrieving, pinning, and removing entries.
/// </summary>
/// <param name="repository">The repository used for storing and retrieving recent files.</param>
public sealed class RecentFilesService(IRecentFileRepository repository) : IRecentFilesService, IDisposable
{
    private readonly ConcurrentDictionary<string, RecentFile> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private volatile bool _initialized;

    /// <inheritdoc/>
    public IReadOnlyList<RecentFile> GetAll()
    {
        EnsureInitializedAsync()
            .GetAwaiter()
            .GetResult();

        return GetOrderedFiles();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<RecentFile>> GetAllAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        return GetOrderedFiles();
    }

    /// <inheritdoc/>
    public RecentFile? GetLast()
    {
        EnsureInitializedAsync()
            .GetAwaiter()
            .GetResult();

        return GetLastCore();
    }

    /// <inheritdoc/>
    public async Task<RecentFile?> GetLastAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        return GetLastCore();
    }

    /// <inheritdoc/>
    public RecentFile? Add(string name, string path, bool isRecovered = false) => AddAsync(name, path, isRecovered)
        .GetAwaiter()
        .GetResult();

    /// <inheritdoc/>
    public async Task<RecentFile?> AddAsync(string name, string path, bool isRecovered = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await EnsureInitializedAsync().ConfigureAwait(false);

        if (_cache.TryGetValue(path, out var existing))
        {
            var refreshed = existing with
            {
                LastAccessedAt = DateTimeOffset.UtcNow,
                LastModifiedAt = GetLastModified(path)
            };

            var updated = await repository
                .UpdateAsync(refreshed)
                .ConfigureAwait(false);

            if (updated is not null)
                _cache[path] = updated;

            return updated;
        }

        var recentFile = new RecentFile
        {
            Name = name,
            Path = path,
            LastAccessedAt = DateTimeOffset.UtcNow,
            LastModifiedAt = GetLastModified(path),
            IsRecovered = isRecovered
        };

        var created = await repository
            .AddAsync(recentFile)
            .ConfigureAwait(false);

        if (created is not null)
            _cache[path] = created;

        return created;
    }

    /// <inheritdoc/>
    public bool Remove(string path) => RemoveAsync(path)
        .GetAwaiter()
        .GetResult();

    /// <inheritdoc/>
    public async Task<bool> RemoveAsync(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await EnsureInitializedAsync().ConfigureAwait(false);

        var removed = await repository
            .RemoveAsync(path)
            .ConfigureAwait(false);

        if (removed)
            _cache.TryRemove(path, out _);

        return removed;
    }

    /// <inheritdoc/>
    public RecentFile? Pin(string path, bool isPinned)
        => PinAsync(path, isPinned)
            .GetAwaiter()
            .GetResult();

    /// <inheritdoc/>
    public async Task<RecentFile?> PinAsync(string path, bool isPinned)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await EnsureInitializedAsync().ConfigureAwait(false);

        if (!_cache.TryGetValue(path, out var existing))
            return null;

        var updatedFile = existing with
        {
            IsPinned = isPinned
        };

        var updated = await repository
            .UpdateAsync(updatedFile)
            .ConfigureAwait(false);

        if (updated is not null)
            _cache[path] = updated;

        return updated;
    }

    /// <inheritdoc/>
    public bool Contains(string path)
    {
        EnsureInitializedAsync()
            .GetAwaiter()
            .GetResult();

        return _cache.ContainsKey(path);
    }

    /// <inheritdoc/>
    public async Task<bool> ContainsAsync(string path)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        return _cache.ContainsKey(path);
    }

    /// <inheritdoc/>
    public void Clear() => ClearAsync()
        .GetAwaiter()
        .GetResult();

    /// <inheritdoc/>
    public async Task ClearAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        foreach (var file in _cache.Keys.ToArray())
            await repository.RemoveAsync(file).ConfigureAwait(false);

        _cache.Clear();
    }

    /// <summary>
    /// Gets the last modified time of the file at the specified path. If the file does not exist, returns null.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The last modified time of the file, or null if the file does not exist.</returns>
    private static DateTimeOffset? GetLastModified(string path) => File.Exists(path)
        ? File.GetLastWriteTimeUtc(path)
        : null;

    /// <summary>
    /// Ensures that the recent files data is loaded into the in-memory cache. This method checks if the cache has already been initialized, and if not, it acquires a lock to prevent concurrent initialization. It then retrieves all recent files from the repository and populates the cache. This method is called before any operation that requires access to the recent files to ensure that the data is available in memory for fast access.
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_initialized)
            return;

        await _initializationLock
            .WaitAsync()
            .ConfigureAwait(false);

        try
        {
            if (_initialized)
                return;

            var files = await repository
                .GetAllAsync()
                .ConfigureAwait(false);

            foreach (var file in files)
                _cache[file.Path] = file;

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    /// <summary>
    /// Retrieves the list of recent files from the in-memory cache, ordered first by pinned status (pinned files appear before unpinned files) and then by last accessed time (most recently accessed files appear first). This method is used to provide an ordered list of recent files for display or retrieval purposes, ensuring that pinned files are prioritized and that the most recently accessed files are easily accessible.
    /// </summary>
    /// <returns>A read-only list of recent files, ordered by pinned status and last accessed time.</returns>
    private IReadOnlyList<RecentFile> GetOrderedFiles() => [.. _cache.Values
        .OrderByDescending(x => x.IsPinned)
        .ThenByDescending(x => x.LastAccessedAt)];

    /// <summary>
    /// Retrieves the most recently accessed file from the in-memory cache that is not marked as recovered. The method filters the cached recent files to exclude those that are marked as recovered, and then orders the remaining files by their last accessed time in descending order. The first file in this ordered list is returned as the most recently accessed file. If there are no non-recovered files in the cache, the method returns null.
    /// </summary>
    /// <returns>The most recently accessed non-recovered file, or null if none exist.</returns>
    private RecentFile? GetLastCore() => _cache.Values
        .Where(x => !x.IsRecovered)
        .OrderByDescending(x => x.LastAccessedAt)
        .FirstOrDefault();

    /// <summary>
    /// Disposes of the resources used by the <see cref="RecentFilesService"/>. This method is responsible for releasing any unmanaged resources and performing any necessary cleanup when the service is no longer needed. In this implementation, it disposes of the semaphore used for synchronization during initialization to ensure that all resources are properly released and to prevent potential memory leaks.
    /// </summary>
    public void Dispose() => _initializationLock.Dispose();
}
