// -----------------------------------------------------------------------
// <copyright file="AddressFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Geography;
using MyNet.Globalization.Localization.Providers;
using MyNet.Generator;
using MyNet.Utilities.Geography;
using MyNet.Text.Randomize;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class AddressFakerTests
{
    [Fact]
    public void Address_ShouldComposeAllParts()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var patternGenerator = new Mock<ITextRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IAddressFakerProvider>>();
        var street = new Mock<IStreetFaker>();
        var country = new Mock<ICountryFaker>();
        var provider = new AddressProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());
        patternGenerator.Setup(x => x.Randomize(It.IsAny<string>())).Returns("75001");
        street.Setup(x => x.Street(culture)).Returns("10 Rue de la Paix");
        country.Setup(x => x.Country()).Returns(Country.France);

        var sut = new AddressFaker(patternGenerator.Object, random.Object, source.Object, street.Object, country.Object);

        var result = sut.Address(culture);

        result.Street.Should().Be("10 Rue de la Paix");
        result.City.Should().Be("Paris");
        result.PostalCode.Should().Be("75001");
        result.Country.Should().NotBeNull();
        result.Country.Alpha2.Should().Be("fr");
    }

    [Fact]
    public void Address_WithoutCulture_ShouldUseCurrentResolverCulture()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var patternGenerator = new Mock<ITextRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IAddressFakerProvider>>();
        var street = new Mock<IStreetFaker>();
        var country = new Mock<ICountryFaker>();
        var provider = new AddressProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());
        patternGenerator.Setup(x => x.Randomize(It.IsAny<string>())).Returns("75001");
        street.Setup(x => x.Street(null)).Returns("10 Rue de la Paix");
        country.Setup(x => x.Country()).Returns(Country.France);

        var sut = new AddressFaker(patternGenerator.Object, random.Object, source.Object, street.Object, country.Object);

        var result = sut.Address();

        result.Street.Should().Be("10 Rue de la Paix");
        result.City.Should().Be("Paris");
        result.PostalCode.Should().Be("75001");
    }

    [Fact]
    public void Coordinates_ShouldUseRandomBounds()
    {
        var random = new Mock<IRandomGenerator>();
        var patternGenerator = new Mock<ITextRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IAddressFakerProvider>>();
        var street = new Mock<IStreetFaker>();
        var country = new Mock<ICountryFaker>();

        random.Setup(x => x.Double(-90, 90)).Returns(48.8566);
        random.Setup(x => x.Double(-180, 180)).Returns(2.3522);

        var sut = new AddressFaker(patternGenerator.Object, random.Object, source.Object, street.Object, country.Object);

        var result = sut.Coordinates();

        result.Latitude.Should().Be(48.8566);
        result.Longitude.Should().Be(2.3522);
    }
}

internal sealed class AddressProvider(CultureInfo culture) : IAddressFakerProvider
{
    public CultureInfo Culture { get; } = culture;

    public IReadOnlyList<string> StreetTypes { get; init; } = ["Rue", "Avenue"];

    public IReadOnlyList<string> StreetNames { get; init; } = ["Victor Hugo", "de la Paix"];

    public IReadOnlyList<string> StreetFormats { get; init; } = ["{Number} {Type} {Name}"];

    public IReadOnlyList<string> StreetSuffixes { get; init; } = [string.Empty, "B"];

    public IReadOnlyList<string> Cities { get; init; } = ["Paris", "Lyon"];

    public IReadOnlyList<string> PostalCodeFormats { get; init; } = ["#####"];
}
