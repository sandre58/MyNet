// -----------------------------------------------------------------------
// <copyright file="IAddressFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Defines a provider for generating fake address data, including street names, city names, and postal code formats. This interface extends <see cref="ICultureScoped"/> to ensure that the generated address data is culturally appropriate based on the specified culture.
/// </summary>
public interface IAddressFakerProvider : ICultureScoped
{
    /// <summary>
    /// Gets the list of street types for the culture. These types will be used when generating random street names to ensure that the generated data is culturally appropriate. Examples of street types include "Street", "Avenue", "Boulevard", etc.
    /// </summary>
    IReadOnlyList<string> StreetTypes { get; }

    /// <summary>
    /// Gets the list of street names for the culture. These names will be used when generating random street names to ensure that the generated data is culturally appropriate.
    /// </summary>
    IReadOnlyList<string> StreetNames { get; }

    /// <summary>
    /// Gets the list of street formats for the culture. These formats will be used when generating random street addresses to ensure that the generated data is culturally appropriate.
    /// </summary>
    IReadOnlyList<string> StreetFormats { get; }

    /// <summary>
    /// Gets the list of street suffixes for the culture. These suffixes will be used when generating random street names to ensure that the generated data is culturally appropriate.
    /// </summary>
    IReadOnlyList<string> StreetSuffixes { get; }

    /// <summary>
    /// Gets the list of city names for the culture. These names will be used when generating random addresses to ensure that the generated data is culturally appropriate.
    /// </summary>
    IReadOnlyList<string> Cities { get; }

    /// <summary>
    /// Gets the list of postal code formats for the culture. Each format can contain placeholders (e.g., "#####", "#####-####") that will be replaced with random digits when generating postal codes.
    /// </summary>
    IReadOnlyList<string> PostalCodeFormats { get; }
}
