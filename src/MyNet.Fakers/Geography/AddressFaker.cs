// -----------------------------------------------------------------------
// <copyright file="AddressFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Localization.Providers;
using MyNet.Utilities.Generator;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Text.Randomize;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Current implementation of <see cref="IAddressFaker"/>.
/// </summary>
public sealed class AddressFaker(
    ITextRandomGenerator patternGenerator,
    IRandomGenerator random,
    ICultureScopedServiceSource<IAddressFakerProvider> source,
    IStreetFaker streetFaker,
    ICountryFaker countryFaker) : IAddressFaker
{
    /// <inheritdoc />
    public Address Address(CultureInfo? culture = null)
    {
        var street = Street(culture);
        var city = City(culture);
        var postal = PostalCode(culture);
        var country = countryFaker.Country();

        return new(
            Street: street,
            PostalCode: postal,
            City: city,
            Country: country);
    }

    /// <inheritdoc />
    public string Street(CultureInfo? culture = null) => streetFaker.Street(culture);

    /// <inheritdoc />
    public string City(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);
        return random.Item(provider.Cities);
    }

    /// <inheritdoc />
    public string PostalCode(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        return patternGenerator.RandomizeWithRandomPattern(random, provider.PostalCodeFormats);
    }

    /// <inheritdoc />
    public Coordinates Coordinates()
    {
        // Realistic global bounds
        var lat = random.Double(-90, 90);
        var lon = random.Double(-180, 180);

        return new(lat, lon);
    }
}
