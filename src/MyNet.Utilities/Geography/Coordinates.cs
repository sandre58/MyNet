// -----------------------------------------------------------------------
// <copyright file="Coordinates.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Geography;

/// <summary>
/// Represents geographic coordinates, consisting of a latitude and longitude. This record can be used to store and manipulate the location of a point on the Earth's surface. The Latitude property represents the north-south position, while the Longitude property represents the east-west position. Both properties are of type double, allowing for precise representation of geographic locations. This record can be utilized in various applications such as mapping, geocoding, and spatial analysis.
/// </summary>
/// <param name="Latitude">The north-south position of the point.</param>
/// <param name="Longitude">The east-west position of the point.</param>
public record Coordinates(double Latitude, double Longitude);
