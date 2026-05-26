// -----------------------------------------------------------------------
// <copyright file="ICountryFlagProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;

namespace MyNet.Geography;

/// <summary>
/// Defines a provider for country flags, allowing retrieval of flag images in various sizes.
/// </summary>
public interface ICountryFlagProvider
{
    /// <summary>
    /// Opens a stream to the flag image for the specified country and size.
    /// </summary>
    /// <param name="country">The country for which to retrieve the flag.</param>
    /// <param name="size">The size of the flag.</param>
    /// <returns>A stream containing the flag image, or null if not available.</returns>
    Stream? Open(Country country, FlagSize size);

    /// <summary>
    /// Gets the flag image as a byte array for the specified country and size.
    /// </summary>
    /// <param name="country">The country for which to retrieve the flag.</param>
    /// <param name="size">The size of the flag.</param>
    /// <returns>A byte array containing the flag image, or null if not available.</returns>
    byte[]? GetBytes(Country country, FlagSize size);
}
