// -----------------------------------------------------------------------
// <copyright file="OpenInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Intervals;

/// <summary>
/// Represents an open interval where both the start and end boundaries are defined and are exclusive. This class extends <see cref="Interval{T, TSelf}"/> and implements <see cref="IOpenInterval{T}"/>, providing specific guarantees that both boundaries are exclusive, meaning that the interval does not include its start and end values. The constructor of this class initializes the start and end boundaries with the provided values, marking them as exclusive by setting the IsInclusive property of the <see cref="IntervalBoundary{T}"/> to false. This makes it suitable for scenarios where strict inequalities are required (e.g., (a, b) instead of [a, b]), ensuring that the interval does not include its endpoints.
/// </summary>
/// <param name="start">The start boundary of the interval.</param>
/// <param name="end">The end boundary of the interval.</param>
public class OpenInterval<T>(T start, T end) : Interval<T, OpenInterval<T>>(new IntervalBoundary<T>(start, false), new IntervalBoundary<T>(end, false)), IOpenInterval<T>
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Gets the start boundary of the interval, which is guaranteed to be exclusive. This property returns an <see cref="IntervalBoundary{T}"/> that encapsulates the start value of the interval along with its exclusivity status. Since this class represents an open interval, the Start boundary will always be marked as exclusive, ensuring that the interval does not include its starting value.
    /// </summary>
    IntervalBoundary<T> IBoundedInterval<T>.Start => Start!.Value;

    /// <summary>
    /// Gets the end boundary of the interval, which is guaranteed to be exclusive. This property returns an <see cref="IntervalBoundary{T}"/> that encapsulates the end value of the interval along with its exclusivity status. Since this class represents an open interval, the End boundary will always be marked as exclusive, ensuring that the interval does not include its ending value.
    /// </summary>
    IntervalBoundary<T> IBoundedInterval<T>.End => End!.Value;

    /// <inheritdoc />
    protected override OpenInterval<T> Create(IntervalBoundary<T>? start, IntervalBoundary<T>? end) =>
        !start.HasValue || !end.HasValue
            ? throw new InvalidOperationException("OpenInterval must be bounded.")
            : start.Value.IsInclusive || end.Value.IsInclusive
                ? throw new InvalidOperationException("OpenInterval only supports exclusive boundaries.")
                : new(start.Value.Value, end.Value.Value);
}
