// -----------------------------------------------------------------------
// <copyright file="TextSanitizationExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Text.Sanitization;
using Xunit;

namespace MyNet.Utilities.Tests.Text;

public class TextSanitizationExtensionsTests
{
    [Fact]
    public void Sanitize_AlphaNumeric_RemovesPunctuation()
    {
        var result = "ab-c_12!?".Sanitize(Sanitizer.AlphaNumeric);

        Assert.Equal("abc12", result);
    }

    [Fact]
    public void Sanitize_WithOptions_RespectsAdditionalCharacters()
    {
        var sanitizer = new CharacterFilterTextSanitizer(new()
        {
            KeepLetters = true,
            KeepDigits = true,
            AdditionalAllowedCharacters = "-_"
        });

        var result = "ab-c_12!?".Sanitize(sanitizer);

        Assert.Equal("ab-c_12", result);
    }

    [Fact]
    public void SanitizeFileName_RemovesInvalidFileNameCharacters()
    {
        var result = "repor:t*2026?.txt".SanitizeFileName();

        Assert.DoesNotContain(':', result);
        Assert.DoesNotContain('*', result);
        Assert.DoesNotContain('?', result);
    }

    [Fact]
    public void Sanitize_IdentifierPreset_OnlyKeepsIdentifierCharacters()
    {
        var result = "User Name-42!".Sanitize(Sanitizer.Identifier);

        Assert.Equal("UserName42", result);
    }

    [Fact]
    public void Sanitize_UrlSegmentPreset_KeepsDashAndUnderscore()
    {
        var result = "My Segment-_42!?".Sanitize(Sanitizer.UrlSegment);

        Assert.Equal("MySegment-_42", result);
    }
}
