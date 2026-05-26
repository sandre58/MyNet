// -----------------------------------------------------------------------
// <copyright file="RecentFilesServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyNet.IO.FileHistory;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class RecentFilesServiceTests
{
    [Fact]
    public async Task GetAllAsync_InitializesCacheOnlyOnce_AndOrdersPinnedFirstAsync()
    {
        var repository = new InMemoryRecentFileRepository([
            new() { Name = "Second", Path = "b.txt", LastAccessedAt = new DateTimeOffset(2026, 5, 1, 10, 0, 0, TimeSpan.Zero) },
            new() { Name = "Pinned", Path = "a.txt", IsPinned = true, LastAccessedAt = new DateTimeOffset(2026, 5, 1, 9, 0, 0, TimeSpan.Zero) },
            new() { Name = "Third", Path = "c.txt", LastAccessedAt = new DateTimeOffset(2026, 5, 1, 8, 0, 0, TimeSpan.Zero) }
        ]);

        using var service = new RecentFilesService(repository);

        var first = await service.GetAllAsync();
        var second = await service.GetAllAsync();

        Assert.Equal(["a.txt", "b.txt", "c.txt"], [.. first.Select(x => x.Path)]);
        Assert.Equal(first.Select(x => x.Path), second.Select(x => x.Path));
        Assert.Equal(1, repository.GetAllCalls);
    }

    [Fact]
    public async Task GetLastAsync_IgnoresRecoveredFilesAsync()
    {
        var repository = new InMemoryRecentFileRepository([
            new() { Name = "Recovered", Path = "recovered.txt", IsRecovered = true, LastAccessedAt = DateTimeOffset.UtcNow },
            new() { Name = "Normal", Path = "normal.txt", LastAccessedAt = DateTimeOffset.UtcNow.AddMinutes(-1) }
        ]);

        using var service = new RecentFilesService(repository);

        var result = await service.GetLastAsync();

        Assert.NotNull(result);
        Assert.Equal("normal.txt", result.Path, StringComparer.Ordinal);
    }

    [Fact]
    public async Task AddAsync_WhenPathIsNew_AddsEntryToRepositoryAndCacheAsync()
    {
        var repository = new InMemoryRecentFileRepository();
        using var service = new RecentFilesService(repository);
        var path = Path.GetTempFileName();

        try
        {
            File.SetLastWriteTimeUtc(path, new(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc));

            var result = await service.AddAsync("document", path, isRecovered: true);

            Assert.NotNull(result);
            Assert.Equal("document", result.Name, StringComparer.Ordinal);
            Assert.Equal(path, result.Path, StringComparer.OrdinalIgnoreCase);
            Assert.True(result.IsRecovered);
            Assert.True(result.LastModifiedAt.HasValue);
            Assert.True(await service.ContainsAsync(path));
            Assert.Equal(1, repository.AddCalls);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task AddAsync_WhenPathAlreadyExists_UpdatesExistingEntryAsync()
    {
        var existing = new RecentFile
        {
            Name = "document",
            Path = "existing.txt",
            LastAccessedAt = new DateTimeOffset(2026, 5, 1, 8, 0, 0, TimeSpan.Zero)
        };
        var repository = new InMemoryRecentFileRepository([existing]);
        using var service = new RecentFilesService(repository);

        var result = await service.AddAsync("document", "existing.txt");

        Assert.NotNull(result);
        Assert.True(result.LastAccessedAt > existing.LastAccessedAt);
        Assert.Equal(1, repository.UpdateCalls);
    }

    [Fact]
    public async Task PinAsync_WhenEntryExists_UpdatesPinnedStateAsync()
    {
        var repository = new InMemoryRecentFileRepository([
            new() { Name = "document", Path = "file.txt", LastAccessedAt = DateTimeOffset.UtcNow }
        ]);
        using var service = new RecentFilesService(repository);

        var result = await service.PinAsync("file.txt", true);

        Assert.NotNull(result);
        Assert.True(result.IsPinned);
    }

    [Fact]
    public async Task RemoveAsync_WhenEntryExists_RemovesEntryAsync()
    {
        var repository = new InMemoryRecentFileRepository([
            new() { Name = "document", Path = "file.txt" }
        ]);
        using var service = new RecentFilesService(repository);

        var removed = await service.RemoveAsync("file.txt");

        Assert.True(removed);
        Assert.False(await service.ContainsAsync("file.txt"));
    }

    [Fact]
    public async Task ClearAsync_RemovesAllEntriesFromCacheAndRepositoryAsync()
    {
        var repository = new InMemoryRecentFileRepository([
            new() { Name = "one", Path = "one.txt" },
            new() { Name = "two", Path = "two.txt" }
        ]);
        using var service = new RecentFilesService(repository);

        await service.ClearAsync();

        Assert.Empty(await service.GetAllAsync());
        Assert.Empty(repository.StoredFiles);
        Assert.Equal(2, repository.RemoveCalls);
    }

    private sealed class InMemoryRecentFileRepository(IEnumerable<RecentFile>? seed = null) : IRecentFileRepository
    {
        private readonly Dictionary<string, RecentFile> _files = (seed ?? [])
            .ToDictionary(x => x.Path, StringComparer.OrdinalIgnoreCase);

        public int GetAllCalls { get; private set; }

        public int AddCalls { get; private set; }

        public int UpdateCalls { get; private set; }

        public int RemoveCalls { get; private set; }

        public IReadOnlyCollection<RecentFile> StoredFiles => [.. _files.Values];

        public Task<IReadOnlyList<RecentFile>> GetAllAsync()
        {
            GetAllCalls++;
            return Task.FromResult<IReadOnlyList<RecentFile>>([.. _files.Values]);
        }

        public Task<RecentFile?> AddAsync(RecentFile file)
        {
            AddCalls++;
            _files[file.Path] = file;
            return Task.FromResult<RecentFile?>(file);
        }

        public Task<RecentFile?> UpdateAsync(RecentFile file)
        {
            UpdateCalls++;

            if (!_files.ContainsKey(file.Path)) return Task.FromResult<RecentFile?>(null);

            _files[file.Path] = file;
            return Task.FromResult<RecentFile?>(file);
        }

        public Task<bool> RemoveAsync(string path)
        {
            RemoveCalls++;
            return Task.FromResult(_files.Remove(path));
        }

        public Task ClearAsync()
        {
            _files.Clear();
            return Task.CompletedTask;
        }
    }
}
