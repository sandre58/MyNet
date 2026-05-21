// -----------------------------------------------------------------------
// <copyright file="TimeZoneService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;

namespace MyNet.Globalization.DateTime;

/// <summary>
/// Provides a service for managing the current time zone in an application.
/// The <see cref="TimeZoneChanged"/> event is raised <b>outside</b> the internal lock to prevent deadlocks.
/// </summary>
public sealed class TimeZoneService(TimeZoneInfo initialTimeZone) : ITimeZoneService
{
    private readonly Lock _lockObject = new();

    /// <summary>Initializes a new instance of the <see cref="TimeZoneService"/> class.Initializes with the local time zone.</summary>
    public TimeZoneService()
        : this(TimeZoneInfo.Local)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TimeZoneService"/> class.Initializes with the default time zone from <see cref="GlobalizationOptions"/>.</summary>
    public TimeZoneService(GlobalizationOptions options)
        : this(options.DefaultTimeZone)
    {
    }

    /// <inheritdoc />
    public System.DateTime Now => TimeZoneInfo.ConvertTime(System.DateTime.UtcNow, CurrentTimeZone);

    /// <inheritdoc />
    public TimeZoneInfo CurrentTimeZone { get; private set; } = initialTimeZone;

    /// <inheritdoc />
    public event EventHandler<TimeZoneChangedEventArgs>? TimeZoneChanged;

    /// <inheritdoc />
    /// <remarks>
    /// This method is thread-safe. The <see cref="TimeZoneChanged"/> event is raised <b>outside</b> the internal lock.
    /// </remarks>
    public void SetTimeZone(TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        TimeZoneChangedEventArgs? args;
        lock (_lockObject)
        {
            if (CurrentTimeZone.Equals(timeZone)) return;

            var old = CurrentTimeZone;
            CurrentTimeZone = timeZone;
            args = new(old, timeZone);
        }

        // Raise event outside the lock to prevent deadlocks when subscribers access the service.
        TimeZoneChanged?.Invoke(this, args);
    }

    /// <inheritdoc />
    public void SetTimeZone(string timeZoneId) => SetTimeZone(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));

    /// <inheritdoc />
    public DateTimeOffset FromUtc(DateTimeOffset utcDateTime) => Convert(utcDateTime, TimeZoneInfo.Utc, CurrentTimeZone);

    /// <inheritdoc />
    public DateTimeOffset ToUtc(DateTimeOffset localDateTime) => Convert(localDateTime, CurrentTimeZone, TimeZoneInfo.Utc);

    /// <inheritdoc />
    public DateTimeOffset Convert(DateTimeOffset dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(dateTime.DateTime, sourceTimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, destinationTimeZone);
    }
}
