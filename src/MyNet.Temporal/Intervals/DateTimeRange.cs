// -----------------------------------------------------------------------
// <copyright file="DateTimeRange.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

using MyNet.Primitives.Intervals;

namespace MyNet.Temporal.Intervals;

/// <summary>
/// Configurable civil <see cref="DateTime"/> interval. Defaults to half-open <c>[start, end)</c>.
/// Use <see cref="DateTimePeriod"/> when the half-open contract must be enforced; use this type when
/// inclusivity is caller-defined. <see cref="DateTime.Kind"/> is not normalized.
/// </summary>
/// <param name="start">Start bound.</param>
/// <param name="end">End bound.</param>
/// <param name="inclusiveStart">Whether start is inclusive (default true).</param>
/// <param name="inclusiveEnd">Whether end is inclusive (default false).</param>
public sealed class DateTimeRange(DateTime start, DateTime end, bool inclusiveStart = true, bool inclusiveEnd = false)
    : DateTimeInterval<DateTimeRange>(start, end, inclusiveStart, inclusiveEnd)
{
    /// <summary>
    /// Enumerates values from start to end using a fixed <paramref name="step"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="step"/> is not positive.</exception>
    public IEnumerable<DateTime> Enumerate(TimeSpan step)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(step, TimeSpan.Zero);

        var current = Start!.Value.Value;
        if (!Start.Value.IsInclusive)
        {
            current += step;
        }

        while (current < End!.Value.Value || (End.Value.IsInclusive && current == End.Value.Value))
        {
            yield return current;
            current += step;
        }
    }

    /// <inheritdoc />
    protected override DateTimeRange Create(IntervalBoundary<DateTime>? start, IntervalBoundary<DateTime>? end) =>
        !start.HasValue || !end.HasValue
            ? throw new InvalidOperationException("DateTimeRange must be bounded.")
            : new DateTimeRange(start.Value.Value, end.Value.Value, start.Value.IsInclusive, end.Value.IsInclusive);
}
