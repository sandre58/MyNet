// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecomposerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives;
using MyNet.Temporal.Decomposition;
using Xunit;

namespace MyNet.Temporal.Tests;

public sealed class TimeSpanDecomposerTests
{
    [Fact]
    public void StrictPreset_59Seconds_RemainsSeconds()
    {
        var options = new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Second,
            MaxUnit = TimeUnit.Minute,
            Mode = TimeSpanDecompositionMode.Hierarchical,
            MaxComponents = 1
        };

        var result = TimeSpan.FromSeconds(59).Decompose(options);

        Assert.Single(result);
        Assert.Equal(new(59, TimeUnit.Second), result[0]);
    }

    [Fact]
    public void DecomposeStrict_OneHourThreeMinutes_ReturnsTwoUnits()
    {
        var result = TimeSpan.FromMinutes(63).Decompose(x => x
            .AsHierarchical()
            .WithMinUnit(TimeUnit.Minute)
            .WithMaxUnit(TimeUnit.Hour)
            .MaxComponents(2));

        Assert.Equal(2, result.Count);
        Assert.Equal(new(1, TimeUnit.Hour), result[0]);
        Assert.Equal(new(3, TimeUnit.Minute), result[1]);
    }

    [Fact]
    public void HierarchicalHourMinuteSecondRange_UsesExpectedBreakdown()
    {
        var result = TimeSpan.FromSeconds(3723).Decompose(new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Second,
            MaxUnit = TimeUnit.Hour,
            Mode = TimeSpanDecompositionMode.Hierarchical
        });

        Assert.Equal(3, result.Count);
        Assert.Equal(new(1, TimeUnit.Hour), result[0]);
        Assert.Equal(new(2, TimeUnit.Minute), result[1]);
        Assert.Equal(new(3, TimeUnit.Second), result[2]);
    }

    [Fact]
    public void YearMonthDayRange_400Days_DecomposesWithoutWeeks()
    {
        var result = TimeSpan.FromDays(400).Decompose(new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Day,
            MaxUnit = TimeUnit.Year,
            Mode = TimeSpanDecompositionMode.Hierarchical
        });

        Assert.NotEmpty(result);
        Assert.DoesNotContain(result, x => x.Unit == TimeUnit.Week);
        Assert.All(result, x => Assert.InRange((int)x.Unit, (int)TimeUnit.Day, (int)TimeUnit.Year));
    }

    [Fact]
    public void DayRange_10Days_DefaultSelectorPrefersWeekThenDay()
    {
        var result = TimeSpan.FromDays(10).Decompose(new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Day,
            MaxUnit = TimeUnit.Year,
            Mode = TimeSpanDecompositionMode.Hierarchical
        });

        Assert.Equal(2, result.Count);
        Assert.Equal(new(1, TimeUnit.Week), result[0]);
        Assert.Equal(new(3, TimeUnit.Day), result[1]);
    }

    [Fact]
    public void DayRange_32Days_PromotesToMonthAndSkipsWeek()
    {
        var result = TimeSpan.FromDays(32).Decompose(new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Day,
            MaxUnit = TimeUnit.Year,
            Mode = TimeSpanDecompositionMode.Hierarchical
        });

        Assert.Equal(2, result.Count);
        Assert.Equal(new(1, TimeUnit.Month), result[0]);
        Assert.Equal(new(1, TimeUnit.Day), result[1]);
        Assert.DoesNotContain(result, x => x.Unit == TimeUnit.Week);
    }

    [Fact]
    public void YearInputViaToTimeSpan_UsesRoundedDays_ThenDecomposesTo11Months()
    {
        var result = 1.ToTimeSpan(TimeUnit.Year).Decompose(new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Month,
            MaxUnit = TimeUnit.Year,
            Mode = TimeSpanDecompositionMode.Hierarchical,
            MaxComponents = 1
        });

        Assert.Single(result);
        Assert.Equal(new(11, TimeUnit.Month), result[0]);
    }

    [Fact]
    public void FlatMode_OneHourThreeMinutes_KeepsIndependentTotals()
    {
        var result = TimeSpan.FromMinutes(63).Decompose(new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Hour,
            Mode = TimeSpanDecompositionMode.Flat
        });

        Assert.Equal(2, result.Count);
        Assert.Equal(new(1, TimeUnit.Hour), result[0]);
        Assert.Equal(new(63, TimeUnit.Minute), result[1]);
    }

    [Fact]
    public void SmallestUnitOnly_OneHourThreeMinutes_ReturnsTotalMinutes()
    {
        var result = TimeSpan.FromMinutes(63).Decompose(new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Hour,
            Mode = TimeSpanDecompositionMode.SmallestUnitOnly
        });

        Assert.Single(result);
        Assert.Equal(new(63, TimeUnit.Minute), result[0]);
    }
}
