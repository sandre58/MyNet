// -----------------------------------------------------------------------
// <copyright file="NumberToTimeSpanExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Intervals;
using MyNet.Primitives.Temporal;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class NumberToTimeSpanExtensions
{
    private static int GetDecadeStart(int year) => year / 10 * 10;

    private static int GetCenturyStart(int year) => year / 100 * 100;

    extension(int value)
    {
        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Years.
        /// </summary>
        public FluentTimeSpan Years() => new() { Years = value };

        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Quarters.
        /// </summary>
        public FluentTimeSpan Quarters() => new() { Months = value * 3 };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> value for given number of Months.
        /// </summary>
        public FluentTimeSpan Months() => new() { Months = value };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of weeks * 7).
        /// </summary>
        public FluentTimeSpan Weeks() => new() { TimeSpan = TimeSpan.FromDays(value * DateTimeHelper.DaysPerWeek) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public FluentTimeSpan Days() => new() { TimeSpan = TimeSpan.FromDays(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public FluentTimeSpan Hours() => new() { TimeSpan = TimeSpan.FromHours(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public FluentTimeSpan Minutes() => new() { TimeSpan = TimeSpan.FromMinutes(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public FluentTimeSpan Seconds() => new() { TimeSpan = TimeSpan.FromSeconds(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public FluentTimeSpan Milliseconds() => new() { TimeSpan = TimeSpan.FromMilliseconds(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of ticks.
        /// </summary>
        public FluentTimeSpan Ticks() => new() { TimeSpan = TimeSpan.FromTicks(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of units.
        /// </summary>
        /// <param name="unit">The unit of time.</param>
        /// <returns>A <see cref="FluentTimeSpan"/> representing the specified number of units.</returns>
        public FluentTimeSpan Unit(TimeUnit unit) => unit switch
        {
            TimeUnit.Millisecond => new() { TimeSpan = TimeSpan.FromMilliseconds(value) },
            TimeUnit.Second => new() { TimeSpan = TimeSpan.FromSeconds(value) },
            TimeUnit.Minute => new() { TimeSpan = TimeSpan.FromMinutes(value) },
            TimeUnit.Hour => new() { TimeSpan = TimeSpan.FromHours(value) },
            TimeUnit.Day => new() { TimeSpan = TimeSpan.FromDays(value) },
            TimeUnit.Week => new() { TimeSpan = TimeSpan.FromDays(value * DateTimeHelper.DaysPerWeek) },
            TimeUnit.Month => new() { Months = value },
            TimeUnit.Year => new() { Years = value },
            _ => default
        };

        /// <summary>
        /// Converts an integer value to a <see cref="TimeSpan"/> based on the specified time unit. For example, if you have a value of 5 and a time unit of <see cref="TimeUnit.Minute"/>, this method will return a <see cref="TimeSpan"/> representing 5 minutes. The method uses a switch expression to determine how to convert the integer value to the appropriate amount of time based on the specified time unit. If an unsupported time unit is provided, it returns a zero <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeUnitToGet">The time unit to convert the integer value to.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the specified value in the specified time unit.</returns>
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
        /// Returns the first year of the decade containing the given year. For example, if the input year is 1995, this method returns 1990.
        /// </summary>
        /// <returns>The first year of the decade containing the given year.</returns>
        public int DecadeStart() => GetDecadeStart(value);

        /// <summary>
        /// Returns the last year of the decade containing the given year. For example, if the input year is 1995, this method returns 1999.
        /// </summary>
        /// <returns>The last year of the decade containing the given year.</returns>
        public int DecadeEnd() => GetDecadeStart(value) + 9;

        /// <summary>
        /// Returns a ClosedInterval representing the decade of the given year. The start of the interval is calculated by dividing the year by 10, multiplying it back by 10 to get the first year of the decade, and the end of the interval is calculated by adding 9 to the start year to get the last year of the decade. For example, if the input year is 1995, the method will return a ClosedInterval with a start of 1990 and an end of 1999, representing the decade of the 1990s.
        /// </summary>
        /// <returns>A ClosedInterval representing the decade of the given year.</returns>
        public ClosedInterval<int> Decade()
        {
            var start = GetDecadeStart(value);
            return new(start, start + 9);
        }

        /// <summary>
        /// Returns the first year of the century containing the given year. For example, if the input year is 1995, this method returns 1900.
        /// </summary>
        /// <returns>The first year of the century containing the given year.</returns>
        public int CenturyStart() => GetCenturyStart(value);

        /// <summary>
        /// Returns the last year of the century containing the given year. For example, if the input year is 1995, this method returns 1999.
        /// </summary>
        /// <returns>The last year of the century containing the given year.</returns>
        public int CenturyEnd() => GetCenturyStart(value) + 99;

        /// <summary>
        /// Returns a ClosedInterval representing the century of the given year. The start of the interval is calculated by dividing the year by 100, multiplying it back by 100 to get the first year of the century, and the end of the interval is calculated by adding 99 to the start year to get the last year of the century. For example, if the input year is 1995, the method will return a ClosedInterval with a start of 1900 and an end of 1999, representing the 20th century.
        /// </summary>
        /// <returns>A ClosedInterval representing the century of the given year.</returns>
        public ClosedInterval<int> Century()
        {
            var start = GetCenturyStart(value);
            return new(start, start + 99);
        }
    }

    extension(double value)
    {
        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of weeks * 7).
        /// </summary>
        public FluentTimeSpan Weeks() => new() { TimeSpan = TimeSpan.FromDays((int)(value * DateTimeHelper.DaysPerWeek)) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public FluentTimeSpan Days() => new() { TimeSpan = TimeSpan.FromDays(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public FluentTimeSpan Hours() => new() { TimeSpan = TimeSpan.FromHours(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public FluentTimeSpan Minutes() => new() { TimeSpan = TimeSpan.FromMinutes(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public FluentTimeSpan Seconds() => new() { TimeSpan = TimeSpan.FromSeconds(value) };

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public FluentTimeSpan Milliseconds() => new() { TimeSpan = TimeSpan.FromMilliseconds(value) };
    }

    /// <summary>
    /// Returns <see cref="TimeSpan"/> for given number of ticks.
    /// </summary>
    public static FluentTimeSpan Ticks(this long ticks) => new() { TimeSpan = TimeSpan.FromTicks(ticks) };
}
