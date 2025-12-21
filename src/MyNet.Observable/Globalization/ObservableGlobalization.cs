// -----------------------------------------------------------------------
// <copyright file="ObservableGlobalization.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using MyNet.Utilities.Localization;
using PropertyChanged;

namespace MyNet.Observable.Globalization;

public sealed class ObservableGlobalization : ObservableObject, IObservableGlobalization
{
    private readonly GlobalizationService _service;

    public ObservableGlobalization(GlobalizationService service)
    {
        _service = service;

        _service.CultureChanged += OnCultureChanged;
        _service.TimeZoneChanged += OnTimeZoneChanged;
    }

    public CultureInfo Culture => _service.Culture;

    public CultureInfo UICulture => _service.Culture;

    public TimeZoneInfo TimeZone => _service.TimeZone;

    public DateTime Now => _service.Date;

    public DateTime UtcNow => _service.UtcDate;

    public ObservableCollection<CultureInfo> SupportedCultures { get; } =
    [
        Cultures.English,
        Cultures.French
    ];

    public ObservableCollection<TimeZoneInfo> SupportedTimeZones { get; } = [..TimeZoneInfo.GetSystemTimeZones()];

    public void SetCulture(string culture) => _service.SetCulture(culture);

    public void SetTimeZone(TimeZoneInfo timeZone) => _service.SetTimeZone(timeZone);

    [SuppressPropertyChangedWarnings]
    private void OnCultureChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(Culture));
        OnPropertyChanged(nameof(UICulture));
    }

    [SuppressPropertyChangedWarnings]
    private void OnTimeZoneChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(TimeZone));
        OnPropertyChanged(nameof(Now));
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        _service.CultureChanged -= OnCultureChanged;
        _service.TimeZoneChanged -= OnTimeZoneChanged;
    }
}
