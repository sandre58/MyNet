// -----------------------------------------------------------------------
// <copyright file="PeriodExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using MyNet.Primitives;
using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public class PeriodExtensionsTests
{
    private static Period MakePeriod(DateTime start, DateTime end) => new(start, end);

    #region Shift

    [Fact]
    public void Shift_MovesPeriodByOffset()
    {
        var period = MakePeriod(new(2024, 1, 1, 8, 0, 0), new(2024, 1, 1, 10, 0, 0));
        var shifted = period.Shift(TimeSpan.FromHours(2));

        Assert.Equal(new(2024, 1, 1, 10, 0, 0), shifted.Start!.Value.Value);
        Assert.Equal(new(2024, 1, 1, 12, 0, 0), shifted.End!.Value.Value);
    }

    [Fact]
    public void Shift_WithNegativeOffset_MovesPeriodBack()
    {
        var period = MakePeriod(new(2024, 1, 1, 10, 0, 0), new(2024, 1, 1, 12, 0, 0));
        var shifted = period.Shift(TimeSpan.FromHours(-2));

        Assert.Equal(new(2024, 1, 1, 8, 0, 0), shifted.Start!.Value.Value);
        Assert.Equal(new(2024, 1, 1, 10, 0, 0), shifted.End!.Value.Value);
    }

    #endregion

    #region Extend

    [Fact]
    public void Extend_LengthensPeriodAtEnd()
    {
        var period = MakePeriod(new(2024, 1, 1, 8, 0, 0), new(2024, 1, 1, 10, 0, 0));
        var extended = period.Extend(TimeSpan.FromHours(3));

        Assert.Equal(new(2024, 1, 1, 8, 0, 0), extended.Start!.Value.Value);
        Assert.Equal(new(2024, 1, 1, 13, 0, 0), extended.End!.Value.Value);
    }

    #endregion

    #region EnumerateHours

    [Fact]
    public void EnumerateHours_ReturnsOneEntryPerHour()
    {
        var start = new DateTime(2024, 1, 1, 8, 0, 0);
        var end = new DateTime(2024, 1, 1, 11, 0, 0);
        var period = MakePeriod(start, end);

        var hours = period.EnumerateHours().ToList();

        Assert.Equal(3, hours.Count);
        Assert.Equal(start, hours[0]);
        Assert.Equal(start.AddHours(1), hours[1]);
        Assert.Equal(start.AddHours(2), hours[2]);
    }

    [Fact]
    public void EnumerateHours_SubHourPeriod_YieldsOnlyStartHour()
    {
        // A period [8:00, 8:30) yields just {8:00} because start < end and second step (9:00) exceeds end
        var start = new DateTime(2024, 1, 1, 8, 0, 0);
        var end = new DateTime(2024, 1, 1, 8, 30, 0);
        var period = MakePeriod(start, end);
        var hours = period.EnumerateHours().ToList();
        Assert.Single(hours);
        Assert.Equal(start, hours[0]);
    }

    #endregion

    #region EnumerateDays

    [Fact]
    public void EnumerateDays_ReturnsOneEntryPerDay()
    {
        var start = new DateTime(2024, 1, 1);
        var end = new DateTime(2024, 1, 4);
        var period = MakePeriod(start, end);

        var days = period.EnumerateDays().ToList();

        Assert.Equal(3, days.Count);
        Assert.Equal(start.Date, days[0]);
        Assert.Equal(start.Date.AddDays(1), days[1]);
        Assert.Equal(start.Date.AddDays(2), days[2]);
    }

    #endregion

    #region IsCurrent / IsPast / IsFuture

    [Fact]
    public void IsCurrent_WhenNowIsInsidePeriod_ReturnsTrue()
    {
        var period = MakePeriod(DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1));

        Assert.True(period.IsCurrent());
    }

    [Fact]
    public void IsPast_WhenPeriodEndedBeforeNow_ReturnsTrue()
    {
        var period = MakePeriod(DateTime.Now.AddHours(-3), DateTime.Now.AddHours(-1));

        Assert.True(period.IsPast());
        Assert.False(period.IsFuture());
    }

    [Fact]
    public void IsFuture_WhenPeriodStartsAfterNow_ReturnsTrue()
    {
        var period = MakePeriod(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));

        Assert.True(period.IsFuture());
        Assert.False(period.IsPast());
    }

    #endregion

    #region Gap

    [Fact]
    public void Gap_WithNonOverlappingPeriods_ReturnsElapsedTimeBetweenThem()
    {
        var left = MakePeriod(new(2024, 1, 1, 8, 0, 0), new(2024, 1, 1, 10, 0, 0));
        var right = MakePeriod(new(2024, 1, 1, 12, 0, 0), new(2024, 1, 1, 14, 0, 0));

        Assert.Equal(TimeSpan.FromHours(2), PeriodExtensions.Gap(left, right));
    }

    [Fact]
    public void Gap_WithOverlappingPeriods_ReturnsZero()
    {
        var left = MakePeriod(new(2024, 1, 1, 8, 0, 0), new(2024, 1, 1, 12, 0, 0));
        var right = MakePeriod(new(2024, 1, 1, 10, 0, 0), new(2024, 1, 1, 14, 0, 0));

        Assert.Equal(TimeSpan.Zero, PeriodExtensions.Gap(left, right));
    }

    #endregion

}
