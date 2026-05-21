// -----------------------------------------------------------------------
// <copyright file="RandomBoolTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Generator.Static;
using Xunit;

namespace MyNet.Utilities.Tests.Generator;

public class RandomBoolTests
{
    [Fact]
    public void Bool_ReturnsOnlyTrueAndFalse()
    {
        for (var i = 0; i < 200; i++)
        {
            var result = RandomGenerator.Current.Bool();
            Assert.True(result || !result);
        }
    }

    [Fact]
    public void Bool_ReturnsBothTrueAndFalseOverManyDraws()
    {
        var seenTrue = false;
        var seenFalse = false;

        for (var i = 0; i < 500; i++)
        {
            if (RandomGenerator.Current.Bool()) seenTrue = true;
            else seenFalse = true;

            if (seenTrue && seenFalse) break;
        }

        Assert.True(seenTrue, "Bool() never returned true over 500 draws.");
        Assert.True(seenFalse, "Bool() never returned false over 500 draws.");
    }

    [Fact]
    public void Weighted_Probability1_AlwaysReturnsTrue()
    {
        for (var i = 0; i < 100; i++)
            Assert.True(RandomGenerator.Current.Weighted(1.0f));
    }

    [Fact]
    public void Weighted_Probability0_AlwaysReturnsFalse()
    {
        for (var i = 0; i < 100; i++)
            Assert.False(RandomGenerator.Current.Weighted(0.0f));
    }

    [Theory]
    [InlineData(-0.01f)]
    [InlineData(1.01f)]
    public void Weighted_InvalidProbability_ThrowsArgumentOutOfRangeException(float probability)
        => Assert.Throws<ArgumentOutOfRangeException>(() => RandomGenerator.Current.Weighted(probability));
}
