// -----------------------------------------------------------------------
// <copyright file="TimeRange.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents a closed interval of time, defined by a start and an end time. The interval includes both the start and end times. It also provides properties to calculate the duration of the time range and to determine if the time range crosses midnight.
/// </summary>
/// <param name="start">The start time of the interval.</param>
/// <param name="end">The end time of the interval.</param>
public sealed class TimeRange(TimeOnly start, TimeOnly end) : Interval<TimeOnly, TimeRange>(new IntervalBoundary<TimeOnly>(start), new IntervalBoundary<TimeOnly>(end), allowWrappedBounds: true)
{
    /// <summary>
    /// Gets the duration of the time range as a TimeSpan value, calculated as the difference between the end time and the start time. If the time range crosses midnight, the duration is calculated accordingly to account for the wrap-around at midnight.
    /// </summary>
    public TimeSpan Duration =>
        !CrossesMidnight
            ? End!.Value.Value - Start!.Value.Value
            : TimeSpan.FromDays(1) - Start!.Value.Value.ToTimeSpan() + End!.Value.Value.ToTimeSpan();

    /// <summary>
    /// Gets a value indicating whether the time range crosses midnight. A time range is considered to cross midnight if the end time is earlier than the start time, which means that the time range extends past midnight and into the next day.
    /// </summary>
    public bool CrossesMidnight => End!.Value.Value < Start!.Value.Value;

    /// <summary>
    /// Determines whether a given TimeOnly value falls within the time range. If the time range does not cross midnight, the method checks if the value is greater than or equal to the start time and less than the end time. If the time range crosses midnight, the method checks if the value is greater than or equal to the start time or less than the end time, since the valid times are split across midnight.
    /// </summary>
    /// <param name="value">The TimeOnly value to check.</param>
    /// <returns><c>true</c> if the value falls within the time range; otherwise, <c>false</c>.</returns>
    public override bool Contains(TimeOnly value)
        => !CrossesMidnight ? base.Contains(value) : value >= Start!.Value.Value || value <= End!.Value.Value;

    /// <inheritdoc />
    protected override TimeRange Create(IntervalBoundary<TimeOnly>? start, IntervalBoundary<TimeOnly>? end) => !start.HasValue || !end.HasValue
        ? throw new InvalidOperationException("TimeRange must be bounded.")
        : !start.Value.IsInclusive || !end.Value.IsInclusive
            ? throw new InvalidOperationException("TimeRange only supports inclusive boundaries.")
            : new(start.Value.Value, end.Value.Value);
}
