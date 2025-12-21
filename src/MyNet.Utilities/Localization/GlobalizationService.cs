// -----------------------------------------------------------------------
// <copyright file="GlobalizationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Utilities.Events;
using MyNet.Utilities.Logging;

namespace MyNet.Utilities.Localization;

public class GlobalizationService : ICultureService, ITimeZoneService
{
    private readonly Action<CultureInfo>? _onCultureChanged;
    private readonly WeakEventSource<EventArgs> _cultureChanged = new();
    private readonly WeakEventSource<EventArgs> _timeZoneChanged = new();

    public event EventHandler<EventArgs> CultureChanged
    {
        add => _cultureChanged.Subscribe(value);
        remove => _cultureChanged.Unsubscribe(value);
    }

    public event EventHandler<EventArgs> TimeZoneChanged
    {
        add => _timeZoneChanged.Subscribe(value);
        remove => _timeZoneChanged.Unsubscribe(value);
    }

    public GlobalizationService()
        : this(CultureInfo.CurrentCulture, TimeZoneInfo.Local) { }

    public GlobalizationService(CultureInfo culture, TimeZoneInfo timeZoneInfo)
        : this(culture, timeZoneInfo, null) { }

    private GlobalizationService(CultureInfo culture, TimeZoneInfo timeZoneInfo, Action<CultureInfo>? onCultureChanged)
    {
        Culture = culture;
        TimeZone = timeZoneInfo;
        _onCultureChanged = onCultureChanged;
    }

    public static GlobalizationService Current { get; } = new(CultureInfo.CurrentCulture, TimeZoneInfo.Local, x =>
    {
        CultureInfo.CurrentCulture = x;
        CultureInfo.CurrentUICulture = x;
    });

    public TimeZoneInfo TimeZone { get; private set; }

    public CultureInfo Culture { get; private set; }

    public DateTime Date => DateTime.UtcNow.ToTimeZone(TimeZone);

    public virtual DateTime UtcDate => DateTime.UtcNow;

    public void SetCulture(string cultureCode) => SetCulture(CultureInfo.GetCultureInfo(cultureCode));

    public void SetCulture(CultureInfo culture)
    {
        if (Equals(culture, Culture)) return;

        LogManager.Debug($"Culture Changed : {Culture} => {culture} for thread {Environment.CurrentManagedThreadId}");
        Culture = culture;
        _onCultureChanged?.Invoke(Culture);

        _cultureChanged.Raise(this, EventArgs.Empty);
    }

    public void SetTimeZone(TimeZoneInfo timeZone)
    {
        if (TimeZone.Equals(timeZone)) return;

        LogManager.Debug($"Time zone Changed : {TimeZone} => {timeZone}");
        TimeZone = timeZone;

        _timeZoneChanged.Raise(this, EventArgs.Empty);
    }

    public DateTime Convert(DateTime dateTime) => dateTime.Kind switch
    {
        DateTimeKind.Local => TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, TimeZone),
        _ => TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), TimeZone)
    };

    public DateTime ConvertToUtc(DateTime dateTime) => dateTime.Kind switch
    {
        DateTimeKind.Unspecified => TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZone),
        DateTimeKind.Local => dateTime.ToUniversalTime(),
        _ => dateTime
    };

    public DateTime ConvertFromTimeZone(DateTime dateTime, TimeZoneInfo sourceTimeZone) => dateTime.Kind switch
    {
        DateTimeKind.Utc => TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, TimeZone),
        DateTimeKind.Local => TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, TimeZone),
        _ => TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, TimeZone)
    };

    public DateTime ConvertToTimeZone(DateTime dateTime, TimeZoneInfo destinationTimeZone) => dateTime.Kind switch
    {
        DateTimeKind.Utc => TimeZoneInfo.ConvertTime(Convert(dateTime), TimeZone, TimeZoneInfo.Utc),
        DateTimeKind.Local => TimeZoneInfo.ConvertTime(Convert(dateTime), TimeZone, TimeZoneInfo.Local),
        _ => TimeZoneInfo.ConvertTime(dateTime, TimeZone, destinationTimeZone)
    };

    public TProvider? GetProvider<TProvider>() => LocalizationService.Get<TProvider>(Culture);

    public string Translate(string key) => TranslationService.Get(Culture)[key];

    public string Translate(string key, string filename) => TranslationService.Get(Culture)[key, filename];

    public string TranslateAbbreviated(string key) => Translate(key.ToAbbreviationKey());

    public string TranslateAbbreviated(string key, string filename) => Translate(key.ToAbbreviationKey(), filename);
}
