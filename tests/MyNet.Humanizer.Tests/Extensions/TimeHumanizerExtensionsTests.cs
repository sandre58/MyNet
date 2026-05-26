// -----------------------------------------------------------------------
// <copyright file="TimeHumanizerExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Humanizer.Static;
using MyNet.Humanizer.Temporal;
using MyNet.Utilities;
using MyNet.Temporal.Decomposition;
using Xunit;

namespace MyNet.Humanizer.Tests.Extensions;

[UseCulture(Culture)]
[Collection("UseCultureSequential")]
public class TimeHumanizerExtensionsTests
{
    public const string Culture = "en-US";

    [Fact]
    public void DefaultPreset_UsesDurationMode()
    {
        Assert.Equal(TimeHumanizationMode.Duration, TimeHumanizationPresets.Default.Mode);
        Assert.Equal(2, TimeHumanizationPresets.Default.MaxComponents);
    }

    [Fact]
    public void RelativePreset_UsesRelativeModeAndQuantizer()
    {
        Assert.Equal(TimeHumanizationMode.Relative, TimeHumanizationPresets.Relative.Mode);
        Assert.NotNull(TimeHumanizationPresets.Relative.Quantizer);
        Assert.Equal(1, TimeHumanizationPresets.Relative.MaxComponents);
    }

    [Fact]
    public void Humanize_DefaultPreset_ReturnsDuration()
    {
        var actual = TimeSpan.FromSeconds(90).Humanize();

        Assert.Equal("1 minute, 30 seconds", actual);
    }

    [Fact]
    public void Humanize_WithRelativePreset_InfersFutureTenseForPositiveDuration()
    {
        var actual = TimeSpan.FromMinutes(2).Humanize(TimeHumanizationPresets.Relative);

        Assert.Equal("2 minutes from now", actual);
    }

    [Fact]
    public void Humanize_WithRelativePreset_InfersPastTenseForNegativeDuration()
    {
        var actual = TimeSpan.FromMinutes(-2).Humanize(TimeHumanizationPresets.Relative);

        Assert.Equal("2 minutes ago", actual);
    }

    [Fact]
    public void Humanize_WithExplicitTense_OverridesInferredTense()
    {
        var actual = TimeSpan.FromMinutes(2).Humanize(new TimeHumanizationOptions
        {
            Mode = TimeHumanizationMode.Relative,
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Minute,
            MaxComponents = 1,
            Tense = Tense.Past
        });

        Assert.Equal("2 minutes ago", actual);
    }

    [Fact]
    public void Humanize_WithBuilder_CanProduceRelativeMultiComponentText()
    {
        var actual = TimeSpan.FromSeconds(90).Humanize(x => _ = x
            .AsRelative()
            .WithTense(Tense.Future)
            .UseUnits(TimeUnit.Second, TimeUnit.Minute)
            .MaxComponents(2));

        Assert.Equal("1 minute and 30 seconds from now", actual);
    }

    [Fact]
    public void Humanize_WithListFormatting_UsesConjunctionInDurationMode()
    {
        var actual = TimeSpan.FromMilliseconds(62020).Humanize(new TimeHumanizationOptions
        {
            Mode = TimeHumanizationMode.Duration,
            MinUnit = TimeUnit.Millisecond,
            MaxUnit = TimeUnit.Minute,
            MaxComponents = 3,
            ListFormatting = new()
            {
                Separator = ", ",
                Conjunction = ListConjunction.And
            }
        });

        Assert.Equal("1 minute, 2 seconds and 20 milliseconds", actual);
    }

    [Fact]
    public void Humanize_WithFuzzyPreset_59Seconds_RemainsSecondsWhenOnlyOneComponentIsAvailable()
    {
        var actual = TimeSpan.FromSeconds(59).Humanize(TimeHumanizationPresets.Fuzzy);

        Assert.Equal("59 seconds from now", actual);
    }

    [Fact]
    public void Humanize_WithRelativeMode_NoQuantizer_UsesStrictDecomposition()
    {
        var actual = TimeSpan.FromMinutes(119).Humanize(new TimeHumanizationOptions
        {
            Mode = TimeHumanizationMode.Relative,
            MaxComponents = 1
        });

        Assert.Equal("1 hour from now", actual);
    }

    [Fact]
    public void Humanize_WithRelativeMode_OneYearInput_UsesRoundedDayConversion()
    {
        var actual = 1.ToTimeSpan(TimeUnit.Year).Humanize(new TimeHumanizationOptions
        {
            Mode = TimeHumanizationMode.Relative,
            MaxComponents = 1,
            Tense = Tense.Future
        });

        Assert.Equal("11 months from now", actual);
    }

    [Fact]
    public void Humanize_WithSmallestUnitOnly_ReturnsFlattenedMinutes()
    {
        var actual = TimeSpan.FromMinutes(63).Humanize(new TimeHumanizationOptions
        {
            Mode = TimeHumanizationMode.Duration,
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Hour,
            DecompositionMode = TimeSpanDecompositionMode.SmallestUnitOnly,
            MaxComponents = 1
        });

        Assert.Equal("63 minutes", actual);
    }

    [Fact]
    public void Tense_ReturnsFutureForZeroAndPastForNegative()
    {
        Assert.Equal(Tense.Future, TimeSpan.Zero.Tense());
        Assert.Equal(Tense.Past, TimeSpan.FromSeconds(-1).Tense());
    }

    [Fact]
    public void ToDecompositionOptions_MapsAllFields()
    {
        var quantizer = HumanFriendlyTimeSpanQuantizer.Default;
        var options = new TimeHumanizationOptions
        {
            MinUnit = TimeUnit.Minute,
            MaxUnit = TimeUnit.Day,
            DecompositionMode = TimeSpanDecompositionMode.Flat,
            MaxComponents = 3,
            IncludeZeroUnits = true,
            Quantizer = quantizer
        };

        var decomposition = options.ToDecompositionOptions();

        Assert.Equal(TimeUnit.Minute, decomposition.MinUnit);
        Assert.Equal(TimeUnit.Day, decomposition.MaxUnit);
        Assert.Equal(TimeSpanDecompositionMode.Flat, decomposition.Mode);
        Assert.Equal(3, decomposition.MaxComponents);
        Assert.True(decomposition.IncludeZeroUnits);
        Assert.Same(quantizer, decomposition.Quantizer);
    }
}
