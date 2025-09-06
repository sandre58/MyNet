// -----------------------------------------------------------------------
// <copyright file="DatePeriod.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities.Sequences;

namespace MyNet.Utilities.DateTimes;

/// <summary>
/// Represents a date interval between two <see cref="DateOnly"/> values (inclusive by consumer semantics).
/// </summary>
public class DatePeriod(DateOnly start, DateOnly end) : Interval<DateOnly, DatePeriod>(start, end)
{
    /// <summary>
    /// Gets the duration of the period expressed as a <see cref="TimeSpan"/>, using midnight as the time component.
    /// </summary>
    public TimeSpan Duration => End.At(TimeOnly.MinValue) - Start.At(TimeOnly.MinValue);

    /// <summary>
    /// Returns all calendar days that belong to this period.
    /// </summary>
    /// <returns>An enumerable of <see cref="DateOnly"/> values for each day in the period.</returns>
    public IEnumerable<DateOnly> ToDays() =>
        Enumerable.Range(0, End.At(TimeOnly.MinValue).Subtract(Start.At(TimeOnly.MinValue)).Days + 1).Select(Start.AddDays);

    /// <summary>
    /// Determines whether this period contains the current UTC date.
    /// </summary>
    /// <returns><c>true</c> if the current UTC date is within the period; otherwise <c>false</c>.</returns>
    public bool IsCurrent() => Start == DateTime.UtcNow.ToDate();

    /// <summary>
    /// Returns an immutable copy of this date period.
    /// </summary>
    /// <returns>An <see cref="ImmutableDatePeriod"/> representing the same interval.</returns>
    public ImmutableDatePeriod AsImmutable() => new(Start, End);

    protected override DatePeriod CreateInstance(DateOnly start, DateOnly end) => new(start, end);
}

/// <summary>
/// An immutable variant of <see cref="DatePeriod"/>. Attempts to change the interval throw an exception.
/// </summary>
public class ImmutableDatePeriod(DateOnly start, DateOnly end) : DatePeriod(start, end)
{
    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> because the instance is immutable.
    /// </summary>
    public override void SetInterval(DateOnly start, DateOnly end) => throw new InvalidOperationException("This period is immutable.");

    protected override DatePeriod CreateInstance(DateOnly start, DateOnly end) => new ImmutableDatePeriod(start, end);
}

/// <summary>
/// Represents a date interval with an optional end. When <see cref="End"/> is null the interval is considered open-ended.
/// </summary>
public class DatePeriodWithOptionalEnd(DateOnly start, DateOnly? end = null) : IntervalWithOptionalEnd<DateOnly>(start, end)
{
    /// <summary>
    /// Gets the nullable duration of the period as <see cref="TimeSpan"/>, or <c>null</c> if the end is not set.
    /// </summary>
    public TimeSpan? NullableDuration => End is null ? null : End.Value.At(TimeOnly.MinValue) - Start.At(TimeOnly.MinValue);

    /// <summary>
    /// Gets the duration of the period; if the end is not set, the duration is computed from the start to the current UTC date/time.
    /// </summary>
    public TimeSpan Duration => End is null ? DateTime.UtcNow - Start.At(TimeOnly.MinValue) : End.Value.At(TimeOnly.MinValue) - Start.At(TimeOnly.MinValue);
}
