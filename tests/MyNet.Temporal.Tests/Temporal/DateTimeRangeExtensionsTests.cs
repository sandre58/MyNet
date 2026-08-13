// -----------------------------------------------------------------------
// <copyright file="DateTimeRangeExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Temporal.Tests.Temporal;

public class DateTimeRangeExtensionsTests
{
    [Fact]
    public void NormalizeDateRange_OrdersAndDiscardsTime()
    {
        var (start, end) = new DateTime(2026, 5, 20, 14, 30, 0).Normalize(new DateTime(2026, 5, 10, 8, 0, 0));

        Assert.Equal(new DateTime(2026, 5, 10), start);
        Assert.Equal(new DateTime(2026, 5, 20), end);
    }

    [Fact]
    public void ToInclusiveDateRange_SingleDay_UsesClosedDateOnlyBounds()
    {
        var date = new DateTime(2026, 6, 15);
        var range = date.ToInclusiveDateRange(date);

        Assert.Equal(DateOnly.FromDateTime(date), range.Start!.Value.Value);
        Assert.Equal(DateOnly.FromDateTime(date), range.End!.Value.Value);
        Assert.Equal(1, range.DayCount);
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
