// -----------------------------------------------------------------------
// <copyright file="Quantity.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Conversion;

/// <summary>
/// Provides a factory method for creating instances of <see cref="Quantity{TUnit}"/>. The <see cref="Of{TUnit}"/> method allows you to create a quantity by specifying a numeric value and a unit of measurement, where the unit is defined as an enum type. This class serves as a convenient entry point for constructing quantities without needing to directly instantiate the <see cref="Quantity{TUnit}"/> struct, promoting cleaner and more readable code when working with quantities in various units.
/// </summary>
public static class Quantity
{
    /// <summary>
    /// Creates a new instance of <see cref="Quantity{TUnit}"/> with the specified value and unit. The <typeparamref name="TUnit"/> type parameter must be an enum, which allows for a predefined set of units to be used when creating quantities. This method provides a simple and intuitive way to construct quantities by directly passing the numeric value and the corresponding unit, enhancing code readability and maintainability when dealing with measurements in different units.
    /// </summary>
    /// <param name="value">The numeric value of the quantity.</param>
    /// <param name="unit">The unit of measurement for the quantity.</param>
    /// <typeparam name="TUnit">The type of the unit, which must be an enum.</typeparam>
    /// <returns>A new instance of <see cref="Quantity{TUnit}"/> with the specified value and unit.</returns>
    public static Quantity<TUnit> Of<TUnit>(double value, TUnit unit)
        where TUnit : struct, Enum
        => new(value, unit);
}

/// <summary>
/// Represents a quantity of a specific unit. The <typeparamref name="TUnit"/> type parameter specifies the type of the unit, which must be an enum. The <see cref="Value"/> property holds the numeric value of the quantity, and the <see cref="Unit"/> property holds the unit of measurement. This struct is immutable and provides a string representation in the format "Value Unit".
/// </summary>
/// <param name="value">The numeric value of the quantity.</param>
/// <param name="unit">The unit of measurement for the quantity.</param>
/// <typeparam name="TUnit">The type of the unit, which must be an enum.</typeparam>
public readonly struct Quantity<TUnit>(double value, TUnit unit) : IEquatable<Quantity<TUnit>>, IComparable<Quantity<TUnit>>
    where TUnit : struct, Enum
{
    /// <summary>
    /// Gets the numeric value of the quantity. This property is read-only and is initialized through the constructor parameter. The value represents the magnitude of the quantity in the specified unit.
    /// </summary>
    public double Value { get; } = value;

    /// <summary>
    /// Gets the unit of measurement for the quantity. This property is read-only and is initialized through the constructor parameter. The unit is of type <typeparamref name="TUnit"/>, which must be an enum, allowing for a predefined set of units to be used with this struct.
    /// </summary>
    public TUnit Unit { get; } = unit;

    /// <summary>
    /// Returns a string that represents the current quantity, formatted as "Value Unit". This method overrides the default <see cref="object.ToString"/> implementation to provide a more meaningful representation of the quantity, combining both the numeric value and its associated unit in a human-readable format.
    /// </summary>
    /// <returns>A string representation of the quantity.</returns>
    public override string ToString() => $"{Value} {Unit}";

    /// <summary>
    /// Determines whether the specified object is equal to the current quantity. This method overrides the default Equals implementation to provide value-based equality comparison for quantities. Two quantities are considered equal if they have the same numeric value and the same unit of measurement. The method first checks if the provided object is of type <see cref="Quantity{TUnit}"/> and then compares both the <see cref="Value"/> and <see cref="Unit"/> properties for equality.
    /// </summary>
    /// <param name="obj">The object to compare with the current quantity.</param>
    /// <returns><c>true</c> if the specified object is equal to the current quantity; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is Quantity<TUnit> other && Equals(other);

    /// <summary>
    /// Determines whether the specified quantity is equal to the current quantity. This method implements the <see cref="IEquatable{T}.Equals(T)"/> method to provide a strongly-typed equality comparison for quantities. Two quantities are considered equal if they have the same numeric value and the same unit of measurement. The method compares both the <see cref="Value"/> and <see cref="Unit"/> properties for equality, returning <c>true</c> if both match and <c>false</c> otherwise.
    /// </summary>
    /// <param name="other">The quantity to compare with the current quantity.</param>
    /// <returns><c>true</c> if the specified quantity is equal to the current quantity; otherwise, <c>false</c>.</returns>
    public bool Equals(Quantity<TUnit> other)
    {
        var converter = UnitConverterRegistry.Get<TUnit>();

        return converter.Convert(Value, Unit, other.Unit).IsCloseTo(other.Value);
    }

    /// <summary>
    /// Returns a hash code for the current quantity. This method overrides the default <see cref="object.GetHashCode"/> implementation to provide a hash code that is based on both the numeric value and the unit of measurement. The hash code is computed by combining the hash codes of the <see cref="Value"/> and <see cref="Unit"/> properties using a bitwise XOR operation. This ensures that quantities with different values or units will produce different hash codes, which is important for the correct behavior of hash-based collections like dictionaries and hash sets.
    /// </summary>
    /// <returns>A hash code for the current quantity.</returns>
    public override int GetHashCode()
    {
        var converter = UnitConverterRegistry.Get<TUnit>();
        var canonicalValue = converter.Convert(Value, Unit, default);

        return HashCode.Combine(Math.Round(canonicalValue, 12), default(TUnit));
    }

    /// <summary>
    /// Compares the current quantity with another quantity of the same type. This method implements the <see cref="IComparable{T}.CompareTo(T)"/> method to provide a way to compare two quantities for ordering. The comparison is based on the numeric value of the quantities, but since they may have different units, the method first converts both quantities to a common base unit using the <see cref="UnitConverterRegistry.Get{TUnit}()"/> method before comparing their values. The method returns a negative integer if the current quantity is less than the other quantity, zero if they are equal, and a positive integer if the current quantity is greater than the other quantity.
    /// </summary>
    /// <param name="other">The quantity to compare with the current quantity.</param>
    /// <returns>A signed integer that indicates the relative order of the quantities being compared.</returns>
    public int CompareTo(Quantity<TUnit> other)
    {
        var converter = UnitConverterRegistry.Get<TUnit>();

        var left = converter.Convert(Value, Unit, other.Unit);

        return left.CompareTo(other.Value);
    }

    /// <summary>
    /// Determines whether two quantities are equal by comparing their values and units. This operator overload provides a convenient way to compare two instances of <see cref="Quantity{TUnit}"/> for equality using the == operator. It internally calls the <see cref="Equals(Quantity{TUnit})"/> method to perform the comparison, returning <c>true</c> if both quantities have the same value and unit, and <c>false</c> otherwise.
    /// </summary>
    /// <param name="left">The first quantity to compare.</param>
    /// <param name="right">The second quantity to compare.</param>
    /// <returns><c>true</c> if the specified quantities are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Quantity<TUnit> left, Quantity<TUnit> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two quantities are not equal by comparing their values and units. This operator overload provides a convenient way to compare two instances of <see cref="Quantity{TUnit}"/> for inequality using the != operator. It internally calls the <see cref="Equals(Quantity{TUnit})"/> method to perform the comparison, returning <c>true</c> if the quantities have different values or units, and <c>false</c> otherwise.
    /// </summary>
    /// <param name="left">The first quantity to compare.</param>
    /// <param name="right">The second quantity to compare.</param>
    /// <returns><c>true</c> if the specified quantities are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Quantity<TUnit> left, Quantity<TUnit> right) => !(left == right);

    /// <summary>
    /// Determines whether one quantity is less than another quantity of the same type. This operator overload provides a convenient way to compare two instances of <see cref="Quantity{TUnit}"/> for ordering using the operator. It internally calls the <see cref="CompareTo(Quantity{TUnit})"/> method to perform the comparison, returning <c>true</c> if the left quantity is less than the right quantity, and <c>false</c> otherwise.
    /// </summary>
    /// <param name="left">The first quantity to compare.</param>
    /// <param name="right">The second quantity to compare.</param>
    /// <returns><c>true</c> if the left quantity is less than the right quantity; otherwise, <c>false</c>.</returns>
    public static bool operator <(Quantity<TUnit> left, Quantity<TUnit> right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether one quantity is greater than another quantity of the same type. This operator overload provides a convenient way to compare two instances of <see cref="Quantity{TUnit}"/> for ordering using the > operator. It internally calls the <see cref="CompareTo(Quantity{TUnit})"/> method to perform the comparison, returning <c>true</c> if the left quantity is greater than the right quantity, and <c>false</c> otherwise.
    /// </summary>
    /// <param name="left">The first quantity to compare.</param>
    /// <param name="right">The second quantity to compare.</param>
    /// <returns><c>true</c> if the left quantity is greater than the right quantity; otherwise, <c>false</c>.</returns>
    public static bool operator >(Quantity<TUnit> left, Quantity<TUnit> right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether one quantity is less than or equal to another quantity of the same type. This operator overload provides a convenient way to compare two instances of <see cref="Quantity{TUnit}"/> for ordering using the operator. It internally calls the <see cref="CompareTo(Quantity{TUnit})"/> method to perform the comparison, returning <c>true</c> if the left quantity is less than or equal to the right quantity, and <c>false</c> otherwise.
    /// </summary>
    /// <param name="left">The first quantity to compare.</param>
    /// <param name="right">The second quantity to compare.</param>
    /// <returns><c>true</c> if the left quantity is less than or equal to the right quantity; otherwise, <c>false</c>.</returns>
    public static bool operator <=(Quantity<TUnit> left, Quantity<TUnit> right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether one quantity is greater than or equal to another quantity of the same type. This operator overload provides a convenient way to compare two instances of <see cref="Quantity{TUnit}"/> for ordering using the >= operator. It internally calls the <see cref="CompareTo(Quantity{TUnit})"/> method to perform the comparison, returning <c>true</c> if the left quantity is greater than or equal to the right quantity, and <c>false</c> otherwise.
    /// </summary>
    /// <param name="left">The first quantity to compare.</param>
    /// <param name="right">The second quantity to compare.</param>
    /// <returns><c>true</c> if the left quantity is greater than or equal to the right quantity; otherwise, <c>false</c>.</returns>
    public static bool operator >=(Quantity<TUnit> left, Quantity<TUnit> right) => left.CompareTo(right) >= 0;
}
