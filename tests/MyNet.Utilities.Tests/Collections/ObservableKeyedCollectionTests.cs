// -----------------------------------------------------------------------
// <copyright file="ObservableKeyedCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities.Collections;
using Xunit;

namespace MyNet.Utilities.Tests.Collections;

public class ObservableKeyedCollectionTests
{
    [Fact]
    public void Indexer_WithExistingKey_ShouldReturnItem()
    {
        // Arrange
        var collection = new TestKeyedCollection();
        var item = new KeyedItem { Key = "key1", Value = "value1" };
        collection.Add(item);

        // Act
        var result = collection["key1"];

        // Assert
        Assert.NotNull(result);
        Assert.Equal("value1", result.Value);
    }

    [Fact]
    public void Indexer_WithNonExistingKey_ShouldReturnNull()
    {
        // Arrange
        var collection = new TestKeyedCollection();

        // Act
        var result = collection["nonexistent"];

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Contains_WithExistingKey_ShouldReturnTrue()
    {
        // Arrange
        var collection = new TestKeyedCollection
        {
            new KeyedItem { Key = "key1", Value = "value1" }
        };

        // Act
        var result = collection.Contains("key1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_WithNonExistingKey_ShouldReturnFalse()
    {
        // Arrange
        var collection = new TestKeyedCollection();

        // Act
        var result = collection.Contains("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryAdd_WithNewKey_ShouldAddAndReturnTrue()
    {
        // Arrange
        var collection = new TestKeyedCollection();
        var item = new KeyedItem { Key = "key1", Value = "value1" };

        // Act
        var result = collection.TryAdd(item);

        // Assert
        Assert.True(result);
        Assert.Contains(item, collection);
    }

    [Fact]
    public void TryAdd_WithExistingKey_ShouldReturnFalse()
    {
        // Arrange
        var collection = new TestKeyedCollection
        {
            new KeyedItem { Key = "key1", Value = "value1" }
        };

        // Act
        var result = collection.TryAdd(new KeyedItem { Key = "key1", Value = "value2" });

        // Assert
        Assert.False(result);
        Assert.Single(collection);
    }

    [Fact]
    public void Remove_ByKey_WithExistingKey_ShouldRemoveAndReturnTrue()
    {
        // Arrange
        var collection = new TestKeyedCollection
        {
            new KeyedItem { Key = "key1", Value = "value1" }
        };

        // Act
        var result = collection.Remove("key1");

        // Assert
        Assert.True(result);
        Assert.Empty(collection);
    }

    [Fact]
    public void Remove_ByKey_WithNonExistingKey_ShouldReturnFalse()
    {
        // Arrange
        var collection = new TestKeyedCollection();

        // Act
        var result = collection.Remove("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryGetValue_WithExistingKey_ShouldReturnTrueAndValue()
    {
        // Arrange
        var collection = new TestKeyedCollection();
        var item = new KeyedItem { Key = "key1", Value = "value1" };
        collection.Add(item);

        // Act
        var result = collection.TryGetValue("key1", out var value);

        // Assert
        Assert.True(result);
        Assert.NotNull(value);
        Assert.Equal("value1", value.Value);
    }

    [Fact]
    public void TryGetValue_WithNonExistingKey_ShouldReturnFalse()
    {
        // Arrange
        var collection = new TestKeyedCollection();

        // Act
        var result = collection.TryGetValue("nonexistent", out var value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void DictionaryCreation_ShouldOccurAtThreshold()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 5);

        // Act - Add 4 items (below threshold)
        for (var i = 0; i < 5; i++)
        {
            collection.Add(new KeyedItem { Key = $"key{i}", Value = $"value{i}" });
        }

        // Assert - Dictionary not created yet
        Assert.False(collection.IsDictionaryCreatedPublic);

        // Act - Add 5th item (at threshold)
        collection.Add(new KeyedItem { Key = "key4", Value = "value4" });

        // Assert - Dictionary should be created now
        Assert.True(collection.IsDictionaryCreatedPublic);
    }

    [Fact]
    public void CreateDictionaryNow_ShouldForceCreation()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 100)
        {
            new KeyedItem { Key = "key1", Value = "value1" }
        };

        // Act
        collection.CreateDictionaryNowPublic();

        // Assert
        Assert.True(collection.IsDictionaryCreatedPublic);
    }

    [Fact]
    public void Indexer_BeforeDictionaryCreated_ShouldStillWork()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 100);
        var item = new KeyedItem { Key = "key1", Value = "value1" };
        collection.Add(item);

        // Act
        var result = collection["key1"];

        // Assert
        Assert.NotNull(result);
        Assert.Equal("value1", result.Value);
    }

    [Fact]
    public void Remove_AfterDictionaryCreated_ShouldUseDictionary()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 0); // Immediate dict
        var item = new KeyedItem { Key = "key1", Value = "value1" };
        collection.Add(item);
        Assert.True(collection.IsDictionaryCreatedPublic);

        // Act
        var result = collection.Remove("key1");

        // Assert
        Assert.True(result);
        Assert.Empty(collection);
    }

    [Fact]
    public void SetItem_ShouldUpdateDictionary()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 0)
        {
            new KeyedItem { Key = "key1", Value = "value1" }
        };

        // Act
        collection[0] = new KeyedItem { Key = "key2", Value = "value2" };

        // Assert
        Assert.False(collection.Contains("key1"));
        Assert.True(collection.Contains("key2"));
        Assert.Equal("value2", collection["key2"]?.Value);
    }

    [Fact]
    public void Clear_ShouldClearDictionary()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 0);
        collection.AddRange(
  [
new KeyedItem { Key = "key1", Value = "value1" },
            new KeyedItem { Key = "key2", Value = "value2" }
  ]);

        // Act
        collection.Clear();

        // Assert
        Assert.Empty(collection);
        Assert.False(collection.Contains("key1"));
        Assert.False(collection.Contains("key2"));
    }

    [Fact]
    public void GetDictionaryStats_ShouldReturnCorrectInfo()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 0);
        collection.AddRange(
        [
           new KeyedItem { Key = "key1", Value = "value1" },
           new KeyedItem { Key = "key2", Value = "value2" },
           new KeyedItem { Key = "key3", Value = "value3" }
         ]);

        // Act
        var (created, count, capacity) = collection.GetDictionaryStatsPublic();

        // Assert
        Assert.True(created);
        Assert.Equal(3, count);
        Assert.True(capacity >= 3);
    }

    [Fact]
    public void ThreadSafety_ConcurrentAccess_ShouldWork()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 0);

        // Act
        System.Threading.Tasks.Parallel.For(0, 100, i =>
   {
       var item = new KeyedItem { Key = $"key{i}", Value = $"value{i}" };
       collection.Add(item);
   });

        // Assert
        Assert.Equal(100, collection.Count);
        Assert.True(collection.IsDictionaryCreatedPublic);
    }

    [Fact]
    public void DuplicateKeys_ShouldHandleGracefully()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 0)
        {
            new KeyedItem { Key = "key1", Value = "value1" }
        };

        // Act & Assert - TryAdd should handle duplicate
        var result = collection.TryAdd(new KeyedItem { Key = "key1", Value = "value2" });
        Assert.False(result);
        Assert.Single(collection);
    }

    [Fact]
    public void NullKey_ShouldBeHandled()
    {
        // Arrange
        var collection = new TestKeyedCollection();
        var item = new KeyedItem { Key = null, Value = "value1" };

        // Act
        collection.Add(item); // Should not add to dictionary

        // Assert
        Assert.Contains(item, collection);
    }

    [Fact]
    public void ChangeItemKey_ShouldUpdateDictionary()
    {
        // Arrange
        var collection = new TestKeyedCollection(threshold: 0);
        var item = new KeyedItem { Key = "key1", Value = "value1" };
        collection.Add(item);

        // Verify initial state
        Assert.True(collection.Contains("key1"));
        Assert.False(collection.Contains("key2"));

        // Act - Capture old key, change property, then notify collection
        var oldKey = item.Key;
        collection.ChangeItemKeyPublic(item, "key2", oldKey); // Pass old key!

        // Assert
        Assert.False(collection.Contains("key1"));
        Assert.True(collection.Contains("key2"));
        Assert.Equal(item, collection["key2"]);
    }

    [Fact]
    public void Sort_WithKeys_ShouldMaintainDictionary()
    {
        // Arrange
        var collection = new TestKeyedCollection(x => x.Value, threshold: 0);
        collection.AddRange(
  [
    new KeyedItem { Key = "key3", Value = "C" },
       new KeyedItem { Key = "key1", Value = "A" },
            new KeyedItem { Key = "key2", Value = "B" }
  ]);

        // Act
        collection.Sort();

        // Assert
        Assert.Equal("A", collection[0].Value);
        Assert.Equal("B", collection[1].Value);
        Assert.Equal("C", collection[2].Value);

        // Dictionary should still work
        Assert.NotNull(collection["key1"]);
        Assert.NotNull(collection["key2"]);
        Assert.NotNull(collection["key3"]);
    }

    // Test helper classes
    private sealed class KeyedItem
    {
        public string? Key { get; set; }

        public string Value { get; set; } = string.Empty;
    }

    private sealed class TestKeyedCollection : ObservableKeyedCollection<string, KeyedItem>
    {
        public TestKeyedCollection(int threshold = 0)
              : base(comparer: null!, dictionaryCreationThreshold: threshold) { }

        public TestKeyedCollection(Func<KeyedItem, object> sortSelector, int threshold = 0)
             : base(sortSelector, dictionaryCreationThreshold: threshold) { }

        protected override string? GetKeyForItem(KeyedItem item) => item.Key;

        public void ChangeItemKeyPublic(KeyedItem item, string? newKey)
            => ChangeItemKey(item, newKey);

        public void ChangeItemKeyPublic(KeyedItem item, string? newKey, string? oldKey)
            => ChangeItemKey(item, newKey, oldKey);

        // Expose protected members for testing
        public bool IsDictionaryCreatedPublic => IsDictionaryCreated;

        public void CreateDictionaryNowPublic() => CreateDictionaryNow();

        public (bool Created, int Count, int Capacity) GetDictionaryStatsPublic() => GetDictionaryStats();
    }
}
