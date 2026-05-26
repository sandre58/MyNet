// -----------------------------------------------------------------------
// <copyright file="QuantityExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Facade;
using MyNet.Primitives;
using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Humanizer.Tests.Extensions;

[UseCulture("en-US")]
[Collection("UseCultureSequential")]
public class QuantityExtensionsTests
{
    [Fact]
    public void ToFileSize_DefaultStyle_UsesFullUnitName()
    {
        var result = 1d.Of(DataSizeUnit.Byte).Humanize();

        Assert.Equal("1 byte", result, StringComparer.Ordinal);
    }

    [Fact]
    public void ToFileSize_AbbreviationStyle_UsesShortUnitName()
    {
        var result = 1d.Of(DataSizeUnit.Byte).Humanize(DisplayStyle.Abbreviation);

        Assert.Equal("1 b", result, StringComparer.Ordinal);
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void ToFileSize_FrCulture_UsesLocalizedUnitName()
    {
        var result = 1d.Of(DataSizeUnit.Byte).Humanize(DisplayStyle.Default);

        Assert.EndsWith("octet", result, StringComparison.Ordinal);
    }

    [Fact]
    public void ToPreferredFileSize_SimplifiesToKilobyteWithinBounds()
    {
        var result = 212454d.Of(DataSizeUnit.Byte)
            .Simplify(DataSizeUnit.Byte, DataSizeUnit.Gigabyte)
            .Humanize();

        Assert.Contains("kilobyte", result, StringComparison.Ordinal);
    }

    [Fact]
    public void ToPreferredFileSize_HonorsMinUnitConstraint()
    {
        var result = 244587587d.Of(DataSizeUnit.Byte)
            .Simplify(DataSizeUnit.Gigabyte, DataSizeUnit.Terabyte)
            .Humanize();

        Assert.Contains("gigabyte", result, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("request", 0, "0 request")]
    [InlineData("request", 1, "1 request")]
    [InlineData("request", 2, "2 request")]
    public void Quantify_StringAndInt_ReturnsCurrentRendererOutput(string word, int quantity, string expected) => Assert.Equal(expected, word.Quantify(quantity));

    [Fact]
    public void Quantify_UsesCustomNumericFormatAndCulture()
    {
        var result = "request".Quantify(12345, "N0", new("fr-FR"));

        Assert.Contains("12", result, StringComparison.Ordinal);
        Assert.Contains("345", result, StringComparison.Ordinal);
        Assert.EndsWith("request", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Quantify_NumberOverload_ForwardsToWordQuantify()
    {
        var result = 2.Quantify("request");

        Assert.Equal("2 request", result);
    }

    [Fact]
    public void Humanize_QuantityOfEnumUnit_CombinesUnitHumanizationAndQuantity()
    {
        var quantity = Quantity.Of(2, TimeUnit.Minute);

        Assert.Equal("2 Minute(s)", quantity.Humanize());
    }

    [Fact]
    public void Humanize_QuantityOfEnumUnit_WithAbbreviationStyle_UsesAbbreviatedUnit()
    {
        var quantity = Quantity.Of(2, TimeUnit.Minute);

        var result = quantity.Humanize(DisplayStyle.Abbreviation);

        Assert.Equal("2 min", result, StringComparer.Ordinal);
    }
}
