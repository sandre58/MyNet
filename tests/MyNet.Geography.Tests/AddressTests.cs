// -----------------------------------------------------------------------
// <copyright file="AddressTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Geography.Tests;

public sealed class AddressTests
{
    [Fact]
    public void ToString_WithAllParts_JoinsNonEmptyComponents()
    {
        var address = new Address("10 rue de Rivoli", "75001", "Paris", Country.France);

        Assert.Equal("10 rue de Rivoli 75001 Paris France", address.ToString());
    }

    [Fact]
    public void ToString_WithMissingParts_OmitsEmptyValues()
    {
        var address = new Address(City: "Paris", Country: Country.France);

        Assert.Equal("Paris France", address.ToString());
    }

    [Fact]
    public void ToString_WithNoParts_ReturnsEmptyString()
    {
        var address = new Address();

        Assert.Equal(string.Empty, address.ToString());
    }

    [Fact]
    public void France_TryFromValue_WithValueProperty_Succeeds()
    {
        Assert.True(Country.TryFromValue(Country.France.Value, out var country));
        Assert.Equal(Country.France, country);
    }

    [Fact]
    public void Record_WithCoordinates_PreservesCoordinates()
    {
        var coordinates = new Coordinates(48.8566, 2.3522);
        var address = new Address(Street: "Test", Coordinates: coordinates);

        Assert.Equal(coordinates, address.Coordinates);
    }
}
