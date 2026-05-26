// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecompositionPresetsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives;
using MyNet.Temporal.Decomposition;
using Xunit;

namespace MyNet.Temporal.Tests;

public sealed class TimeSpanDecompositionPresetsTests
{
    [Fact]
    public void Humanized_LimitsToTwoComponents()
    {
        var result = TimeSpan.FromDays(400).Humanized();

        Assert.True(result.Count <= 2);
        Assert.All(result, unit => Assert.NotEqual(0, unit.Value));
    }

    [Fact]
    public void Compact_ReturnsSingleComponent()
    {
        var result = TimeSpan.FromDays(14).Compact();

        Assert.Single(result);
    }

    [Fact]
    public void Largest_ReturnsLargestUnitOnly()
    {
        var result = TimeSpan.FromHours(25).Largest();

        Assert.Equal(TimeUnit.Day, result.Unit);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Smallest_ReturnsSmallestConfiguredUnit()
    {
        var result = TimeSpan.FromMinutes(90).Smallest(TimeSpanDecompositionPresets.Strict);

        Assert.Equal(TimeUnit.Minute, result.Unit);
    }

    [Fact]
    public void Full_IncludesZeroUnitsWhenConfigured()
    {
        var result = TimeSpan.FromHours(1).Full();

        Assert.Contains(result, unit => unit is { Unit: TimeUnit.Hour, Value: 1 });
    }
}
