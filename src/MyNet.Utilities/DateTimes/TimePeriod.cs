// -----------------------------------------------------------------------
// <copyright file="TimePeriod.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Sequences;

namespace MyNet.Utilities.DateTimes;

/// <summary>
/// Represents a time interval between two <see cref="TimeOnly"/> values.
/// </summary>
public class TimePeriod(TimeOnly start, TimeOnly end) : Interval<TimeOnly, TimePeriod>(start, end)
{
    /// <summary>
    /// Gets the duration of the period as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Duration => End - Start;

    /// <summary>
    /// Determines whether the current UTC time (time-of-day) is contained in the period.
    /// </summary>
    /// <returns><c>true</c> if the current UTC time-of-day is within the period; otherwise <c>false</c>.</returns>
    public bool IsCurrent() => Contains(DateTime.UtcNow.ToTime());

    /// <summary>
    /// Returns an immutable copy of this time period.
    /// </summary>
    /// <returns>An <see cref="ImmutableTimePeriod"/> representing the same interval.</returns>
    public ImmutableTimePeriod AsImmutable() => new(Start, End);

    protected override TimePeriod CreateInstance(TimeOnly start, TimeOnly end) => new(start, end);
}

/// <summary>
/// Immutable variant of <see cref="TimePeriod"/>. Attempts to change the interval will throw an exception.
/// </summary>
public class ImmutableTimePeriod(TimeOnly start, TimeOnly end) : TimePeriod(start, end)
{
    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> because the instance is immutable.
    /// </summary>
    public override void SetInterval(TimeOnly start, TimeOnly end) => throw new InvalidOperationException("This period is immutable.");

    protected override TimePeriod CreateInstance(TimeOnly start, TimeOnly end) => new ImmutableTimePeriod(start, end);
}

/// <summary>
/// Represents a time interval with an optional end. If <see cref="End"/> is null, the period is considered open-ended.
/// </summary>
public class TimePeriodWithOptionalEnd(TimeOnly start, TimeOnly? end = null) : IntervalWithOptionalEnd<TimeOnly>(start, end)
{
    /// <summary>
    /// Gets the nullable duration of the period; returns <c>null</c> when the end is not set.
    /// </summary>
    public TimeSpan? NullableDuration => End is null ? null : End.Value - Start;

    /// <summary>
    /// Gets the duration of the period. When <see cref="End"/> is not set, the duration is computed from the start to the current UTC time-of-day.
    /// </summary>
    public TimeSpan Duration => End is null ? DateTime.UtcNow.ToTime() - Start : End.Value - Start;
}
