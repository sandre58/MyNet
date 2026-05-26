// -----------------------------------------------------------------------
// <copyright file="PluralizationServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Moq;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Inflection.Cultures;
using MyNet.Globalization.Localization.Providers;
using Xunit;

namespace MyNet.Globalization.Tests.Inflection;

public sealed class PluralizationServiceTests
{
    [Fact]
    public void Pluralize_UsesResolverInflectorForCulture()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var sut = CreateService(culture);

        Assert.Equal("children", sut.Pluralize("child", culture));
    }

    [Fact]
    public void Singularize_WithoutCulture_UsesCultureContext()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var sut = CreateService(culture);

        Assert.Equal("child", sut.Singularize("children"));
    }

    [Fact]
    public void IsPlural_ForEnglishFollowsCountRule()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var sut = CreateService(culture);

        Assert.False(sut.IsPlural(1, culture));
        Assert.True(sut.IsPlural(2, culture));
    }

    private static PluralizationService CreateService(CultureInfo culture)
    {
        var cultureContext = new Mock<ICultureContext>();
        cultureContext.Setup(c => c.CurrentCulture).Returns(culture);

        var resolver = new Mock<ILocalizationServiceResolver>();
        resolver
            .Setup(r => r.ForCulture(It.IsAny<CultureInfo>()))
            .Returns<CultureInfo>(c => new LocalizationServiceContext(resolver.Object, c));
        resolver
            .Setup(r => r.GetRequired<IInflector>(It.IsAny<CultureInfo>()))
            .Returns(Inflectors.English);

        return new(resolver.Object, cultureContext.Object);
    }
}
