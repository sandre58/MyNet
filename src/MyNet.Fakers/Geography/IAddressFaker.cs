// -----------------------------------------------------------------------
// <copyright file="IAddressFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Utilities.Geography;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Provides methods for generating fake addresses.
/// </summary>
public interface IAddressFaker
{
    /// <summary>
    /// Generates a random address based on the specified culture.
    /// </summary>
    /// <param name="culture">The culture to use for generating the address.</param>
    /// <returns>A randomly generated address.</returns>
    Address Address(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random street address based on the specified culture.
    /// </summary>
    /// <param name="culture">The culture to use for generating the street address.</param>
    /// <returns>A randomly generated street address.</returns>
    string Street(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random city name based on the specified culture.
    /// </summary>
    /// <param name="culture">The culture to use for generating the city.</param>
    /// <returns>A randomly generated city name.</returns>
    string City(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random state or province name based on the specified culture.
    /// </summary>
    /// <param name="culture">The culture to use for generating the state or province name.</param>
    /// <returns>A randomly generated state or province name.</returns>
    string PostalCode(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random postal code based on the specified culture.
    /// </summary>
    /// <returns>A randomly generated coordinates.</returns>
    Coordinates Coordinates();
}
