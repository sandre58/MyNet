// -----------------------------------------------------------------------
// <copyright file="Period.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Localization;
using MyNet.Utilities.Sequences;

namespace MyNet.Utilities.DateTimes;

/// <summary>
/// Represents a time interval between two <see cref="DateTime"/> values.
/// </summary>
public class Period(DateTime start, DateTime end) : Interval<DateTime, Period>(start, end)
{
    /// <summary>
    /// Gets the duration of the period as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Duration => End - Start;

    /// <summary>
    /// Returns each <see cref="DateTime"/> date (at midnight) that lies within the period.
    /// </summary>
    /// <returns>An enumerable of dates from Start to End inclusive.</returns>
    public IEnumerable<DateTime> ToDates() =>
        Enumerable.Range(0, End.Date.Subtract(Start.Date).Days + 1)
            .Select(offset => Start.Date.AddDays(offset));

    /// <summary>
    /// Splits the period into daily sub-periods, clipped to the original interval boundaries.
    /// </summary>
    /// <returns>An enumerable of <see cref="Period"/>, one per calendar day within the span.</returns>
    public IEnumerable<Period> ByDays() =>
        Enumerable.Range(0, End.Date.Subtract(Start.Date).Days + 1)
            .Select(offset => Start.Date.AddDays(offset))
            .Select(x => new Period(DateTimeHelper.Max(x.BeginningOfDay(), Start), DateTimeHelper.Min(x.EndOfDay(), End)));

    /// <summary>
    /// Determines whether the period contains the current date/time depending on the Start.Kind.
    /// </summary>
    /// <returns><c>true</c> if the current time is inside the period; otherwise <c>false</c>.</returns>
    public bool IsCurrent() => Start.Kind switch
    {
        DateTimeKind.Utc => Contains(DateTime.UtcNow),
        DateTimeKind.Local => Contains(DateTime.Now),
        _ => Contains(GlobalizationService.Current.Date)
    };

    /// <summary>
    /// Returns a new <see cref="Period"/> converted to UTC.
    /// </summary>
    public Period ToUtc() => new(Start.ToUniversalTime(), End.ToUniversalTime());

    /// <summary>
    /// Returns a new <see cref="Period"/> converted to local time.
    /// </summary>
    public Period ToLocal() => new(Start.ToLocalTime(), End.ToLocalTime());

    /// <summary>
    /// Returns a new period with the End moved after by the specified offset.
    /// </summary>
    public Period AddAfter(TimeSpan offset) => new(Start, End.AddFluentTimeSpan(offset));

    /// <summary>
    /// Returns a new period with the Start moved before by the specified offset.
    /// </summary>
    public Period AddBefore(TimeSpan offset) => new(Start.AddFluentTimeSpan(offset), End);

    /// <summary>
    /// Returns a new period with the Start unchanged and the End decreased by the offset.
    /// </summary>
    public Period SubstractBefore(TimeSpan offset) => new(Start, End.SubtractFluentTimeSpan(offset));

    /// <summary>
    /// Returns a new period with the Start decreased by the offset and the End unchanged.
    /// </summary>
    public Period SubstractAfter(TimeSpan offset) => new(Start.SubtractFluentTimeSpan(offset), End);

    /// <summary>
    /// Shifts the period later by the specified offset.
    /// </summary>
    public Period ShiftLater(TimeSpan offset) => new(Start.AddFluentTimeSpan(offset), End.AddFluentTimeSpan(offset));

    /// <summary>
    /// Shifts the period earlier by the specified offset.
    /// </summary>
    public Period ShiftEarlier(TimeSpan offset) => new(Start.SubtractFluentTimeSpan(offset), End.SubtractFluentTimeSpan(offset));

    /// <summary>
    /// Intersects this period with a <see cref="TimePeriod"/>, returning the daily intersections.
    /// </summary>
    /// <param name="interval">The time interval to intersect with.</param>
    /// <returns>An enumerable of <see cref="Period"/> representing intersections.</returns>
    public IEnumerable<Period> Intersect(TimePeriod interval)
        => ByDays().Select(x => x.Intersect(new Period(x.Start.At(interval.Start), x.Start.At(interval.End)))).NotNull();

    /// <summary>
    /// Returns an immutable copy of this period.
    /// </summary>
    public ImmutablePeriod AsImmutable() => new(Start, End);

    protected override Period CreateInstance(DateTime start, DateTime end) => new(start, end);
}

/// <summary>
/// Immutable variant of <see cref="Period"/>. Attempts to modify the interval will throw an exception.
/// </summary>
public class ImmutablePeriod(DateTime start, DateTime end) : Period(start, end)
{
    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> because instances are immutable.
    /// </summary>
    public override void SetInterval(DateTime start, DateTime end) => throw new InvalidOperationException("This period is immutable.");

    protected override Period CreateInstance(DateTime start, DateTime end) => new ImmutablePeriod(start, end);
}

/// <summary>
/// Represents a period with an optional end. If End is null the period is considered open-ended.
/// </summary>
public class PeriodWithOptionalEnd(DateTime start, DateTime? end = null) : IntervalWithOptionalEnd<DateTime>(start, end)
{
    /// <summary>
    /// Gets the duration as a nullable <see cref="TimeSpan"/>, or null when the End is not set.
    /// </summary>
    public TimeSpan? NullableDuration => End is null ? null : End.Value.ToUniversalTime() - Start.ToUniversalTime();

    /// <summary>
    /// Gets the duration; when End is not set the duration is computed from Start to now (UTC).
    /// </summary>
    public TimeSpan Duration => End is null ? DateTime.UtcNow - Start.ToUniversalTime() : End.Value.ToUniversalTime() - Start.ToUniversalTime();
}
