// -----------------------------------------------------------------------
// <copyright file="NumericRange.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents a closed numeric interval where both the start and end boundaries are defined and are inclusive. This class extends <see cref="NumericInterval{T, TSelf}"/> and provides specific guarantees that both boundaries are inclusive, meaning that the interval includes its start and end values. The constructor of this class initializes the start and end boundaries with the provided values, marking them as inclusive by setting the IsInclusive property of the <see cref="IntervalBoundary{T}"/> to true. This makes it suitable for scenarios where non-strict inequalities are required (e.g., [a, b] instead of (a, b]), ensuring that the interval includes its endpoints.
/// </summary>
public sealed class NumericRange<T>(T start, T end) : NumericInterval<T, NumericRange<T>>(new IntervalBoundary<T>(start), new IntervalBoundary<T>(end)), IClosedInterval<T>
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    /// <summary>
    /// Gets the start boundary of the interval, which is guaranteed to be inclusive. This property returns an <see cref="IntervalBoundary{T}"/> that encapsulates the start value of the interval along with its inclusivity status. Since this class represents a closed interval, the Start boundary will always be marked as inclusive, ensuring that the interval includes its starting value.
    /// </summary>
    IntervalBoundary<T> IBoundedInterval<T>.Start => Start!.Value;

    /// <summary>
    /// Gets the end boundary of the interval, which is guaranteed to be inclusive. This property returns an <see cref="IntervalBoundary{T}"/> that encapsulates the end value of the interval along with its inclusivity status. Since this class represents a closed interval, the End boundary will always be marked as inclusive, ensuring that the interval includes its ending value.
    /// </summary>
    IntervalBoundary<T> IBoundedInterval<T>.End => End!.Value;

    /// <inheritdoc />
    protected override NumericRange<T> Create(IntervalBoundary<T>? start, IntervalBoundary<T>? end) =>
        !start.HasValue || !end.HasValue
            ? throw new InvalidOperationException("NumericRange must be bounded.")
            : !start.Value.IsInclusive || !end.Value.IsInclusive
                ? throw new InvalidOperationException("NumericRange only supports inclusive boundaries.")
                : new(start.Value.Value, end.Value.Value);
}
