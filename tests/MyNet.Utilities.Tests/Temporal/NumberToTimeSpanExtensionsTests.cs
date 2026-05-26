// -----------------------------------------------------------------------
// <copyright file="NumberToTimeSpanExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Utilities.Tests.Temporal;

public class NumberToTimeSpanExtensionsTests
{
    [Fact]
    public void IntToMilliseconds()
    {
        const int number = 1;
        Assert.Equal(new() { TimeSpan = new(0, 0, 0, 0, 1) }, number.Milliseconds());
    }

    [Fact]
    public void IntToMinutes()
    {
        const int number = 2;
        Assert.Equal(new() { TimeSpan = new(0, 0, 2, 0) }, number.Minutes());
    }

    [Fact]
    public void IntToSeconds()
    {
        const int number = 3;
        Assert.Equal(new() { TimeSpan = new(0, 0, 0, 3) }, number.Seconds());
    }

    [Fact]
    public void IntToHours()
    {
        const int number = 4;
        Assert.Equal(new() { TimeSpan = new(0, 4, 0, 0) }, number.Hours());
    }

    [Fact]
    public void IntToDays()
    {
        const int number = 5;
        Assert.Equal(new() { TimeSpan = new(5, 0, 0, 0) }, number.Days());
    }

    [Fact]
    public void IntToWeeks()
    {
        const int number = 6;
        var now = DateTime.Now;
        Assert.Equal(now.AddDays(42), now.Add(number.Weeks()));
    }
}
