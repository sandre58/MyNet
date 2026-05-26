// -----------------------------------------------------------------------
// <copyright file="ICountryFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Geography;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Interface for generating fake location-related data such as countries and addresses.
/// </summary>
public interface ICountryFaker
{
    /// <summary>
    /// Generates a random country from the list of available countries.
    /// </summary>
    /// <returns>A random country.</returns>
    Country Country();

    /// <summary>
    /// Generates a random country name.
    /// </summary>
    /// <returns>A random country name.</returns>
    string Name();

    /// <summary>
    /// Generates a random country alpha-2 code.
    /// </summary>
    /// <returns>A random country alpha-2 code.</returns>
    string Alpha2();

    /// <summary>
    /// Generates a random country alpha-3 code.
    /// </summary>
    /// <returns>A random country alpha-3 code.</returns>
    string Alpha3();

    /// <summary>
    /// Generates a random country ISO code.
    /// </summary>
    /// <returns>A random country ISO code.</returns>
    int Iso();
}
