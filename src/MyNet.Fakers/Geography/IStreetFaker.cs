// -----------------------------------------------------------------------
// <copyright file="IStreetFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Defines a contract for generating fake street names based on culture information.
/// </summary>
public interface IStreetFaker
{
    /// <summary>
    /// Generates a random street name with a type and a number (e.g., "123 Main Street", "456 Elm Avenue") based on the provided culture information. The generated street name with type and number should be culturally appropriate and may include common street name formats, suffixes, and numbering conventions used in the specified culture.
    /// </summary>
    /// <param name="culture">The culture information used to generate the street name with type and number.</param>
    /// <returns>A randomly generated street name with type and number.</returns>
    string Number(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random street type (e.g., "Street", "Avenue", "Boulevard") based on the provided culture information. The generated street type should be culturally appropriate and may include common street types used in the specified culture.
    /// </summary>
    /// <param name="culture">The culture information used to generate the street type.</param>
    /// <returns>A randomly generated street type.</returns>
    string Type(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random street name with a type (e.g., "Main Street", "Elm Avenue") based on the provided culture information. The generated street name with type should be culturally appropriate and may include common street name formats and suffixes used in the specified culture.
    /// </summary>
    /// <param name="culture">The culture information used to generate the street name with type.</param>
    /// <returns>A randomly generated street name with type.</returns>
    string Name(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random street name based on the provided culture information. The generated street name should be culturally appropriate and may include common street name formats and suffixes used in the specified culture.
    /// </summary>
    /// <param name="culture">The culture information used to generate the street name.</param>
    /// <returns>A randomly generated street name.</returns>
    string Street(CultureInfo? culture = null);
}
