// -----------------------------------------------------------------------
// <copyright file="NumericInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace MyNet.Utilities.Intervals;

/// <summary>
/// Represents a numeric interval where the type of the values is a numeric type that implements the <see cref="INumber{T}"/> interface. This abstract class extends <see cref="Interval{T, TSelf}"/> and provides additional properties and methods specific to numeric intervals, such as calculating the length of the interval, checking if it is positive or negative, clamping a value to the interval, and normalizing a value within the interval. The constructor of this class initializes the start and end boundaries of the interval, which can be null to represent unbounded intervals. The type parameter T is constrained to be a struct that implements both <see cref="INumber{T}"/> and <see cref="IMinMaxValue{T}"/>, ensuring that it is a numeric type with defined minimum and maximum values.
/// </summary>
public abstract class NumericInterval<T, TSelf>(IntervalBoundary<T>? start, IntervalBoundary<T>? end) : Interval<T, TSelf>(start, end)
    where T : struct, INumber<T>, IMinMaxValue<T>
    where TSelf : NumericInterval<T, TSelf>
{
    /// <summary>
    /// Gets the length of the interval, calculated as the difference between the end and start values. If either the start or end boundary is not defined (i.e., if the interval is unbounded on either side), this property returns null, indicating that the length cannot be determined. The length is only meaningful for intervals that have both a defined start and end, and it represents the distance between these two boundaries in terms of the numeric type T.
    /// </summary>
    public T? Length => !HasStart || !HasEnd ? null : End!.Value.Value - Start!.Value.Value;

    /// <summary>
    /// Gets a value indicating whether the interval is positive, which is determined by checking if the length of the interval is greater than zero. If the length is null (i.e., if the interval is unbounded on either side), this property returns true, indicating that an unbounded interval is considered positive. For bounded intervals, it checks if the end value is greater than the start value, which would indicate a positive interval. This property provides a convenient way to determine the nature of the interval in terms of its direction and magnitude.
    /// </summary>
    public bool IsPositive => Length is null || Length > T.Zero;

    /// <summary>
    /// Gets a value indicating whether the interval is negative, which is determined by checking if the length of the interval is less than zero. If the length is null (i.e., if the interval is unbounded on either side), this property returns false, indicating that an unbounded interval is not considered negative. For bounded intervals, it checks if the end value is less than the start value, which would indicate a negative interval. This property provides a convenient way to determine the nature of the interval in terms of its direction and magnitude.
    /// </summary>
    public bool IsNegative => Length < T.Zero;

    /// <summary>
    /// Clamps a value to the interval, ensuring that it falls within the defined boundaries. If the value is less than the start boundary, it returns the start value. If the value is greater than the end boundary, it returns the end value. If the value is within the interval, it returns the value itself. This method provides a way to constrain a value to the interval's range.
    /// </summary>
    public T Clamp(T value) => HasStart && value.CompareTo(Start!.Value.Value) < 0 ? Start.Value.Value : HasEnd && value.CompareTo(End!.Value.Value) > 0 ? End.Value.Value : value;

    /// <summary>
    /// Normalizes a value within the interval, converting it to a value between 0 and 1 based on its position relative to the start and end boundaries. If the interval is unbounded, this method throws an <see cref="InvalidOperationException"/>. This method provides a way to scale a value to a standard range based on the interval's boundaries.
    /// </summary>
    public T Normalize(T value) => Length is null ? throw new InvalidOperationException("Cannot normalize unbounded interval.") : (value - Start!.Value.Value) / Length.Value;
}
