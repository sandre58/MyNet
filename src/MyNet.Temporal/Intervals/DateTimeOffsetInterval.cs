// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

using MyNet.Primitives.Intervals;

namespace MyNet.Temporal.Intervals;

/// <summary>
/// Base class for intervals of <see cref="DateTimeOffset"/> (absolute instants with offset).
/// Default boundary policy is half-open <c>[start, end)</c> when subclasses do not override inclusivity.
/// Civil / UI time without a reliable offset should use <see cref="DateTimeInterval{TSelf}"/> instead.
/// </summary>
/// <typeparam name="TSelf">The concrete interval type.</typeparam>
/// <param name="start">Optional start instant.</param>
/// <param name="end">Optional end instant.</param>
/// <param name="inclusiveStart">Whether the start bound is inclusive.</param>
/// <param name="inclusiveEnd">Whether the end bound is inclusive.</param>
public abstract class DateTimeOffsetInterval<TSelf>(DateTimeOffset? start, DateTimeOffset? end, bool inclusiveStart = true, bool inclusiveEnd = false)
    : Interval<DateTimeOffset, TSelf>(start.HasValue ? new IntervalBoundary<DateTimeOffset>(start.Value, inclusiveStart) : null,
        end.HasValue ? new IntervalBoundary<DateTimeOffset>(end.Value, inclusiveEnd) : null), IDateTimeOffsetInterval
    where TSelf : DateTimeOffsetInterval<TSelf>
{
    /// <inheritdoc />
    public TimeSpan? Duration => !HasStart || !HasEnd ? null : End!.Value.Value - Start!.Value.Value;

    /// <summary>
    /// Returns whether <paramref name="now"/> lies inside this interval (respecting inclusivity).
    /// </summary>
    public bool IsCurrent(DateTimeOffset now) => Contains(now);

    /// <summary>
    /// Returns whether <paramref name="now"/> is strictly after this interval.
    /// </summary>
    public bool IsPast(DateTimeOffset now)
    {
        if (!HasEnd)
            return false;

        var comparison = End!.Value.Value.CompareTo(now);

        return comparison < 0 || (comparison == 0 && !End.Value.IsInclusive);
    }

    /// <summary>
    /// Returns whether <paramref name="now"/> is strictly before this interval.
    /// </summary>
    public bool IsFuture(DateTimeOffset now)
    {
        if (!HasStart)
            return false;

        var comparison = Start!.Value.Value.CompareTo(now);

        return comparison > 0 || (comparison == 0 && !Start.Value.IsInclusive);
    }
}
