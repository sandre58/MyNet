// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetPeriod.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

using MyNet.Primitives.Intervals;

namespace MyNet.Temporal.Intervals;

/// <summary>
/// Half-open absolute <see cref="DateTimeOffset"/> interval <c>[start, end)</c>: start inclusive, end exclusive.
/// Prefer this type for API / domain occupation spans across offsets. For civil UI time, use
/// <see cref="DateTimePeriod"/> instead.
/// </summary>
/// <param name="start">Inclusive start.</param>
/// <param name="end">Exclusive end.</param>
public sealed class DateTimeOffsetPeriod(DateTimeOffset start, DateTimeOffset end)
    : DateTimeOffsetInterval<DateTimeOffsetPeriod>(start, end, inclusiveStart: true, inclusiveEnd: false)
{
    /// <summary>
    /// Creates a period from a start and a fixed <see cref="TimeSpan"/> duration.
    /// </summary>
    public static DateTimeOffsetPeriod FromDuration(DateTimeOffset start, TimeSpan duration) => new(start, start + duration);

    /// <summary>
    /// Returns a new DateTimeOffsetPeriod shifted by <paramref name="offset"/>.
    /// </summary>
    public DateTimeOffsetPeriod Shift(TimeSpan offset) => new(Start!.Value.Value + offset, End!.Value.Value + offset);

    /// <summary>
    /// Returns a new DateTimeOffsetPeriod with the end extended by <paramref name="offset"/>.
    /// </summary>
    public DateTimeOffsetPeriod Extend(TimeSpan offset) => new(Start!.Value.Value, End!.Value.Value + offset);

    /// <summary>
    /// Preserves wall-clock components as <see cref="DateTimeKind.Unspecified"/> (civil / UI).
    /// Use <see cref="ToUtcDateTimePeriod"/> when you need absolute UTC clock faces.
    /// </summary>
    public DateTimePeriod ToDateTimePeriod() =>
        new(
            DateTime.SpecifyKind(Start!.Value.Value.DateTime, DateTimeKind.Unspecified),
            DateTime.SpecifyKind(End!.Value.Value.DateTime, DateTimeKind.Unspecified));

    /// <summary>
    /// Converts bounds via <see cref="DateTimeOffset.UtcDateTime"/> (<see cref="DateTimeKind.Utc"/>).
    /// </summary>
    public DateTimePeriod ToUtcDateTimePeriod() =>
        new(Start!.Value.Value.UtcDateTime, End!.Value.Value.UtcDateTime);

    /// <inheritdoc />
    protected override DateTimeOffsetPeriod Create(IntervalBoundary<DateTimeOffset>? start, IntervalBoundary<DateTimeOffset>? end) =>
        !start.HasValue || !end.HasValue
            ? throw new InvalidOperationException("DateTimeOffsetPeriod must be bounded.")
            : !start.Value.IsInclusive || end.Value.IsInclusive
                ? throw new InvalidOperationException("DateTimeOffsetPeriod only supports [start, end) boundaries.")
                : new DateTimeOffsetPeriod(start.Value.Value, end.Value.Value);
}
