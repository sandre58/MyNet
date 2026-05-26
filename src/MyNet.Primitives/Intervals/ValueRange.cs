// -----------------------------------------------------------------------
// <copyright file="ValueRange.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents a range of values of type T, defined by optional minimum and maximum boundaries. The range can be inclusive or exclusive at each boundary, allowing for flexible definitions of valid value sets. This class provides methods to check if a value is within the range, to clamp a value to the range, and to ensure that a value is within the range, throwing an exception if it is not. The type parameter T must be a struct that implements <see cref="IComparable{T}"/>, ensuring that the values can be compared to determine their order relative to the boundaries.
/// </summary>
/// <typeparam name="T">The type of the values in the range.</typeparam>
public sealed class ValueRange<T>
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueRange{T}"/> class with optional minimum and maximum boundaries, and flags indicating whether each boundary is inclusive or exclusive. If both minimum and maximum values are provided, the constructor checks that the minimum value is not greater than the maximum value, throwing an <see cref="ArgumentException"/> if this condition is violated. The constructor allows for creating ranges that are unbounded on either side by passing null for the respective boundary, and provides flexibility in defining whether the boundaries are inclusive or exclusive through the boolean parameters. This design enables the creation of a wide variety of ranges to suit different validation and constraint scenarios.
    /// </summary>
    /// <param name="min">The minimum value of the range, or null if the range has no lower bound.</param>
    /// <param name="max">The maximum value of the range, or null if the range has no upper bound.</param>
    /// <param name="minInclusive">Indicates whether the minimum value is inclusive.</param>
    /// <param name="maxInclusive">Indicates whether the maximum value is inclusive.</param>
    /// <exception cref="ArgumentException">Thrown when the minimum value is greater than the maximum value.</exception>
    public ValueRange(T? min = null, T? max = null, bool minInclusive = true, bool maxInclusive = true)
    {
        if (min.HasValue && max.HasValue && min.Value.CompareTo(max.Value) > 0)
        {
            throw new ArgumentException("Min must be lower than max.");
        }

        Min = min;
        Max = max;
        IsMinInclusive = minInclusive;
        IsMaxInclusive = maxInclusive;
    }

    /// <summary>
    /// Gets the minimum value of the range, or null if the range has no lower bound. This property represents the lower boundary of the range and can be used to determine if a value is valid based on whether it falls above this minimum threshold. If the range is unbounded at the lower end, this property will return null, indicating that there is no minimum constraint on the values that can be considered valid within this range.
    /// </summary>
    public T? Min { get; }

    /// <summary>
    /// Gets the maximum value of the range, or null if the range has no upper bound. This property represents the upper boundary of the range and can be used to determine if a value is valid based on whether it falls below this maximum threshold. If the range is unbounded at the upper end, this property will return null, indicating that there is no maximum constraint on the values that can be considered valid within this range.
    /// </summary>
    public T? Max { get; }

    /// <summary>
    /// Gets a value indicating whether the minimum boundary of the range is inclusive. If this property is true, values that are equal to the minimum value are considered valid and are included in the range. If this property is false, values that are equal to the minimum value are considered invalid and are excluded from the range. This property allows for flexibility in defining the range, enabling it to be either inclusive or exclusive at the lower boundary based on the specific requirements of the use case.
    /// </summary>
    public bool IsMinInclusive { get; }

    /// <summary>
    /// Gets a value indicating whether the maximum boundary of the range is inclusive. If this property is true, values that are equal to the maximum value are considered valid and are included in the range. If this property is false, values that are equal to the maximum value are considered invalid and are excluded from the range. This property allows for flexibility in defining the range, enabling it to be either inclusive or exclusive at the upper boundary based on the specific requirements of the use case.
    /// </summary>
    public bool IsMaxInclusive { get; }

    /// <summary>
    /// Gets a value indicating whether the range has a defined minimum boundary. If this property is true, the Min property contains a valid value that represents the lower boundary of the range. If this property is false, the Min property is null, indicating that the range is unbounded at the lower end and there is no minimum constraint on the values that can be considered valid within this range.
    /// </summary>
    public bool HasMin => Min.HasValue;

    /// <summary>
    /// Gets a value indicating whether the range has a defined maximum boundary. If this property is true, the Max property contains a valid value that represents the upper boundary of the range. If this property is false, the Max property is null, indicating that the range is unbounded at the upper end and there is no maximum constraint on the values that can be considered valid within this range.
    /// </summary>
    public bool HasMax => Max.HasValue;

    /// <summary>
    /// Determines whether a specified value falls within the range defined by the minimum and maximum boundaries, taking into account whether each boundary is inclusive or exclusive. The method checks if the value is greater than the minimum boundary (if it exists) and less than the maximum boundary (if it exists), applying the appropriate comparison based on whether the boundaries are inclusive or exclusive. If the value satisfies these conditions, it returns true, indicating that the value is contained within the range; otherwise, it returns false. This method provides a way to validate values against the defined range constraints.
    /// </summary>
    /// <param name="value">The value to check against the range.</param>
    /// <returns>True if the value is within the range; otherwise, false.</returns>
    public bool Contains(T value)
    {
        var validMin =
            !HasMin ||
            value.CompareTo(Min!.Value) > 0 ||
            (value.CompareTo(Min.Value) == 0 && IsMinInclusive);

        var validMax =
            !HasMax ||
            value.CompareTo(Max!.Value) < 0 ||
            (value.CompareTo(Max.Value) == 0 && IsMaxInclusive);

        return validMin && validMax;
    }

    /// <summary>
    /// Clamps a specified value to the range defined by the minimum and maximum boundaries. If the value is less than the minimum boundary, it returns the minimum value. If the value is greater than the maximum boundary, it returns the maximum value. If the value is within the range, it returns the value itself. This method provides a way to ensure that a value falls within the defined range by adjusting it to the nearest boundary if it exceeds the limits of the range.
    /// </summary>
    /// <param name="value">The value to clamp to the range.</param>
    /// <returns>The clamped value.</returns>
    public T Clamp(T value) =>
        HasMin &&
        value.CompareTo(Min!.Value) < 0
            ? Min.Value
            : HasMax &&
              value.CompareTo(Max!.Value) > 0
                ? Max.Value
                : value;

    /// <summary>
    /// Ensures that a specified value falls within the range defined by the minimum and maximum boundaries. If the value is not contained within the range, this method throws an <see cref="ArgumentOutOfRangeException"/> with a message indicating that the value must be inside the defined range. If the value is valid, it returns the value itself. This method provides a way to enforce that a value meets the constraints of the range, and to provide clear feedback when it does not.
    /// </summary>
    /// <param name="value">The value to check against the range.</param>
    /// <param name="propertyName">The name of the property being validated.</param>
    /// <returns>The value if it is within the range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is not within the range.</exception>
    public T EnsureInRange(T value, [CallerMemberName] string propertyName = "") =>
        !Contains(value)
            ? throw new ArgumentOutOfRangeException(propertyName, value, "Value must be inside range.")
            : value;

    /// <summary>
    /// Returns a string representation of the value range, indicating the minimum and maximum boundaries along with their inclusivity or exclusivity. The format of the string is determined by the IsMinInclusive and IsMaxInclusive properties, using square brackets for inclusive boundaries and parentheses for exclusive boundaries. For example, if the range is defined with a minimum of 1 (inclusive) and a maximum of 10 (exclusive), the string representation would be "[1; 10[". If the range is unbounded on either side, it will indicate that with an appropriate symbol (e.g., "(-∞; 10]" for an unbounded minimum). This method provides a clear and concise way to visualize the defined range.
    /// </summary>
    /// <returns>A string representation of the value range.</returns>
    public override string ToString()
    {
        var start = IsMinInclusive ? "[" : "]";
        var end = IsMaxInclusive ? "]" : "[";

        return $"{start}{Min}; {Max}{end}";
    }
}
