// -----------------------------------------------------------------------
// <copyright file="AddressFormatterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Geography;
using MyNet.Humanizer.Formatting.Addresses;
using MyNet.Humanizer.Formatting.Addresses.Cultures;
using Xunit;

namespace MyNet.Humanizer.Tests.Formatting;

public class AddressFormatterTests
{
    [Fact]
    public void AddressFormatter_Format_WithNullAddress_ThrowsArgumentNullException()
    {
        var formatter = new AddressFormatter();

        Assert.Throws<ArgumentNullException>(() => formatter.Format(null!));
    }

    [Fact]
    public void AddressFormatter_Format_WithInvariantFormatter_FormatsAddressUsingDefaultTemplate()
    {
        var formatter = new AddressFormatter();
        var address = new Address("221B Baker Street", "NW1", "London", Country.UnitedKingdomOfGreatBritainAndNorthernIreland);

        var result = formatter.Format(address);

        var expected = string.Join(
            Environment.NewLine,
            "221B Baker Street",
            "NW1 London",
            "UnitedKingdomOfGreatBritainAndNorthernIreland");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EnglishAddressFormatter_Format_FormatsAddressUsingEnglishTemplate()
    {
        var formatter = new EnglishAddressFormatter();
        var address = new Address("10 Downing St", "SW1A 2AA", "London", Country.UnitedKingdomOfGreatBritainAndNorthernIreland);

        var result = formatter.Format(address);

        var expected = string.Join(
            Environment.NewLine,
            "10 Downing St",
            "London",
            "SW1A 2AA",
            "UnitedKingdomOfGreatBritainAndNorthernIreland");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FrenchAddressFormatter_Format_FormatsAddressUsingFrenchTemplate()
    {
        var formatter = new FrenchAddressFormatter();
        var address = new Address("1 Rue de Rivoli", "75001", "Paris", Country.France);

        var result = formatter.Format(address);

        var expected = string.Join(
            Environment.NewLine,
            "1 Rue de Rivoli",
            "75001 Paris",
            "France");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddressFormatter_Format_WithMissingValues_RemovesEmptyLinesAndNormalizesWhitespace()
    {
        var formatter = new AddressFormatter();
        var address = new Address("  5 Avenue Anatole   France ", null, "   Paris   ");

        var result = formatter.Format(address);

        var expected = string.Join(
            Environment.NewLine,
            "5 Avenue Anatole France",
            "Paris");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddressFormatterBase_Format_WithUnknownToken_RemovesUnknownTokenAndExtraWhitespace()
    {
        var formatter = new TestAddressFormatter(new("en-US"));
        var address = new Address("Main street", "90210", "Beverly Hills", Country.UnitedStatesOfAmerica);

        var result = formatter.Format(address);

        var expected = string.Join(
            Environment.NewLine,
            "Main street",
            "90210 Beverly Hills",
            "UnitedStatesOfAmerica");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddressFormatters_StaticProperties_ExposeFormattersWithExpectedCultures()
    {
        Assert.Equal(CultureInfo.InvariantCulture, AddressFormatters.Invariant.Culture);
        Assert.Equal("en", AddressFormatters.English.Culture.TwoLetterISOLanguageName);
        Assert.Equal("fr", AddressFormatters.French.Culture.TwoLetterISOLanguageName);
    }

    private sealed class TestAddressFormatter(CultureInfo culture) : AddressFormatterBase(culture)
    {
        protected override string Template { get; } = "{Street}\n{PostalCode} {City} {UnknownToken}\n{Country}";
    }
}
