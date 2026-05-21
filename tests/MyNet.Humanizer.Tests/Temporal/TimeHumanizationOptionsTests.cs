// -----------------------------------------------------------------------
// <copyright file="TimeHumanizationOptionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Temporal;
using MyNet.Utilities;
using MyNet.Utilities.Temporal.Decomposition;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

public class TimeHumanizationOptionsTests
{
    [Fact]
    public void Constructor_CreatesNewInstanceWithDefaultValues()
    {
        var options = new TimeHumanizationOptions();

        Assert.NotNull(options);
        Assert.Equal(TimeHumanizationMode.Duration, options.Mode);
        Assert.Equal(TimeUnit.Second, options.MinUnit);
        Assert.Equal(TimeUnit.Year, options.MaxUnit);
        Assert.Equal(TimeSpanDecompositionMode.Hierarchical, options.DecompositionMode);
        Assert.False(options.IncludeZeroUnits);
    }

    [Fact]
    public void Constructor_WithMode_SetsMode()
    {
        var options = new TimeHumanizationOptions { Mode = TimeHumanizationMode.Duration };

        Assert.Equal(TimeHumanizationMode.Duration, options.Mode);
    }

    [Fact]
    public void Constructor_WithDecompositionSettings_SetsValues()
    {
        var options = new TimeHumanizationOptions
        {
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Day,
            MaxComponents = 3,
            IncludeZeroUnits = true,
            DecompositionMode = TimeSpanDecompositionMode.Flat
        };

        Assert.Equal(TimeUnit.Minute, options.MinUnit);
        Assert.Equal(TimeUnit.Day, options.MaxUnit);
        Assert.Equal(3, options.MaxComponents);
        Assert.True(options.IncludeZeroUnits);
        Assert.Equal(TimeSpanDecompositionMode.Flat, options.DecompositionMode);
    }

    [Fact]
    public void Constructor_WithAllProperties_SetsAllProperties()
    {
        var decompositionOptions = new TimeSpanDecompositionOptions
        {
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Day,
            Mode = TimeSpanDecompositionMode.SmallestUnitOnly,
            MaxComponents = 5,
            IncludeZeroUnits = true
        };

        var options = new TimeHumanizationOptions
        {
            Mode = TimeHumanizationMode.Duration,
            MinUnit = decompositionOptions.MinUnit,
            MaxUnit = decompositionOptions.MaxUnit,
            DecompositionMode = decompositionOptions.Mode,
            MaxComponents = decompositionOptions.MaxComponents,
            IncludeZeroUnits = decompositionOptions.IncludeZeroUnits
        };

        Assert.Equal(TimeHumanizationMode.Duration, options.Mode);
        Assert.Equal(TimeUnit.Minute, options.MinUnit);
        Assert.Equal(TimeUnit.Day, options.MaxUnit);
        Assert.Equal(TimeSpanDecompositionMode.SmallestUnitOnly, options.DecompositionMode);
        Assert.Equal(5, options.MaxComponents);
        Assert.True(options.IncludeZeroUnits);
    }

    [Theory]
    [InlineData(TimeHumanizationMode.Relative)]
    [InlineData(TimeHumanizationMode.Duration)]
    public void Constructor_WithDifferentModes_WorksProperly(TimeHumanizationMode mode)
    {
        var options = new TimeHumanizationOptions { Mode = mode };

        Assert.Equal(mode, options.Mode);
    }
}
