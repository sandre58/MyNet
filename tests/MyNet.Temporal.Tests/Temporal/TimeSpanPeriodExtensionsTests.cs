// -----------------------------------------------------------------------
// <copyright file="TimeSpanPeriodExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Temporal.Tests.Temporal;

public class TimeSpanPeriodExtensionsTests
{
    private static readonly DateTime ReferenceDate = new(2026, 6, 15);

    [Fact]
    public void CoerceSameDayRange_WhenEndEdited_AdjustsStartToEnd()
    {
        var (start, end) = new TimeSpan(15, 32, 0).CoerceSameDayRange(new TimeSpan(14, 12, 0), editedEnd: true);

        Assert.Equal(new TimeSpan(14, 12, 0), start);
        Assert.Equal(new TimeSpan(14, 12, 0), end);
    }

    [Fact]
    public void ToPeriodFromTimeOfDay_AllowsOvernight()
    {
        var period = new TimeSpan(22, 0, 0).ToDateTimePeriod(new TimeSpan(2, 0, 0), ReferenceDate, allowOvernight: true);

        Assert.True(period.SpansMidnight);
    }

    [Fact]
    public void ToPeriodFromTimeOfDay_WithoutOvernight_ThrowsWhenEndBeforeStart() =>
        Assert.Throws<ArgumentException>(() =>
            new TimeSpan(22, 0, 0).ToDateTimePeriod(new TimeSpan(2, 0, 0), ReferenceDate, allowOvernight: false));

    [Fact]
    public void EndTimeOfDay_CollapsesSubMinuteInterval()
    {
        var period = new TimeSpan(14, 12, 0).ToDateTimePeriod(new TimeSpan(14, 12, 30), ReferenceDate);

        Assert.Equal(new TimeSpan(14, 12, 0), period.StartTimeOfDay);
        Assert.Equal(new TimeSpan(14, 12, 0), period.EndTimeOfDay());
    }
}
