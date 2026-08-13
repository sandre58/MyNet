// -----------------------------------------------------------------------
// <copyright file="DateTimePeriod.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

using MyNet.Primitives.Intervals;

namespace MyNet.Temporal.Intervals;

/// <summary>
/// Half-open civil <see cref="DateTime"/> interval <c>[start, end)</c>: start inclusive, end exclusive.
/// Prefer this type for UI / desktop occupation spans. For absolute instants (APIs, multi-offset), use
/// <see cref="DateTimeOffsetPeriod"/> instead. <see cref="DateTime.Kind"/> is not normalized by constructors.
/// </summary>
/// <param name="start">Inclusive start.</param>
/// <param name="end">Exclusive end.</param>
public sealed class DateTimePeriod(DateTime start, DateTime end) : DateTimeInterval<DateTimePeriod>(start, end, inclusiveStart: true, inclusiveEnd: false)
{
    /// <summary>
    /// Creates a period from a start and a fixed <see cref="TimeSpan"/> duration.
    /// </summary>
    public static DateTimePeriod FromDuration(DateTime start, TimeSpan duration) => new(start, start + duration);

    /// <summary>
    /// Returns a new DateTimePeriod shifted by <paramref name="offset"/>.
    /// </summary>
    public DateTimePeriod Shift(TimeSpan offset) => new(Start!.Value.Value + offset, End!.Value.Value + offset);

    /// <summary>
    /// Returns a new DateTimePeriod with the end extended by <paramref name="offset"/>.
    /// </summary>
    public DateTimePeriod Extend(TimeSpan offset) => new(Start!.Value.Value, End!.Value.Value + offset);

    /// <summary>
    /// Converts bounds to <see cref="DateTimeOffset"/> with the given fixed offset.
    /// <see cref="DateTimeKind.Utc"/> / <see cref="DateTimeKind.Local"/> preserve the absolute instant then project to <paramref name="offset"/>.
    /// <see cref="DateTimeKind.Unspecified"/> is treated as civil wall time with that offset attached.
    /// </summary>
    public DateTimeOffsetPeriod ToOffsetPeriod(TimeSpan offset) =>
        new(ToOffsetBound(Start!.Value.Value, offset), ToOffsetBound(End!.Value.Value, offset));

    /// <summary>
    /// Converts bounds into <paramref name="timeZone"/> (DST-aware).
    /// Utc/Local preserve the absolute instant; Unspecified is treated as wall time in that zone.
    /// </summary>
    public DateTimeOffsetPeriod ToOffsetPeriod(TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        return new(ToOffsetBound(Start!.Value.Value, timeZone), ToOffsetBound(End!.Value.Value, timeZone));
    }

    /// <inheritdoc />
    protected override DateTimePeriod Create(IntervalBoundary<DateTime>? start, IntervalBoundary<DateTime>? end) =>
        !start.HasValue || !end.HasValue
            ? throw new InvalidOperationException("DateTimePeriod must be bounded.")
            : !start.Value.IsInclusive || end.Value.IsInclusive
                ? throw new InvalidOperationException("DateTimePeriod only supports [start, end) boundaries.")
                : new(start.Value.Value, end.Value.Value);

    private static DateTimeOffset ToOffsetBound(DateTime value, TimeSpan offset) => value.Kind switch
    {
        DateTimeKind.Utc or DateTimeKind.Local => new DateTimeOffset(value).ToOffset(offset),
        _ => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), offset)
    };

    private static DateTimeOffset ToOffsetBound(DateTime value, TimeZoneInfo timeZone) => value.Kind switch
    {
        DateTimeKind.Utc or DateTimeKind.Local => TimeZoneInfo.ConvertTime(new DateTimeOffset(value), timeZone),
        _ => new DateTimeOffset(
            DateTime.SpecifyKind(value, DateTimeKind.Unspecified),
            timeZone.GetUtcOffset(DateTime.SpecifyKind(value, DateTimeKind.Unspecified)))
    };
}
