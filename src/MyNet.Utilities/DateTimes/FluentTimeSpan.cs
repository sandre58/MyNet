// -----------------------------------------------------------------------
// <copyright file="FluentTimeSpan.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace MyNet.Utilities.DateTimes;

/// <summary>
/// Represents a time span extended with months and years components to allow fluent arithmetic
/// while still being convertible to a <see cref="TimeSpan"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct FluentTimeSpan :
    IEquatable<FluentTimeSpan>,
    IComparable<TimeSpan>,
    IComparable<FluentTimeSpan>
{
    private const int DaysPerYear = 365;

    /// <summary>
    /// Gets the number of months component of the fluent timespan.
    /// </summary>
    public int Months { get; init; }

    /// <summary>
    /// Gets the number of years component of the fluent timespan.
    /// </summary>
    public int Years { get; init; }

    /// <summary>
    /// Gets the <see cref="TimeSpan"/> component of the fluent timespan (days, hours, minutes, etc.).
    /// </summary>
    public TimeSpan TimeSpan { get; init; }

    /// <summary>
    /// Gets the number of ticks that represent the value of the current <see cref="TimeSpan"/> structure.
    /// </summary>
    public long Ticks => ((TimeSpan)this).Ticks;

    /// <summary>
    /// Gets the days portion of the underlying <see cref="TimeSpan"/>.
    /// </summary>
    public int Days => ((TimeSpan)this).Days;

    /// <summary>
    /// Gets the hours portion of the underlying <see cref="TimeSpan"/>.
    /// </summary>
    public int Hours => ((TimeSpan)this).Hours;

    /// <summary>
    /// Gets the milliseconds portion of the underlying <see cref="TimeSpan"/>.
    /// </summary>
    public int Milliseconds => ((TimeSpan)this).Milliseconds;

    /// <summary>
    /// Gets the minutes portion of the underlying <see cref="TimeSpan"/>.
    /// </summary>
    public int Minutes => ((TimeSpan)this).Minutes;

    /// <summary>
    /// Gets the seconds portion of the underlying <see cref="TimeSpan"/>.
    /// </summary>
    public int Seconds => ((TimeSpan)this).Seconds;

    /// <summary>
    /// Gets the total days represented by this fluent timespan.
    /// </summary>
    public double TotalDays => ((TimeSpan)this).TotalDays;

    /// <summary>
    /// Gets the total hours represented by this fluent timespan.
    /// </summary>
    public double TotalHours => ((TimeSpan)this).TotalHours;

    /// <summary>
    /// Gets the total milliseconds represented by this fluent timespan.
    /// </summary>
    public double TotalMilliseconds => ((TimeSpan)this).TotalMilliseconds;

    /// <summary>
    /// Gets the total minutes represented by this fluent timespan.
    /// </summary>
    public double TotalMinutes => ((TimeSpan)this).TotalMinutes;

    /// <summary>
    /// Gets the total seconds represented by this fluent timespan.
    /// </summary>
    public double TotalSeconds => ((TimeSpan)this).TotalSeconds;

    /// <summary>
    /// Converts the <see cref="FluentTimeSpan"/> to a <see cref="TimeSpan"/>. Years are treated as 365 days and months as 30 days.
    /// </summary>
    /// <param name="fluentTimeSpan">The fluent timespan to convert.</param>
    public static implicit operator TimeSpan(FluentTimeSpan fluentTimeSpan)
    {
        var daysFromYears = DaysPerYear * fluentTimeSpan.Years;
        var daysFromMonths = 30 * fluentTimeSpan.Months;
        var days = daysFromMonths + daysFromYears;
        return new TimeSpan(days, 0, 0, 0) + fluentTimeSpan.TimeSpan;
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a <see cref="FluentTimeSpan"/> with only the <see cref="TimeSpan"/> component set.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to convert.</param>
    public static implicit operator FluentTimeSpan(TimeSpan timeSpan) => new() { TimeSpan = timeSpan };

    /// <summary>
    /// Adds two <see cref="FluentTimeSpan"/> instances.
    /// </summary>
    public static FluentTimeSpan operator +(FluentTimeSpan left, FluentTimeSpan right) => AddInternal(left, right);

    /// <summary>
    /// Adds a <see cref="TimeSpan"/> to a <see cref="FluentTimeSpan"/>.
    /// </summary>
    public static FluentTimeSpan operator +(FluentTimeSpan left, TimeSpan right) => AddInternal(left, right);

    /// <summary>
    /// Adds a <see cref="TimeSpan"/> and a <see cref="FluentTimeSpan"/>.
    /// </summary>
    public static FluentTimeSpan operator +(TimeSpan left, FluentTimeSpan right) => AddInternal(left, right);

    /// <summary>
    /// Subtracts two <see cref="FluentTimeSpan"/> instances.
    /// </summary>
    public static FluentTimeSpan operator -(FluentTimeSpan left, FluentTimeSpan right) => SubtractInternal(left, right);

    /// <summary>
    /// Subtracts a <see cref="FluentTimeSpan"/> from a <see cref="TimeSpan"/>.
    /// </summary>
    public static FluentTimeSpan operator -(TimeSpan left, FluentTimeSpan right) => SubtractInternal(left, right);

    /// <summary>
    /// Subtracts a <see cref="TimeSpan"/> from a <see cref="FluentTimeSpan"/>.
    /// </summary>
    public static FluentTimeSpan operator -(FluentTimeSpan left, TimeSpan right) => SubtractInternal(left, right);

    /// <summary>
    /// Determines whether two <see cref="FluentTimeSpan"/> instances are equal.
    /// </summary>
    public static bool operator ==(FluentTimeSpan left, FluentTimeSpan right) => left.Years == right.Years &&
        left.Months == right.Months &&
        left.TimeSpan == right.TimeSpan;

    /// <summary>
    /// Determines whether a <see cref="TimeSpan"/> and a <see cref="FluentTimeSpan"/> are equal.
    /// </summary>
    public static bool operator ==(TimeSpan left, FluentTimeSpan right) => (FluentTimeSpan)left == right;

    /// <summary>
    /// Determines whether a <see cref="FluentTimeSpan"/> and a <see cref="TimeSpan"/> are equal.
    /// </summary>
    public static bool operator ==(FluentTimeSpan left, TimeSpan right) => left == (FluentTimeSpan)right;

    /// <summary>
    /// Determines whether two <see cref="FluentTimeSpan"/> instances are not equal.
    /// </summary>
    public static bool operator !=(FluentTimeSpan left, FluentTimeSpan right) => !(left == right);

    /// <summary>
    /// Determines whether a <see cref="TimeSpan"/> and a <see cref="FluentTimeSpan"/> are not equal.
    /// </summary>
    public static bool operator !=(TimeSpan left, FluentTimeSpan right) => !(left == right);

    /// <summary>
    /// Determines whether a <see cref="FluentTimeSpan"/> and a <see cref="TimeSpan"/> are not equal.
    /// </summary>
    public static bool operator !=(FluentTimeSpan left, TimeSpan right) => !(left == right);

    /// <summary>
    /// Returns the negation of the specified fluent timespan.
    /// </summary>
    public static FluentTimeSpan operator -(FluentTimeSpan value) => value.Negate();

    /// <summary>
    /// Less-than comparison between two fluent timespans using their <see cref="TimeSpan"/> representation.
    /// </summary>
    public static bool operator <(FluentTimeSpan left, FluentTimeSpan right) => (TimeSpan)left < (TimeSpan)right;

    /// <summary>
    /// Less-than comparison between a fluent timespan and a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator <(FluentTimeSpan left, TimeSpan right) => (TimeSpan)left < right;

    /// <summary>
    /// Less-than comparison between a <see cref="TimeSpan"/> and a fluent timespan.
    /// </summary>
    public static bool operator <(TimeSpan left, FluentTimeSpan right) => left < (TimeSpan)right;

    /// <summary>
    /// Less-than-or-equal comparison between two fluent timespans.
    /// </summary>
    public static bool operator <=(FluentTimeSpan left, FluentTimeSpan right) => (TimeSpan)left <= (TimeSpan)right;

    /// <summary>
    /// Less-than-or-equal comparison between a fluent timespan and a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator <=(FluentTimeSpan left, TimeSpan right) => (TimeSpan)left <= right;

    /// <summary>
    /// Less-than-or-equal comparison between a <see cref="TimeSpan"/> and a fluent timespan.
    /// </summary>
    public static bool operator <=(TimeSpan left, FluentTimeSpan right) => left <= (TimeSpan)right;

    /// <summary>
    /// Greater-than comparison between two fluent timespans.
    /// </summary>
    public static bool operator >(FluentTimeSpan left, FluentTimeSpan right) => (TimeSpan)left > (TimeSpan)right;

    /// <summary>
    /// Greater-than comparison between a fluent timespan and a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator >(FluentTimeSpan left, TimeSpan right) => (TimeSpan)left > right;

    /// <summary>
    /// Greater-than comparison between a <see cref="TimeSpan"/> and a fluent timespan.
    /// </summary>
    public static bool operator >(TimeSpan left, FluentTimeSpan right) => left > (TimeSpan)right;

    /// <summary>
    /// Greater-than-or-equal comparison between two fluent timespans.
    /// </summary>
    public static bool operator >=(FluentTimeSpan left, FluentTimeSpan right) => (TimeSpan)left >= (TimeSpan)right;

    /// <summary>
    /// Greater-than-or-equal comparison between a fluent timespan and a <see cref="TimeSpan"/>.
    /// </summary>
    public static bool operator >=(FluentTimeSpan left, TimeSpan right) => (TimeSpan)left >= right;

    /// <summary>
    /// Greater-than-or-equal comparison between a <see cref="TimeSpan"/> and a fluent timespan.
    /// </summary>
    public static bool operator >=(TimeSpan left, FluentTimeSpan right) => left >= (TimeSpan)right;

    /// <summary>
    /// Creates a <see cref="FluentTimeSpan"/> from a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert.</param>
    /// <returns>A fluent timespan representing the same duration (no months/years component).</returns>
    public static FluentTimeSpan ToFluentTimeSpan(TimeSpan timeSpan) => new() { TimeSpan = timeSpan };

    /// <summary>
    /// Converts a <see cref="FluentTimeSpan"/> into a <see cref="TimeSpan"/> taking months and years into account
    /// (months = 30 days, years = 365 days).
    /// </summary>
    /// <param name="fluentTimeSpan">The fluent timespan to convert.</param>
    /// <returns>The equivalent <see cref="TimeSpan"/>.</returns>
    public static TimeSpan FromFluentTimeSpan(FluentTimeSpan fluentTimeSpan)
    {
        var daysFromYears = DaysPerYear * fluentTimeSpan.Years;
        var daysFromMonths = 30 * fluentTimeSpan.Months;
        var days = daysFromMonths + daysFromYears;
        return new TimeSpan(days, 0, 0, 0) + fluentTimeSpan.TimeSpan;
    }

    /// <summary>
    /// Adds another fluent timespan to this instance.
    /// </summary>
    /// <param name="number">The fluent timespan to add.</param>
    public FluentTimeSpan Add(FluentTimeSpan number) => AddInternal(this, number);

    /// <summary>
    /// Adds a <see cref="TimeSpan"/> to this fluent timespan.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to add.</param>
    public FluentTimeSpan Add(TimeSpan timeSpan) => AddInternal(this, timeSpan);

    /// <summary>
    /// Subtracts the specified fluent timespan from this instance.
    /// </summary>
    /// <param name="fluentTimeSpan">The fluent timespan to subtract.</param>
    public FluentTimeSpan Subtract(FluentTimeSpan fluentTimeSpan) => SubtractInternal(this, fluentTimeSpan);

    /// <summary>
    /// Subtracts the specified <see cref="TimeSpan"/> from this fluent timespan.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to subtract.</param>
    public FluentTimeSpan Subtract(TimeSpan timeSpan) => SubtractInternal(this, timeSpan);

    /// <summary>
    /// Compares this fluent timespan to a <see cref="TimeSpan"/> using the converted value.
    /// </summary>
    public int CompareTo(TimeSpan other) => ((TimeSpan)this).CompareTo(other);

    /// <summary>
    /// Compares this fluent timespan to an object that must be a <see cref="TimeSpan"/>.
    /// </summary>
    public int CompareTo(object value) => value is TimeSpan timeSpan
        ? ((TimeSpan)this).CompareTo(timeSpan)
        : throw new ArgumentException("Value must be a TimeSpan", nameof(value));

    /// <summary>
    /// Compares this fluent timespan to another fluent timespan.
    /// </summary>
    public int CompareTo(FluentTimeSpan other) => ((TimeSpan)this).CompareTo(other);

    /// <summary>
    /// Returns a new fluent timespan that represents the negation of this instance.
    /// </summary>
    public TimeSpan Negate() => new FluentTimeSpan
    {
        TimeSpan = -TimeSpan,
        Months = -Months,
        Years = -Years
    };

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A copy of this instance.</returns>
    public object Clone() => new FluentTimeSpan
    {
        TimeSpan = TimeSpan,
        Months = Months,
        Years = Years
    };

    /// <inheritdoc />
    public override string ToString() => ((TimeSpan)this).ToString();

    /// <summary>
    /// Determines whether the specified fluent timespan is equal to the current instance.
    /// </summary>
    public bool Equals(FluentTimeSpan other) => this == other;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        var type = obj.GetType();
        return type == typeof(FluentTimeSpan) ? this == (FluentTimeSpan)obj : type == typeof(TimeSpan) && this == (TimeSpan)obj;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Months.GetHashCode() ^ Years.GetHashCode() ^ TimeSpan.GetHashCode();

    internal static FluentTimeSpan SubtractInternal(TimeSpan left, FluentTimeSpan right) => new()
    {
        Months = -right.Months,
        Years = -right.Years,
        TimeSpan = left - right.TimeSpan
    };

    private static FluentTimeSpan AddInternal(FluentTimeSpan left, TimeSpan right) => left with { TimeSpan = left.TimeSpan + right };

    private static FluentTimeSpan AddInternal(FluentTimeSpan left, FluentTimeSpan right) => new()
    {
        Years = left.Years + right.Years,
        Months = left.Months + right.Months,
        TimeSpan = left.TimeSpan + right.TimeSpan
    };

    private static FluentTimeSpan SubtractInternal(FluentTimeSpan left, TimeSpan right) => left with { TimeSpan = left.TimeSpan - right };

    private static FluentTimeSpan SubtractInternal(FluentTimeSpan left, FluentTimeSpan right) => new()
    {
        Years = left.Years - right.Years,
        Months = left.Months - right.Months,
        TimeSpan = left.TimeSpan - right.TimeSpan
    };
}
