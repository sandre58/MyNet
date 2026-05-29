// -----------------------------------------------------------------------
// <copyright file="RandomEnumTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Generator;
using MyNet.Generator.Facade;
using MyNet.Primitives;
using Xunit;

namespace MyNet.Generator.Tests;

public class RandomEnumTests
{
    private static readonly DefaultRandomGenerator Generator = new(new SystemRandomSource());

    private enum ThreeValues
    {
        Alpha,
        Beta,
        Gamma
    }

    [Fact]
    public void Enum_NoExclusions_ReturnsValidEnumValue()
    {
        for (var i = 0; i < 50; i++)
        {
            var result = RandomGenerator.Current.Enum<GenderType>();
            Assert.True(Enum.IsDefined(result),
                $"Returned value '{result}' is not a valid GenderType.");
        }
    }

    [Fact]
    public void Enum_WithExclusion_NeverReturnsExcludedValue()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Enum(GenderType.Female);
            Assert.NotEqual(GenderType.Female, result);
        }
    }

    [Fact]
    public void Enum_ExcludeAllButOne_AlwaysReturnsRemainingValue()
    {
        for (var i = 0; i < 50; i++)
        {
            var result = RandomGenerator.Current.Enum(GenderType.Male);
            Assert.Equal(GenderType.Female, result);
        }
    }

    [Fact]
    public void Enum_ExcludeAll_ThrowsArgumentException() =>
        _ = Assert.Throws<ArgumentException>(() =>
            RandomGenerator.Current.Enum(GenderType.Male, GenderType.Female));

    [Fact]
    public void Enum_ThreeValues_CanReturnAnyValue()
    {
        var seen = new HashSet<ThreeValues>();

        for (var i = 0; i < 500; i++)
        {
            seen.Add(Generator.Enum<ThreeValues>());
        }

        // With 500 draws from 3 values, all should appear
        Assert.Contains(ThreeValues.Alpha, seen);
        Assert.Contains(ThreeValues.Beta, seen);
        Assert.Contains(ThreeValues.Gamma, seen);
    }
}
