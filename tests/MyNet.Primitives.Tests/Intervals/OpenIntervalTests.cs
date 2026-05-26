// -----------------------------------------------------------------------
// <copyright file="OpenIntervalTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Primitives.Tests.Intervals;

public class OpenIntervalTests
{
    [Fact]
    public void Constructor_CreatesExclusiveBoundaries()
    {
        var interval = new OpenInterval<int>(1, 5);

        Assert.False(interval.Contains(1));
        Assert.True(interval.Contains(3));
        Assert.False(interval.Contains(5));
    }

    [Fact]
    public void BoundedInterface_ReturnsExclusiveBoundaries()
    {
        var interval = new OpenInterval<int>(1, 5);
        IBoundedInterval<int> bounded = interval;

        Assert.False(bounded.Start.IsInclusive);
        Assert.False(bounded.End.IsInclusive);
    }

    [Fact]
    public void ExpandTo_WithDefaultInclusiveBoundary_ThrowsInvalidOperationException()
    {
        var interval = new OpenInterval<int>(1, 5);

        Assert.Throws<InvalidOperationException>(() => interval.ExpandTo(0));
    }

    [Fact]
    public void ExpandTo_WithExclusiveBoundary_Succeeds()
    {
        var interval = new OpenInterval<int>(1, 5);

        var expanded = interval.ExpandTo(0, inclusive: false);

        Assert.Equal(new(0, 5), expanded);
    }

    [Fact]
    public void Gap_BetweenDisjointOpenIntervals_ThrowsInvalidOperationException()
    {
        var left = new OpenInterval<int>(1, 2);
        var right = new OpenInterval<int>(4, 5);

        Assert.Throws<InvalidOperationException>(() => left.Gap(right));
    }
}
