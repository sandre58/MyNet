// -----------------------------------------------------------------------
// <copyright file="DateRangeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using MyNet.Temporal.Intervals;
using Xunit;

namespace MyNet.Temporal.Tests.Intervals;

public sealed class DateRangeTests
{
    [Fact]
    public void DayCount_IncludesBothBoundaries()
    {
        var range = new DateRange(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 3));

        Assert.Equal(3, range.DayCount);
    }

    [Fact]
    public void EnumerateDays_ReturnsAllDays()
    {
        var range = new DateRange(new DateOnly(2024, 6, 10), new DateOnly(2024, 6, 12));

        Assert.Equal(
            [
                new DateOnly(2024, 6, 10),
                new DateOnly(2024, 6, 11),
                new DateOnly(2024, 6, 12)
            ],
            range.EnumerateDays().ToList());
    }

    [Fact]
    public void IsAdjacentTo_SharedBoundary_ReturnsTrue()
    {
        var left = new DateRange(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 5));
        var right = new DateRange(new DateOnly(2024, 1, 5), new DateOnly(2024, 1, 10));

        Assert.True(left.IsAdjacentTo(right));
        Assert.False(left.IsAdjacentTo(new DateRange(new DateOnly(2024, 1, 8), new DateOnly(2024, 1, 10))));
    }

    [Fact]
    public void SplitByMonth_SplitsAcrossMonths()
    {
        var range = new DateRange(new DateOnly(2024, 1, 15), new DateOnly(2024, 2, 10));

        var parts = range.SplitByMonth().ToList();

        Assert.Equal(2, parts.Count);
        Assert.Equal(new DateOnly(2024, 1, 15), parts[0].Start!.Value.Value);
        Assert.Equal(new DateOnly(2024, 1, 31), parts[0].End!.Value.Value);
    }

    [Fact]
    public void SplitByWeek_SplitsIntoWeekSegments()
    {
        var range = new DateRange(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 14));

        Assert.NotEmpty(range.SplitByWeek());
    }

    [Fact]
    public void Expand_AddsDaysOnBothSides()
    {
        var range = new DateRange(new DateOnly(2024, 5, 10), new DateOnly(2024, 5, 12));
        var expanded = range.Expand(2);

        Assert.Equal(new DateOnly(2024, 5, 8), expanded.Start!.Value.Value);
        Assert.Equal(new DateOnly(2024, 5, 14), expanded.End!.Value.Value);
    }
}
