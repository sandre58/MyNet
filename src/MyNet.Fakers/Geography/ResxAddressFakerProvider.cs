// -----------------------------------------------------------------------
// <copyright file="ResxAddressFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Fakers.Localization;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Provides fake address datasets from localized RESX resources.
/// </summary>
public sealed class ResxAddressFakerProvider : ResxProviderBase, IAddressFakerProvider
{
    /// <summary>
    /// Creates a new instance of the <see cref="ResxAddressFakerProvider"/> class for the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <returns>A new instance of <see cref="ResxAddressFakerProvider"/>.</returns>
    public static ResxAddressFakerProvider Create(CultureInfo culture) => new(culture);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResxAddressFakerProvider"/> class with the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="culture"/> is null.</exception>
    private ResxAddressFakerProvider(CultureInfo culture)
        : base(culture, AddressResources.ResourceManager)
    {
        StreetTypes = LoadDataset(nameof(AddressResources.StreetTypes));
        StreetSuffixes = LoadDataset(nameof(AddressResources.StreetSuffixes));
        StreetNames = LoadDataset(nameof(AddressResources.StreetNames));
        StreetFormats = LoadDataset(nameof(AddressResources.StreetFormats));
        Cities = LoadDataset(nameof(AddressResources.Cities));
        PostalCodeFormats = LoadDataset(nameof(AddressResources.PostalCodeFormats));
    }

    /// <inheritdoc />
    public IReadOnlyList<string> StreetTypes { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> StreetNames { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> StreetFormats { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> StreetSuffixes { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> Cities { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> PostalCodeFormats { get; }
}
