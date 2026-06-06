// -----------------------------------------------------------------------
// <copyright file="TextFormattingExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Text.Formatting;
using Xunit;
using TextPortal = MyNet.Text.Text;

namespace MyNet.Text.Tests;

public class TextFormattingExtensionsTests
{
    [Fact]
    public void ToInitials_BuildsInitialsAcrossWhitespace()
    {
        var result = "john\tfitzgerald\nkennedy".Initials();

        Assert.Equal("JFK", result);
    }

    [Fact]
    public void Initials_Transform_IsAvailable()
    {
        var result = Formatter.Initials.Apply("Ada Lovelace", CultureInfo.InvariantCulture);

        Assert.Equal("AL", result);
    }

    [Fact]
    public void Pipeline_Initials_Works()
    {
        var result = TextPortal.For("Grace Hopper", CultureInfo.InvariantCulture)
            .Initials()
            .Value;

        Assert.Equal("GH", result);
    }

    [Theory]
    [InlineData("CeciEstUnTest", "Ceci est un test")]
    [InlineData("ceciEstUnTest", "Ceci est un test")]
    [InlineData("ceci_est_un_test", "Ceci est un test")]
    [InlineData("ceci-est-un-test", "Ceci est un test")]
    [InlineData("HTTPStatusCode404", "Http status code 404")]
    public void HumanizeKey_ProducesReadableSentence(string input, string expected)
    {
        var result = input.HumanizeKey(CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void HumanizeKey_Transform_IsAvailable()
    {
        var result = Formatter.HumanizeKey.Apply("CeciEstUnTest", CultureInfo.InvariantCulture);

        Assert.Equal("Ceci est un test", result);
    }

    [Fact]
    public void Pipeline_HumanizeKey_Works()
    {
        var result = TextPortal.For("CeciEstUnTest", CultureInfo.InvariantCulture)
            .HumanizeKey()
            .Value;

        Assert.Equal("Ceci est un test", result);
    }
}
