// -----------------------------------------------------------------------
// <copyright file="DateTimeInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

using MyNet.Primitives.Intervals;

namespace MyNet.Temporal.Intervals;

/// <summary>
/// Base class for intervals of <see cref="DateTime"/> (civil / UI time with explicit <see cref="DateTime.Kind"/>).
/// Default boundary policy is half-open <c>[start, end)</c> when subclasses do not override inclusivity.
/// Absolute (API / domain) instants should use <c>DateTimeOffset</c>-based interval types instead.
/// </summary>
/// <typeparam name="TSelf">The concrete interval type.</typeparam>
/// <param name="start">Optional start instant.</param>
/// <param name="end">Optional end instant.</param>
/// <param name="inclusiveStart">Whether the start bound is inclusive.</param>
/// <param name="inclusiveEnd">Whether the end bound is inclusive.</param>
public abstract class DateTimeInterval<TSelf>(DateTime? start, DateTime? end, bool inclusiveStart = true, bool inclusiveEnd = false)
    : Interval<DateTime, TSelf>(start.HasValue ? new IntervalBoundary<DateTime>(start.Value, inclusiveStart) : null,
        end.HasValue ? new IntervalBoundary<DateTime>(end.Value, inclusiveEnd) : null), IDateTimeInterval
    where TSelf : DateTimeInterval<TSelf>
{
    /// <inheritdoc />
    public TimeSpan? Duration => !HasStart || !HasEnd ? null : End!.Value.Value - Start!.Value.Value;

    /// <summary>
    /// Returns whether <paramref name="now"/> lies inside this interval (respecting inclusivity).
    /// </summary>
    public bool IsCurrent(DateTime now) => Contains(now);

    /// <summary>
    /// Returns whether <paramref name="now"/> is strictly after this interval.
    /// </summary>
    public bool IsPast(DateTime now)
    {
        if (!HasEnd)
            return false;

        var comparison = End!.Value.Value.CompareTo(now);

        return comparison < 0 || (comparison == 0 && !End.Value.IsInclusive);
    }

    /// <summary>
    /// Returns whether <paramref name="now"/> is strictly before this interval.
    /// </summary>
    public bool IsFuture(DateTime now)
    {
        if (!HasStart)
            return false;

        var comparison = Start!.Value.Value.CompareTo(now);

        return comparison > 0 || (comparison == 0 && !Start.Value.IsInclusive);
    }
}
