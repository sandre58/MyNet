// -----------------------------------------------------------------------
// <copyright file="ShellCultureViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using MyNet.Globalization.Culture;
using MyNet.UI.ViewModels;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Shell chrome for UI culture selection (status bar, title bar, etc.).
/// </summary>
public sealed class ShellCultureViewModel : ViewModelBase
{
    private readonly ICultureService _cultureService;
    private bool _suppressCultureSync;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellCultureViewModel"/> class.
    /// </summary>
    public ShellCultureViewModel(
        ICultureService cultureService,
        IEnumerable<CultureInfo>? supportedCultures = null)
    {
        _cultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));

        foreach (var culture in supportedCultures ?? [SupportedCultures.French, SupportedCultures.English])
            Cultures.Add(culture);

        SyncSelectedCultureFromService();

        _cultureService.CultureChanged += OnCultureServiceCultureChanged;
        Disposables.Add(Disposable.Create(() => _cultureService.CultureChanged -= OnCultureServiceCultureChanged));
    }

    /// <summary>
    /// Gets the cultures available for selection.
    /// </summary>
    public ObservableCollection<CultureInfo> Cultures { get; } = [];

    /// <summary>
    /// Gets or sets the selected UI culture.
    /// </summary>
    public CultureInfo? SelectedCulture
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _suppressCultureSync)
                return;

            var culture = value ?? CultureInfo.InstalledUICulture;
            _cultureService.SetCulture(culture);
        }
    }

    private void OnCultureServiceCultureChanged(object? sender, CultureChangedEventArgs e)
    {
        _suppressCultureSync = true;
        try
        {
            SyncSelectedCultureFromService();
        }
        finally
        {
            _suppressCultureSync = false;
        }
    }

    private void SyncSelectedCultureFromService()
    {
        var culture = _cultureService.CurrentCulture;
        SelectedCulture = Cultures.Contains(culture)
            ? culture
            : GetNearestSupportedCulture(culture);
    }

    private CultureInfo? GetNearestSupportedCulture(CultureInfo culture)
    {
        while (!culture.Equals(CultureInfo.InvariantCulture))
        {
            if (Cultures.Contains(culture))
                return culture;

            culture = culture.Parent;
        }

        return Cultures.FirstOrDefault();
    }
}
