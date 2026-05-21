// -----------------------------------------------------------------------
// <copyright file="NameFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Identity;
using MyNet.Globalization.Localization.Providers;
using MyNet.Utilities;
using MyNet.Utilities.Generator;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class NameFakerTests
{
    [Fact]
    public void FirstName_ShouldUseFemaleListForFemale()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<INameFakerProvider>>();
        var provider = new NameProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());

        var sut = new NameFaker(random.Object, source.Object);

        sut.FirstName(GenderType.Female, culture).Should().Be("Alice");
    }

    [Fact]
    public void FirstName_WithoutCulture_ShouldUseCurrentResolverCulture()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<INameFakerProvider>>();
        var provider = new NameProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());

        var sut = new NameFaker(random.Object, source.Object);

        sut.FirstName(GenderType.Female).Should().Be("Alice");
    }

    [Theory]
    [InlineData(NameFormat.Standard, "John Doe")]
    [InlineData(NameFormat.Inverse, "Doe John")]
    [InlineData(NameFormat.WithPrefix, "Dr John Doe")]
    [InlineData(NameFormat.InverseWithPrefix, "Dr Doe John")]
    [InlineData(NameFormat.WithSuffix, "John Doe Jr")]
    [InlineData(NameFormat.InverseWithSuffix, "Doe John Jr")]
    public void FullName_ShouldRespectRequestedFormat(NameFormat format, string expected)
    {
        var culture = CultureInfo.GetCultureInfo("en");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<INameFakerProvider>>();
        var provider = new NameProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());

        var sut = new NameFaker(random.Object, source.Object);

        sut.FullName(GenderType.Male, format, culture).Should().Be(expected);
    }

    [Fact]
    public void FullName_WithUnknownFormat_ShouldFallbackToStandard()
    {
        var culture = CultureInfo.GetCultureInfo("en");
        var random = new Mock<IRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<INameFakerProvider>>();
        var provider = new NameProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns((IReadOnlyCollection<string> values) => values.First());

        var sut = new NameFaker(random.Object, source.Object);

        sut.FullName(GenderType.Male, (NameFormat)999, culture).Should().Be("John Doe");
    }
}

internal sealed class NameProvider(CultureInfo culture) : INameFakerProvider
{
    public CultureInfo Culture { get; } = culture;

    public IReadOnlyList<string> MaleFirstNames { get; } = ["John"];

    public IReadOnlyList<string> FemaleFirstNames { get; } = ["Alice"];

    public IReadOnlyList<string> LastNames { get; } = ["Doe"];

    public IReadOnlyList<string> Prefixes { get; } = ["Dr"];

    public IReadOnlyList<string> Suffixes { get; } = ["Jr"];
}
