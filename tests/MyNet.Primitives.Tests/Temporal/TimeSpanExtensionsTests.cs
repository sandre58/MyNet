// -----------------------------------------------------------------------
// <copyright file="TimeSpanExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Temporal;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public sealed class TimeSpanExtensionsTests
{
    [Fact]
    public void ToTime_ConvertsTimeSpanToTimeOnly()
    {
        var time = new TimeSpan(14, 30, 0).ToTime();

        Assert.Equal(new(14, 30), time);
    }

    [Fact]
    public void Before_And_From_AdjustDateTime()
    {
        var origin = new DateTime(2024, 6, 15, 12, 0, 0);
        var span = TimeSpan.FromHours(2);

        Assert.Equal(new(2024, 6, 15, 10, 0, 0), span.Before(origin));
        Assert.Equal(new(2024, 6, 15, 14, 0, 0), span.From(origin));
        Assert.Equal(new(2024, 6, 15, 14, 0, 0), span.Since(origin));
    }

    [Fact]
    public void Round_ToMinute_RoundsCorrectly()
    {
        var span = TimeSpan.FromMinutes(2) + TimeSpan.FromSeconds(45);

        var rounded = span.Round(RoundTo.Minute);

        Assert.Equal(TimeSpan.FromMinutes(3), rounded);
    }

    [Fact]
    public void AddFluentTimeSpan_CombinesValues()
    {
        var span = TimeSpan.FromDays(1);
        var fluent = new FluentTimeSpan { Months = 1 };

        var result = span.AddFluentTimeSpan(fluent);

        Assert.Equal(1, result.Months);
        Assert.Equal(TimeSpan.FromDays(1), result.TimeSpan);
    }
}
