// -----------------------------------------------------------------------
// <copyright file="ColorFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Media;
using MyNet.Utilities.Generator;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class ColorFakerTests
{
    [Fact]
    public void Hex_ShouldFormatRgbAsUppercaseHex()
    {
        var random = new Mock<IRandomGenerator>();
        random.SetupSequence(x => x.Byte(0, 255))
            .Returns(0x0A)
            .Returns(0x1B)
            .Returns(0x2C);

        var sut = new ColorFaker(random.Object);

        sut.Hex().Should().Be("#0A1B2C");
    }

    [Fact]
    public void CssRgba_ShouldFormatAlphaBetween0And1()
    {
        var random = new Mock<IRandomGenerator>();
        random.SetupSequence(x => x.Byte(0, 255))
            .Returns(128)
            .Returns(10)
            .Returns(20)
            .Returns(30);

        var sut = new ColorFaker(random.Object);

        var expectedAlpha = (128 / 255.0).ToString("F2", CultureInfo.CurrentCulture);
        sut.CssRgba().Should().Be($"rgba(10, 20, 30, {expectedAlpha})");
    }

    [Fact]
    public void Name_ShouldReturnNamedColor()
    {
        var random = new Mock<IRandomGenerator>();
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns("teal");

        var sut = new ColorFaker(random.Object);

        sut.Name().Should().Be("teal");
    }
}
