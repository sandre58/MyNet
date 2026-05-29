// -----------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Xunit;

namespace MyNet.Primitives.Tests.Extensions;

public sealed class StringExtensionsTests
{
    [Fact]
    public void OrEmpty_Null_ReturnsEmpty()
    {
        const string? value = null;
        Assert.Equal(string.Empty, value.OrEmpty());
    }

    [Fact]
    public void Or_Whitespace_ReturnsPlaceholder()
    {
        Assert.Equal("fallback", "   ".Or("fallback"));
        Assert.Equal("value", "value".Or("fallback"));
    }

    [Fact]
    public void ContainsAny_MatchesCaseInsensitive()
    {
        Assert.True("Hello World".ContainsAny("world"));
        Assert.False("Hello".ContainsAny("xyz"));
        Assert.False(((string?)null).ContainsAny("a"));
    }

    [Fact]
    public void NotContainsAny_ReturnsExpected()
    {
        Assert.True("Hello".NotContainsAny("xyz"));
        Assert.False("Hello".NotContainsAny("ell"));
        Assert.True(((string?)null).NotContainsAny("a"));
    }

    [Fact]
    public void FormatWith_UsesCurrentCulture()
    {
        var result = "{0:N0}".FormatWith(CultureInfo.CurrentCulture, 1234);
        Assert.Equal(1234.ToString("N0", CultureInfo.CurrentCulture), result);
    }

    [Fact]
    public void FormatWithInvariant_UsesInvariantCulture()
    {
        var result = "{0}".FormatWithInvariant(42);
        Assert.Equal("42", result);
    }

    [Fact]
    public void ToVersion_ParsesVersion() => Assert.Equal(new(1, 2, 3, 4), "1.2.3.4".ToVersion());

    [Fact]
    public void TryToVersion_InvalidInput_ReturnsFalse()
    {
        Assert.False("not-a-version".TryToVersion(out _));
        Assert.True("1.0".TryToVersion(out var version));
        Assert.Equal(new(1, 0), version);
    }
}
