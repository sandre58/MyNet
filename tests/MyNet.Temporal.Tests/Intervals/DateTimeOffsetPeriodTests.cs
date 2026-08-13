// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetPeriodTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Temporal;
using MyNet.Temporal.Intervals;
using Xunit;

namespace MyNet.Temporal.Tests.Intervals;

public class DateTimeOffsetPeriodTests
{
    [Fact]
    public void DateTimeOffsetPeriod_IsStartInclusive_AndEndExclusive()
    {
        var start = new DateTimeOffset(2026, 5, 10, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(1);
        var period = new DateTimeOffsetPeriod(start, end);

        Assert.True(period.Contains(start));
        Assert.False(period.Contains(end));
    }

    [Fact]
    public void FromDuration_Shift_And_Extend_WorkAsExpected()
    {
        var start = new DateTimeOffset(2026, 5, 10, 8, 0, 0, TimeSpan.FromHours(2));
        var period = DateTimeOffsetPeriod.FromDuration(start, TimeSpan.FromHours(2));

        var shifted = period.Shift(TimeSpan.FromMinutes(30));
        var extended = period.Extend(TimeSpan.FromMinutes(45));

        Assert.Equal(start.AddHours(2), period.End!.Value.Value);
        Assert.Equal(start.AddMinutes(30), shifted.Start!.Value.Value);
        Assert.Equal(start.AddHours(2).AddMinutes(30), shifted.End!.Value.Value);
        Assert.Equal(start.AddHours(2).AddMinutes(45), extended.End!.Value.Value);
    }

    [Fact]
    public void Contains_ComparesByInstant_AcrossDifferentOffsets()
    {
        // Same UTC instant: 10:00+00:00 == 12:00+02:00
        var startUtc = new DateTimeOffset(2026, 5, 10, 10, 0, 0, TimeSpan.Zero);
        var endUtc = startUtc.AddHours(2);
        var period = new DateTimeOffsetPeriod(startUtc, endUtc);

        var sameInstantAsStart = new DateTimeOffset(2026, 5, 10, 12, 0, 0, TimeSpan.FromHours(2));
        var sameInstantAsEnd = new DateTimeOffset(2026, 5, 10, 14, 0, 0, TimeSpan.FromHours(2));
        var insideWithOtherOffset = new DateTimeOffset(2026, 5, 10, 5, 30, 0, TimeSpan.FromHours(-5));

        Assert.True(period.Contains(sameInstantAsStart));
        Assert.False(period.Contains(sameInstantAsEnd));
        Assert.True(period.Contains(insideWithOtherOffset));
        Assert.Equal(TimeSpan.FromHours(2), period.Duration);
    }

    [Fact]
    public void ExpandTo_WithExclusiveBoundaryRequest_ThrowsInvalidOperationException()
    {
        var start = new DateTimeOffset(2026, 5, 10, 8, 0, 0, TimeSpan.Zero);
        var period = new DateTimeOffsetPeriod(start, start.AddHours(1));

        Assert.Throws<InvalidOperationException>(() => period.ExpandTo(start.AddHours(-1), inclusive: false));
    }

    [Fact]
    public void ToDateTimePeriod_PreservesWallClockAsUnspecified()
    {
        var start = new DateTimeOffset(2026, 5, 10, 9, 0, 0, TimeSpan.FromHours(2));
        var end = new DateTimeOffset(2026, 5, 10, 10, 0, 0, TimeSpan.FromHours(2));
        var period = new DateTimeOffsetPeriod(start, end);

        var civil = period.ToDateTimePeriod();

        Assert.Equal(new DateTime(2026, 5, 10, 9, 0, 0), civil.Start!.Value.Value);
        Assert.Equal(DateTimeKind.Unspecified, civil.Start!.Value.Value.Kind);
        Assert.Equal(new DateTime(2026, 5, 10, 10, 0, 0), civil.End!.Value.Value);
    }

    [Fact]
    public void ToUtcDateTimePeriod_UsesUtcClockFace()
    {
        var start = new DateTimeOffset(2026, 5, 10, 9, 0, 0, TimeSpan.FromHours(2));
        var end = new DateTimeOffset(2026, 5, 10, 10, 0, 0, TimeSpan.FromHours(2));
        var period = new DateTimeOffsetPeriod(start, end);

        var utc = period.ToUtcDateTimePeriod();

        Assert.Equal(new DateTime(2026, 5, 10, 7, 0, 0, DateTimeKind.Utc), utc.Start!.Value.Value);
        Assert.Equal(new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc), utc.End!.Value.Value);
    }

    [Fact]
    public void ToOffsetPeriod_Unspecified_AttachesOffsetAsWallTime()
    {
        var civil = new DateTimePeriod(
            new DateTime(2026, 5, 10, 9, 0, 0, DateTimeKind.Unspecified),
            new DateTime(2026, 5, 10, 10, 0, 0, DateTimeKind.Unspecified));

        var offsetPeriod = civil.ToOffsetPeriod(TimeSpan.FromHours(2));

        Assert.Equal(new DateTimeOffset(2026, 5, 10, 9, 0, 0, TimeSpan.FromHours(2)), offsetPeriod.Start!.Value.Value);
        Assert.Equal(new DateTimeOffset(2026, 5, 10, 10, 0, 0, TimeSpan.FromHours(2)), offsetPeriod.End!.Value.Value);
    }

    [Fact]
    public void ToOffsetPeriod_Utc_PreservesInstant()
    {
        var civil = new DateTimePeriod(
            new DateTime(2026, 5, 10, 7, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc));

        var offsetPeriod = civil.ToOffsetPeriod(TimeSpan.FromHours(2));

        Assert.Equal(new DateTimeOffset(2026, 5, 10, 9, 0, 0, TimeSpan.FromHours(2)), offsetPeriod.Start!.Value.Value);
        Assert.Equal(TimeSpan.FromHours(1), offsetPeriod.Duration);
    }
}
