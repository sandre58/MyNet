// -----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DateTimeExtensions
{
    /// <summary>
    /// Determines if a collection of DateTime values represents consecutive days. The method first converts the input collection to a list to avoid multiple enumerations. It then orders the list and uses the Zip method to create pairs of consecutive dates. Finally, it checks if the difference in days between each pair is exactly one, indicating that the dates are consecutive.
    /// </summary>
    /// <param name="dates">The collection of DateTime values to check.</param>
    /// <returns>True if the dates are consecutive; otherwise, false.</returns>
    public static bool IsConsecutiveDays(this IEnumerable<DateTime> dates)
    {
        var dateList = dates.ToList();
        return dateList.Order().Zip(dateList.Order().Skip(1), (a, b) => (a, b)).All(pair => (pair.b - pair.a).TotalDays.IsOne());
    }

    extension(DateTime dateTime)
    {
        /// <summary>
        /// Converts the specified <see cref="DateTime"/> to the given time zone.
        /// </summary>
        /// <param name="timeZoneInfo">The target time zone.</param>
        /// <returns>The converted <see cref="DateTime"/> in the specified time zone.</returns>
        public DateTime ToTimeZone(TimeZoneInfo timeZoneInfo) => TimeZoneInfo.ConvertTime(dateTime, timeZoneInfo);

        /// <summary>
        /// Converts the provided <see cref="DateTime"/> to local time and applies the given time-of-day offset.
        /// </summary>
        /// <param name="time">The time-of-day to add after converting to local time.</param>
        /// <returns>A <see cref="DateTime"/> adjusted to local time and the provided time-of-day.</returns>
        public DateTime ToLocalTime(TimeSpan time) => dateTime.ToLocalTime().BeginningOfDay().Add(time);

        /// <summary>
        /// Converts the provided <see cref="DateTime"/> to local time, applies the given time-of-day and converts to UTC.
        /// </summary>
        /// <param name="time">The time-of-day to apply before converting to UTC.</param>
        /// <returns>A UTC <see cref="DateTime"/> instance.</returns>
        public DateTime ToUniversalTime(TimeSpan time) => dateTime.ToLocalTime().BeginningOfDay().Add(time).ToUniversalTime();

        /// <summary>
        /// Returns the date component of the specified <see cref="DateTime"/> as a <see cref="DateOnly"/>.
        /// </summary>
        /// <returns>A <see cref="DateOnly"/> containing the date part.</returns>
        public DateOnly ToDate() => DateOnly.FromDateTime(dateTime);

        /// <summary>
        /// Returns the time component of the specified <see cref="DateTime"/> as a <see cref="TimeOnly"/>.
        /// </summary>
        /// <returns>A <see cref="TimeOnly"/> containing the time-of-day part.</returns>
        public TimeOnly ToTime() => TimeOnly.FromDateTime(dateTime);

        /// <summary>
        /// Creates a <see cref="Period"/> starting at this date/time and lasting the specified <see cref="FluentTimeSpan"/>.
        /// </summary>
        /// <param name="timeSpan">The duration to apply.</param>
        /// <returns>A <see cref="Period"/> representing the interval.</returns>
        public Period ToPeriod(FluentTimeSpan timeSpan) => new(dateTime, dateTime.AddFluentTimeSpan(timeSpan));

        /// <summary>
        /// Creates a <see cref="Period"/> that spans between this date/time and another date/time.
        /// </summary>
        /// <param name="otherDateTime">The other end of the period.</param>
        /// <returns>A <see cref="Period"/> representing the interval between the two dates.</returns>
        public Period ToPeriod(DateTime otherDateTime) => new(dateTime.Min(otherDateTime), dateTime.Max(otherDateTime));

        /// <summary>
        /// Determines whether the current date is strictly before the specified <see cref="DateTime"/> (converted to a <see cref="DateOnly"/>).
        /// </summary>
        public bool IsBefore(DateTime toCompareWith) => dateTime < toCompareWith;

        /// <summary>
        /// Determines whether the current date is strictly after the specified <see cref="DateTime"/> (converted to a <see cref="DateOnly"/>).
        /// </summary>
        public bool IsAfter(DateTime toCompareWith) => dateTime > toCompareWith;

        /// <summary>
        /// Determines whether the current date is strictly before the specified <see cref="DateOnly"/>.
        /// </summary>
        public bool IsBefore(DateOnly toCompareWith) => dateTime.IsBefore(toCompareWith.ToDateTime(TimeOnly.MinValue));

        /// <summary>
        /// Determines whether the current date is strictly after the specified <see cref="DateOnly"/>.
        /// </summary>
        public bool IsAfter(DateOnly toCompareWith) => dateTime.IsAfter(toCompareWith.ToDateTime(TimeOnly.MinValue));

        /// <summary>
        /// Calculates the absolute difference between this date/time and another date/time, returning the result as a <see cref="TimeSpan"/>. The method determines which of the two dates is earlier and subtracts the earlier from the later to ensure a positive duration is returned, regardless of the order of the input dates.
        /// </summary>
        /// <param name="otherDateTime">The other date/time to compare with.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the absolute difference between the two dates.</returns>
        public TimeSpan Between(DateTime otherDateTime) => dateTime > otherDateTime ? dateTime - otherDateTime : otherDateTime - dateTime;

        /// <summary>
        /// Calculates the absolute difference between this date/time and another date/time, returning the result as a <see cref="TimeSpan"/>. The method determines which of the two dates is earlier and subtracts the earlier from the later to ensure a positive duration is returned, regardless of the order of the input dates.
        /// </summary>
        /// <param name="toCompareWith">The other date/time to compare with.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the absolute difference between the two dates.</returns>
        public TimeSpan Between(DateOnly toCompareWith) => dateTime > toCompareWith.ToDateTime(TimeOnly.MinValue) ? dateTime - toCompareWith.ToDateTime(TimeOnly.MinValue) : toCompareWith.ToDateTime(TimeOnly.MinValue) - dateTime;

        /// <summary>
        /// Adds a value expressed in the specified <see cref="TimeUnit"/> to the date.
        /// </summary>
        /// <param name="value">The amount to add.</param>
        /// <param name="timeUnitToGet">The unit of time for the amount.</param>
        /// <returns>The adjusted <see cref="DateTime"/>.</returns>
        public DateTime Add(int value, TimeUnit timeUnitToGet) => dateTime.AddFluentTimeSpan(value.ToTimeSpan(timeUnitToGet));

        /// <summary>
        /// Returns a new <see cref="DateTime"/> that adds the value of the specified <see cref="FluentTimeSpan"/> to the value of this instance.
        /// </summary>
        public DateTime AddFluentTimeSpan(FluentTimeSpan timeSpan)
            => dateTime.AddMonths(timeSpan.Months)
                .AddYears(timeSpan.Years)
                .Add(timeSpan.TimeSpan);

        /// <summary>
        /// Returns a new <see cref="DateTime"/> that subtracts the value of the specified <see cref="FluentTimeSpan"/> to the value of this instance.
        /// </summary>
        public DateTime SubtractFluentTimeSpan(FluentTimeSpan timeSpan)
            => dateTime.AddMonths(-timeSpan.Months)
                .AddYears(-timeSpan.Years)
                .Subtract(timeSpan.TimeSpan);

        /// <summary>
        /// Returns the very end of the given hour (the last millisecond of the hour for the given <see cref="DateTime"/>).
        /// </summary>
        public DateTime EndOfHour() => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 59, 59, 999, dateTime.Kind);

        /// <summary>
        /// Returns the very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
        /// </summary>
        public DateTime EndOfDay() => new(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, dateTime.Kind);

        /// <summary>
        /// Returns the timezone-adjusted very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
        /// </summary>
        public DateTime EndOfDay(int timeZoneOffset)
            => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, dateTime.Kind)
                .AddHours(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the week changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T23:59:59.999.
        /// </summary>
        public DateTime EndOfWeek(DayOfWeek? firstDayOfWeek = null) => dateTime.LastDayOfWeek(firstDayOfWeek).EndOfDay();

        /// <summary>
        /// Returns the last day of the week changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T23:59:59.999.
        /// </summary>
        public DateTime EndOfWeek(int timeZoneOffset, DayOfWeek? firstDayOfWeek = null) => dateTime.LastDayOfWeek(firstDayOfWeek).EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the month changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999.
        /// </summary>
        public DateTime EndOfMonth() => dateTime.LastDayOfMonth().EndOfDay();

        /// <summary>
        /// Returns the last day of the month changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999.
        /// </summary>
        public DateTime EndOfMonth(int timeZoneOffset) => dateTime.LastDayOfMonth().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the quarter changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999.
        /// </summary>
        public DateTime EndOfQuarter() => dateTime.LastDayOfQuarter().EndOfDay();

        /// <summary>
        /// Returns the last day of the quarter changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999.
        /// </summary>
        public DateTime EndOfQuarter(int timeZoneOffset) => dateTime.LastDayOfQuarter().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the year changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999.
        /// </summary>
        public DateTime EndOfYear() => dateTime.LastDayOfYear().EndOfDay();

        /// <summary>
        /// Returns the last day of the year changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999.
        /// </summary>
        public DateTime EndOfYear(int timeZoneOffset) => dateTime.LastDayOfYear().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the year changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2020-12-31T23:59:59.999.
        /// </summary>
        public DateTime EndOfDecade() => dateTime.SetYear(dateTime.Year - (dateTime.Year % 10) + 9).EndOfYear();

        /// <summary>
        /// Returns the Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public DateTime BeginningOfHour() => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0, dateTime.Kind);

        /// <summary>
        /// Returns the Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public DateTime BeginningOfDay() => new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Kind);

        /// <summary>
        /// Returns the timezone-adjusted Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public DateTime BeginningOfDay(int timezoneOffset)
            => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Kind)
                .AddHours(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the week changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-19T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfWeek(DayOfWeek? firstDayOfWeek = null) => dateTime.FirstDayOfWeek(firstDayOfWeek).BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the week changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-19T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfWeek(int timezoneOffset, DayOfWeek? firstDayOfWeek = null) => dateTime.FirstDayOfWeek(firstDayOfWeek).BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the month changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-01T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfMonth() => dateTime.FirstDayOfMonth().BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the month changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-01T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfMonth(int timezoneOffset) => dateTime.FirstDayOfMonth().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the quarter changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-10-01T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfQuarter() => dateTime.FirstDayOfQuarter().BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the quarter changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-10-01T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfQuarter(int timezoneOffset) => dateTime.FirstDayOfQuarter().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the year changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-01-01T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfYear() => dateTime.FirstDayOfYear().BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the year changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-01-01T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfYear(int timezoneOffset) => dateTime.FirstDayOfYear().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the year changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2010-01-01T00:00:00.000. <see cref="DateTime"/>.
        /// </summary>
        public DateTime BeginningOfDecade() => dateTime.SetYear(dateTime.Year - (dateTime.Year % 10)).BeginningOfYear();

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc.) in the next calendar year.
        /// If that day does not exist in next year in same month, number of missing days is added to the last day in same month next year.
        /// </summary>
        public DateTime NextYear()
        {
            var nextYear = dateTime.Year + 1;
            var numberOfDaysInSameMonthNextYear = DateTime.DaysInMonth(nextYear, dateTime.Month);

            if (numberOfDaysInSameMonthNextYear >= dateTime.Day)
                return new(nextYear, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

            var differenceInDays = dateTime.Day - numberOfDaysInSameMonthNextYear;
            var dateTime1 = new DateTime(nextYear, dateTime.Month, numberOfDaysInSameMonthNextYear, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
            return dateTime1 + differenceInDays.Days();
        }

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc.) in the previous calendar year.
        /// If that day does not exist in previous year in same month, number of missing days is added to the last day in same month previous year.
        /// </summary>
        public DateTime PreviousYear()
        {
            var previousYear = dateTime.Year - 1;
            var numberOfDaysInSameMonthPreviousYear = DateTime.DaysInMonth(previousYear, dateTime.Month);

            if (numberOfDaysInSameMonthPreviousYear >= dateTime.Day)
                return new(previousYear, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

            var differenceInDays = dateTime.Day - numberOfDaysInSameMonthPreviousYear;
            var dateTime1 = new DateTime(previousYear, dateTime.Month, numberOfDaysInSameMonthPreviousYear, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
            return dateTime1 + differenceInDays.Days();
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> increased by 24 hours ie Next Day.
        /// </summary>
        public DateTime NextDay() => dateTime + 1.Days();

        /// <summary>
        /// Returns <see cref="DateTime"/> decreased by 24h period ie Previous Day.
        /// </summary>
        public DateTime PreviousDay() => dateTime - 1.Days();

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateTime Next(DayOfWeek day)
        {
            do
            {
                dateTime = dateTime.NextDay();
            }
            while (dateTime.DayOfWeek != day);

            return dateTime;
        }

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateTime Next(int day, int month)
        {
            do
            {
                dateTime = dateTime.NextDay();
            }
            while (dateTime.Month != month || dateTime.Day != day);

            return dateTime;
        }

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateTime Previous(DayOfWeek day)
        {
            do
            {
                dateTime = dateTime.PreviousDay();
            }
            while (dateTime.DayOfWeek != day);

            return dateTime;
        }

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateTime Previous(int day, int month)
        {
            do
            {
                dateTime = dateTime.PreviousDay();
            }
            while (dateTime.Month != month || dateTime.Day != day);

            return dateTime;
        }

        /// <summary>
        /// Increases supplied <see cref="DateTime"/> for 7 days ie returns the Next Week.
        /// </summary>
        public DateTime WeekAfter() => dateTime + 1.Weeks();

        /// <summary>
        /// Decreases supplied <see cref="DateTime"/> for 7 days ie returns the Previous Week.
        /// </summary>
        public DateTime WeekEarlier() => dateTime - 1.Weeks();

        /// <summary>
        /// Increases the <see cref="DateTime"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public DateTime IncreaseTime(TimeSpan toAdd) => dateTime + toAdd;

        /// <summary>
        /// Decreases the <see cref="DateTime"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public DateTime DecreaseTime(TimeSpan toSubtract) => dateTime - toSubtract;

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour part changed to supplied hour parameter.
        /// </summary>
        public DateTime At(TimeOnly time) => new(dateTime.Year, dateTime.Month, dateTime.Day, time.Hour, time.Minute, time.Second, time.Minute, dateTime.Kind);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour part changed to supplied hour parameter.
        /// </summary>
        public DateTime At(TimeSpan time) => new(dateTime.Year, dateTime.Month, dateTime.Day, time.Hours, time.Minutes, time.Seconds, time.Milliseconds, dateTime.Kind);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour and Minute parts changed to supplied hour and minute parameters.
        /// </summary>
        public DateTime At(int hour, int minute) => new(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, 0, 0, dateTime.Kind);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour, Minute and Second parts changed to supplied hour, minute and second parameters.
        /// </summary>
        public DateTime At(int hour, int minute, int second) => new(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second, 0, dateTime.Kind);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour, Minute, Second and Millisecond parts changed to supplied hour, minute, second and millisecond parameters.
        /// </summary>
        public DateTime At(int hour, int minute, int second, int millisecond) => new(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second, millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Hour part.
        /// </summary>
        public DateTime SetHour(int hour) => new(dateTime.Year, dateTime.Month, dateTime.Day, hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Minute part.
        /// </summary>
        public DateTime SetMinute(int minute) => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Second part.
        /// </summary>
        public DateTime SetSecond(int second) => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Millisecond part.
        /// </summary>
        public DateTime SetMillisecond(int millisecond) => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, millisecond, dateTime.Kind);

        /// <summary>
        /// Returns original <see cref="DateTime"/> value with time part set to midnight (alias for <see cref="BeginningOfDay(DateTime)"/> method).
        /// </summary>
        public DateTime Midnight() => dateTime.BeginningOfDay();

        /// <summary>
        /// Returns original <see cref="DateTime"/> value with time part set to Noon (12:00:00h).
        /// </summary>
        /// <returns>A <see cref="DateTime"/> value with time part set to Noon (12:00:00h).</returns>
        public DateTime Noon() => dateTime.At(12, 0, 0, 0);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year part.
        /// </summary>
        public DateTime SetDate(int year) => new(year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year and Month part.
        /// </summary>
        public DateTime SetDate(int year, int month) => new(year, month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year, Month and Day part.
        /// </summary>
        public DateTime SetDate(int year, int month, int day) => new(year, month, day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year part.
        /// </summary>
        public DateTime SetYear(int year) => new(year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Month part.
        /// </summary>
        public DateTime SetMonth(int month) => new(dateTime.Year, month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Day part.
        /// </summary>
        public DateTime SetDay(int day) => new(dateTime.Year, dateTime.Month, day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the first day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html.
        /// </summary>
        /// <returns>given <see cref="DateTime"/> with the day part set to the first day in the quarter.</returns>
        public DateTime FirstDayOfQuarter()
        {
            var currentQuarter = ((dateTime.Month - 1) / 3) + 1;
            var firstDay = new DateTime(dateTime.Year, (3 * currentQuarter) - 2, 1, 0, 0, 0, dateTime.Kind);

            return dateTime.SetDate(firstDay.Year, firstDay.Month, firstDay.Day);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the first day in that month.
        /// </summary>
        /// <returns>given <see cref="DateTime"/> with the day part set to the first day in that month.</returns>
        public DateTime FirstDayOfMonth() => dateTime.SetDay(1);

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the last day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html.
        /// </summary>
        /// <returns>given <see cref="DateTime"/> with the day part set to the last day in the quarter.</returns>
        public DateTime LastDayOfQuarter()
        {
            var currentQuarter = ((dateTime.Month - 1) / 3) + 1;
            var firstDay = dateTime.SetDate(dateTime.Year, (3 * currentQuarter) - 2, 1);
            return firstDay.SetMonth(firstDay.Month + 2).LastDayOfMonth();
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the last day in that month.
        /// </summary>
        /// <returns>given <see cref="DateTime"/> with the day part set to the last day in that month.</returns>
        public DateTime LastDayOfMonth() => dateTime.SetDay(DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

        /// <summary>
        /// Adds the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="days">Number of business days to be added.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public DateTime AddBusinessDays(int days)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    dateTime = dateTime.AddDays(sign);
                }
                while (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            }

            return dateTime;
        }

        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public DateTime SubtractBusinessDays(int days) => dateTime.AddBusinessDays(-days);

        /// <summary>
        /// Determine if a <see cref="DateTime"/> is in the future.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="dateTime"/> is in the future; otherwise <c>false</c>.</returns>
        public bool IsInFuture() => dateTime.ToUniversalTime() > DateTime.UtcNow;

        /// <summary>
        /// Determine if a <see cref="DateTime"/> is in the past.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="dateTime"/> is in the past; otherwise <c>false</c>.</returns>
        public bool IsInPast() => dateTime.ToUniversalTime() < DateTime.UtcNow;

        /// <summary>
        /// Rounds <paramref name="dateTime"/> to the nearest <see cref="RoundTo"/>.
        /// </summary>
        /// <returns>The rounded <see cref="DateTime"/>.</returns>
        public DateTime Round(RoundTo rt)
        {
            DateTime rounded;

            switch (rt)
            {
                case RoundTo.Second:
                    {
                        rounded = new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
                        if (dateTime.Millisecond >= 500)
                        {
                            rounded = rounded.AddSeconds(1);
                        }

                        break;
                    }

                case RoundTo.Minute:
                    {
                        rounded = new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
                        if (dateTime.Second >= 30)
                        {
                            rounded = rounded.AddMinutes(1);
                        }

                        break;
                    }

                case RoundTo.Hour:
                    {
                        rounded = new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind);
                        if (dateTime.Minute >= 30)
                        {
                            rounded = rounded.AddHours(1);
                        }

                        break;
                    }

                case RoundTo.Day:
                    {
                        rounded = new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);
                        if (dateTime.Hour >= 12)
                        {
                            rounded = rounded.AddDays(1);
                        }

                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(rt));
                    }
            }

            return rounded;
        }

        /// <summary>
        /// Returns a DateTime adjusted to the beginning of the week.
        /// </summary>
        /// <returns>A DateTime instance adjusted to the beginning of the current week.</returns>
        /// <remarks>the beginning of the week is controlled by the current Culture.</remarks>
        public DateTime FirstDayOfWeek(DayOfWeek? firstDayOfWeek = null)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var firstDay = firstDayOfWeek ?? currentCulture.DateTimeFormat.FirstDayOfWeek;
            var offset = dateTime.DayOfWeek < firstDay ? 7 : 0;
            var numberOfDaysSinceBeginningOfTheWeek = dateTime.DayOfWeek + offset - firstDay;

            return dateTime.AddDays(-numberOfDaysSinceBeginningOfTheWeek);
        }

        /// <summary>
        /// Returns the first day of the year keeping the time component intact. Eg, 2011-02-04T06:40:20.005 => 2011-01-01T06:40:20.005.
        /// </summary>
        public DateTime FirstDayOfYear() => dateTime.SetDate(dateTime.Year, 1, 1);

        /// <summary>
        /// Returns the last day of the week keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T06:40:20.005.
        /// </summary>
        public DateTime LastDayOfWeek(DayOfWeek? firstDayOfWeek = null) => dateTime.FirstDayOfWeek(firstDayOfWeek).AddDays(6);

        /// <summary>
        /// Returns the last day of the year keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T06:40:20.005.
        /// </summary>
        public DateTime LastDayOfYear() => dateTime.SetDate(dateTime.Year, 12, 31);

        /// <summary>
        /// Determines whether the <see cref="DateTime"/> is the last day of the week. The beginning of the week is determined by the current culture or can be specified by passing a <see cref="DayOfWeek"/> value as parameter.
        /// </summary>
        /// <param name="firstDayOfWeek">The first day of the week. If null, the current culture's first day of the week is used.</param>
        /// <returns><c>true</c> if the <see cref="DateTime"/> is the last day of the week; otherwise, <c>false</c>.</returns>
        public bool IsLastDayOfWeek(DayOfWeek? firstDayOfWeek = null) => dateTime.DayOfWeek == dateTime.LastDayOfWeek(firstDayOfWeek).DayOfWeek;

        /// <summary>
        /// Determines whether the <see cref="DateTime"/> is the first day of the week. The beginning of the week is determined by the current culture or can be specified by passing a <see cref="DayOfWeek"/> value as parameter.
        /// </summary>
        /// <param name="firstDayOfWeek">The first day of the week. If null, the current culture's first day of the week is used.</param>
        /// <returns><c>true</c> if the <see cref="DateTime"/> is the first day of the week; otherwise, <c>false</c>.</returns>
        public bool IsFirstDayOfWeek(DayOfWeek? firstDayOfWeek = null) => dateTime.DayOfWeek == dateTime.FirstDayOfWeek(firstDayOfWeek).DayOfWeek;

        /// <summary>
        /// Determines whether the <see cref="DateTime"/> falls on a weekend day (Saturday or Sunday).
        /// </summary>
        /// <returns><c>true</c> if the <see cref="DateTime"/> is a weekend day; otherwise, <c>false</c>.</returns>
        public bool IsWeekend() => dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        /// <summary>
        /// Returns the previous month keeping the time component intact. Eg, 2010-01-20T06:40:20.005 => 2009-12-20T06:40:20.005
        /// If the previous month doesn't have that many days the last day of the previous month is used. Eg, 2009-03-31T06:40:20.005 => 2009-02-28T06:40:20.005.
        /// </summary>
        public DateTime PreviousMonth()
        {
            var year = dateTime.Month == 1 ? dateTime.Year - 1 : dateTime.Year;

            var month = dateTime.Month == 1 ? 12 : dateTime.Month - 1;

            var firstDayOfPreviousMonth = dateTime.SetDate(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.LastDayOfMonth().Day;

            var day = dateTime.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : dateTime.Day;

            return firstDayOfPreviousMonth.SetDay(day);
        }

        /// <summary>
        /// Returns the next month keeping the time component intact. Eg, 2012-12-05T06:40:20.005 => 2013-01-05T06:40:20.005
        /// If the next month doesn't have that many days the last day of the next month is used. Eg, 2013-01-31T06:40:20.005 => 2013-02-28T06:40:20.005.
        /// </summary>
        public DateTime NextMonth()
        {
            var year = dateTime.Month == 12 ? dateTime.Year + 1 : dateTime.Year;

            var month = dateTime.Month == 12 ? 1 : dateTime.Month + 1;

            var firstDayOfNextMonth = dateTime.SetDate(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfNextMonth.LastDayOfMonth().Day;

            var day = dateTime.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : dateTime.Day;

            return firstDayOfNextMonth.SetDay(day);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is in the same day (day + month + year) as current. The time component is ignored. Eg, 2011-12-24T06:40:20.005 and 2011-12-24T23:59:59.999 => True, 2011-12-24T06:40:20.005 and 2011-12-25T00:00:00.000 => False.
        /// </summary>
        public bool IsToday()
            => dateTime.Kind switch
            {
                DateTimeKind.Local => dateTime.SameDay(DateTime.Now),
                _ => dateTime.SameDay(DateTime.UtcNow)
            };

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same millisecond (millisecond + second + minute + hour + day + month + year) then current. Eg, 2011-12-24T06:40:20.005 and 2011-12-24T06:40:20.005 => True, 2011-12-24T06:40:20.005 and 2011-12-24T06:40:20.006 => False.
        /// </summary>
        /// <param name="date">The date to compare with.</param>
        /// <returns><c>true</c> if the specified date is exactly the same millisecond as the current date; otherwise, <c>false</c>.</returns>
        public bool SameMilliSecond(DateTime date) => dateTime.SameSecond(date) && dateTime.Millisecond == date.Millisecond;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same second (second + minute + hour + day + month + year) then current. Eg, 2011-12-24T06:40:20.005 and 2011-12-24T06:40:20.006 => True, 2011-12-24T06:40:20.005 and 2011-12-24T06:40:21.005 => False.
        /// </summary>
        /// <param name="date">The date to compare with.</param>
        /// <returns><c>true</c> if the specified date is exactly the same second as the current date; otherwise, <c>false</c>.</returns>
        public bool SameSecond(DateTime date) => dateTime.SameMinute(date) && dateTime.Second == date.Second;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same minute (minute + hour + day + month + year) then current. Eg, 2011-12-24T06:40:20.005 and 2011-12-24T06:40:21.005 => True, 2011-12-24T06:40:20.005 and 2011-12-24T06:41:20.005 => False.
        /// </summary>
        /// <param name="date">The date to compare with.</param>
        /// <returns><c>true</c> if the specified date is exactly the same minute as the current date; otherwise, <c>false</c>.</returns>
        public bool SameMinute(DateTime date) => dateTime.SameHour(date) && dateTime.Minute == date.Minute;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same hour (hour + day + month + year) then current. Eg, 2011-12-24T06:40:20.005 and 2011-12-24T07:40:20.005 => True, 2011-12-24T06:40:20.005 and 2011-12-25T06:40:20.005 => False.
        /// </summary>
        /// <param name="date">The date to compare with.</param>
        /// <returns><c>true</c> if the specified date is exactly the same hour as the current date; otherwise, <c>false</c>.</returns>
        public bool SameHour(DateTime date) => dateTime.SameDay(date) && dateTime.Hour == date.Hour;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same day (day + month + year) then current.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same year then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameDay(DateTime date) => dateTime.Date == date.Date;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same month (month + year) then current. Eg, 2015-12-01 and 2014-12-01 => False.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same month and year then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameWeek(DateTime date) => date.IsAfter(dateTime.BeginningOfWeek()) && date < dateTime.EndOfWeek();

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same month (month + year) then current. Eg, 2015-12-01 and 2014-12-01 => False.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same month and year then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameMonth(DateTime date) => dateTime.Month == date.Month && dateTime.Year == date.Year;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same year then current. Eg, 2015-12-01 and 2015-01-01 => True.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same date then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameYear(DateTime date) => dateTime.Year == date.Year;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same year then current. Eg, 2012-12-01 and 2015-01-01 => True.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same date then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameDecade(DateTime date) => dateTime.BeginningOfDecade() == date.BeginningOfDecade();

        /// <summary>
        /// Compare two dates and return difference of days (positive or negative).
        /// </summary>
        /// <param name="dateTo">The date to compare with.</param>
        /// <returns>The difference in days between the current date and the specified date.</returns>
        public int NumberOfDays(DateTime dateTo) => Math.Abs((dateTime - dateTo).Days);

        /// <summary>
        /// Compare two dates and return difference of week (positive or negative).
        /// </summary>
        /// <param name="dateTo">The date to compare with.</param>
        /// <returns>The difference in weeks between the current date and the specified date.</returns>
        public int CompareWeek(DateTime dateTo)
        {
            var ts = dateTo.Subtract(dateTime);
            return ts.Days / 7;
        }

        /// <summary>
        /// Compare two dates and return difference of week.
        /// </summary>
        /// <param name="dt2">The date to compare with.</param>
        /// <returns>The difference in weeks between the current date and the specified date.</returns>
        public int NumberOfWeeks(DateTime dt2) => Math.Abs(dateTime.CompareWeek(dt2));

        /// <summary>
        /// Compare two dates and return difference of month (positive or negative).
        /// </summary>
        public int CompareMonth(DateTime dt2) => ((dt2.Year - dateTime.Year) * 12) + (dt2.Month - dateTime.Month);

        /// <summary>
        /// Compare two dates and return difference of month.
        /// </summary>
        public int NumberOfMonths(DateTime dt2) => Math.Abs(dateTime.CompareMonth(dt2));

        /// <summary>
        /// Compare two dates and return difference of month (positive or negative).
        /// </summary>
        public int CompareYear(DateTime dt2) => dt2.Year - dateTime.Year;

        /// <summary>
        /// Compare two dates and return difference of month.
        /// </summary>
        public int NumberOfYears(DateTime dt2) => Math.Abs(dateTime.CompareYear(dt2));

        /// <summary>
        /// Returns the date with day-of-month discarded (keeps month and year) — useful for monthly calculations.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> set to the first day of the month at midnight.</returns>
        public DateTime DiscardDayTime() => new(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);

        /// <summary>
        /// Returns the date with the time portion discarded (time set to midnight).
        /// </summary>
        /// <returns>A <see cref="DateTime"/> with time portion set to 00:00:00.</returns>
        public DateTime DiscardTime() => dateTime.Date;

        /// <summary>
        /// Determines whether the date lies within the inclusive range defined by start and end.
        /// </summary>
        /// <param name="start">Range start.</param>
        /// <param name="end">Range end.</param>
        /// <param name="discardTime">If <c>true</c> compare only date parts for equality to boundaries.</param>
        /// <returns><c>true</c> when the date is within the range; otherwise <c>false</c>.</returns>
        public bool InRange(DateTime start, DateTime end, bool discardTime = true) => (discardTime && (dateTime.SameDay(start) || dateTime.SameDay(end))) || ComparableExtensions.InRange(dateTime, start, end);

        /// <summary>
        /// Calculates the age in years from a birthdate to now (UTC based).
        /// </summary>
        /// <returns>The number of full years elapsed since <paramref name="dateTime"/>.</returns>
        public int Age()
        {
            var today = DateTime.UtcNow;

            var age = today.Year - dateTime.Year;

            if (today.Month < dateTime.Month || (today.Month == dateTime.Month && today.Day < dateTime.Day))
                age--;

            return age;
        }

        /// <summary>
        /// Generates a sequence of DateTime values starting from a minimum value and ending at a maximum value, with a specified step and time unit. The method first checks if the step is zero and throws an exception if it is. It then defines an increment function based on the specified time unit, which will be used to generate the next value in the sequence. Finally, it uses a loop to yield each value in the range until it reaches the maximum value, taking into account whether the step is positive or negative.
        /// </summary>
        /// <param name="max">The maximum value of the range.</param>
        /// <param name="step">The step value for each iteration.</param>
        /// <param name="unit">The time unit for the step.</param>
        /// <returns>A sequence of DateTime values within the specified range.</returns>
        public IEnumerable<DateTime> Range(DateTime max, int step = 1, TimeUnit unit = TimeUnit.Day)
        {
            ArgumentOutOfRangeException.ThrowIfZero(step);

            Func<DateTime, DateTime> increment = unit switch
            {
                TimeUnit.Millisecond => x => x.AddMilliseconds(step),
                TimeUnit.Second => x => x.AddSeconds(step),
                TimeUnit.Minute => x => x.AddMinutes(step),
                TimeUnit.Hour => x => x.AddHours(step),
                TimeUnit.Day => x => x.AddDays(step),
                TimeUnit.Week => x => x.AddDays(step * 7),
                TimeUnit.Month => x => x.AddMonths(step),
                TimeUnit.Year => x => x.AddYears(step),
                _ => null!
            };

            if (step > 0)
            {
                for (var i = dateTime; i <= max; i = increment(i))
                    yield return i;
            }
            else
            {
                for (var i = dateTime; i >= max; i = increment(i))
                    yield return i;
            }
        }
    }
}
