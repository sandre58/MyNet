// -----------------------------------------------------------------------
// <copyright file="ReadOnlyObservableKeyedCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Collections.Tests;

public sealed class ReadOnlyObservableKeyedCollectionTests
{
    [Fact]
    public void Indexer_WithExistingKey_ReturnsItem()
    {
        var keyed = new TestKeyedCollection { new("k1", "value1") };

        var readOnly = new ReadOnlyObservableKeyedCollection<string, KeyedValue>(keyed);

        Assert.Equal("value1", readOnly["k1"]?.Value);
    }

    [Fact]
    public void Indexer_WithMissingKey_ReturnsNull()
    {
        var keyed = new TestKeyedCollection();
        var readOnly = new ReadOnlyObservableKeyedCollection<string, KeyedValue>(keyed);

        Assert.Null(readOnly["missing"]);
    }

    private sealed record KeyedValue(string Key, string Value);

    private sealed class TestKeyedCollection : ObservableKeyedCollection<string, KeyedValue>
    {
        protected override string GetKeyForItem(KeyedValue item) => item.Key;
    }
}
