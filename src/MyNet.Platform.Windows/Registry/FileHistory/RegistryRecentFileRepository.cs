// -----------------------------------------------------------------------
// <copyright file="RegistryRecentFileRepository.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using MyNet.IO.FileHistory;
using MyNet.IO.Registry;

namespace MyNet.Platform.Windows.Registry.FileHistory;

/// <summary>
/// Implementation of <see cref="IRecentFileRepository"/> that uses the Windows Registry to store recent file entries.
/// </summary>
/// <param name="registryService">The registry service used to interact with the Windows Registry.</param>
/// <param name="options">The options for configuring the recent files repository.</param>
[SupportedOSPlatform("windows")]
public sealed class RegistryRecentFileRepository(IRegistryService registryService, RecentFilesOptions options) : IRecentFileRepository
{
    private readonly HashSet<string> _supportedExtensions = new(options.SupportedExtensions.Select(NormalizeExtension), StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public Task<IReadOnlyList<RecentFile>> GetAllAsync()
    {
        var result = new List<RecentFile>();

        foreach (var entries in _supportedExtensions.Select(GetExtensionRegistryPath).Select(registryService.GetAll<RegistryRecentFileEntry>))
        {
            result.AddRange(entries.Select(x => MapToRecentFile(x.Item)));
        }

        return Task.FromResult<IReadOnlyList<RecentFile>>(result);
    }

    /// <inheritdoc/>
    public async Task<RecentFile?> AddAsync(RecentFile file)
    {
        var extension = GetExtension(file.Path);

        if (extension is null)
            return null;

        if (!_supportedExtensions.Contains(extension))
            return null;

        var existing = FindByPath(file.Path);

        if (existing is not null)
        {
            existing = existing with { LastAccessedAt = DateTimeOffset.UtcNow };

            return await UpdateAsync(existing).ConfigureAwait(false);
        }

        var entry = MapToRegistryRecentFileEntry(file);

        var registryEntry = new RegistryEntry<RegistryRecentFileEntry>(GetExtensionRegistryPath(extension), entry);

        registryService.AddOrUpdate(registryEntry);

        await CleanupAsync(extension).ConfigureAwait(false);

        return file;
    }

    /// <inheritdoc/>
    public Task<RecentFile?> UpdateAsync(RecentFile file)
    {
        var existing = FindRegistryEntry(file.Path);

        if (existing is null)
            return Task.FromResult<RecentFile?>(null);

        registryService.AddOrUpdate(new RegistryEntry<RegistryRecentFileEntry>(existing.Path, MapToRegistryRecentFileEntry(file)));

        return Task.FromResult<RecentFile?>(file);
    }

    /// <inheritdoc/>
    public Task<bool> RemoveAsync(string path)
    {
        var existing = FindRegistryEntry(path);

        if (existing is null)
            return Task.FromResult(false);

        registryService.Remove(existing.Path);

        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public Task ClearAsync()
    {
        foreach (var entry in _supportedExtensions.Select(GetExtensionRegistryPath).Select(registryService.GetAll<RegistryRecentFileEntry>).SelectMany(entries => entries))
        {
            registryService.Remove(entry.Path);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Maps a <see cref="RegistryRecentFileEntry"/> to a <see cref="RecentFile"/> instance.
    /// </summary>
    /// <param name="entry">The registry entry to map.</param>
    /// <returns>A <see cref="RecentFile"/> instance.</returns>
    private static RecentFile MapToRecentFile(RegistryRecentFileEntry entry)
        => new()
        {
            Name = entry.Name,
            Path = entry.Path,
            IsPinned = entry.IsPinned,
            IsRecovered = entry.IsRecovered,
            LastAccessedAt = DateTimeOffset.FromFileTime(
                entry.LastAccessUtc)
        };

    /// <summary>
    /// Maps a <see cref="RecentFile"/> to a <see cref="RegistryRecentFileEntry"/> instance for storage in the registry.
    /// </summary>
    /// <param name="file">The recent file to map.</param>
    /// <returns>A <see cref="RegistryRecentFileEntry"/> instance.</returns>
    private static RegistryRecentFileEntry MapToRegistryRecentFileEntry(RecentFile file)
        => new()
        {
            Name = file.Name,
            Path = file.Path,
            IsPinned = file.IsPinned,
            IsRecovered = file.IsRecovered,
            LastAccessUtc =
                file.LastAccessedAt?.UtcDateTime.ToFileTimeUtc() ?? 0
        };

    /// <summary>
    /// Normalizes a file extension by trimming whitespace and leading dots, ensuring consistent formatting for registry keys.
    /// </summary>
    /// <param name="extension">The file extension to normalize.</param>
    /// <returns>The normalized file extension.</returns>
    private static string NormalizeExtension(string extension) => extension.Trim().TrimStart('.');

    /// <summary>
    /// Extracts and normalizes the file extension from a given file path. If the path does not have an extension, returns null.
    /// </summary>
    /// <param name="path">The file path to extract the extension from.</param>
    /// <returns>The normalized file extension, or null if the path does not have an extension.</returns>
    private static string? GetExtension(string path)
    {
        var extension = Path.GetExtension(path);

        return string.IsNullOrWhiteSpace(extension)
            ? null
            : NormalizeExtension(extension);
    }

    /// <summary>
    /// Cleans up old entries for a given file extension, ensuring that the number of recent file entries does not exceed the configured maximum. This method retrieves all entries for the specified extension, identifies those that are not pinned or recovered, and removes the oldest entries if the total count exceeds the maximum allowed. This helps maintain a manageable list of recent files while respecting user preferences for pinned and recovered items.
    /// </summary>
    /// <param name="extension">The file extension for which to clean up old entries.</param>
    /// <returns>A task representing the asynchronous cleanup operation.</returns>
    private Task CleanupAsync(string extension)
    {
        if (options.MaxEntries <= 0)
            return Task.CompletedTask;

        var entries = registryService
            .GetAll<RegistryRecentFileEntry>(GetExtensionRegistryPath(extension))
            .ToList();

        var removable = entries
            .Where(x => x.Item is { IsPinned: false, IsRecovered: false })
            .OrderByDescending(x => x.Item.LastAccessUtc)
            .Skip(options.MaxEntries)
            .ToList();

        foreach (var item in removable)
            registryService.Remove(item.Path);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Constructs the registry path for a given file extension by combining the base path from options with the normalized extension. This method ensures that recent file entries for different extensions are stored under separate registry keys, allowing for organized storage and retrieval based on file type.
    /// </summary>
    /// <param name="extension">The file extension for which to construct the registry path.</param>
    /// <returns>The constructed registry path for the specified file extension.</returns>
    private RegistryPath GetExtensionRegistryPath(string extension) => RegistryPath.Combine(options.BasePath, extension);

    /// <summary>
    /// Finds a registry entry for a given file path by searching through the registry entries for the corresponding file extension. This method retrieves all entries for the file's extension and checks for a matching path, returning the first found entry or null if no match is found. This allows for efficient retrieval of recent file entries based on their file paths, enabling operations such as updates and removals to be performed accurately.
    /// </summary>
    /// <param name="path">The file path for which to find the registry entry.</param>
    /// <returns>The registry entry for the specified file path, or null if no match is found.</returns>
    private RegistryEntry<RegistryRecentFileEntry>? FindRegistryEntry(string path)
    {
        var extension = GetExtension(path);

        return extension is null
            ? null
            : registryService
                .GetAll<RegistryRecentFileEntry>(GetExtensionRegistryPath(extension))
                .FirstOrDefault(x =>
                    string.Equals(
                        x.Item.Path,
                        path,
                        StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds a recent file entry by its file path by first locating the corresponding registry entry and then mapping it to a <see cref="RecentFile"/> instance. This method utilizes the <see cref="FindRegistryEntry"/> method to retrieve the registry entry for the specified path and, if found, converts it to a <see cref="RecentFile"/> using the mapping function. If no matching registry entry is found, this method returns null, indicating that there is no recent file entry for the given path.
    /// </summary>
    /// <param name="path">The file path for which to find the recent file entry.</param>
    /// <returns>The recent file entry for the specified path, or null if no match is found.</returns>
    private RecentFile? FindByPath(string path) => FindRegistryEntry(path)?.Item is { } item ? MapToRecentFile(item) : null;
}
