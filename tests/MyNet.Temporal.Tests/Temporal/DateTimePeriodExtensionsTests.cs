// -----------------------------------------------------------------------
// <copyright file="DateTimePeriodExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using MyNet.Temporal.Intervals;
using Xunit;

namespace MyNet.Temporal.Tests.Temporal;

public class DateTimePeriodExtensionsTests
{
    private static DateTimePeriod MakeDateTimePeriod(DateTime start, DateTime end) => new(start, end);

    [Fact]
    public void Shift_MovesPeriodByOffset()
    {
        var period = MakeDateTimePeriod(new DateTime(2024, 1, 1, 8, 0, 0), new DateTime(2024, 1, 1, 10, 0, 0));
        var shifted = period.Shift(TimeSpan.FromHours(2));

        Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), shifted.Start!.Value.Value);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), shifted.End!.Value.Value);
    }

    [Fact]
    public void Shift_WithNegativeOffset_MovesPeriodBack()
    {
        var period = MakeDateTimePeriod(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0));
        var shifted = period.Shift(TimeSpan.FromHours(-2));

        Assert.Equal(new DateTime(2024, 1, 1, 8, 0, 0), shifted.Start!.Value.Value);
        Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), shifted.End!.Value.Value);
    }

    [Fact]
    public void Extend_LengthensPeriodAtEnd()
    {
        var period = MakeDateTimePeriod(new DateTime(2024, 1, 1, 8, 0, 0), new DateTime(2024, 1, 1, 10, 0, 0));
        var extended = period.Extend(TimeSpan.FromHours(3));

        Assert.Equal(new DateTime(2024, 1, 1, 8, 0, 0), extended.Start!.Value.Value);
        Assert.Equal(new DateTime(2024, 1, 1, 13, 0, 0), extended.End!.Value.Value);
    }

    [Fact]
    public void EnumerateHours_ReturnsOneEntryPerHour()
    {
        var start = new DateTime(2024, 1, 1, 8, 0, 0);
        var end = new DateTime(2024, 1, 1, 11, 0, 0);
        var period = MakeDateTimePeriod(start, end);

        var hours = period.EnumerateHours().ToList();

        Assert.Equal(3, hours.Count);
        Assert.Equal(start, hours[0]);
        Assert.Equal(start.AddHours(1), hours[1]);
        Assert.Equal(start.AddHours(2), hours[2]);
    }

    [Fact]
    public void EnumerateHours_SubHourPeriod_YieldsOnlyStartHour()
    {
        var start = new DateTime(2024, 1, 1, 8, 0, 0);
        var end = new DateTime(2024, 1, 1, 8, 30, 0);
        var period = MakeDateTimePeriod(start, end);
        var hours = period.EnumerateHours().ToList();

        Assert.Single(hours);
        Assert.Equal(start, hours[0]);
    }

    [Fact]
    public void EnumerateDays_ReturnsOneEntryPerDay()
    {
        var start = new DateTime(2024, 1, 1, 8, 0, 0);
        var end = new DateTime(2024, 1, 4, 8, 0, 0);
        var period = MakeDateTimePeriod(start, end);

        var days = period.EnumerateDays().ToList();

        Assert.Equal(4, days.Count);
        Assert.Equal(start.Date, days[0]);
        Assert.Equal(start.Date.AddDays(1), days[1]);
        Assert.Equal(start.Date.AddDays(2), days[2]);
        Assert.Equal(start.Date.AddDays(3), days[3]);
    }

    [Fact]
    public void IsCurrent_WhenNowIsInsidePeriod_ReturnsTrue()
    {
        var now = new DateTime(2024, 6, 1, 12, 0, 0);
        var period = MakeDateTimePeriod(now.AddHours(-1), now.AddHours(1));

        Assert.True(period.IsCurrent(now));
    }

    [Fact]
    public void IsPast_WhenPeriodEndedBeforeNow_ReturnsTrue()
    {
        var now = new DateTime(2024, 6, 1, 12, 0, 0);
        var period = MakeDateTimePeriod(now.AddHours(-3), now.AddHours(-1));

        Assert.True(period.IsPast(now));
        Assert.False(period.IsFuture(now));
    }

    [Fact]
    public void IsFuture_WhenPeriodStartsAfterNow_ReturnsTrue()
    {
        var now = new DateTime(2024, 6, 1, 12, 0, 0);
        var period = MakeDateTimePeriod(now.AddHours(1), now.AddHours(3));

        Assert.True(period.IsFuture(now));
        Assert.False(period.IsPast(now));
    }

    [Fact]
    public void Gap_WithNonOverlappingPeriods_ReturnsElapsedTimeBetweenThem()
    {
        var left = MakeDateTimePeriod(new DateTime(2024, 1, 1, 8, 0, 0), new DateTime(2024, 1, 1, 10, 0, 0));
        var right = MakeDateTimePeriod(new DateTime(2024, 1, 1, 12, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0));

        Assert.Equal(TimeSpan.FromHours(2), left.TimeGap(right));
    }

    [Fact]
    public void Gap_WithOverlappingPeriods_ReturnsZero()
    {
        var left = MakeDateTimePeriod(new DateTime(2024, 1, 1, 8, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0));
        var right = MakeDateTimePeriod(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0));

        Assert.Equal(TimeSpan.Zero, left.TimeGap(right));
    }
}
