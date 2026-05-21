// -----------------------------------------------------------------------
// <copyright file="TemperatureUnit.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Specifies a unit of measurement for temperature.
/// </summary>
public enum TemperatureUnit
{
    [Symbol("°C")]
    Celsius,

    [Symbol("°F")]
    Fahrenheit,

    [Symbol("K")]
    Kelvin
}
