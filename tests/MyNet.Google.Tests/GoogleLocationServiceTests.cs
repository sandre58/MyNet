// -----------------------------------------------------------------------
// <copyright file="GoogleLocationServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Xml;
using System.Xml.Linq;
using MyNet.Geography;
using MyNet.Google.Maps;
using Xunit;

namespace MyNet.Google.Tests;

public sealed class GoogleLocationServiceTests
{
    private const string RegionGeocodeXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <GeocodeResponse>
          <status>OK</status>
          <result>
            <address_component>
              <long_name>Île-de-France</long_name>
              <short_name>IDF</short_name>
              <type>administrative_area_level_1</type>
            </address_component>
          </result>
        </GeocodeResponse>
        """;

    private const string AddressGeocodeXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <GeocodeResponse>
          <status>OK</status>
          <result>
            <address_component>
            <long_name>France</long_name>
            <short_name>FR</short_name>
            <type>country</type>
            </address_component>
            <address_component>
              <long_name>Paris</long_name>
              <short_name>Paris</short_name>
              <type>locality</type>
            </address_component>
            <address_component>
              <long_name>75001</long_name>
              <short_name>75001</short_name>
              <type>postal_code</type>
            </address_component>
            <address_component>
              <long_name>Rue de Rivoli</long_name>
              <short_name>Rue de Rivoli</short_name>
              <type>route</type>
            </address_component>
            <address_component>
              <long_name>10</long_name>
              <short_name>10</short_name>
              <type>street_number</type>
            </address_component>
          </result>
        </GeocodeResponse>
        """;

    private const string CoordinatesGeocodeXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <GeocodeResponse>
          <status>OK</status>
          <result>
            <geometry>
              <location>
                <lat>48.856614</lat>
                <lng>2.3522219</lng>
              </location>
            </geometry>
          </result>
        </GeocodeResponse>
        """;

    private const string AddressListGeocodeXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <GeocodeResponse>
          <status>OK</status>
          <result>
            <formatted_address>10 Rue de Rivoli, Paris, France</formatted_address>
          </result>
          <result>
            <formatted_address>11 Rue de Rivoli, Paris, France</formatted_address>
          </result>
        </GeocodeResponse>
        """;

    private const string DirectionsOkXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <DirectionsResponse>
          <status>OK</status>
          <route>
            <leg>
              <distance><text>5 km</text></distance>
              <duration><text>10 mins</text></duration>
              <step>
                <html_instructions>Head north</html_instructions>
                <distance><text>1 km</text></distance>
              </step>
            </leg>
          </route>
        </DirectionsResponse>
        """;

    [Fact]
    public void GetRegionFromCoordinates_ParsesAdministrativeArea()
    {
        var sut = CreateService(xDocumentXml: RegionGeocodeXml);

        var region = sut.GetRegionFromCoordinates(48.8566, 2.3522);

        Assert.NotNull(region);
        Assert.Equal("Île-de-France", region.Name);
        Assert.Equal("IDF", region.ShortCode);
    }

    [Fact]
    public void GetAddressFromCoordinates_ParsesAddressComponents()
    {
        var sut = CreateService(xmlDocumentXml: AddressGeocodeXml);

        var address = sut.GetAddressFromCoordinates(48.8566, 2.3522);

        Assert.NotNull(address);
        Assert.Equal("Paris", address.City);
        Assert.Equal("75001", address.PostalCode);
        Assert.Equal("10 Rue de Rivoli ", address.Street);
        Assert.Equal(Country.France, address.Country);
        Assert.Equal(new(48.8566, 2.3522), address.Coordinates);
    }

    [Fact]
    public void GetAddressFromCoordinates_WhenZeroResults_ReturnsNull()
    {
        const string zeroResultsXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <GeocodeResponse>
              <status>ZERO_RESULTS</status>
            </GeocodeResponse>
            """;
        var sut = CreateService(xmlDocumentXml: zeroResultsXml);

        var address = sut.GetAddressFromCoordinates(0, 0);

        Assert.Null(address);
    }

    [Fact]
    public void GetCoordinatesFromAddress_ParsesLatLng()
    {
        var sut = CreateService(xDocumentXml: CoordinatesGeocodeXml);

        var coordinates = sut.GetCoordinatesFromAddress("Paris, France");

        Assert.NotNull(coordinates);
        Assert.Equal(48.856614, coordinates.Latitude, precision: 5);
        Assert.Equal(2.3522219, coordinates.Longitude, precision: 5);
    }

    [Fact]
    public void GetCoordinatesFromAddress_WhenOverQueryLimit_Throws()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <GeocodeResponse>
              <status>OVER_QUERY_LIMIT</status>
            </GeocodeResponse>
            """;
        var sut = CreateService(xDocumentXml: xml);

        Assert.Throws<QueryLimitExceededException>(() => sut.GetCoordinatesFromAddress("Paris"));
    }

    [Fact]
    public void GetCoordinatesFromAddress_WhenRequestDenied_Throws()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <GeocodeResponse>
              <status>REQUEST_DENIED</status>
            </GeocodeResponse>
            """;
        var sut = CreateService(xDocumentXml: xml);

        Assert.Throws<RequestDeniedException>(() => sut.GetCoordinatesFromAddress("Paris"));
    }

    [Fact]
    public void GetAddressesListFromAddress_ReturnsFormattedAddresses()
    {
        var sut = CreateService(xDocumentXml: AddressListGeocodeXml);

        var addresses = sut.GetAddressesListFromAddress("Rue de Rivoli, Paris");

        Assert.Equal(2, addresses.Length);
        Assert.Contains("10 Rue de Rivoli, Paris, France", addresses);
        Assert.Contains("11 Rue de Rivoli, Paris, France", addresses);
    }

    [Fact]
    public void GetDirections_WhenOk_ReturnsDistanceDurationAndSteps()
    {
        var sut = CreateService(xDocumentXml: DirectionsOkXml);
        var from = new Address("Paris");
        var to = new Address("Lyon");

        var directions = sut.GetDirections(from, to);

        Assert.Equal(Directions.Status.Ok, directions.StatusCode);
        Assert.Equal("1 km", directions.Distance);
        Assert.Equal("10 mins", directions.Duration);
        Assert.Single(directions.Steps);
        Assert.Equal("Head north", directions.Steps[0].Instruction);
    }

    private static GoogleLocationService CreateService(string? xDocumentXml = null, string? xmlDocumentXml = null)
    {
        Func<string, XDocument>? loadX = xDocumentXml is null
            ? null
            : _ => XDocument.Parse(xDocumentXml);
        Func<string, XmlDocument>? loadXml = xmlDocumentXml is null
            ? null
            : _ =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlDocumentXml);
                return doc;
            };

        return new("test-key", true, loadX, loadXml);
    }
}
