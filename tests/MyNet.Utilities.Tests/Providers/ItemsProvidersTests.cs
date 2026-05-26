// -----------------------------------------------------------------------
// <copyright file="ItemsProvidersTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Primitives.Providers;
using MyNet.Primitives.Providers;
using Xunit;

namespace MyNet.Utilities.Tests.Providers;

public class ItemsProvidersTests
{
    [Fact]
    public void ItemsProvider_GetItems_ReturnsProvidedItems()
    {
        var provider = new ItemsProvider<int>([1, 2, 3]);

        Assert.Equal([1, 2, 3], provider.GetItems());
    }

    [Fact]
    public void ItemsProvider_GetItems_WhenSourceIsNull_ThrowsArgumentNullException()
    {
        var provider = new ItemsProvider<int>(null!);

        Assert.Throws<ArgumentNullException>(provider.GetItems);
    }

    [Fact]
    public async Task ItemsProviderBase_GetItemsAsync_StreamsItemsFromGetItemsAsync()
    {
        var provider = new TestItemsProvider([10, 20, 30]);

        var result = await ToListAsync(provider.GetItemsAsync());

        Assert.Equal([10, 20, 30], result);
    }

    [Fact]
    public async Task PredicateItemsProvider_WithProvider_FiltersItemsAsync()
    {
        var baseProvider = new ItemsProvider<int>([1, 2, 3, 4, 5]);
        var provider = new PredicateItemsProvider<int>(baseProvider, x => x % 2 == 0);

        var result = await ToListAsync(provider.GetItemsAsync());

        Assert.Equal([2, 4], result);
    }

    [Fact]
    public async Task PredicateItemsProvider_WithEnumerableCtor_FiltersItemsAsync()
    {
        var provider = new PredicateItemsProvider<int>([1, 2, 3, 4], x => x > 2);

        var result = await ToListAsync(provider.GetItemsAsync());

        Assert.Equal([3, 4], result);
    }

    [Fact]
    public async Task PredicateItemsProvider_WhenUnderlyingProviderThrows_PropagatesExceptionAsync()
    {
        var provider = new PredicateItemsProvider<int>(new ThrowingItemsProvider(), _ => true);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => _ = await ToListAsync(provider.GetItemsAsync()).ConfigureAwait(false)).ConfigureAwait(true);
    }

    [Fact]
    public async Task PredicateItemsProvider_RespectsCancellationAsync()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var provider = new PredicateItemsProvider<int>(new CancellationAwareItemsProvider(), _ => true);

        await Assert.ThrowsAsync<OperationCanceledException>(async () => _ = await ToListAsync(provider.GetItemsAsync(cts.Token)).ConfigureAwait(false)).ConfigureAwait(true);
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source.ConfigureAwait(false))
            list.Add(item);

        return list;
    }

    private sealed class TestItemsProvider(IEnumerable<int> items) : ItemsProviderBase<int>
    {
        public override IEnumerable<int> GetItems() => items;
    }

    private sealed class ThrowingItemsProvider : IItemsProvider<int>
    {
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode", Justification = "Testing that the exception is propagated and the method is not enumerated.")]
        public async IAsyncEnumerable<int> GetItemsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            throw new InvalidOperationException("boom");
#pragma warning disable CS0162
            yield break;
#pragma warning restore CS0162
        }
    }

    private sealed class CancellationAwareItemsProvider : IItemsProvider<int>
    {
        public async IAsyncEnumerable<int> GetItemsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return 1;
        }
    }
}
