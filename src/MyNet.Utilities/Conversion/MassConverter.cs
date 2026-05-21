// -----------------------------------------------------------------------
// <copyright file="MassConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Conversion;

/// <summary>
/// Provides conversion between different mass units. The conversion is based on powers of 10, where each unit is 10 times larger than the previous one. For example, 1 kilogram (kg) is 1000 grams (g), and 1 gram is 1000 milligrams (mg). The converter uses the difference in unit values to calculate the conversion factor.
/// </summary>
public sealed class MassConverter : IUnitConverter<MassUnit>
{
    /// <summary>
    /// Converts a mass value from one unit to another. The conversion is performed by multiplying the input value by 10 raised to the power of the difference between the target unit and the source unit. This allows for easy conversion between units that are based on powers of 10, such as milligrams, grams, kilograms, etc.
    /// </summary>
    /// <param name="value">The mass value to convert.</param>
    /// <param name="from">The unit of the input value.</param>
    /// <param name="to">The unit to convert the value to.</param>
    /// <returns>The converted mass value.</returns>
    public double Convert(double value, MassUnit from, MassUnit to) => value * Math.Pow(10, from - to);
}
