// -----------------------------------------------------------------------
// <copyright file="TimeLocalizationOptionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Temporal;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

public class TimeLocalizationOptionsTests
{
    [Fact]
    public void Constructor_CreatesNewInstanceWithDefaultValues()
    {
        var options = new TimeLocalizationOptions();

        Assert.NotNull(options);
        Assert.Equal("RelativeDateNow", options.NowKey);
        Assert.Equal("RelativeDateNever", options.NeverKey);
        Assert.Equal("RelativeDateZero", options.ZeroKey);
        Assert.Equal("RelativeDateTomorrow", options.TomorrowKey);
        Assert.Equal("RelativeDateYesterday", options.YesterdayKey);
        Assert.Equal("RelativeDate{0}{1}", options.RelativeDateKeyFormat);
        Assert.Equal("Duration{0}", options.DurationKeyFormat);
    }

    [Fact]
    public void Constructor_WithNowKey_SetsNowKey()
    {
        const string customKey = "CustomNow";
        var options = new TimeLocalizationOptions { NowKey = customKey };

        Assert.Equal(customKey, options.NowKey);
    }

    [Fact]
    public void Constructor_WithNeverKey_SetsNeverKey()
    {
        const string customKey = "CustomNever";
        var options = new TimeLocalizationOptions { NeverKey = customKey };

        Assert.Equal(customKey, options.NeverKey);
    }

    [Fact]
    public void Constructor_WithZeroKey_SetsZeroKey()
    {
        const string customKey = "CustomZero";
        var options = new TimeLocalizationOptions { ZeroKey = customKey };

        Assert.Equal(customKey, options.ZeroKey);
    }

    [Fact]
    public void Constructor_WithTomorrowKey_SetsTomorrowKey()
    {
        const string customKey = "CustomTomorrow";
        var options = new TimeLocalizationOptions { TomorrowKey = customKey };

        Assert.Equal(customKey, options.TomorrowKey);
    }

    [Fact]
    public void Constructor_WithYesterdayKey_SetsYesterdayKey()
    {
        const string customKey = "CustomYesterday";
        var options = new TimeLocalizationOptions { YesterdayKey = customKey };

        Assert.Equal(customKey, options.YesterdayKey);
    }

    [Fact]
    public void Constructor_WithRelativeDateKeyFormat_SetsRelativeDateKeyFormat()
    {
        const string customFormat = "Date{0}{1}Custom";
        var options = new TimeLocalizationOptions { RelativeDateKeyFormat = customFormat };

        Assert.Equal(customFormat, options.RelativeDateKeyFormat);
    }

    [Fact]
    public void Constructor_WithDurationKeyFormat_SetsDurationKeyFormat()
    {
        const string customFormat = "Time{0}Custom";
        var options = new TimeLocalizationOptions { DurationKeyFormat = customFormat };

        Assert.Equal(customFormat, options.DurationKeyFormat);
    }

    [Fact]
    public void Constructor_WithAllProperties_SetsAllProperties()
    {
        var options = new TimeLocalizationOptions
        {
            NowKey = "Now",
            NeverKey = "Never",
            ZeroKey = "Zero",
            TomorrowKey = "Tomorrow",
            YesterdayKey = "Yesterday",
            RelativeDateKeyFormat = "Date{0}{1}",
            DurationKeyFormat = "Duration{0}"
        };

        Assert.Equal("Now", options.NowKey);
        Assert.Equal("Never", options.NeverKey);
        Assert.Equal("Zero", options.ZeroKey);
        Assert.Equal("Tomorrow", options.TomorrowKey);
        Assert.Equal("Yesterday", options.YesterdayKey);
        Assert.Equal("Date{0}{1}", options.RelativeDateKeyFormat);
        Assert.Equal("Duration{0}", options.DurationKeyFormat);
    }
}
