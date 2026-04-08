// -----------------------------------------------------------------------
// <copyright file="Address.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;

namespace MyNet.Utilities.Geography;

/// <summary>
/// Represents a physical address, including optional geographic coordinates. This record can be used to store and manipulate address information in a structured way, allowing for easy access to individual components such as street, postal code, city, country, latitude, and longitude. The ToString method provides a convenient way to get a formatted string representation of the address, combining the available components while ignoring any that are null or empty.
/// </summary>
/// <param name="Street">The street address.</param>
/// <param name="PostalCode">The postal code.</param>
/// <param name="City">The city.</param>
/// <param name="Country">The country.</param>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
public record Address(string? Street = null, string? PostalCode = null, string? City = null, Country? Country = null, double? Latitude = null, double? Longitude = null)
{
    /// <summary>
    /// Returns a string that represents the address, formatted as a space-separated list of the street, postal code,
    /// city, and country name.
    /// </summary>
    /// <remarks>This method concatenates the address components, ensuring that only non-empty values are
    /// included in the final output. The country name is accessed through the Country property, which may be
    /// null.</remarks>
    /// <returns>A string containing the formatted address. If any of the address components are null or empty, they are omitted
    /// from the result.</returns>
    public override string ToString() => string.Join(" ", new[] { Street, PostalCode, City, Country?.Name }.Where(x => !string.IsNullOrEmpty(x)));
}
