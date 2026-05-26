// -----------------------------------------------------------------------
// <copyright file="DomainFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Internet;
using MyNet.Globalization.Localization.Providers;
using MyNet.Generator;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class DomainFakerTests
{
    [Fact]
    public void Domain_ShouldSelectFromProviderDomains()
    {
        var culture = CultureInfo.GetCultureInfo("en");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IDomainFakerProvider>>();
        var provider = new DomainProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());

        var sut = new DomainFaker(random.Object, source.Object);

        sut.Domain(culture).Should().Be("example");
    }

    [Fact]
    public void Host_ShouldComposeHostAndDomain()
    {
        var culture = CultureInfo.GetCultureInfo("en");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IDomainFakerProvider>>();
        var provider = new DomainProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());

        var sut = new DomainFaker(random.Object, source.Object);

        sut.Host(culture).Should().Be("www.example");
    }

    [Fact]
    public void Domain_WithoutCulture_ShouldUseCurrentResolverCulture()
    {
        var culture = CultureInfo.GetCultureInfo("en");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IDomainFakerProvider>>();
        var provider = new DomainProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());

        var sut = new DomainFaker(random.Object, source.Object);

        sut.Domain().Should().Be("example");
    }
}

internal sealed class DomainProvider(CultureInfo culture) : IDomainFakerProvider
{
    public CultureInfo Culture { get; } = culture;

    public IReadOnlyList<string> Domains { get; } = ["example"];

    public IReadOnlyList<string> Hosts { get; } = ["www"];
}
