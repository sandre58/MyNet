// -----------------------------------------------------------------------
// <copyright file="RandomNumberTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Generator.Static;
using Xunit;

namespace MyNet.Utilities.Tests.Generator;

public class RandomNumberTests
{
    [Fact]
    public void Decimal_DefaultRange_ReturnsValueBetween0And1()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Decimal();
            Assert.InRange(result, 0.0m, 1.0m);
        }
    }

    [Fact]
    public void Decimal_CustomRange_ReturnsValueInRange()
    {
        const decimal min = 50.0m;
        const decimal max = 100.0m;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Decimal(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void Decimal_DefaultRange_IsNotAlwaysTheSame()
    {
        var first = RandomGenerator.Current.Decimal();
        var different = false;

        for (var i = 0; i < 20; i++)
        {
            if (RandomGenerator.Current.Decimal() != first)
            {
                different = true;
                break;
            }
        }

        Assert.True(different, "Decimal() returned the same value every time — likely broken.");
    }

    [Fact]
    public void Float_DefaultRange_ReturnsValueBetween0And1()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Float();
            Assert.InRange(result, 0.0f, 1.0f);
        }
    }

    [Fact]
    public void Float_CustomRange_ReturnsValueInRange()
    {
        const float min = -5.0f;
        const float max = 5.0f;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Float(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void UInt_CustomRange_ReturnsValueInRange()
    {
        const uint min = 10;
        const uint max = 100;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.UInt(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void UInt_DefaultRange_ReturnsValueInRange()
    {
        var result = RandomGenerator.Current.UInt();
        Assert.InRange(result, uint.MinValue, uint.MaxValue);
    }

    [Fact]
    public void ULong_CustomRange_ReturnsValueInRange()
    {
        const ulong min = 0;
        const ulong max = 500;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.ULong(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void Long_CustomRange_ReturnsValueInRange()
    {
        const long min = -1000L;
        const long max = 1000L;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Long(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void Long_DefaultRange_ReturnsValueInRange()
    {
        var result = RandomGenerator.Current.Long();
        Assert.InRange(result, long.MinValue, long.MaxValue);
    }

    [Fact]
    public void Short_CustomRange_ReturnsValueInRange()
    {
        const short min = -100;
        const short max = 100;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Short(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void Short_DefaultRange_ReturnsValueInRange()
    {
        var result = RandomGenerator.Current.Short();
        Assert.InRange(result, short.MinValue, short.MaxValue);
    }

    [Fact]
    public void UShort_CustomRange_ReturnsValueInRange()
    {
        const ushort min = 0;
        const ushort max = 500;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.UShort(min, max);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void Even_ReturnsEvenNumber()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Even(0, 100);
            Assert.Equal(0, result % 2);
        }
    }

    [Fact]
    public void Odd_ReturnsOddNumber()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Odd(1, 101);
            Assert.NotEqual(0, result % 2);
        }
    }

    [Fact]
    public void Int_ReturnsExclusiveUpperBound()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Int(0, 1);
            Assert.Equal(0, result);
        }
    }

    [Fact]
    public void Double_CustomRange_ReturnsValueInRange()
    {
        const double min = -10.0;
        const double max = 10.0;

        for (var i = 0; i < 100; i++)
        {
            var result = RandomGenerator.Current.Double(min, max);
            Assert.InRange(result, min, max);
        }
    }
}
