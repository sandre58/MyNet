// -----------------------------------------------------------------------
// <copyright file="TemporalJsonConverterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using MyNet.Temporal.Intervals;
using MyNet.Temporal.Serialization;
using Xunit;

namespace MyNet.Temporal.Tests;

public class TemporalJsonConverterTests
{
    private static JsonSerializerOptions Options() => new JsonSerializerOptions().AddMyNetTemporalConverters();

    [Fact]
    public void DateTimeOffsetPeriod_RoundTrips()
    {
        var period = new DateTimeOffsetPeriod(
            new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.FromHours(2)),
            new DateTimeOffset(2026, 6, 15, 10, 30, 0, TimeSpan.FromHours(2)));

        var json = JsonSerializer.Serialize(period, Options());
        var restored = JsonSerializer.Deserialize<DateTimeOffsetPeriod>(json, Options());

        Assert.NotNull(restored);
        Assert.Equal(period.Start!.Value.Value, restored.Start!.Value.Value);
        Assert.Equal(period.End!.Value.Value, restored.End!.Value.Value);
    }

    [Fact]
    public void CalendarAmount_RoundTrips()
    {
        var amount = new CalendarAmount { Years = 1, Months = 2, TimeSpan = TimeSpan.FromHours(3) };

        var json = JsonSerializer.Serialize(amount, Options());
        var restored = JsonSerializer.Deserialize<CalendarAmount>(json, Options());

        Assert.Equal(amount, restored);
    }

    [Fact]
    public void DateTimePeriod_RoundTrips()
    {
        var period = new DateTimePeriod(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 9, 0, 0));

        var json = JsonSerializer.Serialize(period, Options());
        var restored = JsonSerializer.Deserialize<DateTimePeriod>(json, Options());

        Assert.NotNull(restored);
        Assert.Equal(period.Start!.Value.Value, restored.Start!.Value.Value);
        Assert.Equal(period.End!.Value.Value, restored.End!.Value.Value);
    }
}
