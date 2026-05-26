// -----------------------------------------------------------------------
// <copyright file="StreetFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Geography;
using MyNet.Generator;
using MyNet.Globalization.Localization.Providers;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class StreetFakerTests
{
    [Fact]
    public void Number_ShouldAppendSuffix_WhenSuffixIsNotEmpty()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IAddressFakerProvider>>();
        var provider = new AddressProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Int(1, 200)).Returns(12);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.Last());

        var sut = new StreetFaker(random.Object, source.Object);

        sut.Number(culture).Should().Be("12B");
    }

    [Fact]
    public void Street_ShouldApplyTemplateAndTrimExtraSpaces()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IAddressFakerProvider>>();
        var provider = new AddressProvider(culture) { StreetFormats = ["{Number} {Suffix} {Type} {Name}"] };

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Int(1, 200)).Returns(7);
        random.SetupSequence(x => x.Item(It.IsAny<IReadOnlyCollection<string>>()))
            .Returns(string.Empty)
            .Returns("Rue")
            .Returns("Victor Hugo")
            .Returns("{Number} {Suffix} {Type} {Name}");

        var sut = new StreetFaker(random.Object, source.Object);

        sut.Street(culture).Should().Be("7 Rue Victor Hugo");
    }

    [Fact]
    public void Street_WithoutCulture_ShouldUseCurrentResolverCulture()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IAddressFakerProvider>>();
        var provider = new AddressProvider(culture) { StreetFormats = ["{Number} {Type} {Name}"] };

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Int(1, 200)).Returns(7);
        random.SetupSequence(x => x.Item(It.IsAny<IReadOnlyCollection<string>>()))
            .Returns(string.Empty)
            .Returns("Rue")
            .Returns("Victor Hugo")
            .Returns("{Number} {Type} {Name}");

        var sut = new StreetFaker(random.Object, source.Object);

        sut.Street().Should().Be("7 Rue Victor Hugo");
    }
}
