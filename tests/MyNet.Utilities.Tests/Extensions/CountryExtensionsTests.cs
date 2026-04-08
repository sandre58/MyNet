// -----------------------------------------------------------------------
// <copyright file="CountryExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Geography.Extensions;
using MyNet.Utilities.Localization;
using Xunit;

namespace MyNet.Utilities.Tests.Extensions;

[Collection("UseCultureSequential")]
public class CountryExtensionsTests
{
    #region GetFlag

    [Theory]
    [InlineData(FlagSize.Pixel16)]
    [InlineData(FlagSize.Pixel24)]
    [InlineData(FlagSize.Pixel32)]
    [InlineData(FlagSize.Pixel48)]
    [InlineData(FlagSize.Pixel64)]
    [InlineData(FlagSize.Pixel128)]
    public void GetFlag_AllSizes_ReturnsNonEmptyBytes(FlagSize size)
    {
        var flag = Country.France.GetFlag(size);

        Assert.NotNull(flag);
        Assert.NotEmpty(flag);
    }

    [Fact]
    public void GetFlag_DefaultSize_IsPixel32()
    {
        var flagDefault = Country.France.GetFlag();
        var flagExplicit = Country.France.GetFlag(FlagSize.Pixel32);

        Assert.Equal(flagDefault, flagExplicit);
    }

    [Fact]
    public void GetFlag_LargerSize_ReturnsBiggerImage()
    {
        var flag16 = Country.France.GetFlag(FlagSize.Pixel16)!;
        var flag128 = Country.France.GetFlag(FlagSize.Pixel128)!;

        Assert.True(flag128.Length > flag16.Length);
    }

    [Fact]
    public void GetFlag_DifferentCountries_ReturnDifferentImages()
    {
        var flagFrance = Country.France.GetFlag(FlagSize.Pixel32);
        var flagGermany = Country.Germany.GetFlag(FlagSize.Pixel32);
        var flagJapan = Country.Japan.GetFlag(FlagSize.Pixel32);

        Assert.NotEqual(flagFrance, flagGermany);
        Assert.NotEqual(flagFrance, flagJapan);
        Assert.NotEqual(flagGermany, flagJapan);
    }

    [Fact]
    public void GetFlag_SameCountrySameSize_ReturnsSameImageEachCall()
    {
        var flag1 = Country.France.GetFlag(FlagSize.Pixel32);
        var flag2 = Country.France.GetFlag(FlagSize.Pixel32);

        Assert.Equal(flag1, flag2);
    }

    [Theory]
    [InlineData("fr")]
    [InlineData("de")]
    [InlineData("us")]
    [InlineData("jp")]
    [InlineData("gb")]
    [InlineData("br")]
    [InlineData("cn")]
    [InlineData("in")]
    public void GetFlag_VariousCountries_ReturnsNonEmptyBytes(string alpha2)
    {
        var country = EnumClass.GetAll<Country>().Single(c => c.Alpha2 == alpha2);
        var flag = country.GetFlag(FlagSize.Pixel24);

        Assert.NotNull(flag);
        Assert.NotEmpty(flag);
    }

    [Fact]
    public void GetFlag_PngSignature_IsValid()
    {
        // PNG files start with the 8-byte signature: 137 80 78 71 13 10 26 10
        var flag = Country.France.GetFlag(FlagSize.Pixel32)!;

        Assert.Equal(0x89, flag[0]);
        Assert.Equal(0x50, flag[1]); // 'P'
        Assert.Equal(0x4E, flag[2]); // 'N'
        Assert.Equal(0x47, flag[3]); // 'G'
    }

    #endregion

    #region GetDisplayName

    [Fact]
    public void GetDisplayName_French_Germany_ReturnsAllemagne()
    {
        GlobalizationService.Current.SetCulture("fr-FR");

        Assert.Equal("Allemagne", Country.Germany.GetDisplayName());
    }

    [Fact]
    public void GetDisplayName_English_Germany_ReturnsGermany()
    {
        GlobalizationService.Current.SetCulture("en-US");

        Assert.Equal("Germany", Country.Germany.GetDisplayName());
    }

    [Fact]
    public void GetDisplayName_French_Japan_ReturnsJapon()
    {
        GlobalizationService.Current.SetCulture("fr-FR");

        Assert.Equal("Japon", Country.Japan.GetDisplayName());
    }

    [Fact]
    public void GetDisplayName_English_Japan_ReturnsJapan()
    {
        GlobalizationService.Current.SetCulture("en-US");

        Assert.Equal("Japan", Country.Japan.GetDisplayName());
    }

    [Fact]
    public void GetDisplayName_French_France_ReturnsFrance()
    {
        GlobalizationService.Current.SetCulture("fr-FR");

        Assert.Equal("France", Country.France.GetDisplayName());
    }

    [Fact]
    public void GetDisplayName_ChangingCulture_ReturnsUpdatedName()
    {
        GlobalizationService.Current.SetCulture("fr-FR");
        var nameFr = Country.Germany.GetDisplayName();

        GlobalizationService.Current.SetCulture("en-US");
        var nameEn = Country.Germany.GetDisplayName();

        Assert.Equal("Allemagne", nameFr);
        Assert.Equal("Germany", nameEn);
        Assert.NotEqual(nameFr, nameEn);
    }

    #endregion
}
