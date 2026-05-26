// -----------------------------------------------------------------------
// <copyright file="TextRandomGeneratorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Moq;
using MyNet.Generator;
using MyNet.Text.Randomize;
using Xunit;

namespace MyNet.Text.Tests;

public sealed class TextRandomGeneratorTests
{
    [Fact]
    public void Randomize_DigitToken_UsesRandomGenerator()
    {
        var random = new Mock<IRandomGenerator>();
        random.Setup(r => r.Int(0, 10)).Returns(7);

        var result = new TextRandomGenerator(random.Object).Randomize("#");

        Assert.Equal("7", result);
    }

    [Fact]
    public void Randomize_EscapesLiteralCharacter()
    {
        var random = new Mock<IRandomGenerator>();

        var result = new TextRandomGenerator(random.Object).Randomize(@"\#");

        Assert.Equal("#", result);
    }

    [Fact]
    public void Randomize_UnclosedBrace_ThrowsFormatException()
    {
        var random = new Mock<IRandomGenerator>();

        Assert.Throws<FormatException>(() => new TextRandomGenerator(random.Object).Randomize("{abc"));
    }
}
