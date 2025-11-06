// -----------------------------------------------------------------------
// <copyright file="DateTimeHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Localization;
using MyNet.Utilities.Sequences;
using MyNet.Utilities.Units;

namespace MyNet.Utilities.Helpers;

public static class DateTimeHelper
{
    public static DateTimeFormatInfo GetCurrentDateTimeFormatInfo()
    {
        if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar) return CultureInfo.CurrentCulture.DateTimeFormat;
        Calendar? calendar =
            CultureInfo.CurrentCulture.OptionalCalendars.OfType<GregorianCalendar>().FirstOrDefault();
        var cultureName = calendar is null ? CultureInfo.InvariantCulture.Name : CultureInfo.CurrentCulture.Name;
        var dt = new CultureInfo(cultureName).DateTimeFormat;
        dt.Calendar = calendar ?? new GregorianCalendar();
        return dt;
    }

    public static DateTime Max(DateTime date1, DateTime date2) =>
        date1 > date2
            ? date1
            : date2;

    public static DateOnly Max(DateOnly date1, DateOnly date2) =>
        date1 > date2
            ? date1
            : date2;

    public static TimeOnly Max(TimeOnly time1, TimeOnly time2) =>
        time1 > time2
            ? time1
            : time2;

    public static TimeSpan Max(TimeSpan time1, TimeSpan time2) =>
        time1 > time2
            ? time1
            : time2;

    public static DateTime Min(DateTime date1, DateTime date2) =>
        date1 > date2
            ? date2
            : date1;

    public static DateOnly Min(DateOnly date1, DateOnly date2) =>
        date1 > date2
            ? date2
            : date1;

    public static TimeOnly Min(TimeOnly time1, TimeOnly time2) =>
        time1 > time2
            ? time2
            : time1;

    public static TimeSpan Min(TimeSpan time1, TimeSpan time2) =>
        time1 > time2
            ? time2
            : time1;

    public static bool IsConsecutiveDays(this IEnumerable<DateTime> dates) => dates.Order().Zip(dates.Order().Skip(1), (a, b) => (a, b)).All(pair => (pair.b - pair.a).TotalDays == 1);

    public static IEnumerable<DateTime> Range(DateTime min, DateTime max, int step = 1, TimeUnit unit = TimeUnit.Day)
    {
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

        for (var i = min; i <= max; i = increment.Invoke(i))
            yield return i;
    }

    public static IEnumerable<DateOnly> Range(DateOnly min, DateOnly max, int step = 1, TimeUnit unit = TimeUnit.Day)
    {
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
        for (var i = min; i <= max; i = increment.Invoke(i))
            yield return i;
    }

    public static IEnumerable<TimeOnly> Range(TimeOnly min, TimeOnly max, int step = 1, TimeUnit unit = TimeUnit.Hour)
    {
        Func<TimeOnly, TimeOnly> increment = unit switch
        {
            TimeUnit.Millisecond => x => x.Add(step.Milliseconds()),
            TimeUnit.Second => x => x.Add(step.Seconds()),
            TimeUnit.Minute => x => x.AddMinutes(step),
            TimeUnit.Hour => x => x.AddHours(step),
            TimeUnit.Day => throw new InvalidOperationException(),
            TimeUnit.Week => throw new InvalidOperationException(),
            TimeUnit.Month => throw new InvalidOperationException(),
            TimeUnit.Year => throw new InvalidOperationException(),
            _ => x => x.AddHours(step)
        };
        for (var i = min; i <= max; i = increment.Invoke(i))
            yield return i;
    }

    public static Interval<int> GetDecade(int year)
    {
        var start = year / 10 * 10;
        return new(start, start + 10);
    }

    public static Interval<int> GetCentury(int year)
    {
        var start = year / 100 * 100;
        return new(start, start + 100);
    }

    public static int NumberOfDaysInWeek() => Enum.GetValues<DayOfWeek>().Length;

    public static int MaxNumberOfWeeksPerMonth() => 6;

    public static int MinNumberOfWeeksPerMonth() => 4;

    public static string TranslateDatePattern(string key, CultureInfo culture)
    {
        var format = culture.DateTimeFormat;
        var prop = format.GetType().GetProperty(key);
        return prop != null ? prop.GetValue(format)?.ToString() ?? string.Empty : TranslationService.Get(culture)[key];
    }
}
