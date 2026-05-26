// -----------------------------------------------------------------------
// <copyright file="CharacterFilterTextSanitizerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Text.Sanitization;
using Xunit;

namespace MyNet.Text.Tests;

public sealed class CharacterFilterTextSanitizerTests
{
    [Fact]
    public void Apply_KeepsOnlyConfiguredCharacterClasses()
    {
        var options = new TextSanitizationOptions
        {
            KeepLetters = true,
            KeepDigits = false,
            KeepWhitespace = false,
            AdditionalAllowedCharacters = "-"
        };
        var sut = new CharacterFilterTextSanitizer(options);

        var result = sut.Apply("Ab12!- 9", CultureInfo.InvariantCulture);

        Assert.Equal("Ab-", result);
    }

    [Fact]
    public void Apply_WithDigitsAndWhitespace_PreservesBoth()
    {
        var options = new TextSanitizationOptions { KeepLetters = false, KeepDigits = true, KeepWhitespace = true };
        var sut = new CharacterFilterTextSanitizer(options);

        var result = sut.Apply("A 1!", CultureInfo.InvariantCulture);

        Assert.Equal(" 1", result);
    }

    [Fact]
    public void Apply_EmptyInput_ReturnsEmpty()
    {
        var sut = new CharacterFilterTextSanitizer(new());

        var result = sut.Apply(string.Empty, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }
}
