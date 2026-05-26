// -----------------------------------------------------------------------
// <copyright file="NumericRangeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Utilities.Tests.Intervals;

public class NumericRangeTests
{
    [Fact]
    public void BoundedInterface_ReturnsInclusiveBoundaries()
    {
        var range = new NumericRange<int>(1, 3);
        IBoundedInterval<int> bounded = range;

        Assert.True(bounded.Start.IsInclusive);
        Assert.True(bounded.End.IsInclusive);
    }

    [Fact]
    public void LengthAndSignProperties_AreComputedFromBounds()
    {
        var range = new NumericRange<int>(1, 6);
        var point = new NumericRange<int>(5, 5);

        Assert.Equal(5, range.Length);
        Assert.True(range.IsPositive);
        Assert.False(range.IsNegative);

        Assert.Equal(0, point.Length);
        Assert.False(point.IsPositive);
        Assert.False(point.IsNegative);
    }

    [Fact]
    public void Clamp_UsesRangeBoundaries()
    {
        var range = new NumericRange<int>(10, 20);

        Assert.Equal(10, range.Clamp(4));
        Assert.Equal(20, range.Clamp(99));
        Assert.Equal(15, range.Clamp(15));
    }

    [Fact]
    public void Normalize_OnBoundedRange_ReturnsRelativePosition()
    {
        var range = new NumericRange<decimal>(10m, 20m);

        Assert.Equal(0.5m, range.Normalize(15m));
    }

    [Fact]
    public void Normalize_OnUnboundedNumericInterval_ThrowsInvalidOperationException()
    {
        var interval = new TestNumericInterval(null, new IntervalBoundary<decimal>(20m));

        Assert.Throws<InvalidOperationException>(() => interval.Normalize(10m));
    }

    [Fact]
    public void Intersection_OfOverlappingRanges_ReturnsBoundedInclusiveRange()
    {
        var left = new NumericRange<int>(1, 5);
        var right = new NumericRange<int>(3, 8);

        var intersection = left.Intersection(right);

        Assert.NotNull(intersection);
        Assert.Equal(new(3, 5), intersection);
    }

    [Fact]
    public void ExpandTo_WithExclusiveBoundaryRequest_ThrowsInvalidOperationException()
    {
        var range = new NumericRange<int>(10, 20);

        Assert.Throws<InvalidOperationException>(() => range.ExpandTo(5, inclusive: false));
    }

    [Fact]
    public void Gap_BetweenDisjointRanges_ThrowsInvalidOperationException()
    {
        var left = new NumericRange<int>(1, 2);
        var right = new NumericRange<int>(5, 6);

        Assert.Throws<InvalidOperationException>(() => left.Gap(right));
    }

    private sealed class TestNumericInterval(IntervalBoundary<decimal>? start, IntervalBoundary<decimal>? end) : NumericInterval<decimal, TestNumericInterval>(start, end)
    {
        protected override TestNumericInterval Create(IntervalBoundary<decimal>? start, IntervalBoundary<decimal>? end) => new(start, end);
    }
}
