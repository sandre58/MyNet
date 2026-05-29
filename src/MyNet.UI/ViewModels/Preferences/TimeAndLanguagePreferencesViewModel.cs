// -----------------------------------------------------------------------
// <copyright file="TimeAndLanguagePreferencesViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using MyNet.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.UI.Resources;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Preferences;

/// <summary>
/// Preferences page for culture and time zone settings.
/// </summary>
public sealed class TimeAndLanguagePreferencesViewModel : WorkspaceViewModel
{
    private readonly IGlobalizationService _globalization;
    private bool _suppressApply;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeAndLanguagePreferencesViewModel"/> class.
    /// </summary>
    public TimeAndLanguagePreferencesViewModel(
        IGlobalizationService globalization,
        IEnumerable<CultureInfo>? supportedCultures = null,
        IEnumerable<TimeZoneInfo>? supportedTimeZones = null)
        : base(cultureService: globalization)
    {
        ArgumentNullException.ThrowIfNull(globalization);

        _globalization = globalization;

        foreach (var culture in supportedCultures ?? [SupportedCultures.French, SupportedCultures.English])
            Cultures.Add(culture);

        foreach (var timeZone in supportedTimeZones ?? TimeZoneInfo.GetSystemTimeZones())
            TimeZones.Add(timeZone);

        _globalization.CultureChanged += OnGlobalizationCultureChanged;
        _globalization.TimeZoneChanged += OnGlobalizationTimeZoneChanged;

        Disposables.Add(Disposable.Create(() =>
        {
            _globalization.CultureChanged -= OnGlobalizationCultureChanged;
            _globalization.TimeZoneChanged -= OnGlobalizationTimeZoneChanged;
        }));

        RefreshFromService();
    }

    /// <summary>
    /// Gets or sets the selected culture.
    /// </summary>
    public CultureInfo? SelectedCulture
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            ApplyCulture();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the installed UI culture is used.
    /// </summary>
    public bool AutomaticCulture
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            ApplyCulture();
        }
    }

    /// <summary>
    /// Gets or sets the selected time zone.
    /// </summary>
    public TimeZoneInfo? SelectedTimeZone
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            ApplyTimeZone();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the local system time zone is used.
    /// </summary>
    public bool AutomaticTimeZone
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            ApplyTimeZone();
        }
    }

    /// <summary>
    /// Gets the cultures available for selection.
    /// </summary>
    public ObservableCollection<CultureInfo> Cultures { get; } = [];

    /// <summary>
    /// Gets the time zones available for selection.
    /// </summary>
    public ObservableCollection<TimeZoneInfo> TimeZones { get; } = [];

    /// <inheritdoc />
    protected override string? CreateTitle(CultureInfo culture) => UiResources.TimeAndLanguage;

    /// <inheritdoc />
    public override void OnEvent(CultureChangedEvent e)
    {
        base.OnEvent(e);
        RefreshCultureFromService();
    }

    private void OnGlobalizationCultureChanged(object? sender, CultureChangedEventArgs e)
        => RefreshCultureFromService();

    private void OnGlobalizationTimeZoneChanged(object? sender, TimeZoneChangedEventArgs e)
        => RefreshTimeZoneFromService();

    private void RefreshFromService()
    {
        _suppressApply = true;
        try
        {
            RefreshCultureFromService();
            RefreshTimeZoneFromService();
        }
        finally
        {
            _suppressApply = false;
        }
    }

    private void RefreshCultureFromService()
    {
        _suppressApply = true;
        try
        {
            SelectedCulture = ResolveSupportedCulture(_globalization.CurrentCulture);
        }
        finally
        {
            _suppressApply = false;
        }
    }

    private void RefreshTimeZoneFromService()
    {
        _suppressApply = true;
        try
        {
            SelectedTimeZone = _globalization.CurrentTimeZone;
        }
        finally
        {
            _suppressApply = false;
        }
    }

    private void ApplyCulture()
    {
        if (_suppressApply)
            return;

        var cultureCode = SelectedCulture is null || AutomaticCulture
            ? CultureInfo.InstalledUICulture.ToString()
            : SelectedCulture.ToString();

        var current = _globalization.CurrentCulture;
        if (cultureCode != current.Name && cultureCode != current.Parent.Name)
            _globalization.SetCulture(cultureCode);
    }

    private void ApplyTimeZone()
    {
        if (_suppressApply)
            return;

        var timeZone = SelectedTimeZone is null || AutomaticTimeZone
            ? TimeZoneInfo.Local
            : SelectedTimeZone;

        if (!timeZone.Equals(_globalization.CurrentTimeZone))
            _globalization.SetTimeZone(timeZone);
    }

    private CultureInfo ResolveSupportedCulture(CultureInfo culture)
    {
        while (!culture.Equals(CultureInfo.InvariantCulture))
        {
            if (Cultures.Contains(culture))
                return culture;

            culture = culture.Parent;
        }

        return Cultures.FirstOrDefault() ?? _globalization.CurrentCulture;
    }
}
