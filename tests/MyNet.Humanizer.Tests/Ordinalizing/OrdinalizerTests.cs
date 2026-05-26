// -----------------------------------------------------------------------
// <copyright file="OrdinalizerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Ordinalizing;
using MyNet.Humanizer.Ordinalizing.Cultures;
using MyNet.Text;
using Xunit;

namespace MyNet.Humanizer.Tests.Ordinalizing;

public class OrdinalizerTests
{
    [Fact]
    public void Ordinalizers_Invariant_IsNotNull() => Assert.NotNull(Ordinalizers.Invariant);

    [Fact]
    public void Ordinalizers_English_IsNotNull() => Assert.NotNull(Ordinalizers.English);

    [Fact]
    public void Ordinalizers_French_IsNotNull() => Assert.NotNull(Ordinalizers.French);

    [Theory]
    [InlineData(1, "1st")]
    [InlineData(2, "2nd")]
    [InlineData(3, "3rd")]
    [InlineData(4, "4th")]
    [InlineData(5, "5th")]
    [InlineData(10, "10th")]
    [InlineData(11, "11th")]
    [InlineData(12, "12th")]
    [InlineData(13, "13th")]
    [InlineData(20, "20th")]
    [InlineData(21, "21st")]
    [InlineData(22, "22nd")]
    [InlineData(23, "23rd")]
    [InlineData(100, "100th")]
    [InlineData(101, "101st")]
    public void EnglishOrdinalizer_Ordinalize_ReturnsCorrectEnglishOrdinals(long number, string expected)
    {
        var ordinalizer = Ordinalizers.English;

        var result = ordinalizer.Ordinalize(number);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(100)]
    public void InvariantOrdinalizer_Ordinalize_ReturnsNonEmptyString(long number)
    {
        var ordinalizer = Ordinalizers.Invariant;

        var result = ordinalizer.Ordinalize(number);

        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(100)]
    public void EnglishOrdinalizer_Ordinalize_WithNullOptions_Works(long number)
    {
        var ordinalizer = Ordinalizers.English;

        var result = ordinalizer.Ordinalize(number);

        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(100)]
    public void EnglishOrdinalizer_Ordinalize_WithOptions_Works(long number)
    {
        var ordinalizer = Ordinalizers.English;
        var options = new OrdinalizationOptions();

        var result = ordinalizer.Ordinalize(number, options);

        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public void FrenchOrdinalizer_Ordinalize_ReturnsNonEmptyString(long number)
    {
        var ordinalizer = Ordinalizers.French;

        var result = ordinalizer.Ordinalize(number);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void OrdinalizationOptions_Constructor_CreatesNewInstance()
    {
        var options = new OrdinalizationOptions();

        Assert.NotNull(options);
        Assert.Null(options.Gender);
    }

    [Fact]
    public void OrdinalizationOptions_WithGender_SetsGender()
    {
        var options = new OrdinalizationOptions { Gender = GrammaticalGender.Feminine };

        Assert.Equal(GrammaticalGender.Feminine, options.Gender);
    }

    [Theory]
    [InlineData(GrammaticalGender.Masculine)]
    [InlineData(GrammaticalGender.Feminine)]
    public void FrenchOrdinalizer_WithGenderOption_Works(GrammaticalGender gender)
    {
        var ordinalizer = Ordinalizers.French;
        var options = new OrdinalizationOptions { Gender = gender };

        var result = ordinalizer.Ordinalize(1, options);

        Assert.NotEmpty(result);
    }
}
