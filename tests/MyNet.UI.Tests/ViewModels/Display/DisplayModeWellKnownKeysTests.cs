// -----------------------------------------------------------------------
// <copyright file="DisplayModeWellKnownKeysTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.ViewModels.Display;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Display;

public class DisplayModeWellKnownKeysTests
{
    [Theory]
    [InlineData(DisplayModeWellKnownKeys.Grid)]
    [InlineData(DisplayModeWellKnownKeys.Detailed)]
    [InlineData(DisplayModeWellKnownKeys.Chart)]
    [InlineData(DisplayModeWellKnownKeys.List)]
    [InlineData(DisplayModeWellKnownKeys.Hour)]
    [InlineData(DisplayModeWellKnownKeys.Day)]
    [InlineData(DisplayModeWellKnownKeys.Week)]
    [InlineData(DisplayModeWellKnownKeys.Month)]
    [InlineData(DisplayModeWellKnownKeys.Year)]
    public void BuiltInViewModels_UseWellKnownKeys(string expectedKey)
    {
        IDisplayModeViewModel instance = expectedKey switch
        {
            DisplayModeWellKnownKeys.Grid => new GridDisplayModeViewModel(),
            DisplayModeWellKnownKeys.Detailed => new DetailedDisplayModeViewModel(),
            DisplayModeWellKnownKeys.Chart => new ChartDisplayModeViewModel(),
            DisplayModeWellKnownKeys.List => new ListDisplayModeViewModel(),
            DisplayModeWellKnownKeys.Hour => new HourDisplayModeViewModel(),
            DisplayModeWellKnownKeys.Day => new DayDisplayModeViewModel(),
            DisplayModeWellKnownKeys.Week => new WeekDisplayModeViewModel(),
            DisplayModeWellKnownKeys.Month => new MonthDisplayModeViewModel(),
            DisplayModeWellKnownKeys.Year => new YearDisplayModeViewModel(),
            _ => throw new Xunit.Sdk.XunitException($"Unexpected key: {expectedKey}"),
        };

        instance.Key.Should().Be(expectedKey);
    }

    [Fact]
    public void LegacyAliases_MatchWellKnownKeys()
    {
        DisplayModeViewModel.GridKey.Should().Be(DisplayModeWellKnownKeys.Grid);
        DisplayModeViewModel.DetailedKey.Should().Be(DisplayModeWellKnownKeys.Detailed);
        DisplayModeViewModel.ChartKey.Should().Be(DisplayModeWellKnownKeys.Chart);
        DisplayModeViewModel.ListKey.Should().Be(DisplayModeWellKnownKeys.List);
        DisplayModeViewModel.HourKey.Should().Be(DisplayModeWellKnownKeys.Hour);
        DisplayModeViewModel.DayKey.Should().Be(DisplayModeWellKnownKeys.Day);
        DisplayModeViewModel.WeekKey.Should().Be(DisplayModeWellKnownKeys.Week);
        DisplayModeViewModel.MonthKey.Should().Be(DisplayModeWellKnownKeys.Month);
        DisplayModeViewModel.YearKey.Should().Be(DisplayModeWellKnownKeys.Year);
    }
}
