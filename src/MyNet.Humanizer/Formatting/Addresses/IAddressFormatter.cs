// -----------------------------------------------------------------------
// <copyright file="IAddressFormatter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Geography;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Humanizer.Formatting.Addresses;

/// <summary>
/// Defines a contract for formatting addresses based on their components and culture information.
/// </summary>
public interface IAddressFormatter : ICultureScoped
{
    /// <summary>
    /// Formats an address into a human readable string based on the provided address components and culture information.
    /// </summary>
    /// <param name="address">The address to format.</param>
    /// <returns>A formatted address string.</returns>
    string Format(Address address);
}
