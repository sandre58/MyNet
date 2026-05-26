// -----------------------------------------------------------------------
// <copyright file="IdentityFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Moq;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Text;
using MyNet.Generator;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class IdentityFakerTests
{
    [Fact]
    public void Username_ShouldAppendRandomNumberToWord()
    {
        var random = new Mock<IRandomGenerator>();
        var text = new Mock<ITextFaker>();
        text.Setup(x => x.Word(null)).Returns("john");
        random.Setup(x => x.Int(100, 9999)).Returns(123);

        var sut = new IdentityFaker(random.Object, text.Object);

        sut.Username().Should().Be("john123");
    }

    [Fact]
    public void Password_ShouldGenerateRequestedLength()
    {
        var random = new Mock<IRandomGenerator>();
        var text = new Mock<ITextFaker>();
        random.Setup(x => x.Int(33, 127)).Returns('A');

        var sut = new IdentityFaker(random.Object, text.Object);

        sut.Password(5).Should().Be("AAAAA");
    }
}
