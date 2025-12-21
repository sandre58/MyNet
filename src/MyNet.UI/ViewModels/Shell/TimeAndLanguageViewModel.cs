// -----------------------------------------------------------------------
// <copyright file="TimeAndLanguageViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using DynamicData;
using DynamicData.Binding;
using MyNet.Observable.Globalization;
using MyNet.UI.Resources;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.Shell;

public class TimeAndLanguageViewModel : NavigableWorkspaceViewModel
{
    private readonly IObservableGlobalization _globalization;

    public virtual CultureInfo? SelectedCulture { get; set; }

    public virtual bool AutomaticCulture { get; set; }

    public virtual TimeZoneInfo? SelectedTimeZone { get; set; }

    public virtual bool AutomaticTimeZone { get; set; }

    public ObservableCollection<CultureInfo> Cultures { get; } = [];

    public ObservableCollection<TimeZoneInfo> TimeZones { get; } = [];

    public TimeAndLanguageViewModel(IObservableGlobalization globalization)
    {
        _globalization = globalization;
        Cultures.AddRange(globalization.SupportedCultures);
        TimeZones.AddRange(globalization.SupportedTimeZones);

        Disposables.AddRange(
        [
            this.WhenAnyPropertyChanged(nameof(AutomaticCulture), nameof(SelectedCulture)).Subscribe(_ => ApplyCulture()),
            this.WhenAnyPropertyChanged(nameof(AutomaticTimeZone), nameof(SelectedTimeZone)).Subscribe(_ => ApplyTimeZone())
        ]);

        using (IsModifiedSuspender.Suspend())
        {
            RefreshCulture();
            RefreshTimeZone();
        }
    }

    protected override string CreateTitle() => UiResources.TimeAndLanguage;

    #region Culture

    private CultureInfo GetSelectedCulture(CultureInfo culture)
    {
        while (true)
        {
            if (Cultures.Contains(culture))
                return culture;
            culture = culture.Parent;
        }
    }

    private void RefreshCulture() => SelectedCulture = GetSelectedCulture(_globalization.Culture);

    protected override void OnCultureChanged()
    {
        base.OnCultureChanged();

        using (IsModifiedSuspender.Suspend())
            RefreshCulture();
    }

    private void ApplyCulture()
    {
        if (IsModifiedSuspender.IsSuspended) return;

        var culture = SelectedCulture is null || AutomaticCulture ? CultureInfo.InstalledUICulture.ToString() : SelectedCulture.ToString();

        if (culture != _globalization.Culture.Name && culture != _globalization.Culture.Parent.Name)
            _globalization.SetCulture(culture);
    }

    #endregion

    #region TimeZone

    private void RefreshTimeZone() => SelectedTimeZone = _globalization.TimeZone;

    protected override void OnTimeZoneChanged()
    {
        base.OnTimeZoneChanged();

        using (IsModifiedSuspender.Suspend())
            RefreshTimeZone();
    }

    private void ApplyTimeZone()
    {
        if (IsModifiedSuspender.IsSuspended) return;

        var timeZone = SelectedTimeZone is null || AutomaticTimeZone ? TimeZoneInfo.Local : SelectedTimeZone;

        if (!timeZone.Equals(_globalization.TimeZone))
            _globalization.SetTimeZone(timeZone);
    }

    #endregion
}
