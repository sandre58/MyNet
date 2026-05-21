// -----------------------------------------------------------------------
// <copyright file="ITimeZoneService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Globalization.DateTime;

/// <summary>
/// Defines a service for managing the application's time zone settings.
/// </summary>
public interface ITimeZoneService
{
    /// <summary>
    /// Gets the current application date and time, adjusted to the current time zone.
    /// </summary>
    System.DateTime Now { get; }

    /// <summary>
    /// Gets the current application time zone.
    /// </summary>
    TimeZoneInfo CurrentTimeZone { get; }

    /// <summary>
    /// Occurs when the current time zone changes.
    /// </summary>
    event EventHandler<TimeZoneChangedEventArgs> TimeZoneChanged;

    /// <summary>
    /// Changes the current time zone.
    /// </summary>
    /// <param name="timeZone">Target time zone.</param>
    void SetTimeZone(TimeZoneInfo timeZone);

    /// <summary>
    /// Changes the current time zone by time zone ID.
    /// </summary>
    /// <param name="timeZoneId">Target time zone ID.</param>
    void SetTimeZone(string timeZoneId);

    /// <summary>
    /// Converts a UTC date to the current time zone.
    /// </summary>
    DateTimeOffset FromUtc(DateTimeOffset utcDateTime);

    /// <summary>
    /// Converts a date from the current time zone to UTC.
    /// </summary>
    DateTimeOffset ToUtc(DateTimeOffset localDateTime);

    /// <summary>
    /// Converts a date from a source time zone to a destination time zone.
    /// </summary>
    /// <param name="dateTime">The date and time value to convert.</param>
    /// <param name="sourceTimeZone">The source time zone of the date and time value.</param>
    /// <param name="destinationTimeZone">The destination time zone to convert the date and time value to.</param>
    /// <returns>The converted date and time value in the destination time zone.</returns>
    DateTimeOffset Convert(DateTimeOffset dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone);
}
