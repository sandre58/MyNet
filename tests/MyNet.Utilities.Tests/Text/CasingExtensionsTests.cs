// -----------------------------------------------------------------------
// <copyright file="CasingExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Text.TextCasing;
using Xunit;

namespace MyNet.Utilities.Tests.Text;

public class CasingExtensionsTests
{
    [Theory]
    [InlineData("lower case statement", "Lower Case Statement")]
    [InlineData("Sentence casing", "Sentence Casing")]
    [InlineData("honors UPPER case", "Honors UPPER Case")]
    [InlineData("Title Case", "Title Case")]
    [InlineData("title case (with parenthesis)", "Title Case (With Parenthesis)")]
    public void ApplyCaseTitle(string input, string expectedOutput) => Assert.Equal(expectedOutput, input.ApplyCase(LetterCasing.Title));

    [Theory]
    [InlineData("lower case statement", "lower case statement")]
    [InlineData("Sentence casing", "sentence casing")]
    [InlineData("No honor for UPPER case", "no honor for upper case")]
    [InlineData("Title Case", "title case")]
    public void ApplyCaseLower(string input, string expectedOutput) => Assert.Equal(expectedOutput, input.ApplyCase(LetterCasing.Lower));

    [Theory]
    [InlineData("lower case statement", "Lower case statement")]
    [InlineData("Sentence casing", "Sentence casing")]
    [InlineData("honors UPPER case", "Honors UPPER case")]
    public void ApplyCaseSentence(string input, string expectedOutput) => Assert.Equal(expectedOutput, input.ApplyCase(LetterCasing.Sentence));

    [Theory]
    [InlineData("lower case statement", "LOWER CASE STATEMENT")]
    [InlineData("Sentence casing", "SENTENCE CASING")]
    [InlineData("Title Case", "TITLE CASE")]
    public void ApplyCaseAllCaps(string input, string expectedOutput) => Assert.Equal(expectedOutput, input.ApplyCase(LetterCasing.Upper));
}
