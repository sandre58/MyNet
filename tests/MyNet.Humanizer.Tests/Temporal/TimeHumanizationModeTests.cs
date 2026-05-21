// -----------------------------------------------------------------------
// <copyright file="TimeHumanizationModeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Temporal;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

public class TimeHumanizationModeTests
{
    [Fact]
    public void TimeHumanizationMode_HasRelativeValue() => Assert.Equal(TimeHumanizationMode.Relative, TimeHumanizationMode.Relative);

    [Fact]
    public void TimeHumanizationMode_HasDurationValue() => Assert.Equal(TimeHumanizationMode.Duration, TimeHumanizationMode.Duration);

    [Fact]
    public void TimeHumanizationMode_RelativeAndDuration_AreNotEqual() => Assert.NotEqual(TimeHumanizationMode.Relative, TimeHumanizationMode.Duration);

    [Fact]
    public void TimeHumanizationMode_CanCastToInt()
    {
        const int relative = (int)TimeHumanizationMode.Relative;
        const int duration = (int)TimeHumanizationMode.Duration;

        Assert.True(relative >= 0);
        Assert.True(duration >= 0);
    }

    [Theory]
    [InlineData(TimeHumanizationMode.Relative)]
    [InlineData(TimeHumanizationMode.Duration)]
    public void TimeHumanizationMode_CanConvertFromInt(TimeHumanizationMode mode)
    {
        var value = (int)mode;
        var converted = (TimeHumanizationMode)value;

        Assert.Equal(mode, converted);
    }
}
