// -----------------------------------------------------------------------
// <copyright file="TimeHumanizationBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Formatting.Collections;
using MyNet.Humanizer.Temporal;
using MyNet.Utilities;
using MyNet.Utilities.Temporal.Decomposition;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

public class TimeHumanizationBuilderTests
{
    [Fact]
    public void Build_DefaultsToDurationWithTwoComponents()
    {
        var options = new TimeHumanizationBuilder().Build();

        Assert.Equal(TimeHumanizationMode.Duration, options.Mode);
        Assert.Equal(2, options.MaxComponents);
        Assert.Equal(TimeUnit.Second, options.MinUnit);
        Assert.Equal(TimeUnit.Year, options.MaxUnit);
    }

    [Fact]
    public void Fuzzy_AddsHumanFriendlyQuantizer()
    {
        var options = new TimeHumanizationBuilder()
            .AsRelative()
            .Fuzzy()
            .Build();

        Assert.Equal(TimeHumanizationMode.Relative, options.Mode);
        Assert.Same(HumanFriendlyTimeSpanQuantizer.Default, options.Quantizer);
    }

    [Fact]
    public void Precise_ClearsQuantizer()
    {
        var options = new TimeHumanizationBuilder()
            .Fuzzy()
            .Precise()
            .Build();

        Assert.Null(options.Quantizer);
    }

    [Fact]
    public void From_CopiesExistingOptions()
    {
        var source = new TimeHumanizationOptions
        {
            Mode = TimeHumanizationMode.Relative,
            Tense = Tense.Past,
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Day,
            MaxComponents = 3,
            IncludeZeroUnits = true,
            DecompositionMode = TimeSpanDecompositionMode.Flat,
            ListFormatting = new() { Conjunction = ListConjunction.Or }
        };

        var options = new TimeHumanizationBuilder().From(source).Build();

        Assert.Equal(TimeHumanizationMode.Relative, options.Mode);
        Assert.Equal(Tense.Past, options.Tense);
        Assert.Equal(TimeUnit.Minute, options.MinUnit);
        Assert.Equal(TimeUnit.Day, options.MaxUnit);
        Assert.Equal(3, options.MaxComponents);
        Assert.True(options.IncludeZeroUnits);
        Assert.Equal(TimeSpanDecompositionMode.Flat, options.DecompositionMode);
        Assert.Equal(ListConjunction.Or, options.ListFormatting.Conjunction);
    }
}
