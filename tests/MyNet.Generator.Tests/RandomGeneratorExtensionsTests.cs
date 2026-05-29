// -----------------------------------------------------------------------
// <copyright file="RandomGeneratorExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Generator.Facade;
using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Generator.Tests;

[Collection(RandomGeneratorTestCollection.Name)]
public sealed class RandomGeneratorExtensionsTests
{
    [Fact]
    public void SafeSubset_ClampsCountToListSize()
    {
        IReadOnlyList<int> list = [1, 2, 3, 4, 5];

        var subset = RandomGenerator.Current.SafeSubset(list, 100);

        Assert.Equal(5, subset.Count);
    }

    [Fact]
    public void TryItem_EmptyList_ReturnsDefault()
    {
        IReadOnlyList<string> empty = [];

        Assert.Null(RandomGenerator.Current.TryItem(empty));
    }

    [Fact]
    public void TryItem_NonEmptyList_ReturnsItem()
    {
        IReadOnlyList<string> list = ["a", "b"];

        Assert.Contains(RandomGenerator.Current.TryItem(list)!, list);
    }

    [Fact]
    public void SafeDate_SwapsInvertedBounds()
    {
        var min = new DateTime(2025, 1, 1);
        var max = new DateTime(2020, 1, 1);

        var result = RandomGenerator.Current.SafeDate(min, max);

        Assert.InRange(result, max, min);
    }

    [Fact]
    public void Date_WithInterval_UsesIntervalBounds()
    {
        var interval = new ClosedInterval<DateTime>(
            new(2020, 1, 1),
            new(2020, 12, 31));

        for (var i = 0; i < 20; i++)
        {
            var result = RandomGenerator.Current.Date(interval);
            Assert.InRange(result, interval.Start!.Value.Value, interval.End!.Value.Value);
        }
    }
}
