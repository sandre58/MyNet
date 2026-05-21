// -----------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DateOnlyExtensions
{
    extension(DateOnly date)
    {
        /// <summary>
        /// Returns the last day of the decade for the provided date.
        /// </summary>
        public DateOnly EndOfDecade() => date.SetYear(date.Year - (date.Year % 10) + 9).EndOfYear();

        /// <summary>
        /// Returns the first day of the decade for the provided date.
        /// </summary>
        public DateOnly BeginningOfDecade() => date.SetYear(date.Year - (date.Year % 10)).BeginningOfYear();

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc.) in the next calendar year.
        /// If that day does not exist in next year in same month, number of missing days is added to the last day in same month next year.
        /// </summary>
        public DateOnly NextYear()
        {
            var nextYear = date.Year + 1;
            var numberOfDaysInSameMonthNextYear = DateTime.DaysInMonth(nextYear, date.Month);

            if (numberOfDaysInSameMonthNextYear >= date.Day) return new(nextYear, date.Month, date.Day);
            var differenceInDays = date.Day - numberOfDaysInSameMonthNextYear;
            var dateTime = new DateOnly(nextYear, date.Month, numberOfDaysInSameMonthNextYear);
            return dateTime.AddDays(differenceInDays);
        }

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc.) in the previous calendar year.
        /// If that day does not exist in previous year in same month, number of missing days is added to the last day in same month previous year.
        /// </summary>
        public DateOnly PreviousYear()
        {
            var previousYear = date.Year - 1;
            var numberOfDaysInSameMonthPreviousYear = DateTime.DaysInMonth(previousYear, date.Month);

            if (numberOfDaysInSameMonthPreviousYear >= date.Day)
                return new(previousYear, date.Month, date.Day);
            var differenceInDays = date.Day - numberOfDaysInSameMonthPreviousYear;
            var dateTime = new DateOnly(previousYear, date.Month, numberOfDaysInSameMonthPreviousYear);
            return dateTime.AddDays(differenceInDays);
        }

        /// <summary>
        /// Returns the next calendar day.
        /// </summary>
        /// <returns>The date one day after <paramref name="date"/>.</returns>
        public DateOnly NextDay() => date.AddDays(1);

        /// <summary>
        /// Returns the previous calendar day.
        /// </summary>
        /// <returns>The date one day before <paramref name="date"/>.</returns>
        public DateOnly PreviousDay() => date.AddDays(-1);

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateOnly Next(DayOfWeek day)
        {
            do
            {
                date = date.NextDay();
            }
            while (date.DayOfWeek != day);

            return date;
        }

        /// <summary>
        /// Returns the next occurrence of a specific day and month.
        /// </summary>
        public DateOnly Next(int day, int month)
        {
            do
            {
                date = date.NextDay();
            }
            while (date.Month != month || date.Day != day);

            return date;
        }

        /// <summary>
        /// Returns the previous occurrence of the specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateOnly Previous(DayOfWeek day)
        {
            do
            {
                date = date.PreviousDay();
            }
            while (date.DayOfWeek != day);

            return date;
        }

        /// <summary>
        /// Returns the previous occurrence of the specified day/month.
        /// </summary>
        public DateOnly Previous(int day, int month)
        {
            do
            {
                date = date.PreviousDay();
            }
            while (date.Month != month || date.Day != day);

            return date;
        }

        /// <summary>
        /// Increases the date by one calendar week (number of days determined by <see cref="DateTimeHelper.DaysPerWeek"/>).
        /// </summary>
        public DateOnly WeekAfter() => date.AddDays(DateTimeHelper.DaysPerWeek);

        /// <summary>
        /// Decreases the date by one calendar week.
        /// </summary>
        public DateOnly WeekEarlier() => date.AddDays(-DateTimeHelper.DaysPerWeek);

        /// <summary>
        /// Returns <see cref="DateOnly"/> with changed year component.
        /// </summary>
        public DateOnly SetYear(int year) => new(year, date.Month, date.Day);

        /// <summary>
        /// Returns <see cref="DateOnly"/> with changed month component.
        /// </summary>
        public DateOnly SetMonth(int month) => new(date.Year, month, date.Day);

        /// <summary>
        /// Returns <see cref="DateOnly"/> with changed day component.
        /// </summary>
        public DateOnly SetDay(int day) => new(date.Year, date.Month, day);

        /// <summary>
        /// Determines whether the current date is strictly before the specified <see cref="DateTime"/> (converted to a <see cref="DateOnly"/>).
        /// </summary>
        public bool IsBefore(DateTime toCompareWith) => date.ToDateTime(TimeOnly.MinValue).IsBefore(toCompareWith);

        /// <summary>
        /// Determines whether the current date is strictly after the specified <see cref="DateTime"/> (converted to a <see cref="DateOnly"/>).
        /// </summary>
        public bool IsAfter(DateTime toCompareWith) => date.ToDateTime(TimeOnly.MinValue).IsAfter(toCompareWith);

        /// <summary>
        /// Determines whether the current date is strictly before the specified <see cref="DateOnly"/>.
        /// </summary>
        public bool IsBefore(DateOnly toCompareWith) => date.IsBefore(toCompareWith.ToDateTime(TimeOnly.MinValue));

        /// <summary>
        /// Determines whether the current date is strictly after the specified <see cref="DateOnly"/>.
        /// </summary>
        public bool IsAfter(DateOnly toCompareWith) => date.IsAfter(toCompareWith.ToDateTime(TimeOnly.MinValue));

        /// <summary>
        /// Returns a <see cref="DateTime"/> with the time part set to the provided <see cref="TimeOnly"/>.
        /// </summary>
        /// <param name="time">The time to apply.</param>
        /// <returns>A <see cref="DateTime"/> representing the combined date and time.</returns>
        public DateTime At(TimeOnly time) => date.ToDateTime(time);

        /// <summary>
        /// Returns a <see cref="DateTime"/> with the time part set to the provided <see cref="TimeOnly"/> and specified <see cref="DateTimeKind"/>.
        /// </summary>
        public DateTime At(TimeOnly time, DateTimeKind kind) => date.ToDateTime(time, kind);

        /// <summary>
        /// Returns a <see cref="DateTime"/> representing the provided hour and minute on the same date.
        /// </summary>
        public DateTime At(int hour, int minute) => date.At(new(hour, minute));

        /// <summary>
        /// Returns a <see cref="DateTime"/> representing the provided hour, minute and second on the same date.
        /// </summary>
        public DateTime At(int hour, int minute, int second) => date.At(new(hour, minute, second));

        /// <summary>
        /// Returns a <see cref="DateTime"/> representing the provided hour, minute, second and milliseconds on the same date.
        /// </summary>
        public DateTime At(int hour, int minute, int second, int milliseconds) => date.At(new(hour, minute, second, milliseconds));

        /// <summary>
        /// Gets the earliest possible <see cref="DateTime"/> for the date (midnight).
        /// </summary>
        public DateTime BeginningOfDay() => date.At(TimeOnly.MinValue);

        /// <summary>
        /// Gets the latest possible <see cref="DateTime"/> for the date (end of day).
        /// </summary>
        public DateTime EndOfDay() => date.At(TimeOnly.MaxValue);

        /// <summary>
        /// Sets the day of the <see cref="DateOnly"/> to the first day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html.
        /// </summary>
        /// <returns>given <see cref="DateTime"/> with the day part set to the first day in the quarter.</returns>
        public DateOnly BeginningOfQuarter()
        {
            var currentQuarter = ((date.Month - 1) / 3) + 1;
            return new(date.Year, (3 * currentQuarter) - 2, 1);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateOnly"/> to the first day in that month.
        /// </summary>
        /// <returns>given <see cref="DateOnly"/> with the day part set to the first day in that month.</returns>
        public DateOnly BeginningOfMonth() => date.SetDay(1);

        /// <summary>
        /// Sets the day of the <see cref="DateOnly"/> to the last day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html.
        /// </summary>
        /// <returns>given <see cref="DateOnly"/> with the day part set to the last day in the quarter.</returns>
        public DateOnly EndOfQuarter()
        {
            var currentQuarter = ((date.Month - 1) / 3) + 1;
            var firstDay = new DateOnly(date.Year, (3 * currentQuarter) - 2, 1);
            return new(firstDay.Year, firstDay.Month + 2, 1);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateOnly"/> to the last day in that month.
        /// </summary>
        /// <returns>given <see cref="DateOnly"/> with the day part set to the last day in that month.</returns>
        public DateOnly EndOfMonth() => date.SetDay(DateTime.DaysInMonth(date.Year, date.Month));

        /// <summary>
        /// Adds the given number of business days to the <see cref="DateOnly"/>.
        /// </summary>
        /// <param name="days">Number of business days to be added.</param>
        /// <returns>A <see cref="DateOnly"/> increased by a given number of business days.</returns>
        public DateOnly AddBusinessDays(int days)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    date = date.AddDays(sign);
                }
                while (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            }

            return date;
        }

        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateOnly"/>.
        /// </summary>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <returns>A <see cref="DateOnly"/> increased by a given number of business days.</returns>
        public DateOnly SubtractBusinessDays(int days) => date.AddBusinessDays(-days);

        /// <summary>
        /// Determine if a <see cref="DateOnly"/> is in the future compared to the current UTC date.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="date"/> is in the future; otherwise <c>false</c>.</returns>
        public bool IsInFuture() => date.At(TimeOnly.MinValue) > DateTime.UtcNow;

        /// <summary>
        /// Determine if a <see cref="DateOnly"/> is in the past compared to the current UTC date.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="date"/> is in the past; otherwise <c>false</c>.</returns>
        public bool IsInPast() => date.At(TimeOnly.MinValue) < DateTime.UtcNow;

        /// <summary>
        /// Returns a DateOnly adjusted to the beginning of the week.
        /// </summary>
        /// <returns>A DateOnly instance adjusted to the beginning of the current week.</returns>
        /// <remarks>the beginning of the week is controlled by the current Culture.</remarks>
        public DateOnly BeginningOfWeek(DayOfWeek? firstDayOfWeek = null)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var firstDay = firstDayOfWeek ?? currentCulture.DateTimeFormat.FirstDayOfWeek;
            var offset = date.DayOfWeek < firstDay ? 7 : 0;
            var numberOfDaysSinceBeginningOfTheWeek = date.DayOfWeek + offset - firstDay;

            return date.AddDays(-numberOfDaysSinceBeginningOfTheWeek);
        }

        /// <summary>
        /// Returns the first day of the year keeping the time component intact.
        /// </summary>
        /// <returns>New date.</returns>
        public DateOnly BeginningOfYear() => new(date.Year, 1, 1);

        /// <summary>
        /// Returns the last day of the week keeping the time component intact.
        /// </summary>
        /// <returns>New date.</returns>
        public DateOnly EndOfWeek(DayOfWeek? firstDayOfWeek = null) => date.BeginningOfWeek(firstDayOfWeek).AddDays(6);

        /// <summary>
        /// Returns the last day of the year keeping the time component intact.
        /// </summary>
        /// <returns>New date.</returns>
        public DateOnly EndOfYear() => new(date.Year, 12, 31);

        /// <summary>
        /// Determines whether the current date is the last day of the week.
        /// </summary>
        public bool IsLastDayOfWeek(DayOfWeek? firstDayOfWeek = null) => date.DayOfWeek == date.EndOfWeek(firstDayOfWeek).DayOfWeek;

        /// <summary>
        /// Determines whether the current date is the first day of the week.
        /// </summary>
        public bool IsFirstDayOfWeek(DayOfWeek? firstDayOfWeek = null) => date.DayOfWeek == date.EndOfWeek(firstDayOfWeek).DayOfWeek;

        /// <summary>
        /// Determines whether the current date falls on a weekend (Saturday or Sunday).
        /// </summary>
        public bool IsWeekend() => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        /// <summary>
        /// Returns the previous month keeping the time component intact.
        /// </summary>
        /// <returns>New date.</returns>
        public DateOnly PreviousMonth()
        {
            var year = date.Month == 1 ? date.Year - 1 : date.Year;

            var month = date.Month == 1 ? 12 : date.Month - 1;

            var firstDayOfPreviousMonth = new DateOnly(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.EndOfMonth().Day;

            var day = date.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : date.Day;

            return firstDayOfPreviousMonth.SetDay(day);
        }

        /// <summary>
        /// Returns the next month keeping the time component intact.
        /// </summary>
        /// <returns>New date.</returns>
        public DateOnly NextMonth()
        {
            var year = date.Month == 12 ? date.Year + 1 : date.Year;

            var month = date.Month == 12 ? 1 : date.Month + 1;

            var firstDayOfNextMonth = new DateOnly(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfNextMonth.EndOfMonth().Day;

            var day = date.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : date.Day;

            return firstDayOfNextMonth.SetDay(day);
        }

        /// <summary>
        /// Determines whether the date corresponds to today's day (UTC based).
        /// </summary>
        public bool IsToday() => date.Day == DateTime.UtcNow.Day;

        /// <summary>
        /// Determines whether the specified date falls in the same week as the current date.
        /// </summary>
        /// <param name="date1">Value to compare with.</param>
        /// <returns><c>true</c> if the specified date is within the same week; otherwise <c>false</c>.</returns>
        public bool SameWeek(DateOnly date1) => date1.IsAfter(date.BeginningOfWeek()) && date1.IsBefore(date.EndOfWeek());

        /// <summary>
        /// Determines whether the specified date is in the same month as the current date.
        /// </summary>
        public bool SameMonth(DateOnly otherDate) => date.Month == otherDate.Month && date.Year == otherDate.Year;

        /// <summary>
        /// Determines whether the specified date is in the same year as the current date.
        /// </summary>
        public bool SameYear(DateOnly otherDate) => date.Year == otherDate.Year;

        /// <summary>
        /// Determines whether the specified date is in the same decade as the current date.
        /// </summary>
        public bool SameDecade(DateOnly otherDate) => date.BeginningOfDecade() == otherDate.BeginningOfDecade();

        /// <summary>
        /// Returns the absolute number of days between two dates.
        /// </summary>
        public int NumberOfDays(DateOnly dateTo) => Math.Abs((date.ToDateTime(TimeOnly.MinValue) - dateTo.ToDateTime(TimeOnly.MinValue)).Days);

        /// <summary>
        /// Returns the difference in weeks between two dates (signed integer).
        /// </summary>
        public int CompareWeek(DateOnly dateTo)
        {
            var ts = dateTo.ToDateTime(TimeOnly.MinValue).Subtract(date.ToDateTime(TimeOnly.MinValue));
            return ts.Days / 7;
        }

        /// <summary>
        /// Returns the absolute number of weeks between two dates.
        /// </summary>
        public int NumberOfWeeks(DateOnly dateTo) => Math.Abs(date.CompareWeek(dateTo));

        /// <summary>
        /// Compare two dates and return difference of months (signed integer).
        /// </summary>
        public int CompareMonth(DateTime dateTo) => ((dateTo.Year - date.Year) * 12) + (dateTo.Month - date.Month);

        /// <summary>
        /// Returns the absolute number of months between two dates.
        /// </summary>
        public int NumberOfMonths(DateTime dateTo) => Math.Abs(date.CompareMonth(dateTo));

        /// <summary>
        /// Compare two dates and return difference of years (signed integer).
        /// </summary>
        public int CompareYear(DateTime dateTo) => dateTo.Year - date.Year;

        /// <summary>
        /// Returns the absolute number of years between two dates.
        /// </summary>
        public int NumberOfYears(DateTime dateTo) => Math.Abs(date.CompareYear(dateTo));

        /// <summary>
        /// Calculates the age in years from a birthdate to now (UTC based).
        /// </summary>
        /// <returns>The number of full years elapsed since <paramref name="date"/>.</returns>
        public int Age()
        {
            var today = DateTime.UtcNow;

            var age = today.Year - date.Year;

            if (today.Month < date.Month || (today.Month == date.Month && today.Day < date.Day))
                age--;

            return age;
        }

    /// <summary>
    /// Generates a sequence of DateOnly values starting from a minimum value and ending at a maximum value, with a specified step and time unit. The method first checks if the step is zero and throws an exception if it is. It then defines an increment function based on the specified time unit, which will be used to generate the next value in the sequence. Finally, it uses a loop to yield each value in the range until it reaches the maximum value, taking into account whether the step is positive or negative. Note that only Day, Week, Month, and Year time units are valid for DateOnly values; using other time units will throw an InvalidOperationException.
    /// </summary>
    /// <param name="max">The maximum value of the range.</param>
    /// <param name="step">The step value for each iteration.</param>
    /// <param name="unit">The time unit for the step.</param>
    /// <returns>A sequence of DateOnly values within the specified range.</returns>
    public IEnumerable<DateOnly> Range(DateOnly max, int step = 1, TimeUnit unit = TimeUnit.Day)
    {
        ArgumentOutOfRangeException.ThrowIfZero(step);

        Func<DateOnly, DateOnly> increment = unit switch
        {
            TimeUnit.Day => x => x.AddDays(step),
            TimeUnit.Week => x => x.AddDays(step * 7),
            TimeUnit.Month => x => x.AddMonths(step),
            TimeUnit.Year => x => x.AddYears(step),
            TimeUnit.Millisecond => throw new InvalidOperationException(),
            TimeUnit.Second => throw new InvalidOperationException(),
            TimeUnit.Minute => throw new InvalidOperationException(),
            TimeUnit.Hour => throw new InvalidOperationException(),
            _ => x => x.AddDays(step)
        };

        if (step > 0)
        {
            for (var i = date; i <= max; i = increment(i))
                yield return i;
        }
        else
        {
            for (var i = date; i >= max; i = increment(i))
                yield return i;
        }
    }
    }
}
