// -----------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Text.Formatting;
using Xunit;

namespace MyNet.Utilities.Tests.Text;

public class StringExtensionsTests
{
    private const string Format = "This is a format with three numbers: {0}-{1}-{2}.";
    private const string Expected = "This is a format with three numbers: 1-2-3.";

    [Fact]
    public void CanFormatStringWithExactNumberOfArguments() => Assert.Equal(Expected, Format.FormatWith(CultureInfo.CurrentCulture, 1, 2, 3));

    [Fact]
    public void CanFormatStringWithMoreArguments() => Assert.Equal(Expected, Format.FormatWith(CultureInfo.CurrentCulture, 1, 2, 3, 4, 5));

    [Fact]
    public void CannotFormatStringWithLessArguments() => Assert.Throws<FormatException>(() => Format.FormatWith(CultureInfo.CurrentCulture, 1, 2));

    [Theory]
    [InlineData("en-US", "6,666.66")]
    [InlineData("ru-RU", "6 666,66")]
    public void CanSpecifyCultureExplicitly(string culture, string expected) => Assert.Equal(expected, "{0:N2}".FormatWith(new CultureInfo(culture), 6666.66));

    [Fact]
    public void IncrementAlphaCanReachZ()
    {
        var existing = Enumerable.Range(1, 25).Select(i => $"Item{(char)('A' + i - 1)}");

        Assert.Equal("ItemZ", "Item".IncrementAlpha(existing));
    }

    [Fact]
    public void IncrementAlphaCanContinueAfterZ()
    {
        var existing = Enumerable.Range(1, 26).Select(i => $"Item{(char)('A' + i - 1)}");

        Assert.Equal("ItemAA", "Item".IncrementAlpha(existing));
    }

    [Fact]
    public void ToIncrementWithOptionsCanUseNumericKind()
    {
        string[] existing = ["Item01", "Item02"];

        var result = "Item".Increment(new IncrementTransformOptions
        {
            ExistingStrings = existing,
            Kind = IncrementKind.Numeric,
            MinIncrement = 1,
            Step = 1,
            NumericFormat = "00"
        });

        Assert.Equal("Item03", result);
    }

    [Fact]
    public void ToIncrementWithOptionsCanUseAlphaKind()
    {
        var existing = Enumerable.Range(1, 26).Select(i => $"Item{(char)('A' + i - 1)}");

        var result = "Item".Increment(new IncrementTransformOptions
        {
            ExistingStrings = existing,
            Kind = IncrementKind.Alpha
        });

        Assert.Equal("ItemAA", result);
    }

    [Fact]
    public void ToRelativeUriSkipsEmptySegments()
    {
        var uri = "root".ToRelativeUri("child", string.Empty, null, "leaf");

        Assert.Equal("root/child/leaf", uri.OriginalString);
    }

    [Fact]
    public void ToRelativeUriEncodesSegments()
    {
        var uri = "root".ToRelativeUri("hello world", "a+b");

        Assert.Equal("root/helloworld/ab", uri.OriginalString);
    }

    [Fact]
    public void ToRelativeUriRejectsNavigationSegments() => Assert.Throws<ArgumentException>(() => "root".ToRelativeUri(".."));

    [Fact]
    public void ToWebUriBuildsQueryStringWithoutExtraSlash()
    {
        var uri = "https://example.com/api".ToWebUri(("q", "hello world"), ("lang", "fr"));

        Assert.Equal("https://example.com:443/api?q=helloworld&lang=fr", uri.OriginalString);
    }

    [Fact]
    public void ToWebUriAppendsToExistingQueryString()
    {
        var uri = "https://example.com/api?existing=1".ToWebUri(("lang", "fr"));

        Assert.Equal("https://example.com:443/api?existing=1&lang=fr", uri.OriginalString);
    }

    [Fact]
    public void ToWebUriThrowsWhenBaseIsNotAbsolute() => Assert.Throws<ArgumentException>(() => "api/resource".ToWebUri(("q", "x")));
}
