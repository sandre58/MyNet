// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Temporal;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public sealed class DateTimeOffsetExtensionsTests
{
    private static readonly DateTimeOffset Sample = new(2024, 6, 15, 14, 30, 45, 123, TimeSpan.Zero);

    [Fact]
    public void BeginningOfDay_And_EndOfDay_SetBoundaries()
    {
        var start = Sample.BeginningOfDay();
        var end = Sample.EndOfDay();

        Assert.Equal(0, start.Hour);
        Assert.Equal(23, end.Hour);
        Assert.Equal(59, end.Second);
    }

    [Fact]
    public void NextDay_And_PreviousDay_MoveByOneDay()
    {
        Assert.Equal(Sample.Date.AddDays(1), Sample.NextDay().Date);
        Assert.Equal(Sample.Date.AddDays(-1), Sample.PreviousDay().Date);
    }

    [Fact]
    public void SetTime_UpdatesTimeParts()
    {
        var updated = Sample.SetTime(8, 15, 20, 500);

        Assert.Equal(8, updated.Hour);
        Assert.Equal(15, updated.Minute);
        Assert.Equal(20, updated.Second);
        Assert.Equal(500, updated.Millisecond);
    }

    [Fact]
    public void Noon_SetsMidday()
    {
        var noon = Sample.Noon();
        Assert.Equal(12, noon.Hour);
        Assert.Equal(0, noon.Minute);
    }

    [Fact]
    public void Round_ToMinute_RoundsCorrectly()
    {
        var value = new DateTimeOffset(2024, 1, 1, 10, 14, 45, TimeSpan.Zero);
        var rounded = value.Round(RoundTo.Minute);

        Assert.Equal(10, rounded.Hour);
        Assert.Equal(15, rounded.Minute);
    }

    [Fact]
    public void AddBusinessDays_SkipsWeekends()
    {
        var friday = new DateTimeOffset(2024, 6, 14, 9, 0, 0, TimeSpan.Zero);
        var result = friday.AddBusinessDays(1);

        Assert.Equal(DayOfWeek.Monday, result.DayOfWeek);
    }

    [Fact]
    public void SameDayMonthYear_CompareCalendarParts()
    {
        var other = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

        Assert.True(Sample.SameDay(other));
        Assert.True(Sample.SameMonth(other));
        Assert.True(Sample.SameYear(other));
    }

    [Fact]
    public void FirstAndLastDayOfMonth_ReturnExpectedDates()
    {
        Assert.Equal(1, Sample.FirstDayOfMonth().Day);
        Assert.Equal(30, Sample.LastDayOfMonth().Day);
    }

    [Fact]
    public void NextYear_And_PreviousYear_PreserveMonthWhenPossible()
    {
        Assert.Equal(2025, Sample.NextYear().Year);
        Assert.Equal(2023, Sample.PreviousYear().Year);
    }

    [Fact]
    public void AddFluentTimeSpan_AddsMonthsYearsAndTimeSpan()
    {
        var span = new FluentTimeSpan { Years = 1, Months = 2, TimeSpan = TimeSpan.FromDays(3) };
        var result = Sample.AddFluentTimeSpan(span);

        Assert.Equal(2025, result.Year);
        Assert.Equal(8, result.Month);
    }

    [Fact]
    public void IsInFuture_And_IsInPast_WorkRelativeToNow()
    {
        Assert.True(DateTimeOffset.Now.AddDays(1).IsInFuture());
        Assert.True(DateTimeOffset.Now.AddDays(-1).IsInPast());
    }

    [Fact]
    public void FirstAndLastDayOfWeek_And_Year_ReturnExpectedDates()
    {
        var value = new DateTimeOffset(2024, 6, 12, 10, 0, 0, TimeSpan.Zero);

        Assert.True(value.FirstDayOfWeek() <= value);
        Assert.True(value.LastDayOfWeek() >= value);
        Assert.Equal(new(2024, 1, 1, 10, 0, 0, TimeSpan.Zero), value.FirstDayOfYear());
        Assert.Equal(new(2024, 12, 31, 10, 0, 0, TimeSpan.Zero), value.LastDayOfYear());
    }

    [Fact]
    public void NextAndPreviousMonth_AdjustMonthSafely()
    {
        var value = new DateTimeOffset(2024, 3, 31, 8, 0, 0, TimeSpan.Zero);

        Assert.Equal(4, value.NextMonth().Month);
        Assert.Equal(2, value.PreviousMonth().Month);
    }

    [Fact]
    public void Next_And_Previous_DayOfWeek_FindExpectedDay()
    {
        var wednesday = new DateTimeOffset(2024, 6, 12, 9, 0, 0, TimeSpan.Zero);

        Assert.Equal(DayOfWeek.Thursday, wednesday.Next(DayOfWeek.Thursday).DayOfWeek);
        Assert.Equal(DayOfWeek.Tuesday, wednesday.Previous(DayOfWeek.Tuesday).DayOfWeek);
    }
}
