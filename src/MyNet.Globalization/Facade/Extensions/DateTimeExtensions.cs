// -----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives;
using TimeOnly = System.TimeOnly;
using TimeZoneInfo = System.TimeZoneInfo;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Globalization.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DateTimeExtensions
{
    extension(System.DateTime dateTime)
    {
        /// <summary>
        /// Converts the specified <see cref="DateTime"/> to the application's current time zone.
        /// </summary>
        /// <returns>The converted <see cref="DateTime"/> in the configured current time zone.</returns>
        public System.DateTime ToTimeZone(TimeZoneInfo? timeZone = null) => GlobalizationServices.Current.Convert(dateTime.ToUniversalTime(), TimeZoneInfo.Utc, timeZone ?? GlobalizationServices.Current.CurrentTimeZone).DateTime;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> falls on the current day in the application's current time zone. This method converts the provided <see cref="DateTime"/> to the configured current time zone and checks if it is on the same day as the current date in that time zone. It returns true if both dates are on the same day, and false otherwise.
        /// </summary>
        /// <param name="timeZone">The time zone to use for the comparison. If null, the application's current time zone is used.</param>
        /// <returns>True if the specified <see cref="DateTime"/> falls on the current day in the specified or current time zone; otherwise, false.</returns>
        public bool IsTodayForTimeZone(TimeZoneInfo? timeZone = null) => dateTime.SameDay(dateTime.ToTimeZone(timeZone));
    }

    extension(TimeOnly time)
    {
        /// <summary>
        /// Converts the given <see cref="DateTime"/> from a source time zone to a destination time zone.
        /// </summary>
        /// <param name="sourceTimeZone">The source time zone.</param>
        /// <param name="destinationTimeZone">The destination time zone.</param>
        /// <returns>The converted <see cref="TimeOnly"/> value.</returns>
        public TimeOnly ToTimeZone(TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
            => GlobalizationServices.Current.Convert(System.DateTime.UtcNow.ToTimeZone(sourceTimeZone).At(time), sourceTimeZone, destinationTimeZone).TimeOfDay.ToTime();

        /// <summary>
        /// Converts the given <see cref="DateTime"/> from a source time zone to the current time zone.
        /// </summary>
        /// <param name="sourceTimeZone">The source time zone.</param>
        /// <returns>The converted <see cref="System.TimeOnly"/> value.</returns>
        public TimeOnly ToCurrentTime(TimeZoneInfo sourceTimeZone) => time.ToTimeZone(sourceTimeZone, GlobalizationServices.Current.CurrentTimeZone);
    }
}
