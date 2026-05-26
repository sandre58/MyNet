// -----------------------------------------------------------------------
// <copyright file="RandomDateTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Generator.Facade;
using Xunit;

namespace MyNet.Generator.Tests;

public class RandomDateTests
{
    [Fact]
    public void Date_ReturnsValueInRange()
    {
        var min = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var max = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Date(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void Date_PreservesDateTimeKind()
    {
        var min = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var max = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = RandomGenerator.Current.Date(min, max);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void Date_SameDates_ReturnsThatExactDate()
    {
        var date = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);

        var result = RandomGenerator.Current.Date(date, date);
        Assert.Equal(date, result);
    }

    [Fact]
    public void Date_IsNotAlwaysTheSame()
    {
        var min = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var max = new DateTime(2099, 12, 31, 0, 0, 0, DateTimeKind.Utc);

        var first = RandomGenerator.Current.Date(min, max);
        var different = false;
        for (var i = 0; i < 20; i++)
        {
            if (RandomGenerator.Current.Date(min, max) != first)
            {
                different = true;
                break;
            }
        }

        Assert.True(different, "Date() returned the same value every time.");
    }
}
