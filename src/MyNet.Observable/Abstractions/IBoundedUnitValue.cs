// -----------------------------------------------------------------------
// <copyright file="IBoundedUnitValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for a bounded unit value that supports conversion between different units and normalization. This interface extends <see cref="IBoundedValue{T}"/>, allowing implementing classes to represent a value of type T that is constrained within specified minimum and maximum bounds, while also associating it with a specific unit of measurement represented by the TUnit enum. The Convert method allows for converting the value to a different unit, while the Normalize method provides a way to normalize the value within the specified bounds, optionally using different minimum and maximum units for normalization. This is particularly useful in scenarios where values need to be represented in different units or normalized for comparison or display purposes, such as in applications involving measurements, physics calculations, or any context where unit conversions and normalization are relevant. By implementing this interface, an object can provide a robust mechanism for handling bounded values with associated units and conversions, enhancing its versatility and usability in various applications.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <typeparam name="TUnit">The type of the unit.</typeparam>
public interface IBoundedUnitValue<T, TUnit> : IBoundedValue<T>
where T : struct, INumber<T>
where TUnit : Enum
{
    /// <summary>
    /// Gets the unit of measurement associated with the value. This property represents the specific unit in which the value is expressed, such as meters, seconds, or any other relevant unit defined by the TUnit enum. The unit is essential for understanding the context of the value and for performing conversions between different units when necessary. Implementing classes should ensure that the Unit property is properly set and maintained to allow for accurate conversions and normalization based on the associated unit of measurement.
    /// </summary>
    TUnit Unit { get; }

    /// <summary>
    /// Converts the value to a different unit of measurement specified by the unit parameter. This method allows for converting the value from its current unit to another unit defined by the TUnit enum, enabling flexibility in how the value is represented and used in different contexts. The conversion logic should be implemented based on the specific units involved and their relationships, ensuring that the converted value is accurate and meaningful in the context of the new unit. By providing this method, implementing classes can facilitate seamless conversions between different units, enhancing the usability and versatility of the bounded unit value in various applications.
    /// </summary>
    /// <param name="unit">The target unit to which the value should be converted.</param>
    /// <returns>The value converted to the specified unit.</returns>
    T Convert(TUnit unit);

    /// <summary>
    /// Normalizes the value within the specified bounds, optionally using different minimum and maximum units for normalization. This method allows for adjusting the value to fit within a specified range, taking into account the units of measurement. The normalization logic should be implemented based on the specific units and their relationships, ensuring that the normalized value is accurate and meaningful in the context of the specified bounds. By providing this method, implementing classes can facilitate consistent and meaningful comparisons or representations of bounded unit values across different units.
    /// </summary>
    /// <param name="minUnit">The optional minimum unit for normalization.</param>
    /// <param name="maxUnit">The optional maximum unit for normalization.</param>
    /// <returns>The normalized bounded unit value.</returns>
    IBoundedUnitValue<T, TUnit> Normalize(TUnit? minUnit = default, TUnit? maxUnit = default);
}
