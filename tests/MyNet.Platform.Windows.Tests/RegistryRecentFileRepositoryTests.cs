// -----------------------------------------------------------------------
// <copyright file="RegistryRecentFileRepositoryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using MyNet.IO.FileHistory;
using MyNet.IO.Registry;
using MyNet.Platform.Windows.Registry.FileHistory;
using Xunit;

namespace MyNet.Platform.Windows.Tests;

[SupportedOSPlatform("windows")]
public sealed class RegistryRecentFileRepositoryTests
{
    private readonly InMemoryRegistryService _registry = new();
    private readonly RegistryRecentFileRepository _repository;

    public RegistryRecentFileRepositoryTests()
    {
        _repository = new RegistryRecentFileRepository(
            _registry,
            new RecentFilesOptions
            {
                BasePath = "Software\\MyNetRecentFileTests",
                MaxEntries = 2,
                SupportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "txt", ".docx" }
            });
    }

    [Fact]
    public async Task AddAsync_StoresSupportedFileUnderExtensionKey()
    {
        var file = CreateFile(@"C:\temp\alpha.txt", "alpha.txt");

        var result = await _repository.AddAsync(file);

        Assert.NotNull(result);
        Assert.Single(_registry.Entries, x => x.Item.Path == file.Path);
    }

    [Fact]
    public async Task AddAsync_WithUnsupportedExtension_ReturnsNull()
    {
        var result = await _repository.AddAsync(CreateFile(@"C:\temp\alpha.pdf", "alpha.pdf"));

        Assert.Null(result);
        Assert.Empty(_registry.Entries);
    }

    [Fact]
    public async Task AddAsync_WithExistingPath_UpdatesLastAccess()
    {
        var path = @"C:\temp\existing.txt";
        var firstAccess = DateTimeOffset.UtcNow.AddHours(-2);
        var secondAccess = DateTimeOffset.UtcNow;

        await _repository.AddAsync(CreateFile(path, "existing.txt", firstAccess));
        var updated = await _repository.AddAsync(CreateFile(path, "existing.txt", secondAccess));

        Assert.NotNull(updated);
        Assert.True(updated!.LastAccessedAt > firstAccess);
        Assert.Single(_registry.Entries);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsStoredFiles()
    {
        await _repository.AddAsync(CreateFile(@"C:\temp\one.txt", "one.txt"));
        await _repository.AddAsync(CreateFile(@"C:\temp\two.docx", "two.docx"));

        var all = await _repository.GetAllAsync();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingEntry()
    {
        var path = @"C:\temp\update.txt";
        await _repository.AddAsync(CreateFile(path, "before.txt"));

        var updated = await _repository.UpdateAsync(CreateFile(path, "after.txt", isPinned: true));

        Assert.NotNull(updated);
        Assert.Equal("after.txt", updated!.Name, StringComparer.Ordinal);
        Assert.True(updated.IsPinned);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownPath_ReturnsNull()
    {
        var result = await _repository.UpdateAsync(CreateFile(@"C:\temp\missing.txt", "missing.txt"));

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_RemovesExistingEntry()
    {
        var path = @"C:\temp\remove.txt";
        await _repository.AddAsync(CreateFile(path, "remove.txt"));

        var removed = await _repository.RemoveAsync(path);

        Assert.True(removed);
        Assert.Empty(_registry.Entries);
    }

    [Fact]
    public async Task RemoveAsync_WithUnknownPath_ReturnsFalse()
    {
        var removed = await _repository.RemoveAsync(@"C:\temp\unknown.txt");

        Assert.False(removed);
    }

    [Fact]
    public async Task ClearAsync_RemovesAllEntries()
    {
        await _repository.AddAsync(CreateFile(@"C:\temp\clear-one.txt", "clear-one.txt"));
        await _repository.AddAsync(CreateFile(@"C:\temp\clear-two.txt", "clear-two.txt"));

        await _repository.ClearAsync();

        Assert.Empty(await _repository.GetAllAsync());
    }

    [Fact]
    public async Task AddAsync_EnforcesMaxEntriesByRemovingOldestUnpinnedFiles()
    {
        await _repository.AddAsync(CreateFile(@"C:\temp\oldest.txt", "oldest.txt", DateTimeOffset.UtcNow.AddHours(-3)));
        await _repository.AddAsync(CreateFile(@"C:\temp\middle.txt", "middle.txt", DateTimeOffset.UtcNow.AddHours(-2)));
        await _repository.AddAsync(CreateFile(@"C:\temp\newest.txt", "newest.txt", DateTimeOffset.UtcNow.AddHours(-1)));

        var all = await _repository.GetAllAsync();

        Assert.Equal(2, all.Count);
        Assert.DoesNotContain(all, x => x.Name == "oldest.txt");
        Assert.Contains(all, x => x.Name == "newest.txt");
    }

    private static RecentFile CreateFile(
        string path,
        string name,
        DateTimeOffset? lastAccessedAt = null,
        bool isPinned = false)
        => new()
        {
            Path = path,
            Name = name,
            LastAccessedAt = lastAccessedAt ?? DateTimeOffset.UtcNow,
            IsPinned = isPinned
        };

    private sealed class InMemoryRegistryService : IRegistryService
    {
        private readonly List<RegistryEntry<RegistryRecentFileEntry>> _entries = [];

        public IReadOnlyList<RegistryEntry<RegistryRecentFileEntry>> Entries => _entries;

        public void AddOrUpdate<T>(RegistryEntry<T> entry)
        {
            if (entry.Item is not RegistryRecentFileEntry recentEntry)
                return;

            var existing = _entries.FirstOrDefault(x =>
                string.Equals(x.Item.Path, recentEntry.Path, StringComparison.OrdinalIgnoreCase));

            if (existing is not null)
            {
                existing.Item.Name = recentEntry.Name;
                existing.Item.IsPinned = recentEntry.IsPinned;
                existing.Item.IsRecovered = recentEntry.IsRecovered;
                existing.Item.LastAccessUtc = recentEntry.LastAccessUtc;
                return;
            }

            var storagePath = RegistryPath.Combine(entry.Path.ToString(), Guid.NewGuid().ToString("N"));
            _entries.Add(new RegistryEntry<RegistryRecentFileEntry>(storagePath, recentEntry));
        }

        public RegistryEntry<T>? Get<T>(RegistryPath path)
            where T : new()
        {
            var match = _entries.FirstOrDefault(x => x.Path.ToString() == path.ToString());
            return match is null ? null : new RegistryEntry<T>(match.Path, (T)(object)match.Item!);
        }

        public IEnumerable<RegistryEntry<T>> GetAll<T>(RegistryPath parent)
            where T : new()
            => _entries
                .Where(x => x.Path.ToString().StartsWith(parent.ToString() + "\\", StringComparison.OrdinalIgnoreCase))
                .Select(x => new RegistryEntry<T>(x.Path, (T)(object)x.Item))
                .ToList();

        public void Remove(RegistryPath path)
        {
            var match = _entries.FirstOrDefault(x => x.Path.ToString() == path.ToString());
            if (match is not null)
                _entries.Remove(match);
        }
    }
}
