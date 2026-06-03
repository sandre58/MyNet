// -----------------------------------------------------------------------
// <copyright file="IntervalExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Intervals;
using Xunit;

namespace MyNet.Primitives.Tests.Intervals;

public class IntervalExtensionsTests
{
    [Fact]
    public void StartValue_OnClosedInterval_ReturnsStartValue()
    {
        var interval = new ClosedInterval<int>(1990, 1999);

        Assert.Equal(1990, interval.StartValue());
    }

    [Fact]
    public void EndValue_OnClosedInterval_ReturnsEndValue()
    {
        var interval = new ClosedInterval<int>(1990, 1999);

        Assert.Equal(1999, interval.EndValue());
    }

    [Fact]
    public void StartValue_OnNumericRange_ReturnsStartValue()
    {
        IBoundedInterval<int> interval = new NumericRange<int>(10, 20);

        Assert.Equal(10, interval.StartValue());
    }

    [Fact]
    public void EndValue_OnOpenInterval_ReturnsEndValue()
    {
        IBoundedInterval<int> interval = new OpenInterval<int>(1, 5);

        Assert.Equal(5, interval.EndValue());
    }

    [Fact]
    public void StartValue_OnDateRange_ReturnsStartValue()
    {
        var interval = new DateRange(new(2020, 1, 1), new(2020, 12, 31));

        Assert.Equal(new(2020, 1, 1), interval.StartValue());
    }
}
