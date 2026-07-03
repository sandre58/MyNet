// -----------------------------------------------------------------------
// <copyright file="TimeSpanPeriodExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public class TimeSpanPeriodExtensionsTests
{
    private static readonly DateTime ReferenceDate = new(2026, 6, 15);

    [Fact]
    public void CoerceSameDayRange_WhenEndEdited_AdjustsStartToEnd()
    {
        var (start, end) = new TimeSpan(15, 32, 0).CoerceSameDayRange(new(14, 12, 0), editedEnd: true);

        Assert.Equal(new(14, 12, 0), start);
        Assert.Equal(new(14, 12, 0), end);
    }

    [Fact]
    public void ToPeriodFromTimeOfDay_AllowsOvernight()
    {
        var period = new TimeSpan(22, 0, 0).ToPeriod(new(2, 0, 0), ReferenceDate, allowOvernight: true);

        Assert.True(period.SpansMidnight);
    }

    [Fact]
    public void EndTimeOfDay_CollapsesSubMinuteInterval()
    {
        var period = new TimeSpan(14, 12, 0).ToPeriod(new(14, 12, 30), ReferenceDate);

        Assert.Equal(new(14, 12, 0), period.StartTimeOfDay);
        Assert.Equal(new(14, 12, 0), period.EndTimeOfDay());
    }
}
