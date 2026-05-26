// -----------------------------------------------------------------------
// <copyright file="TimeUnitRuleSelectorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives;
using MyNet.Temporal.Decomposition;
using Xunit;

namespace MyNet.Temporal.Tests;

public sealed class TimeUnitRuleSelectorTests
{
    [Fact]
    public void Select_MinGreaterThanMax_Throws() => Assert.Throws<ArgumentException>(() =>
        TimeUnitRuleSelector.Default.Select(TimeSpan.FromMinutes(1), TimeUnit.Hour, TimeUnit.Minute));

    [Fact]
    public void Select_SixtyThreeMinutes_ReturnsHourAndMinute()
    {
        var units = TimeUnitRuleSelector.Default.Select(
            TimeSpan.FromMinutes(63),
            TimeUnit.Minute,
            TimeUnit.Hour);

        Assert.Equal([TimeUnit.Hour, TimeUnit.Minute], units);
    }

    [Fact]
    public void Select_SpanBelowMinUnit_ReturnsOnlyMinUnit()
    {
        var units = TimeUnitRuleSelector.Default.Select(
            TimeSpan.FromSeconds(30),
            TimeUnit.Minute,
            TimeUnit.Hour);

        Assert.Equal([TimeUnit.Minute], units);
    }

    [Fact]
    public void Select_MultiMonthSpan_SkipsWeekUnit()
    {
        var units = TimeUnitRuleSelector.Default.Select(
            TimeSpan.FromDays(60),
            TimeUnit.Day,
            TimeUnit.Year);

        Assert.DoesNotContain(TimeUnit.Week, units);
        Assert.Contains(TimeUnit.Month, units);
    }
}
