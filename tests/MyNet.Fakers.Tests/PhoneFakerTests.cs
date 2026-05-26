// -----------------------------------------------------------------------
// <copyright file="PhoneFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Contacts;
using MyNet.Globalization.Localization.Providers;
using MyNet.Generator;
using MyNet.Text.Randomize;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class PhoneFakerTests
{
    [Fact]
    public void Number_ShouldUseProviderNumberFormats()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var patternGenerator = new Mock<ITextRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IPhoneFakerProvider>>();
        var provider = new PhoneProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns("## ## ## ## ##");
        patternGenerator.Setup(x => x.Randomize("## ## ## ## ##")).Returns("01 23 45 67 89");

        var sut = new PhoneFaker(patternGenerator.Object, random.Object, source.Object);

        sut.Number(culture).Should().Be("01 23 45 67 89");
    }

    [Fact]
    public void Number_WithoutCulture_ShouldUseCurrentResolverCulture()
    {
        var culture = CultureInfo.GetCultureInfo("fr");
        var random = new Mock<IRandomGenerator>();
        var patternGenerator = new Mock<ITextRandomGenerator>();
        var source = new Mock<ICultureScopedServiceSource<IPhoneFakerProvider>>();
        var provider = new PhoneProvider(culture);

        source.Setup(x => x.Get()).Returns(provider);
        source.Setup(x => x.Get(It.IsAny<CultureInfo>())).Returns(provider);
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns("## ## ## ## ##");
        patternGenerator.Setup(x => x.Randomize("## ## ## ## ##")).Returns("01 23 45 67 89");

        var sut = new PhoneFaker(patternGenerator.Object, random.Object, source.Object);

        sut.Number().Should().Be("01 23 45 67 89");
    }
}

internal sealed class PhoneProvider(CultureInfo culture) : IPhoneFakerProvider
{
    public CultureInfo Culture { get; } = culture;

    public IReadOnlyList<string> NumberFormats { get; } = ["## ## ## ## ##"];

    public IReadOnlyList<string> MobileNumberFormats { get; } = ["06 ## ## ## ##"];

    public IReadOnlyList<string> InternationalNumberFormats { get; } = ["+33 # ## ## ## ##"];
}
