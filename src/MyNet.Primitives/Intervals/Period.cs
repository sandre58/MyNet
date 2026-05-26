// -----------------------------------------------------------------------
// <copyright file="Period.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents a closed temporal interval where both the start and end boundaries are defined and are inclusive. This class extends <see cref="TemporalInterval{T}"/> and provides specific guarantees that both boundaries are inclusive, meaning that the interval includes its start and end DateTime values. The constructor of this class initializes the interval with the specified start and end DateTime values, and it sets the inclusiveStart and inclusiveEnd parameters to true and false respectively, ensuring that the interval is defined as a period that includes its start time but excludes its end time. This design allows for clear representation of time intervals where the end time is not considered part of the period, which can be useful in various scenarios such as scheduling or time-based calculations.
/// </summary>
/// <param name="start">The start DateTime of the interval.</param>
/// <param name="end">The end DateTime of the interval.</param>
public sealed class Period(DateTime start, DateTime end) : TemporalInterval<Period>(start, end, inclusiveStart: true, inclusiveEnd: false)
{
    /// <summary>
    /// Creates a new instance of the <see cref="Period"/> class based on a specified start DateTime and a duration represented as a TimeSpan. The method calculates the end DateTime by adding the duration to the start DateTime, effectively creating a period that starts at the given time and extends for the specified duration. This factory method provides a convenient way to create a period when you have a starting point and a length of time rather than an explicit end time.
    /// </summary>
    /// <param name="start">The start DateTime of the period.</param>
    /// <param name="duration">The duration of the period.</param>
    /// <returns>A new instance of the <see cref="Period"/> class.</returns>
    public static Period FromDuration(DateTime start, TimeSpan duration) => new(start, start + duration);

    /// <summary>
    /// Shifts the period by a specified TimeSpan offset, creating a new period that starts and ends at times that are offset from the original start and end times by the given duration. The method adds the offset to both the start and end DateTime values of the original period, effectively moving the entire period forward or backward in time depending on whether the offset is positive or negative. This allows for easy manipulation of periods when you need to adjust their position in time without changing their duration.
    /// </summary>
    /// <param name="offset">The TimeSpan offset to apply to the period.</param>
    /// <returns>A new instance of the <see cref="Period"/> class with the shifted start and end times.</returns>
    public Period Shift(TimeSpan offset) => new(Start!.Value.Value + offset, End!.Value.Value + offset);

    /// <summary>
    /// Extends the period by a specified TimeSpan offset, creating a new period that starts at the original start time and ends at a time that is offset from the original end time by the given duration. This allows for easy manipulation of periods when you need to adjust their duration without changing their start time.
    /// </summary>
    /// <param name="offset">The TimeSpan offset to apply to the end of the period.</param>
    /// <returns>A new instance of the <see cref="Period"/> class with the extended end time.</returns>
    public Period Extend(TimeSpan offset) => new(Start!.Value.Value, End!.Value.Value + offset);

    /// <inheritdoc />
    protected override Period Create(IntervalBoundary<DateTime>? start, IntervalBoundary<DateTime>? end) =>
        !start.HasValue || !end.HasValue
            ? throw new InvalidOperationException("Period must be bounded.")
            : !start.Value.IsInclusive || end.Value.IsInclusive
                ? throw new InvalidOperationException("Period only supports [start, end) boundaries.")
                : new(start.Value.Value, end.Value.Value);
}
