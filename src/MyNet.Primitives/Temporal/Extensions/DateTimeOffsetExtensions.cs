// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Primitives.Temporal;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DateTimeOffsetExtensions
{
    extension(DateTimeOffset dateTimeOffset)
    {
        /// <summary>
        /// Returns a new <see cref="DateTime"/> that adds the value of the specified <see cref="FluentTimeSpan"/> to the value of this instance.
        /// </summary>
        public DateTimeOffset AddFluentTimeSpan(FluentTimeSpan timeSpan) => dateTimeOffset.AddMonths(timeSpan.Months)
            .AddYears(timeSpan.Years)
            .Add(timeSpan.TimeSpan);

        /// <summary>
        /// Returns a new <see cref="DateTime"/> that subtracts the value of the specified <see cref="FluentTimeSpan"/> to the value of this instance.
        /// </summary>
        public DateTimeOffset SubtractFluentTimeSpan(FluentTimeSpan timeSpan) => dateTimeOffset.AddMonths(-timeSpan.Months)
            .AddYears(-timeSpan.Years)
            .Subtract(timeSpan.TimeSpan);

        /// <summary>
        /// Returns the very end of the given day (the last millisecond of the last hour for the given <see cref="DateTimeOffset"/>).
        /// </summary>
        public DateTimeOffset EndOfDay() => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 23, 59, 59, 999, dateTimeOffset.Offset);

        /// <summary>
        /// Returns the Start of the given day (the first millisecond of the given <see cref="DateTimeOffset"/>).
        /// </summary>
        public DateTimeOffset BeginningOfDay() => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, dateTimeOffset.Offset);

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc.) in the next calendar year.
        /// If that day does not exist in next year in same month, number of missing days is added to the last day in same month next year.
        /// </summary>
        public DateTimeOffset NextYear()
        {
            var nextYear = dateTimeOffset.Year + 1;
            var numberOfDaysInSameMonthNextYear = DateTime.DaysInMonth(nextYear, dateTimeOffset.Month);

            if (numberOfDaysInSameMonthNextYear >= dateTimeOffset.Day)
                return new(nextYear, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);
            var differenceInDays = dateTimeOffset.Day - numberOfDaysInSameMonthNextYear;
            var dateTimeOffset1 = new DateTimeOffset(nextYear, dateTimeOffset.Month, numberOfDaysInSameMonthNextYear, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);
            return dateTimeOffset1 + differenceInDays.Days();
        }

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc.) in the previous calendar year.
        /// If that day does not exist in previous year in same month, number of missing days is added to the last day in same month previous year.
        /// </summary>
        public DateTimeOffset PreviousYear()
        {
            var previousYear = dateTimeOffset.Year - 1;
            var numberOfDaysInSameMonthPreviousYear = DateTime.DaysInMonth(previousYear, dateTimeOffset.Month);

            if (numberOfDaysInSameMonthPreviousYear >= dateTimeOffset.Day)
                return new(previousYear, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);
            var differenceInDays = dateTimeOffset.Day - numberOfDaysInSameMonthPreviousYear;
            var dateTime = new DateTimeOffset(previousYear, dateTimeOffset.Month, numberOfDaysInSameMonthPreviousYear, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);
            return dateTime + differenceInDays.Days();
        }

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> increased by 24 hours ie Next Day.
        /// </summary>
        public DateTimeOffset NextDay() => dateTimeOffset + 1.Days();

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> decreased by 24h period ie Previous Day.
        /// </summary>
        public DateTimeOffset PreviousDay() => dateTimeOffset - 1.Days();

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateTimeOffset Next(DayOfWeek day)
        {
            do
                dateTimeOffset = dateTimeOffset.NextDay();
            while (dateTimeOffset.DayOfWeek != day);

            return dateTimeOffset;
        }

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public DateTimeOffset Previous(DayOfWeek day)
        {
            do
                dateTimeOffset = dateTimeOffset.PreviousDay();
            while (dateTimeOffset.DayOfWeek != day);

            return dateTimeOffset;
        }

        /// <summary>
        /// Increases supplied <see cref="DateTimeOffset"/> for 7 days ie returns the Next Week.
        /// </summary>
        public DateTimeOffset WeekAfter() => dateTimeOffset + 1.Weeks();

        /// <summary>
        /// Decreases supplied <see cref="DateTimeOffset"/> for 7 days ie returns the Previous Week.
        /// </summary>
        public DateTimeOffset WeekEarlier() => dateTimeOffset - 1.Weeks();

        /// <summary>
        /// Increases the <see cref="DateTimeOffset"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public DateTimeOffset IncreaseTime(TimeSpan toAdd) => dateTimeOffset + toAdd;

        /// <summary>
        /// Decreases the <see cref="DateTimeOffset"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public DateTimeOffset DecreaseTime(TimeSpan toSubtract) => dateTimeOffset - toSubtract;

        /// <summary>
        /// Returns the original <see cref="DateTimeOffset"/> with Hour part changed to supplied hour parameter.
        /// </summary>
        public DateTimeOffset SetTime(int hour) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns the original <see cref="DateTimeOffset"/> with Hour and Minute parts changed to supplied hour and minute parameters.
        /// </summary>
        public DateTimeOffset SetTime(int hour, int minute) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns the original <see cref="DateTimeOffset"/> with Hour, Minute and Second parts changed to supplied hour, minute and second parameters.
        /// </summary>
        public DateTimeOffset SetTime(int hour, int minute, int second) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns the original <see cref="DateTimeOffset"/> with Hour, Minute, Second and Millisecond parts changed to supplied hour, minute, second and millisecond parameters.
        /// </summary>
        public DateTimeOffset SetTime(int hour, int minute, int second, int millisecond) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Hour part.
        /// </summary>
        public DateTimeOffset SetHour(int hour) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Minute part.
        /// </summary>
        public DateTimeOffset SetMinute(int minute) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Second part.
        /// </summary>
        public DateTimeOffset SetSecond(int second) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Millisecond part.
        /// </summary>
        public DateTimeOffset SetMillisecond(int millisecond) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns original <see cref="DateTimeOffset"/> value with time part set to midnight (alias for <see cref="BeginningOfDay"/> method).
        /// </summary>
        public DateTimeOffset Midnight() => dateTimeOffset.BeginningOfDay();

        /// <summary>
        /// Returns original <see cref="DateTimeOffset"/> value with time part set to Noon (12:00:00h).
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> value with time part set to Noon (12:00:00h).</returns>
        public DateTimeOffset Noon() => dateTimeOffset.SetTime(12, 0, 0, 0);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Year part.
        /// </summary>
        public DateTimeOffset SetDate(int year) => new(year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Year and Month part.
        /// </summary>
        public DateTimeOffset SetDate(int year, int month) => new(year, month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Year, Month and Day part.
        /// </summary>
        public DateTimeOffset SetDate(int year, int month, int day) => new(year, month, day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Year part.
        /// </summary>
        public DateTimeOffset SetYear(int year) => new(year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Month part.
        /// </summary>
        public DateTimeOffset SetMonth(int month) => new(dateTimeOffset.Year, month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns <see cref="DateTimeOffset"/> with changed Day part.
        /// </summary>
        public DateTimeOffset SetDay(int day) => new(dateTimeOffset.Year, dateTimeOffset.Month, day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, dateTimeOffset.Offset);

        /// <summary>
        /// Returns the given <see cref="DateTimeOffset"/> with hour and minutes set At given values.
        /// </summary>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <returns><see cref="DateTimeOffset"/> with hour and minute set to given values.</returns>
        public DateTimeOffset At(int hour, int minute) => dateTimeOffset.SetTime(hour, minute);

        /// <summary>
        /// Returns the given <see cref="DateTimeOffset"/> with hour and minutes and seconds set At given values.
        /// </summary>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <returns><see cref="DateTimeOffset"/> with hour and minutes and seconds set to given values.</returns>
        public DateTimeOffset At(int hour, int minute, int second) => dateTimeOffset.SetTime(hour, minute, second);

        /// <summary>
        /// Returns the given <see cref="DateTimeOffset"/> with hour and minutes and seconds and milliseconds set At given values.
        /// </summary>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <param name="milliseconds">The milliseconds to set time to.</param>
        /// <returns><see cref="DateTimeOffset"/> with hour and minutes and seconds set to given values.</returns>
        public DateTimeOffset At(int hour, int minute, int second, int milliseconds) => dateTimeOffset.SetTime(hour, minute, second, milliseconds);

        /// <summary>
        /// Sets the day of the <see cref="DateTimeOffset"/> to the first day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html.
        /// </summary>
        /// <returns>given <see cref="DateTimeOffset"/> with the day part set to the first day in the quarter.</returns>
        public DateTimeOffset FirstDayOfQuarter()
        {
            var currentQuarter = ((dateTimeOffset.Month - 1) / 3) + 1;
            return dateTimeOffset.SetDate(dateTimeOffset.Year, (3 * currentQuarter) - 2, 1);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTimeOffset"/> to the first day in that month.
        /// </summary>
        /// <returns>given <see cref="DateTimeOffset"/> with the day part set to the first day in that month.</returns>
        public DateTimeOffset FirstDayOfMonth() => dateTimeOffset.SetDay(1);

        /// <summary>
        /// Sets the day of the <see cref="DateTimeOffset"/> to the last day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html.
        /// </summary>
        /// <returns>given <see cref="DateTimeOffset"/> with the day part set to the last day in the quarter.</returns>
        public DateTimeOffset LastDayOfQuarter()
        {
            var currentQuarter = ((dateTimeOffset.Month - 1) / 3) + 1;
            var firstDay = dateTimeOffset.SetDate(dateTimeOffset.Year, (3 * currentQuarter) - 2, 1);
            return firstDay.SetMonth(firstDay.Month + 2).LastDayOfMonth();
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTimeOffset"/> to the last day in that month.
        /// </summary>
        /// <returns>given <see cref="DateTimeOffset"/> with the day part set to the last day in that month.</returns>
        public DateTimeOffset LastDayOfMonth() => dateTimeOffset.SetDay(DateTime.DaysInMonth(dateTimeOffset.Year, dateTimeOffset.Month));

        /// <summary>
        /// Adds the given number of business days to the <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="days">Number of business days to be added.</param>
        /// <returns>A <see cref="DateTimeOffset"/> increased by a given number of business days.</returns>
        public DateTimeOffset AddBusinessDays(int days)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                    dateTimeOffset = dateTimeOffset.AddDays(sign);
                while (dateTimeOffset.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            }

            return dateTimeOffset;
        }

        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <returns>A <see cref="DateTimeOffset"/> increased by a given number of business days.</returns>
        public DateTimeOffset SubtractBusinessDays(int days) => dateTimeOffset.AddBusinessDays(-days);

        /// <summary>
        /// Determine if a <see cref="DateTimeOffset"/> is in the future.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="dateTimeOffset"/> is in the future; otherwise <c>false</c>.</returns>
        public bool IsInFuture() => dateTimeOffset > DateTimeOffset.Now;

        /// <summary>
        /// Determine if a <see cref="DateTimeOffset"/> is in the past.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="dateTimeOffset"/> is in the past; otherwise <c>false</c>.</returns>
        public bool IsInPast() => dateTimeOffset < DateTimeOffset.Now;

        /// <summary>
        /// Rounds <paramref name="dateTimeOffset"/> to the nearest <see cref="RoundTo"/>.
        /// </summary>
        /// <returns>The rounded <see cref="DateTimeOffset"/>.</returns>
        public DateTimeOffset Round(RoundTo rt)
        {
            DateTimeOffset rounded;

            switch (rt)
            {
                case RoundTo.Second:
                    {
                        rounded = new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Offset);
                        if (dateTimeOffset.Millisecond >= 500)
                            rounded = rounded.AddSeconds(1);
                        break;
                    }

                case RoundTo.Minute:
                    {
                        rounded = new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, 0, dateTimeOffset.Offset);
                        if (dateTimeOffset.Second >= 30)
                            rounded = rounded.AddMinutes(1);
                        break;
                    }

                case RoundTo.Hour:
                    {
                        rounded = new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, 0, 0, dateTimeOffset.Offset);
                        if (dateTimeOffset.Minute >= 30)
                            rounded = rounded.AddHours(1);
                        break;
                    }

                case RoundTo.Day:
                    {
                        rounded = new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, dateTimeOffset.Offset);
                        if (dateTimeOffset.Hour >= 12)
                            rounded = rounded.AddDays(1);
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
        /// Returns a DateTimeOffset adjusted to the beginning of the week.
        /// </summary>
        /// <returns>A DateTimeOffset instance adjusted to the beginning of the current week.</returns>
        /// <remarks>the beginning of the week is controlled by the current Culture.</remarks>
        public DateTimeOffset FirstDayOfWeek()
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
            var offset = dateTimeOffset.DayOfWeek < firstDayOfWeek ? 7 : 0;
            var numberOfDaysSinceBeginningOfTheWeek = dateTimeOffset.DayOfWeek + offset - firstDayOfWeek;

            return dateTimeOffset.AddDays(-numberOfDaysSinceBeginningOfTheWeek);
        }

        /// <summary>
        /// Returns the first day of the year keeping the time component intact. Eg, 2011-02-04T06:40:20.005 => 2011-01-01T06:40:20.005.
        /// </summary>
        public DateTimeOffset FirstDayOfYear() => dateTimeOffset.SetDate(dateTimeOffset.Year, 1, 1);

        /// <summary>
        /// Returns the last day of the week keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T06:40:20.005.
        /// </summary>
        public DateTimeOffset LastDayOfWeek() => dateTimeOffset.FirstDayOfWeek().AddDays(6);

        /// <summary>
        /// Returns the last day of the year keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T06:40:20.005.
        /// </summary>
        public DateTimeOffset LastDayOfYear() => dateTimeOffset.SetDate(dateTimeOffset.Year, 12, 31);

        /// <summary>
        /// Returns the previous month keeping the time component intact. Eg, 2010-01-20T06:40:20.005 => 2009-12-20T06:40:20.005
        /// If the previous month doesn't have that many days the last day of the previous month is used. Eg, 2009-03-31T06:40:20.005 => 2009-02-28T06:40:20.005.
        /// </summary>
        public DateTimeOffset PreviousMonth()
        {
            var year = dateTimeOffset.Month == 1 ? dateTimeOffset.Year - 1 : dateTimeOffset.Year;

            var month = dateTimeOffset.Month == 1 ? 12 : dateTimeOffset.Month - 1;

            var firstDayOfPreviousMonth = dateTimeOffset.SetDate(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.LastDayOfMonth().Day;

            var day = dateTimeOffset.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : dateTimeOffset.Day;

            return firstDayOfPreviousMonth.SetDay(day);
        }

        /// <summary>
        /// Returns the next month keeping the time component intact. Eg, 2012-12-05T06:40:20.005 => 2013-01-05T06:40:20.005
        /// If the next month doesn't have that many days the last day of the next month is used. Eg, 2013-01-31T06:40:20.005 => 2013-02-28T06:40:20.005.
        /// </summary>
        public DateTimeOffset NextMonth()
        {
            var year = dateTimeOffset.Month == 12 ? dateTimeOffset.Year + 1 : dateTimeOffset.Year;

            var month = dateTimeOffset.Month == 12 ? 1 : dateTimeOffset.Month + 1;

            var firstDayOfNextMonth = dateTimeOffset.SetDate(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfNextMonth.LastDayOfMonth().Day;

            var day = dateTimeOffset.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : dateTimeOffset.Day;

            return firstDayOfNextMonth.SetDay(day);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTimeOffset"/> value is exactly the same day (day + month + year) then current.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same year then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameDay(DateTimeOffset date) => dateTimeOffset.Date == date.Date;

        /// <summary>
        /// Determines whether the specified <see cref="DateTimeOffset"/> value is exactly the same month (month + year) then current. Eg, 2015-12-01 and 2014-12-01 => False.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same month and year then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameMonth(DateTimeOffset date) => dateTimeOffset.Month == date.Month && dateTimeOffset.Year == date.Year;

        /// <summary>
        /// Determines whether the specified <see cref="DateTimeOffset"/> value is exactly the same year then current. Eg, 2015-12-01 and 2015-01-01 => True.
        /// </summary>
        /// <param name="date">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is exactly the same date then current; otherwise, <c>false</c>.
        /// </returns>
        public bool SameYear(DateTimeOffset date) => dateTimeOffset.Year == date.Year;
    }
}
