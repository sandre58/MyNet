// -----------------------------------------------------------------------
// <copyright file="LocalizationExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Localization;
using MyNet.Utilities.Tests.Data;
using Xunit;

namespace MyNet.Utilities.Tests.Extensions;

[Collection("UseCultureSequential")]
public class LocalizationExtensionsTests
{
    public LocalizationExtensionsTests() => TranslationService.RegisterResources(nameof(DataResources), DataResources.ResourceManager);

    #region GetCountry

    [Fact]
    public void GetCountry_NullCulture_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => ((CultureInfo)null!).GetCountry());

    [Fact]
    public void GetCountry_InvariantCulture_ReturnsNull()
        => Assert.Null(CultureInfo.InvariantCulture.GetCountry());

    [Theory]
    [InlineData("fr-FR", "fr")]
    [InlineData("en-US", "us")]
    [InlineData("de-DE", "de")]
    [InlineData("ja-JP", "jp")]
    [InlineData("es-ES", "es")]
    [InlineData("pt-BR", "br")]
    [InlineData("zh-CN", "cn")]
    [InlineData("ar-SA", "sa")]
    [InlineData("it-IT", "it")]
    [InlineData("nl-NL", "nl")]
    public void GetCountry_SpecificCulture_ReturnsMatchingCountry(string cultureName, string expectedAlpha2)
    {
        var culture = new CultureInfo(cultureName);
        var country = culture.GetCountry();

        Assert.NotNull(country);
        Assert.Equal(expectedAlpha2, country.Alpha2, StringComparer.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("fr", "fr")]
    [InlineData("de", "de")]
    [InlineData("ja", "jp")]
    [InlineData("it", "it")]
    public void GetCountry_NeutralCulture_ReturnsMatchingCountry(string cultureName, string expectedAlpha2)
    {
        var culture = new CultureInfo(cultureName);
        var country = culture.GetCountry();

        Assert.NotNull(country);
        Assert.Equal(expectedAlpha2, country.Alpha2, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetCountry_FrFR_ReturnsFrance()
        => Assert.Equal(Country.France, new CultureInfo("fr-FR").GetCountry());

    [Fact]
    public void GetCountry_EnUS_ReturnsUnitedStatesOfAmerica()
        => Assert.Equal(Country.UnitedStatesOfAmerica, new CultureInfo("en-US").GetCountry());

    [Fact]
    public void GetCountry_DeDe_ReturnsGermany()
        => Assert.Equal(Country.Germany, new CultureInfo("de-DE").GetCountry());

    [Fact]
    public void GetCountry_JaJP_ReturnsJapan()
        => Assert.Equal(Country.Japan, new CultureInfo("ja-JP").GetCountry());

    #endregion

    #region ToAbbreviationKey

    [Theory]
    [InlineData("Value1", "Value1Abbr")]
    [InlineData("MyKey", "MyKeyAbbr")]
    [InlineData("", "Abbr")]
    public void ToAbbreviationKey_ReturnsKeyWithAbbrSuffix(string key, string expected)
        => Assert.Equal(expected, key.ToAbbreviationKey());

    #endregion

    #region Translate

    [Fact]
    public void Translate_Key_WithCurrentCulture_ReturnsCultureValue()
    {
        GlobalizationService.Current.SetCulture("fr-FR");

        Assert.Equal("Valeur Une", nameof(DataResources.Value1).Translate());
    }

    [Fact]
    public void Translate_Key_WithExplicitCulture_ReturnsCultureValue()
        => Assert.Equal("Value One", nameof(DataResources.Value1).Translate(new CultureInfo("en-US")));

    [Fact]
    public void Translate_Key_WithExplicitSpanishCulture_ReturnsCultureValue()
        => Assert.Equal("Valor Uno", nameof(DataResources.Value1).Translate(new CultureInfo("es-ES")));

    [Fact]
    public void Translate_UnknownKey_ReturnsKeyItself()
        => Assert.Equal("NonExistentKey", "NonExistentKey".Translate(new CultureInfo("fr-FR")));

    [Fact]
    public void Translate_Key_WithFilename_WithExplicitCulture_ReturnsCultureValue()
        => Assert.Equal("Value One", nameof(DataResources.Value1).Translate(nameof(DataResources), new CultureInfo("en-US")));

    [Fact]
    public void Translate_Key_WithFilename_WithUnknownFilename_ReturnsKeyItself()
        => Assert.Equal("Value1", nameof(DataResources.Value1).Translate("UnknownFile", new CultureInfo("fr-FR")));

    [Fact]
    public void Translate_CultureInfo_Key_ReturnsCultureValue()
        => Assert.Equal("Value One", new CultureInfo("en-US").Translate(nameof(DataResources.Value1)));

    [Fact]
    public void Translate_CultureInfo_Key_WithFilename_ReturnsCultureValue()
        => Assert.Equal("Value One", new CultureInfo("en-US").Translate(nameof(DataResources.Value1), nameof(DataResources)));

    #endregion

    #region TranslateAbbreviated

    [Fact]
    public void TranslateAbbreviated_UnknownAbbrKey_ReturnsAbbrKeyItself()
        => Assert.Equal("Value1Abbr", nameof(DataResources.Value1).TranslateAbbreviated(new CultureInfo("fr-FR")));

    [Fact]
    public void TranslateAbbreviated_WithFilename_UnknownAbbrKey_ReturnsAbbrKeyItself()
        => Assert.Equal("Value1Abbr", nameof(DataResources.Value1).TranslateAbbreviated(nameof(DataResources), new CultureInfo("fr-FR")));

    [Fact]
    public void TranslateAbbreviated_CultureInfo_UnknownAbbrKey_ReturnsAbbrKeyItself()
        => Assert.Equal("Value1Abbr", new CultureInfo("fr-FR").TranslateAbbreviated(nameof(DataResources.Value1)));

    [Fact]
    public void TranslateAbbreviated_CultureInfo_WithFilename_UnknownAbbrKey_ReturnsAbbrKeyItself()
        => Assert.Equal("Value1Abbr", new CultureInfo("fr-FR").TranslateAbbreviated(nameof(DataResources.Value1), nameof(DataResources)));

    #endregion

    #region GetProvider

    [Fact]
    public void GetProvider_NoProviderRegistered_ReturnsNull()
        => Assert.Null(new CultureInfo("fr-FR").GetProvider<ILocalizationTestProvider>());

    [Fact]
    public void GetProvider_ProviderRegistered_ReturnsProvider()
    {
        var culture = new CultureInfo("fr-FR");
        var provider = new LocalizationTestProvider();
        LocalizationService.Register<ILocalizationTestProvider, LocalizationTestProvider>(culture, provider);

        Assert.NotNull(culture.GetProvider<ILocalizationTestProvider>());
    }

    #endregion

    private interface ILocalizationTestProvider;

    private sealed class LocalizationTestProvider : ILocalizationTestProvider;
}
