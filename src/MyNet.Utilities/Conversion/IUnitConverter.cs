// -----------------------------------------------------------------------
// <copyright file="IUnitConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Conversion;

/// <summary>
/// Interface for converting values between different units of measurement.
/// </summary>
/// <typeparam name="TUnit">The type of the unit enumeration.</typeparam>
public interface IUnitConverter<in TUnit>
    where TUnit : struct, Enum
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="from">The unit to convert from.</param>
    /// <param name="to">The unit to convert to.</param>
    /// <returns>The converted value.</returns>
    double Convert(double value, TUnit from, TUnit to);
}
