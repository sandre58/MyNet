// -----------------------------------------------------------------------
// <copyright file="IntervalsRegressionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Primitives.Tests.Intervals;

public class IntervalsRegressionTests
{
    [Fact]
    public void CompareTo_Should_Use_End_As_TieBreaker_When_Starts_Are_Equal()
    {
        var left = new TestInterval(new IntervalBoundary<int>(1), new IntervalBoundary<int>(3));
        var right = new TestInterval(new IntervalBoundary<int>(1), new IntervalBoundary<int>(5));

        Assert.True(left.CompareTo(right) < 0);
        Assert.True(right.CompareTo(left) > 0);
    }

    [Fact]
    public void CompareTo_Should_Use_Start_Inclusivity_As_TieBreaker_When_Start_Values_Are_Equal()
    {
        var inclusiveStart = new TestInterval(new IntervalBoundary<int>(1), new IntervalBoundary<int>(4));
        var exclusiveStart = new TestInterval(new IntervalBoundary<int>(1, false), new IntervalBoundary<int>(4));

        Assert.True(inclusiveStart.CompareTo(exclusiveStart) < 0);
        Assert.True(exclusiveStart.CompareTo(inclusiveStart) > 0);
    }

    [Fact]
    public void Touches_Should_Be_Symmetric_And_True_For_Exclusive_Exclusive_Boundary()
    {
        var left = new TestInterval(new IntervalBoundary<int>(1), new IntervalBoundary<int>(2, false));
        var right = new TestInterval(new IntervalBoundary<int>(2, false), new IntervalBoundary<int>(3));

        Assert.True(left.Touches(right));
        Assert.True(right.Touches(left));
    }

    [Fact]
    public void Clamp_Extension_Should_Work_With_Unbounded_Interval()
    {
        var interval = new TestInterval(null, new IntervalBoundary<int>(5));

        Assert.Equal(-10, interval.Clamp(-10));
        Assert.Equal(5, interval.Clamp(12));
    }

    [Fact]
    public void DateTimeRange_Enumerate_Should_Respect_Inclusive_And_Exclusive_Bounds()
    {
        var start = new DateTime(2026, 5, 8, 10, 0, 0, DateTimeKind.Utc);
        var end = start.AddHours(2);
        var range = new DateTimeRange(start, end, inclusiveStart: false, inclusiveEnd: true);

        var values = range.Enumerate(TimeSpan.FromHours(1)).ToList();

        Assert.Equal([start.AddHours(1), end], values);
    }

    [Fact]
    public void DateTimeRange_Intersection_Should_Preserve_Inclusive_End_When_Creating_Result()
    {
        var t0 = new DateTime(2026, 5, 8, 8, 0, 0, DateTimeKind.Utc);
        var t2 = t0.AddHours(2);
        var t3 = t0.AddHours(3);

        var left = new DateTimeRange(t0, t2, inclusiveStart: true, inclusiveEnd: true);
        var right = new DateTimeRange(t2, t3, inclusiveStart: true, inclusiveEnd: true);

        var intersection = left.Intersection(right);

        Assert.NotNull(intersection);
        Assert.True(intersection.Contains(t2));
        Assert.True(intersection.Start!.Value.IsInclusive);
        Assert.True(intersection.End!.Value.IsInclusive);
    }

    [Fact]
    public void TemporalInterval_Status_Methods_Should_Respect_Boundary_Inclusivity()
    {
        var start = new DateTime(2026, 5, 8, 8, 0, 0, DateTimeKind.Utc);
        var end = start.AddHours(1);

        var endExclusive = new DateTimeRange(start, end, inclusiveStart: true, inclusiveEnd: false);
        var endInclusive = new DateTimeRange(start, end, inclusiveStart: true, inclusiveEnd: true);
        var startExclusive = new DateTimeRange(start, end, inclusiveStart: false, inclusiveEnd: true);

        Assert.True(endExclusive.IsPast(end));
        Assert.False(endInclusive.IsPast(end));
        Assert.True(startExclusive.IsFuture(start));
    }

    [Fact]
    public void TimeRange_Should_Support_CrossMidnight_And_Closed_End()
    {
        var range = new TimeRange(new(22, 0), new(2, 0));

        Assert.True(range.CrossesMidnight);
        Assert.Equal(TimeSpan.FromHours(4), range.Duration);
        Assert.True(range.Contains(new TimeOnly(23, 0)));
        Assert.True(range.Contains(new TimeOnly(1, 0)));
        Assert.True(range.Contains(new TimeOnly(2, 0)));
    }

    [Fact]
    public void TimeRange_Enumerate_Should_Include_End_When_It_Matches_Step_Across_Midnight()
    {
        var range = new TimeRange(new(22, 0), new(2, 0));

        var values = range.Enumerate(TimeSpan.FromHours(1)).ToList();

        Assert.Equal(
            [
                new(22, 0),
                new(23, 0),
                new(0, 0),
                new(1, 0),
                new(2, 0)
            ],
            values);
    }

    [Fact]
    public void ClosedInterval_Should_Reject_Open_Boundaries_During_Gap_Computation()
    {
        var left = new ClosedInterval<int>(1, 2);
        var right = new ClosedInterval<int>(5, 6);

        Assert.Throws<InvalidOperationException>(() => left.Gap(right));
    }

    private sealed class TestInterval(IntervalBoundary<int>? start, IntervalBoundary<int>? end) : Interval<int, TestInterval>(start, end)
    {
        protected override TestInterval Create(IntervalBoundary<int>? start, IntervalBoundary<int>? end)
            => new(start, end);
    }
}
