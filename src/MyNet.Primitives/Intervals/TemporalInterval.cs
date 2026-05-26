// -----------------------------------------------------------------------
// <copyright file="TemporalInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents a temporal interval defined by optional start and end DateTime boundaries. This class extends the generic <see cref="Interval{T, TSelf}"/> class, specializing it for DateTime values. The constructor allows for specifying whether the start and end boundaries are inclusive or exclusive, providing flexibility in how the interval is defined. The class also includes properties and methods to calculate the duration of the interval and to determine if a given DateTime value is current, past, or future relative to the interval. This makes it a useful tool for managing time-based intervals in various applications, such as scheduling, event management, or time-based data analysis.
/// </summary>
/// <param name="start">The start DateTime of the interval.</param>
/// <param name="end">The end DateTime of the interval.</param>
/// <param name="inclusiveStart">Indicates whether the start boundary is inclusive.</param>
/// <param name="inclusiveEnd">Indicates whether the end boundary is inclusive.</param>
public abstract class TemporalInterval<TSelf>(DateTime? start, DateTime? end, bool inclusiveStart = true, bool inclusiveEnd = false)
    : Interval<DateTime, TSelf>(start.HasValue ? new IntervalBoundary<DateTime>(start.Value, inclusiveStart) : null,
        end.HasValue ? new IntervalBoundary<DateTime>(end.Value, inclusiveEnd) : null), ITemporalInterval
    where TSelf : TemporalInterval<TSelf>
{
    /// <summary>
    /// Gets the duration of the interval as a TimeSpan. If either the start or end boundary is not defined, this property returns null, indicating that the duration cannot be calculated due to the unbounded nature of the interval on one or both sides. If both boundaries are defined, it calculates the duration by subtracting the start value from the end value, providing a TimeSpan that represents the length of time covered by the interval.
    /// </summary>
    public TimeSpan? Duration => !HasStart || !HasEnd ? null : End!.Value.Value - Start!.Value.Value;

    /// <summary>
    /// Gets the duration of the interval as a TimeSpan. If the duration cannot be calculated because either the start or end boundary is not defined, this property returns TimeSpan.MaxValue, indicating that the interval is effectively unbounded in terms of duration. This allows for a consistent return type while still conveying the concept of an infinite duration when one or both boundaries are missing.
    /// </summary>
    TimeSpan ITemporalInterval.Duration => Duration ?? TimeSpan.MaxValue;

    /// <summary>
    /// Determines whether the specified DateTime value falls within the interval. This method checks if the value is greater than or equal to the start boundary (if it exists) and less than or equal to the end boundary (if it exists), taking into account whether the boundaries are inclusive or exclusive. If the value satisfies these conditions, it returns true, indicating that the value is current with respect to the interval; otherwise, it returns false.
    /// </summary>
    /// <param name="now">The DateTime value to check.</param>
    /// <returns>True if the value is within the interval; otherwise, false.</returns>
    public bool IsCurrent(DateTime now) => Contains(now);

    /// <summary>
    /// Determines whether the specified DateTime value is in the past relative to the interval. This method checks if the value is greater than the end boundary (if it exists), indicating that the value is past the interval.
    /// </summary>
    /// <param name="now">The DateTime value to check.</param>
    /// <returns>True if the value is in the past relative to the interval; otherwise, false.</returns>
    public bool IsPast(DateTime now)
    {
        if (!HasEnd)
            return false;

        var comparison = End!.Value.Value.CompareTo(now);

        return comparison < 0 || (comparison == 0 && !End.Value.IsInclusive);
    }

    /// <summary>
    /// Determines whether the specified DateTime value is in the future relative to the interval. This method checks if the value is less than the start boundary (if it exists), indicating that the value is future relative to the interval.
    /// </summary>
    /// <param name="now">The DateTime value to check.</param>
    /// <returns>True if the value is in the future relative to the interval; otherwise, false.</returns>
    public bool IsFuture(DateTime now)
    {
        if (!HasStart)
            return false;

        var comparison = Start!.Value.Value.CompareTo(now);

        return comparison > 0 || (comparison == 0 && !Start.Value.IsInclusive);
    }
}
