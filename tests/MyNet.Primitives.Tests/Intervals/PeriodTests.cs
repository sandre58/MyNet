// -----------------------------------------------------------------------
// <copyright file="PeriodTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Primitives.Tests.Intervals;

public class PeriodTests
{
    [Fact]
    public void Period_IsStartInclusive_AndEndExclusive()
    {
        var start = new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc);
        var end = start.AddHours(1);
        var period = new Period(start, end);

        Assert.True(period.Contains(start));
        Assert.False(period.Contains(end));
    }

    [Fact]
    public void FromDuration_Shift_And_Extend_WorkAsExpected()
    {
        var start = new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc);
        var period = Period.FromDuration(start, TimeSpan.FromHours(2));

        var shifted = period.Shift(TimeSpan.FromMinutes(30));
        var extended = period.Extend(TimeSpan.FromMinutes(45));

        Assert.Equal(start.AddHours(2), period.End!.Value.Value);
        Assert.Equal(start.AddMinutes(30), shifted.Start!.Value.Value);
        Assert.Equal(start.AddHours(2).AddMinutes(30), shifted.End!.Value.Value);
        Assert.Equal(start.AddHours(2).AddMinutes(45), extended.End!.Value.Value);
    }

    [Fact]
    public void Gap_BetweenDisjointPeriods_ReturnsPeriodWithValidBoundaries()
    {
        var start = new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc);
        var left = new Period(start, start.AddHours(1));
        var right = new Period(start.AddHours(3), start.AddHours(4));

        var gap = left.Gap(right);

        Assert.NotNull(gap);
        Assert.Equal(start.AddHours(1), gap.Start!.Value.Value);
        Assert.True(gap.Start!.Value.IsInclusive);
        Assert.Equal(start.AddHours(3), gap.End!.Value.Value);
        Assert.False(gap.End!.Value.IsInclusive);
    }

    [Fact]
    public void ExpandTo_WithExclusiveBoundaryRequest_ThrowsInvalidOperationException()
    {
        var start = new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc);
        var period = new Period(start, start.AddHours(1));

        Assert.Throws<InvalidOperationException>(() => period.ExpandTo(start.AddHours(-1), inclusive: false));
    }
}
