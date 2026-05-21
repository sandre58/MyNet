// -----------------------------------------------------------------------
// <copyright file="IInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Intervals;

/// <summary>
/// Represents a generic interval with optional start and end boundaries. The interval can be bounded or unbounded on either side, and provides methods to check if a value is contained within the interval, if it contains another interval, or if it intersects with another interval.
/// </summary>
/// <typeparam name="T">The type of the values in the interval.</typeparam>
public interface IInterval<T> : IComparable<IInterval<T>>
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Gets the start boundary of the interval, which may be null if the interval is unbounded at the start. The boundary includes information about whether it is inclusive or exclusive.
    /// </summary>
    IntervalBoundary<T>? Start { get; }

    /// <summary>
    /// Gets the end boundary of the interval, which may be null if the interval is unbounded at the end. The boundary includes information about whether it is inclusive or exclusive.
    /// </summary>
    IntervalBoundary<T>? End { get; }

    /// <summary>
    /// Gets a value indicating whether the interval has a start boundary.
    /// </summary>
    bool HasStart { get; }

    /// <summary>
    /// Gets a value indicating whether the interval has an end boundary.
    /// </summary>
    bool HasEnd { get; }

    /// <summary>
    /// Gets a value indicating whether the interval is unbounded at the start.
    /// </summary>
    bool IsUnboundedStart { get; }

    /// <summary>
    /// Gets a value indicating whether the interval is unbounded at the end.
    /// </summary>
    bool IsUnboundedEnd { get; }

    /// <summary>
    /// Determines whether the interval contains a specified value.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the interval contains the specified value; otherwise, <c>false</c>.</returns>
    bool Contains(T value);

    /// <summary>
    /// Determines whether the interval contains another interval.
    /// </summary>
    /// <param name="other">The interval to check.</param>
    /// <returns><c>true</c> if the interval contains the specified interval; otherwise, <c>false</c>.</returns>
    bool Contains(IInterval<T> other);

    /// <summary>
    /// Determines whether the interval intersects with another interval.
    /// </summary>
    /// <param name="other">The interval to check.</param>
    /// <returns><c>true</c> if the interval intersects with the specified interval; otherwise, <c>false</c>.</returns>
    bool Intersects(IInterval<T> other);
}
