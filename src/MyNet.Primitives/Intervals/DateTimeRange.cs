// -----------------------------------------------------------------------
// <copyright file="DateTimeRange.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents a closed interval of date and time, defined by a start and an end DateTime. The interval can be configured to include or exclude the start and end boundaries, allowing for flexible definitions of time intervals. The class provides functionality to enumerate the DateTime values within the range based on a specified step, enabling iteration over the interval at regular intervals of time.
/// </summary>
/// <param name="start">The start DateTime of the interval.</param>
/// <param name="end">The end DateTime of the interval.</param>
/// <param name="inclusiveStart">Indicates whether the start DateTime is inclusive.</param>
/// <param name="inclusiveEnd">Indicates whether the end DateTime is inclusive.</param>
public sealed class DateTimeRange(DateTime start, DateTime end, bool inclusiveStart = true, bool inclusiveEnd = false) : TemporalInterval<DateTimeRange>(start, end, inclusiveStart, inclusiveEnd)
{
    /// <summary>
    /// Enumerates the DateTime values within the range at regular intervals defined by the specified step. The method takes a TimeSpan parameter that determines the interval between each enumerated DateTime value. The enumeration starts from the start DateTime and continues until it reaches the end DateTime, yielding each DateTime value at the specified step. If the step is less than or equal to zero, an ArgumentOutOfRangeException is thrown to ensure that the enumeration is valid and does not result in an infinite loop or invalid intervals.
    /// </summary>
    /// <param name="step">The interval between each enumerated DateTime value.</param>
    /// <returns>An enumerable collection of DateTime values within the range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the step is less than or equal to zero.</exception>
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
    protected override DateTimeRange Create(IntervalBoundary<DateTime>? start, IntervalBoundary<DateTime>? end)
        => new(start!.Value.Value, end!.Value.Value, start.Value.IsInclusive, end.Value.IsInclusive);
}
