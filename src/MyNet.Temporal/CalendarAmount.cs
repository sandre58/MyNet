// -----------------------------------------------------------------------
// <copyright file="CalendarAmount.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace MyNet.Temporal;

/// <summary>
/// Represents a calendar amount composed of years and/or months plus a fixed <see cref="TimeSpan"/>.
/// This is not a universal duration: calendar components must be applied via <c>AddYears</c>/<c>AddMonths</c>
/// (for example through <c>AddCalendarAmount</c>). Use <see cref="ToApproximateTimeSpan"/> only when an
/// approximate fixed duration is acceptable (365 days/year, 30 days/month).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct CalendarAmount :
    IEquatable<CalendarAmount>,
    IComparable<TimeSpan>,
    IComparable<CalendarAmount>
{
    private const int DaysPerYear = 365;
    private const int DaysPerMonth = 30;

    /// <summary>
    /// Gets the number of months component of the calendar amount.
    /// </summary>
    public int Months { get; init; }

    /// <summary>
    /// Gets the number of years component of the calendar amount.
    /// </summary>
    public int Years { get; init; }

    /// <summary>
    /// Gets the fixed <see cref="TimeSpan"/> component of the calendar amount (days, hours, minutes, etc.).
    /// </summary>
    public TimeSpan TimeSpan { get; init; }

    /// <summary>
    /// Gets the approximate number of ticks, using <see cref="ToApproximateTimeSpan"/>.
    /// </summary>
    public long Ticks => ToApproximateTimeSpan().Ticks;

    /// <summary>
    /// Gets the days portion of the approximate <see cref="TimeSpan"/>.
    /// </summary>
    public int Days => ToApproximateTimeSpan().Days;

    /// <summary>
    /// Gets the hours portion of the approximate <see cref="TimeSpan"/>.
    /// </summary>
    public int Hours => ToApproximateTimeSpan().Hours;

    /// <summary>
    /// Gets the milliseconds portion of the approximate <see cref="TimeSpan"/>.
    /// </summary>
    public int Milliseconds => ToApproximateTimeSpan().Milliseconds;

    /// <summary>
    /// Gets the minutes portion of the approximate <see cref="TimeSpan"/>.
    /// </summary>
    public int Minutes => ToApproximateTimeSpan().Minutes;

    /// <summary>
    /// Gets the seconds portion of the approximate <see cref="TimeSpan"/>.
    /// </summary>
    public int Seconds => ToApproximateTimeSpan().Seconds;

    /// <summary>
    /// Gets the approximate total days, using <see cref="ToApproximateTimeSpan"/>.
    /// </summary>
    public double TotalDays => ToApproximateTimeSpan().TotalDays;

    /// <summary>
    /// Gets the approximate total hours, using <see cref="ToApproximateTimeSpan"/>.
    /// </summary>
    public double TotalHours => ToApproximateTimeSpan().TotalHours;

    /// <summary>
    /// Gets the approximate total milliseconds, using <see cref="ToApproximateTimeSpan"/>.
    /// </summary>
    public double TotalMilliseconds => ToApproximateTimeSpan().TotalMilliseconds;

    /// <summary>
    /// Gets the approximate total minutes, using <see cref="ToApproximateTimeSpan"/>.
    /// </summary>
    public double TotalMinutes => ToApproximateTimeSpan().TotalMinutes;

    /// <summary>
    /// Gets the approximate total seconds, using <see cref="ToApproximateTimeSpan"/>.
    /// </summary>
    public double TotalSeconds => ToApproximateTimeSpan().TotalSeconds;

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a <see cref="CalendarAmount"/> with only the <see cref="TimeSpan"/> component set.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to convert.</param>
    public static implicit operator CalendarAmount(TimeSpan timeSpan) => FromTimeSpan(timeSpan);

    /// <summary>
    /// Adds two <see cref="CalendarAmount"/> instances.
    /// </summary>
    public static CalendarAmount operator +(CalendarAmount left, CalendarAmount right) => AddInternal(left, right);

    /// <summary>
    /// Adds a <see cref="TimeSpan"/> to a <see cref="CalendarAmount"/>.
    /// </summary>
    public static CalendarAmount operator +(CalendarAmount left, TimeSpan right) => AddInternal(left, right);

    /// <summary>
    /// Adds a <see cref="TimeSpan"/> and a <see cref="CalendarAmount"/>.
    /// </summary>
    public static CalendarAmount operator +(TimeSpan left, CalendarAmount right) => AddInternal(left, right);

    /// <summary>
    /// Subtracts two <see cref="CalendarAmount"/> instances.
    /// </summary>
    public static CalendarAmount operator -(CalendarAmount left, CalendarAmount right) => SubtractInternal(left, right);

    /// <summary>
    /// Subtracts a <see cref="CalendarAmount"/> from a <see cref="TimeSpan"/>.
    /// </summary>
    public static CalendarAmount operator -(TimeSpan left, CalendarAmount right) => SubtractInternal(left, right);

    /// <summary>
    /// Subtracts a <see cref="TimeSpan"/> from a <see cref="CalendarAmount"/>.
    /// </summary>
    public static CalendarAmount operator -(CalendarAmount left, TimeSpan right) => SubtractInternal(left, right);

    /// <summary>
    /// Determines whether two <see cref="CalendarAmount"/> instances are equal by comparing years, months, and <see cref="TimeSpan"/> components.
    /// </summary>
    public static bool operator ==(CalendarAmount left, CalendarAmount right) => left.Years == right.Years &&
        left.Months == right.Months &&
        left.TimeSpan == right.TimeSpan;

    /// <summary>
    /// Determines whether a <see cref="TimeSpan"/> and a <see cref="CalendarAmount"/> are equal.
    /// Equal only when the calendar amount has zero years and months and its <see cref="TimeSpan"/> equals <paramref name="left"/>.
    /// </summary>
    public static bool operator ==(TimeSpan left, CalendarAmount right) => right.Years == 0 && right.Months == 0 && right.TimeSpan == left;

    /// <summary>
    /// Determines whether a <see cref="CalendarAmount"/> and a <see cref="TimeSpan"/> are equal.
    /// Equal only when the calendar amount has zero years and months and its <see cref="TimeSpan"/> equals <paramref name="right"/>.
    /// </summary>
    public static bool operator ==(CalendarAmount left, TimeSpan right) => left.Years == 0 && left.Months == 0 && left.TimeSpan == right;

    /// <summary>
    /// Determines whether two <see cref="CalendarAmount"/> instances are not equal.
    /// </summary>
    public static bool operator !=(CalendarAmount left, CalendarAmount right) => !(left == right);

    /// <summary>
    /// Determines whether a <see cref="TimeSpan"/> and a <see cref="CalendarAmount"/> are not equal.
    /// </summary>
    public static bool operator !=(TimeSpan left, CalendarAmount right) => !(left == right);

    /// <summary>
    /// Determines whether a <see cref="CalendarAmount"/> and a <see cref="TimeSpan"/> are not equal.
    /// </summary>
    public static bool operator !=(CalendarAmount left, TimeSpan right) => !(left == right);

    /// <summary>
    /// Returns the negation of the specified calendar amount.
    /// </summary>
    public static CalendarAmount operator -(CalendarAmount value) => value.Negate();

    /// <summary>
    /// Less-than comparison using structural ordering (years, then months, then <see cref="TimeSpan"/>).
    /// </summary>
    public static bool operator <(CalendarAmount left, CalendarAmount right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Less-than comparison. When years/months are zero, compares the fixed component; otherwise compares approximate durations.
    /// </summary>
    public static bool operator <(CalendarAmount left, TimeSpan right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Less-than comparison. When years/months are zero, compares the fixed component; otherwise compares approximate durations.
    /// </summary>
    public static bool operator <(TimeSpan left, CalendarAmount right) => right.CompareTo(left) > 0;

    /// <summary>
    /// Less-than-or-equal comparison using structural ordering (years, then months, then <see cref="TimeSpan"/>).
    /// </summary>
    public static bool operator <=(CalendarAmount left, CalendarAmount right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Less-than-or-equal comparison against a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator <=(CalendarAmount left, TimeSpan right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Less-than-or-equal comparison against a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator <=(TimeSpan left, CalendarAmount right) => right.CompareTo(left) >= 0;

    /// <summary>
    /// Greater-than comparison using structural ordering (years, then months, then <see cref="TimeSpan"/>).
    /// </summary>
    public static bool operator >(CalendarAmount left, CalendarAmount right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Greater-than comparison against a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator >(CalendarAmount left, TimeSpan right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Greater-than comparison against a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator >(TimeSpan left, CalendarAmount right) => right.CompareTo(left) < 0;

    /// <summary>
    /// Greater-than-or-equal comparison using structural ordering (years, then months, then <see cref="TimeSpan"/>).
    /// </summary>
    public static bool operator >=(CalendarAmount left, CalendarAmount right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Greater-than-or-equal comparison against a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator >=(CalendarAmount left, TimeSpan right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Greater-than-or-equal comparison against a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator >=(TimeSpan left, CalendarAmount right) => right.CompareTo(left) <= 0;

    /// <summary>
    /// Creates a <see cref="CalendarAmount"/> from a <see cref="TimeSpan"/> (no years/months component).
    /// </summary>
    /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert.</param>
    /// <returns>A calendar amount representing the same fixed duration.</returns>
    public static CalendarAmount FromTimeSpan(TimeSpan timeSpan) => new() { TimeSpan = timeSpan };

    /// <summary>
    /// Converts this calendar amount into an approximate <see cref="TimeSpan"/>
    /// (months = 30 days, years = 365 days). Prefer calendar-aware application via AddYears/AddMonths when possible.
    /// </summary>
    /// <returns>The approximate <see cref="TimeSpan"/>.</returns>
    public TimeSpan ToApproximateTimeSpan()
    {
        var daysFromYears = DaysPerYear * Years;
        var daysFromMonths = DaysPerMonth * Months;
        var days = daysFromMonths + daysFromYears;
        return new TimeSpan(days, 0, 0, 0) + TimeSpan;
    }

    /// <summary>
    /// Adds another calendar amount to this instance.
    /// </summary>
    /// <param name="number">The calendar amount to add.</param>
    public CalendarAmount Add(CalendarAmount number) => AddInternal(this, number);

    /// <summary>
    /// Adds a <see cref="TimeSpan"/> to this calendar amount.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to add.</param>
    public CalendarAmount Add(TimeSpan timeSpan) => AddInternal(this, timeSpan);

    /// <summary>
    /// Subtracts the specified calendar amount from this instance.
    /// </summary>
    /// <param name="calendarAmount">The calendar amount to subtract.</param>
    public CalendarAmount Subtract(CalendarAmount calendarAmount) => SubtractInternal(this, calendarAmount);

    /// <summary>
    /// Subtracts the specified <see cref="TimeSpan"/> from this calendar amount.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to subtract.</param>
    public CalendarAmount Subtract(TimeSpan timeSpan) => SubtractInternal(this, timeSpan);

    /// <summary>
    /// Compares this calendar amount to a <see cref="TimeSpan"/>.
    /// When years and months are zero, compares the fixed component exactly; otherwise uses <see cref="ToApproximateTimeSpan"/>.
    /// </summary>
    public int CompareTo(TimeSpan other) =>
        Years == 0 && Months == 0
            ? TimeSpan.CompareTo(other)
            : ToApproximateTimeSpan().CompareTo(other);

    /// <summary>
    /// Compares this calendar amount to an object that must be a <see cref="TimeSpan"/> or <see cref="CalendarAmount"/>.
    /// </summary>
    public int CompareTo(object value) => value switch
    {
        TimeSpan timeSpan => CompareTo(timeSpan),
        CalendarAmount amount => CompareTo(amount),
        _ => throw new ArgumentException("Value must be a TimeSpan or CalendarAmount", nameof(value))
    };

    /// <summary>
    /// Compares this calendar amount to another using structural ordering: years, then months, then <see cref="TimeSpan"/>.
    /// </summary>
    public int CompareTo(CalendarAmount other)
    {
        var years = Years.CompareTo(other.Years);
        if (years != 0)
            return years;

        var months = Months.CompareTo(other.Months);
        return months != 0 ? months : TimeSpan.CompareTo(other.TimeSpan);
    }

    /// <summary>
    /// Returns a new calendar amount that represents the negation of this instance.
    /// </summary>
    public CalendarAmount Negate() => new()
    {
        TimeSpan = -TimeSpan,
        Months = -Months,
        Years = -Years
    };

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A copy of this instance.</returns>
    public object Clone() => new CalendarAmount
    {
        TimeSpan = TimeSpan,
        Months = Months,
        Years = Years
    };

    /// <inheritdoc />
    public override string ToString() => ToApproximateTimeSpan().ToString();

    /// <summary>
    /// Determines whether the specified calendar amount is equal to the current instance.
    /// </summary>
    public bool Equals(CalendarAmount other) => this == other;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        var type = obj.GetType();
        return type == typeof(CalendarAmount) ? this == (CalendarAmount)obj : type == typeof(TimeSpan) && this == (TimeSpan)obj;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Months.GetHashCode() ^ Years.GetHashCode() ^ TimeSpan.GetHashCode();

    /// <summary>
    /// Subtracts a <see cref="CalendarAmount"/> from a <see cref="TimeSpan"/> by negating the months and years components and subtracting the TimeSpan components.
    /// </summary>
    /// <param name="left">The TimeSpan from which to subtract.</param>
    /// <param name="right">The CalendarAmount to subtract.</param>
    /// <returns>A new CalendarAmount representing the result of the subtraction.</returns>
    internal static CalendarAmount SubtractInternal(TimeSpan left, CalendarAmount right) => new()
    {
        Months = -right.Months,
        Years = -right.Years,
        TimeSpan = left - right.TimeSpan
    };

    /// <summary>
    /// Adds a <see cref="TimeSpan"/> to a <see cref="CalendarAmount"/> by adding the TimeSpan components and keeping the months and years components unchanged.
    /// </summary>
    /// <param name="left">The CalendarAmount to which to add.</param>
    /// <param name="right">The TimeSpan to add.</param>
    /// <returns>A new CalendarAmount representing the result of the addition.</returns>
    private static CalendarAmount AddInternal(CalendarAmount left, TimeSpan right) => left with { TimeSpan = left.TimeSpan + right };

    /// <summary>
    /// Adds two <see cref="CalendarAmount"/> instances by adding their months, years, and TimeSpan components together.
    /// </summary>
    /// <param name="left">The first CalendarAmount to add.</param>
    /// <param name="right">The second CalendarAmount to add.</param>
    /// <returns>A new CalendarAmount representing the result of the addition.</returns>
    private static CalendarAmount AddInternal(CalendarAmount left, CalendarAmount right) => new()
    {
        Years = left.Years + right.Years,
        Months = left.Months + right.Months,
        TimeSpan = left.TimeSpan + right.TimeSpan
    };

    /// <summary>
    /// Subtracts a <see cref="TimeSpan"/> from a <see cref="CalendarAmount"/> by subtracting the TimeSpan components and keeping the months and years components unchanged.
    /// </summary>
    /// <param name="left">The CalendarAmount from which to subtract.</param>
    /// <param name="right">The TimeSpan to subtract.</param>
    /// <returns>A new CalendarAmount representing the result of the subtraction.</returns>
    private static CalendarAmount SubtractInternal(CalendarAmount left, TimeSpan right) => left with { TimeSpan = left.TimeSpan - right };

    /// <summary>
    /// Subtracts two <see cref="CalendarAmount"/> instances by subtracting their months, years, and TimeSpan components.
    /// </summary>
    /// <param name="left">The first CalendarAmount to subtract.</param>
    /// <param name="right">The second CalendarAmount to subtract.</param>
    /// <returns>A new CalendarAmount representing the result of the subtraction.</returns>
    private static CalendarAmount SubtractInternal(CalendarAmount left, CalendarAmount right) => new()
    {
        Years = left.Years - right.Years,
        Months = left.Months - right.Months,
        TimeSpan = left.TimeSpan - right.TimeSpan
    };
}
