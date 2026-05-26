// -----------------------------------------------------------------------
// <copyright file="BoundedUnitValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using MyNet.Primitives;
using MyNet.Primitives.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Observable bounded numeric value with a measurement unit and conversion helpers.
/// </summary>
/// <typeparam name="T">The numeric type.</typeparam>
/// <typeparam name="TUnit">The unit enum type.</typeparam>
public class BoundedUnitValue<T, TUnit> : BoundedValue<T>, IBoundedUnitValue<T, TUnit>
    where T : struct, INumber<T>
    where TUnit : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedUnitValue{T, TUnit}"/> class.
    /// </summary>
    /// <param name="unit">The measurement unit.</param>
    /// <param name="propertyDisplayKey">Translation key used in validation messages.</param>
    /// <param name="defaultValue">Optional default value.</param>
    public BoundedUnitValue(TUnit unit, string propertyDisplayKey = nameof(Value), T? defaultValue = null)
        : base(propertyDisplayKey, defaultValue) => Unit = unit;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedUnitValue{T, TUnit}"/> class with bounds.
    /// </summary>
    /// <param name="unit">The measurement unit.</param>
    /// <param name="min">Inclusive minimum, if any.</param>
    /// <param name="max">Inclusive maximum, if any.</param>
    /// <param name="propertyDisplayKey">Translation key used in validation messages.</param>
    /// <param name="defaultValue">Optional default value.</param>
    public BoundedUnitValue(TUnit unit, T? min, T? max, string propertyDisplayKey = nameof(Value), T? defaultValue = null)
        : base(min, max, propertyDisplayKey, defaultValue) => Unit = unit;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedUnitValue{T, TUnit}"/> class.
    /// </summary>
    /// <param name="unit">The measurement unit.</param>
    /// <param name="range">Inclusive bounds.</param>
    /// <param name="propertyDisplayKey">Translation key used in validation messages.</param>
    /// <param name="defaultValue">Optional default value.</param>
    public BoundedUnitValue(TUnit unit, Interval<T> range, string propertyDisplayKey = nameof(Value), T? defaultValue = null)
        : base(range, propertyDisplayKey, defaultValue) => Unit = unit;

    /// <inheritdoc />
    public TUnit Unit { get; }

    /// <inheritdoc />
    public T Convert(TUnit unit)
    {
        if (!Value.HasValue)
            return default;

        var numeric = double.CreateChecked(Value.Value);
        return T.CreateChecked(numeric.To(Unit, unit));
    }

    /// <summary>
    /// Normalizes using the default unit range.
    /// </summary>
    public IBoundedUnitValue<T, TUnit> Normalize() => Normalize(null, null);

    /// <inheritdoc />
    public IBoundedUnitValue<T, TUnit> Normalize(TUnit? minUnit, TUnit? maxUnit)
    {
        if (!Value.HasValue)
            return this;

        var numeric = double.CreateChecked(Value.Value);
        var (newValue, newUnit) = numeric.Simplify(Unit, minUnit, maxUnit);
        return new BoundedUnitValue<T, TUnit>(newUnit, Range) { Value = T.CreateChecked(newValue) };
    }

    /// <inheritdoc />
    public override string? ToString() => Value.HasValue ? $"{Value} {Unit}" : null;
}

/// <summary>
/// Bounded file-size value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FileSizeValue"/> class.
/// </remarks>
public class FileSizeValue(DataSizeUnit unit = DataSizeUnit.Byte) : BoundedUnitValue<double, DataSizeUnit>(unit);

/// <summary>
/// Bounded length value.
/// </summary>
public class LengthValue : BoundedUnitValue<double, LengthUnit>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LengthValue"/> class.
    /// </summary>
    public LengthValue(LengthUnit unit = LengthUnit.Meter)
        : base(unit) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LengthValue"/> class with bounds.
    /// </summary>
    public LengthValue(double? min, double? max, LengthUnit unit = LengthUnit.Meter)
        : base(unit, min, max) { }
}

/// <summary>
/// Bounded mass value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MassValue"/> class.
/// </remarks>
public class MassValue(MassUnit unit = MassUnit.Gram) : BoundedUnitValue<double, MassUnit>(unit);

/// <summary>
/// Bounded temperature value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TemperatureValue"/> class.
/// </remarks>
public class TemperatureValue(TemperatureUnit unit = TemperatureUnit.Celsius) : BoundedUnitValue<double, TemperatureUnit>(unit);
