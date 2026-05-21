// -----------------------------------------------------------------------
// <copyright file="TemperatureConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Conversion;

/// <summary>
/// Provides conversion between Celsius, Fahrenheit and Kelvin.
/// </summary>
public sealed class TemperatureConverter : IUnitConverter<TemperatureUnit>
{
    public double Convert(double value, TemperatureUnit from, TemperatureUnit to)
    {
        var celsius = ToCelsius(value, from);
        return FromCelsius(celsius, to);
    }

    private static double ToCelsius(double value, TemperatureUnit unit) => unit switch
    {
        TemperatureUnit.Celsius => value,
        TemperatureUnit.Fahrenheit => (value - 32d) * 5d / 9d,
        TemperatureUnit.Kelvin => value - 273.15d,
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported temperature unit.")
    };

    private static double FromCelsius(double value, TemperatureUnit unit) => unit switch
    {
        TemperatureUnit.Celsius => value,
        TemperatureUnit.Fahrenheit => (value * 9d / 5d) + 32d,
        TemperatureUnit.Kelvin => value + 273.15d,
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported temperature unit.")
    };
}
