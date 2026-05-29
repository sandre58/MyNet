// -----------------------------------------------------------------------
// <copyright file="DefaultRandomGeneratorAdditionalTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Generator.Facade;
using Xunit;

namespace MyNet.Generator.Tests;

[Collection(RandomGeneratorTestCollection.Name)]
public sealed class DefaultRandomGeneratorAdditionalTests
{
    [Fact]
    public void Bytes_ReturnsRequestedLength()
    {
        var bytes = RandomGenerator.Current.Bytes(16);

        Assert.Equal(16, bytes.Length);
    }

    [Fact]
    public void Bytes_NegativeCount_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => RandomGenerator.Current.Bytes(-1));

    [Fact]
    public void String_ReturnsRequestedLength()
    {
        var text = RandomGenerator.Current.String(8);

        Assert.Equal(8, text.Length);
    }

    [Fact]
    public void Letter_Uppercase_IsUppercase()
    {
        for (var i = 0; i < 50; i++)
        {
            var letter = RandomGenerator.Current.Letter(uppercase: true);
            Assert.True(char.IsUpper(letter));
        }
    }

    [Fact]
    public void Digit_ReturnsNumericCharacter()
    {
        for (var i = 0; i < 50; i++)
        {
            var digit = RandomGenerator.Current.Digit();
            Assert.True(char.IsDigit(digit));
        }
    }

    [Fact]
    public void Char_And_Chars_ReturnValuesInRange()
    {
        var value = RandomGenerator.Current.Char('a', 'd');
        Assert.InRange(value, 'a', 'c');

        var chars = RandomGenerator.Current.Chars('a', 'd', count: 4);
        Assert.Equal(4, chars.Length);
        Assert.All(chars, c => Assert.InRange(c, 'a', 'c'));
    }

    [Fact]
    public void Byte_And_SByte_ReturnValuesInRange()
    {
        for (var i = 0; i < 20; i++)
        {
            Assert.InRange(RandomGenerator.Current.Byte(1, 5), (byte)1, (byte)5);
            Assert.InRange(RandomGenerator.Current.SByte(-3, 3), (sbyte)-3, (sbyte)3);
        }
    }

    [Fact]
    public void Even_NoEvenNumbersInRange_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => RandomGenerator.Current.Even(1, 2));

    [Fact]
    public void Odd_NoOddNumbersInRange_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => RandomGenerator.Current.Odd(2, 3));

    [Fact]
    public void Date_MinGreaterThanMax_Throws()
    {
        var min = new DateTime(2025, 1, 1);
        var max = new DateTime(2020, 1, 1);

        Assert.Throws<ArgumentOutOfRangeException>(() => RandomGenerator.Current.Date(min, max));
    }

    [Fact]
    public void RandomGenerator_Reset_RestoresDefaultService()
    {
        var custom = new DefaultRandomGenerator(new FixedRandomSource(0.5));

        using (RandomGeneratorTestGate.Replace(custom))
        {
            RandomGenerator.Reset();

            Assert.NotSame(custom, RandomGenerator.Current);
        }
    }

    [Fact]
    public void SystemRandomSource_ProducesValues()
    {
        var source = new SystemRandomSource();

        Assert.InRange(source.NextInt32(0, 10), 0, 9);
        Assert.InRange(source.NextDouble(), 0.0, 1.0);

        var buffer = new byte[4];
        source.NextBytes(buffer);
        Assert.Equal(4, buffer.Length);
    }

    private sealed class FixedRandomSource(double value) : IRandomSource
    {
        public int NextInt32(int minInclusive, int maxExclusive) => minInclusive;

        public double NextDouble() => value;

        public void NextBytes(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = 1;
        }
    }
}
