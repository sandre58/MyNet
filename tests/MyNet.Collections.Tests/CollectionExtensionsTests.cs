// -----------------------------------------------------------------------
// <copyright file="CollectionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Xunit;

namespace MyNet.Collections.Tests;

public class CollectionExtensionsTests
{
    [Fact]
    public void Set_ClearsAndAddsItemsToCollection()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };
        var itemsToAdd = new[] { 4, 5, 6 };

        // Act
        collection.Set(itemsToAdd);

        // Assert
        Assert.Equal(itemsToAdd, collection);
    }

    [Fact]
    public void Set_ClearsCollectionWhenItemsIsNull()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };

        // Act
        collection.Set(null);

        // Assert
        Assert.Empty(collection);
    }

    [Fact]
    public void AddRange_AddsItemsToCollection()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };
        var itemsToAdd = new[] { 4, 5, 6 };

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, collection);
    }

    [Fact]
    public void Sort_SortsCollectionByAscendingOrderUsingKeySelector()
    {
        // Arrange
        var collection = new List<int> { 3, 1, 2 };

        // Act
        collection.Sort(x => x);

        // Assert
        Assert.Equal(new[] { 1, 2, 3 }, collection);
    }

    [Fact]
    public void SortDescending_SortsCollectionByDescendingOrderUsingKeySelector()
    {
        // Arrange
        var collection = new List<int> { 3, 1, 2 };

        // Act
        collection.SortDescending(x => x);

        // Assert
        Assert.Equal(new[] { 3, 2, 1 }, collection);
    }
}
