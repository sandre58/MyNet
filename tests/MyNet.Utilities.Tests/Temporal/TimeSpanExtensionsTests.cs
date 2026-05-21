// -----------------------------------------------------------------------
// <copyright file="TimeSpanExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Temporal.Decomposition;
using Xunit;

namespace MyNet.Utilities.Tests.Temporal;

public class TimeSpanExtensionsTests
{
    [Fact]
    public void Decompose_WithHumanizedPreset_ReturnsTwoLargestComponents()
    {
        var result = TimeSpan.FromSeconds(3723).Decompose(TimeSpanDecompositionPresets.Humanized);

        Assert.Equal(2, result.Count);
        Assert.Equal(new(1, TimeUnit.Hour), result[0]);
        Assert.Equal(new(2, TimeUnit.Minute), result[1]);
    }

    [Fact]
    public void Decompose_WithCompactPreset_ReturnsSingleLargestUnit()
    {
        var result = TimeSpan.FromMinutes(30).Decompose(TimeSpanDecompositionPresets.Compact);

        Assert.Single(result);
        Assert.Equal(new(30, TimeUnit.Minute), result[0]);
    }

    [Fact]
    public void Decompose_WithBuilder_AndLargestUnitOnly_ReturnsSingleMinuteUnitWhenBelowHour()
    {
        var result = TimeSpan.FromMinutes(30).Decompose(x => x
            .UseLargestUnitOnly()
            .WithMinUnit(TimeUnit.Minute)
            .WithMaxUnit(TimeUnit.Minute)
            .MaxComponents(1));

        Assert.Single(result);
        Assert.Equal(new(30, TimeUnit.Minute), result[0]);
    }
}
