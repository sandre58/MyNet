// -----------------------------------------------------------------------
// <copyright file="TextFormattingExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Utilities.Text.Formatting;
using Xunit;
using TextPortal = MyNet.Utilities.Text.Text;

namespace MyNet.Utilities.Tests.Text;

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
}
