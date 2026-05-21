// -----------------------------------------------------------------------
// <copyright file="DateRange.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Intervals;

/// <summary>
/// Represents a closed interval of dates, defined by a start and an end date. The interval includes both the start and end dates.
/// </summary>
/// <param name="start">The start date of the interval.</param>
/// <param name="end">The end date of the interval.</param>
public sealed class DateRange(DateOnly start, DateOnly end) : Interval<DateOnly, DateRange>(new IntervalBoundary<DateOnly>(start), new IntervalBoundary<DateOnly>(end)), IClosedInterval<DateOnly>
{
    /// <summary>
    /// Gets the number of days in the date range, including both the start and end dates.
    /// </summary>
    public int DayCount => End!.Value.Value.DayNumber - Start!.Value.Value.DayNumber + 1;

    /// <inheritdoc />
    protected override DateRange Create(IntervalBoundary<DateOnly>? start, IntervalBoundary<DateOnly>? end) =>
        !start.HasValue || !end.HasValue
            ? throw new InvalidOperationException("DateRange must be bounded.")
            : !start.Value.IsInclusive || !end.Value.IsInclusive
                ? throw new InvalidOperationException("DateRange only supports inclusive boundaries.")
                : new(start.Value.Value, end.Value.Value);

    /// <summary>
    /// Enumerates all the dates in the date range, starting from the start date and ending with the end date. Each date is returned as a DateOnly value.
    /// </summary>
    /// <returns>An enumerable collection of DateOnly values representing each day in the date range.</returns>
    public IEnumerable<DateOnly> EnumerateDays()
    {
        for (var date = Start!.Value.Value;
             date <= End!.Value.Value;
             date = date.AddDays(1))
        {
            yield return date;
        }
    }

    /// <summary>
    /// Gets the start and end boundaries of the date range as <see cref="IntervalBoundary{DateOnly}"/> values. The start boundary is inclusive, and the end boundary is inclusive as well, since this class represents a closed interval.
    /// </summary>
    IntervalBoundary<DateOnly> IBoundedInterval<DateOnly>.Start => Start!.Value;

    /// <summary>
    /// Gets the start and end boundaries of the date range as <see cref="IntervalBoundary{DateOnly}"/> values. The start boundary is inclusive, and the end boundary is inclusive as well, since this class represents a closed interval.
    /// </summary>
    IntervalBoundary<DateOnly> IBoundedInterval<DateOnly>.End => End!.Value;
}
