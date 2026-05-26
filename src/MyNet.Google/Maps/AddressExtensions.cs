// -----------------------------------------------------------------------
// <copyright file="AddressExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Geography;

namespace MyNet.Google.Maps;

/// <summary>
/// Extension methods for <see cref="Address"/> objects.
/// </summary>
public static class AddressExtensions
{
    /// <summary>
    /// Opens the address in Google Maps using configured settings; coordinates are used if available.
    /// </summary>
    /// <param name="address">The address to open.</param>
    public static void OpenInGoogleMaps(this Address address)
    {
        var coordinates = address is { Coordinates: not null } ? new Coordinates(address.Coordinates.Latitude, address.Coordinates.Longitude) : null;
        var fullAddress = address.ToString();
        GoogleMapsHelper.OpenGoogleMaps(new() { Coordinates = coordinates, Address = fullAddress });
    }
}
