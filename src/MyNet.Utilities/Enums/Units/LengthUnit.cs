// -----------------------------------------------------------------------
// <copyright file="LengthUnit.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Specifies a unit of measurement for length, ranging from nanometers to kilometers.
/// </summary>
public enum LengthUnit
{
    Nanometer = -9,
    Micrometer = -6,
    Millimeter = -3,
    Centimeter = -2,
    Decimeter = -1,
    Meter = 0,
    Decameter = 1,
    Hectometer = 2,
    Kilometer = 3
}
