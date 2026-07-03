// -----------------------------------------------------------------------
// <copyright file="DateTimeRangeExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public class DateTimeRangeExtensionsTests
{
    [Fact]
    public void NormalizeDateRange_OrdersAndDiscardsTime()
    {
        var (start, end) = new DateTime(2026, 5, 20, 14, 30, 0).Normalize(new(2026, 5, 10, 8, 0, 0));

        Assert.Equal(new(2026, 5, 10), start);
        Assert.Equal(new(2026, 5, 20), end);
    }

    [Fact]
    public void ToInclusiveDatePeriod_SingleDay_UsesEndOfDay()
    {
        var date = new DateTime(2026, 6, 15);
        var period = date.ToInclusivePeriod(date);

        Assert.Equal(date, period.Start!.Value.Value.DiscardTime());
        Assert.Equal(date, period.End!.Value.Value.DiscardTime());
    }

    [Fact]
    public void MinOrNull_ReturnsEarliestDate()
    {
        var dates = new[] { new DateTime(2026, 3, 10), new DateTime(2026, 1, 5), new DateTime(2026, 6, 1) };

        Assert.Equal(new DateTime(2026, 1, 5), dates.MinOrNull());
        Assert.Equal(new DateTime(2026, 6, 1), dates.MaxOrNull());
    }

    [Fact]
    public void OrMinValue_UsesFallbackWhenNull() =>
        Assert.Equal(DateTime.MinValue, ((DateTime?)null).OrMinValue());
}
