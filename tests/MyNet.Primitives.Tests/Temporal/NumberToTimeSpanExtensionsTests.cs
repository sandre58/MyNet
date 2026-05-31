// -----------------------------------------------------------------------
// <copyright file="NumberToTimeSpanExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

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

    [Theory]
    [InlineData(1995, 1990)]
    [InlineData(2000, 2000)]
    [InlineData(2009, 2000)]
    [InlineData(2010, 2010)]
    [InlineData(2024, 2020)]
    public void DecadeStart_ReturnsFirstYearOfDecade(int year, int expectedStart) => Assert.Equal(expectedStart, year.DecadeStart());

    [Theory]
    [InlineData(1995, 1999)]
    [InlineData(2000, 2009)]
    [InlineData(2010, 2019)]
    [InlineData(2024, 2029)]
    public void DecadeEnd_ReturnsLastYearOfDecade(int year, int expectedEnd) => Assert.Equal(expectedEnd, year.DecadeEnd());

    [Theory]
    [InlineData(1995, 1990, 1999)]
    [InlineData(2024, 2020, 2029)]
    public void Decade_ReturnsClosedIntervalForYear(int year, int expectedStart, int expectedEnd)
    {
        var decade = year.Decade();

        Assert.Equal(expectedStart, decade.StartValue());
        Assert.Equal(expectedEnd, decade.EndValue());
    }

    [Theory]
    [InlineData(1995, 1900)]
    [InlineData(2000, 2000)]
    [InlineData(1801, 1800)]
    [InlineData(2024, 2000)]
    public void CenturyStart_ReturnsFirstYearOfCentury(int year, int expectedStart) => Assert.Equal(expectedStart, year.CenturyStart());

    [Theory]
    [InlineData(1995, 1999)]
    [InlineData(1801, 1899)]
    [InlineData(2024, 2099)]
    public void CenturyEnd_ReturnsLastYearOfCentury(int year, int expectedEnd) => Assert.Equal(expectedEnd, year.CenturyEnd());

    [Theory]
    [InlineData(1995, 1900, 1999)]
    [InlineData(2024, 2000, 2099)]
    public void Century_ReturnsClosedIntervalForYear(int year, int expectedStart, int expectedEnd)
    {
        var century = year.Century();

        Assert.Equal(expectedStart, century.StartValue());
        Assert.Equal(expectedEnd, century.EndValue());
    }
}
