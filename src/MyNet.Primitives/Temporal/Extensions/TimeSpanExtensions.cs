// -----------------------------------------------------------------------
// <copyright file="TimeSpanExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Temporal;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TimeSpanExtensions
{
    extension(TimeSpan timeSpan)
    {
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a <see cref="TimeOnly"/> by taking the time component of the <see cref="TimeSpan"/> and ignoring the date component.
        /// </summary>
        /// <returns>A <see cref="TimeOnly"/> representing the time component of the <see cref="TimeSpan"/>.</returns>
        public TimeOnly ToTime() => TimeOnly.FromTimeSpan(timeSpan);

        /// <summary>
        /// Adds the given <see cref="FluentTimeSpan"/> from a <see cref="TimeSpan"/> and returns resulting <see cref="FluentTimeSpan"/>.
        /// </summary>
        public FluentTimeSpan AddFluentTimeSpan(FluentTimeSpan fluentTimeSpan) => fluentTimeSpan.Add(timeSpan);

        /// <summary>
        /// Subtracts the given <see cref="FluentTimeSpan"/> from a <see cref="TimeSpan"/> and returns resulting <see cref="FluentTimeSpan"/>.
        /// </summary>
        public FluentTimeSpan SubtractFluentTimeSpan(FluentTimeSpan fluentTimeSpan) => FluentTimeSpan.SubtractInternal(timeSpan, fluentTimeSpan);

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from current date (<see cref="DateTime.Now"/>) and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public DateTime Ago() => timeSpan.Before(DateTime.Now);

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public DateTime Ago(DateTime originalValue) => timeSpan.Before(originalValue);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(TimeSpan, DateTime)"/>
        /// <remarks>
        /// Synonym of <see cref="From(TimeSpan, DateTime)"/> method.
        /// </remarks>
        public DateTime Since(DateTime originalValue) => timeSpan.From(originalValue);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public DateTime FromNow() => timeSpan.From(DateTime.Now);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public DateTime From(DateTime originalValue) => originalValue + timeSpan;

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public DateTime Before(DateTime originalValue) => originalValue - timeSpan;

        /// <summary>
        /// Rounds the <see cref="TimeSpan"/> to the nearest specified time unit (second, minute, hour, or day) based on the value of the smaller time units. For example, if rounding to the nearest minute and the seconds are 30 or more, it will round up to the next minute; if the seconds are less than 30, it will round down to the current minute. The method throws an <see cref="ArgumentException"/> if an unsupported rounding unit is provided.
        /// </summary>
        /// <param name="rt">The time unit to which the <see cref="TimeSpan"/> should be rounded.</param>
        /// <returns>The rounded <see cref="TimeSpan"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when an unsupported rounding unit is provided.</exception>
        public TimeSpan Round(RoundTo rt)
        {
            TimeSpan rounded;

            switch (rt)
            {
                case RoundTo.Second:
                    {
                        rounded = new(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                        if (timeSpan.Milliseconds >= 500)
                        {
                            rounded += 1.Seconds();
                        }

                        break;
                    }

                case RoundTo.Minute:
                    {
                        rounded = new(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
                        if (timeSpan.Seconds >= 30)
                        {
                            rounded += 1.Minutes();
                        }

                        break;
                    }

                case RoundTo.Hour:
                    {
                        rounded = new(timeSpan.Days, timeSpan.Hours, 0, 0);
                        if (timeSpan.Minutes >= 30)
                        {
                            rounded += 1.Hours();
                        }

                        break;
                    }

                case RoundTo.Day:
                    {
                        rounded = new(timeSpan.Days, 0, 0, 0);
                        if (timeSpan.Hours >= 12)
                        {
                            rounded += 1.Days();
                        }

                        break;
                    }

                default:
                    {
                        throw new ArgumentException(null, nameof(rt));
                    }
            }

            return rounded;
        }

        /// <summary>
        /// Adds a specified value of a given time unit to the current <see cref="TimeSpan"/> and returns the resulting <see cref="TimeSpan"/>. The method takes an integer value and a <see cref="TimeUnit"/> enumeration to determine how much time to add. For example, if you want to add 5 minutes to the current <see cref="TimeSpan"/>, you would call this method with a value of 5 and a time unit of <see cref="TimeUnit.Minute"/>. The method will then convert the value to the appropriate amount of time based on the specified time unit and add it to the current <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The amount of time to add.</param>
        /// <param name="timeUnitToGet">The unit of time for the value to add.</param>
        /// <returns>The resulting <see cref="TimeSpan"/> after adding the specified time.</returns>
        public TimeSpan Add(int value, TimeUnit timeUnitToGet) => timeSpan.Add(value.ToTimeSpan(timeUnitToGet));

        /// <summary>
        /// Converts the current <see cref="TimeSpan"/> to an integer value representing the total amount of time in the specified time unit. For example, if you have a <see cref="TimeSpan"/> of 90 seconds and you want to convert it to minutes, you would call this method with a time unit of <see cref="TimeUnit.Minute"/>, and it would return 1 (since 90 seconds is equal to 1.5 minutes, which rounds down to 1 minute). The method uses the total time in the specified unit, so if you have a <see cref="TimeSpan"/> of 1 hour and 30 minutes and you convert it to minutes, it would return 90 (since 1 hour is equal to 60 minutes and 30 minutes is equal to 30 minutes, for a total of 90 minutes).
        /// </summary>
        /// <param name="unit">The time unit to convert to.</param>
        /// <returns>The total amount of time in the specified time unit.</returns>
        public double ConvertTo(TimeUnit unit)
        {
            var abs = timeSpan.Duration();

            return unit switch
            {
                TimeUnit.Millisecond => Math.Round(abs.TotalMilliseconds),
                TimeUnit.Second => Math.Round(abs.TotalSeconds),
                TimeUnit.Minute => Math.Round(abs.TotalMinutes),
                TimeUnit.Hour => Math.Round(abs.TotalHours),
                TimeUnit.Day => Math.Round(abs.TotalDays),
                TimeUnit.Week => Math.Round(abs.TotalDays / DateTimeHelper.DaysPerWeek),
                TimeUnit.Month => Math.Round(abs.TotalDays / DateTimeHelper.DaysPerMonth),
                TimeUnit.Year => Math.Round(abs.TotalDays / DateTimeHelper.DaysPerYear),
                _ => 0
            };
        }
    }

    extension(FluentTimeSpan fluentTimeSpan)
    {
        /// <summary>
        /// Converts a <see cref="FluentTimeSpan"/> to a <see cref="TimeOnly"/> by taking the time component of the <see cref="FluentTimeSpan"/> and ignoring the date component.
        /// </summary>
        /// <returns>A <see cref="TimeOnly"/> representing the time component of the <see cref="FluentTimeSpan"/>.</returns>
        public TimeOnly ToTime() => TimeOnly.FromTimeSpan(fluentTimeSpan);

        /// <summary>
        /// Subtracts given <see cref="FluentTimeSpan"/> from current date (<see cref="DateTime.Now"/>) and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public DateTime Ago() => fluentTimeSpan.Before(DateTime.Now);

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public DateTime Ago(DateTime originalValue) => fluentTimeSpan.Before(originalValue);

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public DateTime Before(DateTime originalValue) => originalValue.AddMonths(-fluentTimeSpan.Months).AddYears(-fluentTimeSpan.Years).Add(-fluentTimeSpan.TimeSpan);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public DateTime FromNow() => fluentTimeSpan.From(DateTime.Now);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public DateTime From(DateTime originalValue) => originalValue.AddMonths(fluentTimeSpan.Months).AddYears(fluentTimeSpan.Years).Add(fluentTimeSpan.TimeSpan);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(FluentTimeSpan, DateTime)"/>
        /// <remarks>
        /// Synonym of <see cref="From(FluentTimeSpan, DateTime)"/> method.
        /// </remarks>
        public DateTime Since(DateTime originalValue) => fluentTimeSpan.From(originalValue);
    }
}
