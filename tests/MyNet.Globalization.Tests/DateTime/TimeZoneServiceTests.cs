// -----------------------------------------------------------------------
// <copyright file="TimeZoneServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.DateTime;
using Xunit;

namespace MyNet.Globalization.Tests.DateTime;

public sealed class TimeZoneServiceTests
{
    [Fact]
    public void Constructor_WithTimeZone_SetsCurrentTimeZone()
    {
        var expected = TimeZoneInfo.Utc;
        var sut = new TimeZoneService(expected);

        Assert.Equal(expected, sut.CurrentTimeZone);
    }

    [Fact]
    public void Constructor_WithGlobalizationOptions_UsesDefaultTimeZone()
    {
        var expected = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        var options = new GlobalizationOptions { DefaultTimeZone = expected };
        var sut = new TimeZoneService(options);

        Assert.Equal(expected, sut.CurrentTimeZone);
    }

    [Fact]
    public void SetTimeZone_UpdatesCurrentTimeZone()
    {
        var sut = new TimeZoneService(TimeZoneInfo.Utc);
        var paris = TimeZoneInfo.FindSystemTimeZoneById(GetParisTimeZoneId());

        sut.SetTimeZone(paris);

        Assert.Equal(paris, sut.CurrentTimeZone);
    }

    [Fact]
    public void SetTimeZone_WithTimeZoneId_ResolvesTimeZone()
    {
        var sut = new TimeZoneService(TimeZoneInfo.Utc);

        sut.SetTimeZone("UTC");

        Assert.Equal(TimeZoneInfo.Utc.Id, sut.CurrentTimeZone.Id);
    }

    [Fact]
    public void SetTimeZone_SameTimeZone_DoesNotRaiseTimeZoneChanged()
    {
        var timeZone = TimeZoneInfo.Utc;
        var sut = new TimeZoneService(timeZone);
        var eventCount = 0;
        sut.TimeZoneChanged += (_, _) => eventCount++;

        sut.SetTimeZone(timeZone);

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void SetTimeZone_DifferentTimeZone_RaisesTimeZoneChangedWithArgs()
    {
        var oldTimeZone = TimeZoneInfo.Utc;
        var newTimeZone = TimeZoneInfo.FindSystemTimeZoneById(GetParisTimeZoneId());
        var sut = new TimeZoneService(oldTimeZone);
        TimeZoneChangedEventArgs? captured = null;
        sut.TimeZoneChanged += (_, args) => captured = args;

        sut.SetTimeZone(newTimeZone);

        Assert.NotNull(captured);
        Assert.Equal(oldTimeZone, captured.OldTimeZone);
        Assert.Equal(newTimeZone, captured.NewTimeZone);
    }

    [Fact]
    public void FromUtc_And_ToUtc_AreInverseForCurrentTimeZone()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(GetParisTimeZoneId());
        var sut = new TimeZoneService(timeZone);
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);

        var local = sut.FromUtc(utc);
        var roundTrip = sut.ToUtc(local);

        Assert.Equal(utc, roundTrip);
    }

    [Fact]
    public void Convert_UtcToUtc_ReturnsSameInstant()
    {
        var sut = new TimeZoneService(TimeZoneInfo.Utc);
        var instant = new DateTimeOffset(2024, 1, 1, 8, 30, 0, TimeSpan.Zero);

        var result = sut.Convert(instant, TimeZoneInfo.Utc, TimeZoneInfo.Utc);

        Assert.Equal(instant, result);
    }

    [Fact]
    public void Now_ReturnsTimeInCurrentTimeZone()
    {
        var sut = new TimeZoneService(TimeZoneInfo.Utc);
        var expected = TimeZoneInfo.ConvertTime(System.DateTime.UtcNow, TimeZoneInfo.Utc);

        var now = sut.Now;

        Assert.Equal(expected, now, precision: TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void SetTimeZone_NullTimeZone_ThrowsArgumentNullException()
    {
        var sut = new TimeZoneService(TimeZoneInfo.Utc);

        Assert.Throws<ArgumentNullException>(() => sut.SetTimeZone((TimeZoneInfo)null!));
    }

    private static string GetParisTimeZoneId()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris").Id;
        }
        catch (TimeZoneNotFoundException)
        {
            return "Romance Standard Time";
        }
    }
}
