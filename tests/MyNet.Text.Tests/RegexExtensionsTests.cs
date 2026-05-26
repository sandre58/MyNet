// -----------------------------------------------------------------------
// <copyright file="RegexExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using Xunit;

namespace MyNet.Text.Tests;

public partial class RegexExtensionsTests
{
    [Fact]
    public void Matches_WithNullPattern_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => "abc".Matches(null!));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Matches_WithNullOrEmptyValue_ReturnsFalse(string? value) => Assert.False(value.Matches(MyRegex()));

    [Fact]
    public void Matches_WhenPatternFound_ReturnsTrue() => Assert.True("prefix-abc-suffix".Matches(MyRegex()));

    [Theory]
    [InlineData("alice@example.com", true)]
    [InlineData("alice.example.com", false)]
    public void IsEmailAddress_UsesEmailPattern(string? value, bool expected) => Assert.Equal(expected, value.IsEmailAddress());

    [Theory]
    [InlineData("+33 6 12 34 56 78", true)]
    [InlineData("phone-number", false)]
    public void IsPhoneNumber_UsesPhonePattern(string? value, bool expected) => Assert.Equal(expected, value.IsPhoneNumber());

    [GeneratedRegex("abc", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
