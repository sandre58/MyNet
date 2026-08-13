// -----------------------------------------------------------------------
// <copyright file="NumberToTimeSpanExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives;
using MyNet.Primitives.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Temporal;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class NumberToTimeSpanExtensions
{
    private static int GetDecadeStart(int year) => year / 10 * 10;

    private static int GetCenturyStart(int year) => year / 100 * 100;

    extension(int value)
    {
        /// <summary>
        /// Generates a <see cref="CalendarAmount"/> for the given number of years.
        /// </summary>
        public CalendarAmount Years() => new() { Years = value };

        /// <summary>
        /// Generates a <see cref="CalendarAmount"/> for the given number of quarters.
        /// </summary>
        public CalendarAmount Quarters() => new() { Months = value * 3 };

        /// <summary>
        /// Returns a <see cref="CalendarAmount"/> for the given number of months.
        /// </summary>
        public CalendarAmount Months() => new() { Months = value };

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of weeks (weeks * 7 days).
        /// </summary>
        public TimeSpan Weeks() => TimeSpan.FromDays(value * DateTimeHelper.DaysPerWeek);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of days.
        /// </summary>
        public TimeSpan Days() => TimeSpan.FromDays(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of hours.
        /// </summary>
        public TimeSpan Hours() => TimeSpan.FromHours(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of minutes.
        /// </summary>
        public TimeSpan Minutes() => TimeSpan.FromMinutes(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of seconds.
        /// </summary>
        public TimeSpan Seconds() => TimeSpan.FromSeconds(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of milliseconds.
        /// </summary>
        public TimeSpan Milliseconds() => TimeSpan.FromMilliseconds(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of ticks.
        /// </summary>
        public TimeSpan Ticks() => TimeSpan.FromTicks(value);

        /// <summary>
        /// Returns a <see cref="CalendarAmount"/> for calendar units (month/year) or a fixed-duration amount for other units.
        /// </summary>
        public CalendarAmount Unit(TimeUnit unit) => unit switch
        {
            TimeUnit.Millisecond => TimeSpan.FromMilliseconds(value),
            TimeUnit.Second => TimeSpan.FromSeconds(value),
            TimeUnit.Minute => TimeSpan.FromMinutes(value),
            TimeUnit.Hour => TimeSpan.FromHours(value),
            TimeUnit.Day => TimeSpan.FromDays(value),
            TimeUnit.Week => TimeSpan.FromDays(value * DateTimeHelper.DaysPerWeek),
            TimeUnit.Month => new CalendarAmount { Months = value },
            TimeUnit.Year => new CalendarAmount { Years = value },
            _ => default
        };

        /// <summary>
        /// Converts an integer value to a fixed <see cref="TimeSpan"/>. Month/Year use approximate average lengths
        /// (<see cref="DateTimeHelper.DaysPerMonth"/> / <see cref="DateTimeHelper.DaysPerYear"/>).
        /// Prefer <see cref="Months"/> / <see cref="Years"/> with <c>AddCalendarAmount</c> for calendar arithmetic.
        /// </summary>
        public TimeSpan ToTimeSpan(TimeUnit timeUnitToGet) => timeUnitToGet switch
        {
            TimeUnit.Millisecond => new(0, 0, 0, 0, value),
            TimeUnit.Second => new(0, 0, 0, value, 0),
            TimeUnit.Minute => new(0, 0, value, 0, 0),
            TimeUnit.Hour => new(0, value, 0, 0, 0),
            TimeUnit.Day => new(value, 0, 0, 0, 0),
            TimeUnit.Week => new(value * DateTimeHelper.DaysPerWeek, 0, 0, 0, 0),
            TimeUnit.Month => new((int)Math.Round(value * DateTimeHelper.DaysPerMonth), 0, 0, 0, 0),
            TimeUnit.Year => new((int)Math.Round(value * DateTimeHelper.DaysPerYear), 0, 0, 0, 0),
            _ => TimeSpan.Zero
        };

        /// <summary>
        /// Returns the first year of the decade containing the given year.
        /// </summary>
        public int DecadeStart() => GetDecadeStart(value);

        /// <summary>
        /// Returns the last year of the decade containing the given year.
        /// </summary>
        public int DecadeEnd() => GetDecadeStart(value) + 9;

        /// <summary>
        /// Returns a closed interval covering the decade of the given year.
        /// </summary>
        public ClosedInterval<int> Decade()
        {
            var start = GetDecadeStart(value);
            return new(start, start + 9);
        }

        /// <summary>
        /// Returns the first year of the century containing the given year.
        /// </summary>
        public int CenturyStart() => GetCenturyStart(value);

        /// <summary>
        /// Returns the last year of the century containing the given year.
        /// </summary>
        public int CenturyEnd() => GetCenturyStart(value) + 99;

        /// <summary>
        /// Returns a closed interval covering the century of the given year.
        /// </summary>
        public ClosedInterval<int> Century()
        {
            var start = GetCenturyStart(value);
            return new(start, start + 99);
        }
    }

    extension(double value)
    {
        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of weeks (fractional weeks allowed).
        /// </summary>
        public TimeSpan Weeks() => TimeSpan.FromDays(value * DateTimeHelper.DaysPerWeek);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of days.
        /// </summary>
        public TimeSpan Days() => TimeSpan.FromDays(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of hours.
        /// </summary>
        public TimeSpan Hours() => TimeSpan.FromHours(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of minutes.
        /// </summary>
        public TimeSpan Minutes() => TimeSpan.FromMinutes(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of seconds.
        /// </summary>
        public TimeSpan Seconds() => TimeSpan.FromSeconds(value);

        /// <summary>
        /// Returns a fixed <see cref="TimeSpan"/> for the given number of milliseconds.
        /// </summary>
        public TimeSpan Milliseconds() => TimeSpan.FromMilliseconds(value);
    }

    /// <summary>
    /// Returns a fixed <see cref="TimeSpan"/> for the given number of ticks.
    /// </summary>
    public static TimeSpan Ticks(this long ticks) => TimeSpan.FromTicks(ticks);
}
