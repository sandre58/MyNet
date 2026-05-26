// -----------------------------------------------------------------------
// <copyright file="HumanFriendlyTimeSpanQuantizerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Temporal;
using MyNet.Utilities;
using MyNet.Temporal.Decomposition;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

public class HumanFriendlyTimeSpanQuantizerTests
{
    [Fact]
    public void Quantize_59Seconds_PromotesToOneMinute()
    {
        var values = new[]
        {
            new TimeUnitValue(0, TimeUnit.Minute),
            new TimeUnitValue(59, TimeUnit.Second)
        };

        var result = HumanFriendlyTimeSpanQuantizer.Default.Quantize(values);

        Assert.Equal(new(1, TimeUnit.Minute), result[0]);
        Assert.Equal(new(0, TimeUnit.Second), result[1]);
    }

    [Fact]
    public void Quantize_OneHour59Minutes_PromotesAndDropsSmallerDetails()
    {
        var values = new[]
        {
            new TimeUnitValue(1, TimeUnit.Hour),
            new TimeUnitValue(59, TimeUnit.Minute),
            new TimeUnitValue(30, TimeUnit.Second)
        };

        var result = HumanFriendlyTimeSpanQuantizer.Default.Quantize(values);

        Assert.Equal(new(2, TimeUnit.Hour), result[0]);
        Assert.Equal(new(0, TimeUnit.Minute), result[1]);
        Assert.Equal(new(0, TimeUnit.Second), result[2]);
    }
}
