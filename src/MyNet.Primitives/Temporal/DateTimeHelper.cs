// -----------------------------------------------------------------------
// <copyright file="DateTimeHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Linq;

namespace MyNet.Primitives.Temporal;

/// <summary>
/// Helper class for DateTime operations.
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Defines the number of days in a week, and the maximum and minimum number of weeks in a month.
    /// </summary>
    public const int DaysPerWeek = 7;

    /// <summary>
    /// Defines the average number of days in a year and a month. The average number of days in a year is calculated based on the Gregorian calendar, which has 365 days in a common year and 366 days in a leap year, resulting in an average of approximately 365.2425 days per year. The average number of days in a month is calculated by dividing the average number of days in a year by 12, resulting in approximately 30.436875 days per month.
    /// </summary>
    public const double DaysPerYear = 365.2425; // see https://en.wikipedia.org/wiki/Gregorian_calendar

    /// <summary>
    /// Defines the average number of days in a month, calculated by dividing the average number of days in a year by 12. This results in approximately 30.436875 days per month, which is an average value that accounts for the varying lengths of different months in the Gregorian calendar.
    /// </summary>
    public const double DaysPerMonth = DaysPerYear / 12;

    /// <summary>
    /// Defines the maximum and minimum number of weeks in a month. The maximum is 6 because some months can start on the last day of the week and end on the first day of the next week, resulting in 6 weeks in total. The minimum is 4 because some months can start on the first day of the week and end on the last day of the same week, resulting in 4 weeks in total.
    /// </summary>
    public const int MaxNumberOfWeeksPerMonth = 6;

    /// <summary>
    /// Defines the minimum number of weeks in a month. The minimum is 4 because some months can start on the first day of the week and end on the last day of the same week, resulting in 4 weeks in total.
    /// </summary>
    public const int MinNumberOfWeeksPerMonth = 4;

    /// <summary>
    /// Gets the DateTimeFormatInfo for the current culture, ensuring that it uses a Gregorian calendar. If the current culture's calendar is not Gregorian, it will attempt to find a Gregorian calendar in the optional calendars of the current culture. If found, it will use that calendar; otherwise, it will fall back to the invariant culture with a Gregorian calendar.
    /// </summary>
    /// <remarks>This method ensures that the returned DateTimeFormatInfo always uses a Gregorian calendar, which is useful for consistent date and time formatting across different cultures.</remarks>
    /// <returns>The DateTimeFormatInfo for the current culture, using a Gregorian calendar.</returns>
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
}
