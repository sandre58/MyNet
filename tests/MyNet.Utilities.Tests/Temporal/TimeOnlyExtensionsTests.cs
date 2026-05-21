// -----------------------------------------------------------------------
// <copyright file="TimeOnlyExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Xunit;

namespace MyNet.Utilities.Tests.Temporal;

public class TimeOnlyExtensionsTests
{
    #region BeginningOfHour / EndOfHour

    [Fact]
    public void BeginningOfHour_ReturnsFirstMomentOfCurrentHour()
    {
        var time = new TimeOnly(14, 37, 22, 500);
        var result = time.BeginningOfHour();
        Assert.Equal(new(14, 0, 0, 0), result);
    }

    [Fact]
    public void EndOfHour_ReturnsLastMomentOfCurrentHour()
    {
        var time = new TimeOnly(14, 37, 22, 500);
        var result = time.EndOfHour();
        Assert.Equal(new(14, 59, 59, 999), result);
    }

    #endregion

    #region BeginningOfMinute / EndOfMinute

    [Fact]
    public void BeginningOfMinute_ReturnsFirstMomentOfCurrentMinute()
    {
        var time = new TimeOnly(14, 37, 22, 500);
        var result = time.BeginningOfMinute();
        Assert.Equal(new(14, 37, 0, 0), result);
    }

    [Fact]
    public void EndOfMinute_ReturnsLastMomentOfCurrentMinute()
    {
        var time = new TimeOnly(14, 37, 22, 500);
        var result = time.EndOfMinute();
        Assert.Equal(new(14, 37, 59, 999), result);
    }

    #endregion

    #region SetHour / SetMinute / SetSecond / SetMillisecond

    [Fact]
    public void SetHour_ReturnsTimeWithChangedHour()
    {
        var time = new TimeOnly(10, 20, 30, 400);
        var result = time.SetHour(18);
        Assert.Equal(new(18, 20, 30, 400), result);
    }

    [Fact]
    public void SetMinute_ReturnsTimeWithChangedMinute()
    {
        var time = new TimeOnly(10, 20, 30, 400);
        var result = time.SetMinute(55);
        Assert.Equal(new(10, 55, 30, 400), result);
    }

    [Fact]
    public void SetSecond_ReturnsTimeWithChangedSecond()
    {
        var time = new TimeOnly(10, 20, 30, 400);
        var result = time.SetSecond(59);
        Assert.Equal(new(10, 20, 59, 400), result);
    }

    [Fact]
    public void SetMillisecond_ReturnsTimeWithChangedMillisecond()
    {
        var time = new TimeOnly(10, 20, 30, 400);
        var result = time.SetMillisecond(999);
        Assert.Equal(new(10, 20, 30, 999), result);
    }

    #endregion

    #region SameXxx

    [Fact]
    public void SameHour_ReturnsTrueWhenSameHour()
    {
        var t1 = new TimeOnly(10, 0, 0);
        var t2 = new TimeOnly(10, 59, 59);
        Assert.True(t1.SameHour(t2));
    }

    [Fact]
    public void SameHour_ReturnsFalseWhenDifferentHour()
    {
        var t1 = new TimeOnly(10, 0, 0);
        var t2 = new TimeOnly(11, 0, 0);
        Assert.False(t1.SameHour(t2));
    }

    [Fact]
    public void SameMinute_ReturnsTrueWhenSameHourAndMinute()
    {
        var t1 = new TimeOnly(10, 30, 0);
        var t2 = new TimeOnly(10, 30, 59);
        Assert.True(t1.SameMinute(t2));
    }

    [Fact]
    public void SameMinute_ReturnsFalseWhenDifferentMinute()
    {
        var t1 = new TimeOnly(10, 30, 0);
        var t2 = new TimeOnly(10, 31, 0);
        Assert.False(t1.SameMinute(t2));
    }

    [Fact]
    public void SameSecond_ReturnsTrueWhenSameHourMinuteSecond()
    {
        var t1 = new TimeOnly(10, 30, 45, 0);
        var t2 = new TimeOnly(10, 30, 45, 999);
        Assert.True(t1.SameSecond(t2));
    }

    [Fact]
    public void SameSecond_ReturnsFalseWhenDifferentSecond()
    {
        var t1 = new TimeOnly(10, 30, 45);
        var t2 = new TimeOnly(10, 30, 46);
        Assert.False(t1.SameSecond(t2));
    }

    [Fact]
    public void SameMilliSecond_ReturnsTrueWhenExactlySame()
    {
        var t1 = new TimeOnly(10, 30, 45, 123);
        var t2 = new TimeOnly(10, 30, 45, 123);
        Assert.True(t1.SameMilliSecond(t2));
    }

    [Fact]
    public void SameMilliSecond_ReturnsFalseWhenDifferentMs()
    {
        var t1 = new TimeOnly(10, 30, 45, 123);
        var t2 = new TimeOnly(10, 30, 45, 124);
        Assert.False(t1.SameMilliSecond(t2));
    }

    #endregion

    #region Range

    [Fact]
    public void Range_ByHour_ReturnsCorrectSequence()
    {
        var start = new TimeOnly(8, 0, 0);
        var end = new TimeOnly(11, 0, 0);
        var result = start.Range(end).ToList();

        Assert.Equal([new(8, 0, 0), new(9, 0, 0), new(10, 0, 0), new(11, 0, 0)], result);
    }

    [Fact]
    public void Range_ByMinute_ReturnsCorrectSequence()
    {
        var start = new TimeOnly(10, 0, 0);
        var end = new TimeOnly(10, 3, 0);
        var result = start.Range(end, 1, TimeUnit.Minute).ToList();

        Assert.Equal(4, result.Count);
        Assert.Equal(new(10, 0, 0), result[0]);
        Assert.Equal(new(10, 3, 0), result[3]);
    }

    [Fact]
    public void Range_BySecond_ReturnsCorrectCount()
    {
        var start = new TimeOnly(10, 0, 0);
        var end = new TimeOnly(10, 0, 3);
        var result = start.Range(end, 1, TimeUnit.Second).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Range_ByMillisecond_ReturnsCorrectCount()
    {
        var start = new TimeOnly(10, 0, 0, 0);
        var end = new TimeOnly(10, 0, 0, 2);
        var result = start.Range(end, 1, TimeUnit.Millisecond).ToList();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Range_WithZeroStep_ThrowsArgumentOutOfRangeException()
    {
        var start = new TimeOnly(8, 0, 0);
        var end = new TimeOnly(10, 0, 0);
        Assert.Throws<ArgumentOutOfRangeException>(() => start.Range(end, 0).ToList());
    }

    [Fact]
    public void Range_WithDayUnit_ThrowsInvalidOperationException()
    {
        var start = new TimeOnly(8, 0, 0);
        var end = new TimeOnly(10, 0, 0);
        Assert.Throws<InvalidOperationException>(() => start.Range(end, 1, TimeUnit.Day).ToList());
    }

    [Fact]
    public void Range_WithNegativeStep_ReturnsDescendingSequence()
    {
        var start = new TimeOnly(10, 0, 0);
        var end = new TimeOnly(8, 0, 0);
        var result = start.Range(end, -1).ToList();
        Assert.Equal([new(10, 0, 0), new(9, 0, 0), new(8, 0, 0)], result);
    }

    #endregion
}
