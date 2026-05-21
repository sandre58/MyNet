// -----------------------------------------------------------------------
// <copyright file="GlobalizationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;
using MyNet.Utilities;

namespace MyNet.Globalization;

/// <summary>
/// Provides a service for managing globalization settings, including culture and time zone information.
/// </summary>
public sealed class GlobalizationService(ICultureService cultureService, ITimeZoneService timeZoneService)
    : IGlobalizationService
{
    /// <inheritdoc />
    public CultureInfo CurrentCulture => cultureService.CurrentCulture;

    /// <inheritdoc />
    public TimeZoneInfo CurrentTimeZone => timeZoneService.CurrentTimeZone;

    /// <inheritdoc />
    public System.DateTime Now => timeZoneService.Now;

    /// <inheritdoc />
    public event EventHandler<CultureChangedEventArgs> CultureChanged
    {
        add => cultureService.CultureChanged += value;
        remove => cultureService.CultureChanged -= value;
    }

    /// <inheritdoc />
    public event EventHandler<TimeZoneChangedEventArgs> TimeZoneChanged
    {
        add => timeZoneService.TimeZoneChanged += value;
        remove => timeZoneService.TimeZoneChanged -= value;
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) => cultureService.SetCulture(culture);

    /// <inheritdoc />
    public void SetCulture(string cultureCode) => cultureService.SetCulture(cultureCode);

    /// <inheritdoc />
    public void SetTimeZone(TimeZoneInfo timeZone) => timeZoneService.SetTimeZone(timeZone);

    /// <inheritdoc />
    public void SetTimeZone(string timeZoneId) => timeZoneService.SetTimeZone(timeZoneId);

    /// <inheritdoc />
    public DateTimeOffset FromUtc(DateTimeOffset utcDateTime) => timeZoneService.FromUtc(utcDateTime);

    /// <inheritdoc />
    public DateTimeOffset ToUtc(DateTimeOffset localDateTime) => timeZoneService.ToUtc(localDateTime);

    /// <inheritdoc />
    public DateTimeOffset Convert(DateTimeOffset dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone) => timeZoneService.Convert(dateTime, sourceTimeZone, destinationTimeZone);

    /// <inheritdoc />
    public IDisposable UseCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var previousCulture = CurrentCulture;

        cultureService.SetCulture(culture);

        return new DelegateDisposable(() => cultureService.SetCulture(previousCulture));
    }

    /// <inheritdoc />
    public IDisposable UseTimeZone(TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var previousTimeZone = CurrentTimeZone;

        timeZoneService.SetTimeZone(timeZone);

        return new DelegateDisposable(() => timeZoneService.SetTimeZone(previousTimeZone));
    }
}
