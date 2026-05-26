// -----------------------------------------------------------------------
// <copyright file="LengthConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Primitives.Conversion;

/// <summary>
/// Provides conversion between different length units based on the metric system. The conversion is performed using powers of 10, where each unit is defined as a multiple of the base unit (meter). For example, converting from kilometers to meters involves multiplying by 10^3, while converting from centimeters to meters involves multiplying by 10^-2.
/// </summary>
public sealed class LengthConverter : IUnitConverter<LengthUnit>
{
    /// <summary>
    /// Converts a length value from one metric unit to another by applying the appropriate power of 10 based on the difference in unit magnitudes. The conversion is calculated as value multiplied by 10 raised to the power of the difference between the target unit and the source unit, effectively scaling the value up or down according to the metric prefixes.
    /// </summary>
    /// <param name="value">The length value to convert.</param>
    /// <param name="from">The unit of the input value.</param>
    /// <param name="to">The unit to convert the value to.</param>
    /// <returns>The converted length value.</returns>
    public double Convert(double value, LengthUnit from, LengthUnit to) => value * Math.Pow(10, from - to);
}
