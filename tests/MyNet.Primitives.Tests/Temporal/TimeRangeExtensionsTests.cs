// -----------------------------------------------------------------------
// <copyright file="TimeRangeExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using MyNet.Primitives;
using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public class TimeRangeExtensionsTests
{
    [Fact]
    public void Shift_MovesRangeByOffset()
    {
        var range = new TimeRange(new(8, 0), new(10, 0));

        var shifted = range.Shift(TimeSpan.FromHours(1));

        Assert.Equal(new TimeOnly(9, 0), shifted.Start!.Value.Value);
        Assert.Equal(new TimeOnly(11, 0), shifted.End!.Value.Value);
    }

    [Fact]
    public void Expand_WidensRangeByDuration()
    {
        var range = new TimeRange(new(8, 0), new(10, 0));

        var expanded = range.Expand(TimeSpan.FromMinutes(30));

        Assert.Equal(new TimeOnly(7, 30), expanded.Start!.Value.Value);
        Assert.Equal(new TimeOnly(10, 30), expanded.End!.Value.Value);
    }

    [Fact]
    public void Enumerate_WithZeroStep_ThrowsArgumentOutOfRangeException()
    {
        var range = new TimeRange(new(8, 0), new(10, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() => range.Enumerate(TimeSpan.Zero).ToList());
    }
}
