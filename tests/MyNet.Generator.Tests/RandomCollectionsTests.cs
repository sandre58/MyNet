// -----------------------------------------------------------------------
// <copyright file="RandomCollectionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Generator.Facade;
using Xunit;

namespace MyNet.Generator.Tests;

[Collection(RandomGeneratorTestCollection.Name)]
public class RandomCollectionsTests
{
    [Fact]
    public void Item_ReturnsElementFromList()
    {
        IReadOnlyList<int> list = [1, 2, 3, 4, 5];

        for (var i = 0; i < 50; i++)
        {
            var result = RandomGenerator.Current.Item(list);
            Assert.Contains(result, list);
        }
    }

    [Fact]
    public void Item_EmptyList_ThrowsArgumentException()
    {
        IReadOnlyList<int> empty = [];
        _ = Assert.Throws<ArgumentException>(() => RandomGenerator.Current.Item(empty));
    }

    [Fact]
    public void Item_SingleElement_AlwaysReturnsThatElement()
    {
        IReadOnlyList<string> list = ["only"];
        for (var i = 0; i < 20; i++)
            Assert.Equal("only", RandomGenerator.Current.Item(list));
    }

    [Fact]
    public void Shuffle_PreservesAllElements()
    {
        var source = Enumerable.Range(1, 10).ToList();
        var shuffled = RandomGenerator.Current.Shuffle(source).ToList();

        Assert.Equal(source.Count, shuffled.Count);
        Assert.Equal(source.Order(), shuffled.Order());
    }

    [Fact]
    public void Shuffle_EmptySource_ReturnsEmpty()
    {
        var result = RandomGenerator.Current.Shuffle(Array.Empty<int>());
        Assert.Empty(result);
    }

    [Fact]
    public void Subset_ReturnsCorrectCount()
    {
        IReadOnlyList<int> list = [1, 2, 3, 4, 5];

        for (var count = 0; count <= list.Count; count++)
        {
            var result = RandomGenerator.Current.Subset(list, count);
            Assert.Equal(count, result.Count);
        }
    }

    [Fact]
    public void Subset_ElementsAreFromSource()
    {
        IReadOnlyList<int> list = [1, 2, 3, 4, 5];
        var result = RandomGenerator.Current.Subset(list, 3);

        foreach (var item in result)
            Assert.Contains(item, list);
    }

    [Fact]
    public void Subset_CountGreaterThanListSize_ThrowsArgumentOutOfRangeException()
    {
        IReadOnlyList<int> list = [1, 2, 3];
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => RandomGenerator.Current.Subset(list, 4));
    }

    [Fact]
    public void Subset_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        IReadOnlyList<int> list = [1, 2, 3];
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => RandomGenerator.Current.Subset(list, -1));
    }

    [Fact]
    public void Subset_FullList_ReturnsAllElementsInAnyOrder()
    {
        IReadOnlyList<int> list = [10, 20, 30];
        var result = RandomGenerator.Current.Subset(list, 3);

        Assert.Equal(3, result.Count);
        Assert.Equal(list.Order(), result.Order());
    }

    [Fact]
    public void Subset_NoRepetitions()
    {
        IReadOnlyList<int> list = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var result = RandomGenerator.Current.Subset(list, 5);
        Assert.Equal(result.Count, result.Distinct().Count());
    }
}
