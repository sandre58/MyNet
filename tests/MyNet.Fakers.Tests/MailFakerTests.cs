// -----------------------------------------------------------------------
// <copyright file="MailFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Contacts;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using MyNet.Utilities.Generator;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class MailFakerTests
{
    [Fact]
    public void Email_WithExplicitDomain_ShouldUseUsernameAndProvidedDomain()
    {
        var random = new Mock<IRandomGenerator>();
        var identity = new Mock<IIdentityFaker>();
        var domain = new Mock<IDomainFaker>();
        identity.Setup(x => x.Username()).Returns("john123");

        var sut = new MailFaker(random.Object, identity.Object, domain.Object);

        var result = sut.Email("contoso.com");

        result.Should().Be("john123@contoso.com");
    }

    [Fact]
    public void Email_WithoutDomain_ShouldBuildDefaultExampleDomain()
    {
        var random = new Mock<IRandomGenerator>();
        var identity = new Mock<IIdentityFaker>();
        var domain = new Mock<IDomainFaker>();
        identity.Setup(x => x.Username()).Returns("john123");
        random.Setup(x => x.Int(1, 999)).Returns(42);

        var sut = new MailFaker(random.Object, identity.Object, domain.Object);

        var result = sut.Email();

        result.Should().Be("john123@example42.com");
    }

    [Fact]
    public void Email_WithCulture_ShouldUseDomainFakerParts()
    {
        var culture = CultureInfo.GetCultureInfo("en");
        var random = new Mock<IRandomGenerator>();
        var identity = new Mock<IIdentityFaker>();
        var domain = new Mock<IDomainFaker>();

        identity.Setup(x => x.Username()).Returns("user42");
        domain.Setup(x => x.Host(culture)).Returns("mail");
        domain.Setup(x => x.Domain(culture)).Returns("example");

        var sut = new MailFaker(random.Object, identity.Object, domain.Object);

        var result = sut.Email(culture: culture);

        result.Should().Be("user42@mail.example");
    }

    [Fact]
    public void Email_WithoutCulture_ShouldUseDomainFakerDefaultPath()
    {
        var random = new Mock<IRandomGenerator>();
        var identity = new Mock<IIdentityFaker>();
        var domain = new Mock<IDomainFaker>();

        identity.Setup(x => x.Username()).Returns("user42");
        domain.Setup(x => x.Host(null)).Returns("mail.example");

        var sut = new MailFaker(random.Object, identity.Object, domain.Object);

        var result = sut.Email();

        result.Should().Be("user42@mail.example");
    }
}
