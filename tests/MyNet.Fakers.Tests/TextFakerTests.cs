// -----------------------------------------------------------------------
// <copyright file="TextFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Text;
using MyNet.Generator;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class TextFakerTests
{
    [Fact]
    public void Word_WithLength_ShouldTruncateWord()
    {
        var random = new Mock<IRandomGenerator>();
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns("consectetur");

        var sut = new TextFaker(random.Object);

        sut.Word(4).Should().Be("cons");
    }

    [Fact]
    public void Sentence_ShouldStartWithUppercase_AndEndWithPeriod()
    {
        var random = new Mock<IRandomGenerator>();
        random.Setup(x => x.Int(4, 12)).Returns(4);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns("lorem");

        var sut = new TextFaker(random.Object);

        var result = sut.Sentence();

        result.Should().StartWith("L");
        result.Should().EndWith(".");
    }

    [Fact]
    public void Paragraph_ShouldContainRequestedSentenceCount()
    {
        var random = new Mock<IRandomGenerator>();
        random.SetupSequence(x => x.Int(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(2)
            .Returns(4)
            .Returns(4);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns("ipsum");

        var sut = new TextFaker(random.Object);

        var result = sut.Paragraph();

        result.Count(x => x == '.').Should().Be(2);
    }

    [Fact]
    public void Lorem_WithWordCount_ShouldLimitOutputWords()
    {
        var random = new Mock<IRandomGenerator>();
        var sut = new TextFaker(random.Object);

        sut.Lorem(5).Split(' ', StringSplitOptions.RemoveEmptyEntries).Length.Should().Be(5);
    }
}
