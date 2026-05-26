// -----------------------------------------------------------------------
// <copyright file="ItemsFileProviderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Primitives.Providers;
using MyNet.IO;
using Xunit;

namespace MyNet.Utilities.Tests.IO;

public sealed class ItemsFileProviderTests
{
    [Fact]
    public async Task GetItemsAsync_WhenFilenameIsMissing_ThrowsInvalidOperationExceptionAsync()
    {
        var provider = new TestItemsFileProvider();

        await Assert.ThrowsAsync<InvalidOperationException>(async () => _ = await ToListAsync(provider.GetItemsAsync()).ConfigureAwait(false)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetItemsAsync_WhenFileDoesNotExist_ThrowsFileNotFoundExceptionAsync()
    {
        var provider = new TestItemsFileProvider(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        await Assert.ThrowsAsync<FileNotFoundException>(async () => _ = await ToListAsync(provider.GetItemsAsync()).ConfigureAwait(false)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetItemsAsync_UsesConfiguredFilenameAndStreamsItemsAsync()
    {
        var path = Path.GetTempFileName();

        try
        {
            var provider = new TestItemsFileProvider(path);

            var result = await ToListAsync(provider.GetItemsAsync());

            Assert.Equal([1, 2, 3], result);
            Assert.Equal(path, provider.LastFilename, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ValidateAsync_ReturnsErrorsFromOverrideAsync()
    {
        var path = Path.GetTempFileName();

        try
        {
            var provider = new TestItemsFileProvider(path);

            var result = await provider.ValidateAsync();

            Assert.Single(result);
            Assert.Equal("invalid row", result[0].Context, StringComparer.Ordinal);
            Assert.Equal("boom", result[0].Exception.Message, StringComparer.Ordinal);
            Assert.Equal(path, provider.ValidatedFilename, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void SetFilename_UpdatesFilename()
    {
        var provider = new TestItemsFileProvider();

        provider.SetFilename("file.txt");

        Assert.Equal("file.txt", provider.Filename, StringComparer.Ordinal);
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var result = new List<T>();
        await foreach (var item in source.ConfigureAwait(false)) result.Add(item);
        return result;
    }

    private sealed class TestItemsFileProvider(string? filename = null) : ItemsFileProvider<int>(filename)
    {
        public string? LastFilename { get; private set; }

        public string? ValidatedFilename { get; private set; }

        protected override async IAsyncEnumerable<int> GetItemsCoreAsync(string filename, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            LastFilename = filename;
            await Task.Yield();
            yield return 1;
            yield return 2;
            yield return 3;
        }

        protected override Task<List<ItemLoadError>> ValidateCoreAsync(string filename, CancellationToken cancellationToken)
        {
            ValidatedFilename = filename;
            return Task.FromResult(new List<ItemLoadError> { new(new InvalidOperationException("boom"), "invalid row") });
        }
    }
}
