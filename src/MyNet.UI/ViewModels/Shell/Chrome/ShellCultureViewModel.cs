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
using MyNet.Utilities.Suspending;

namespace MyNet.UI.ViewModels.Shell.Chrome;

/// <summary>
/// Shell chrome for UI culture selection (status bar, title bar, etc.).
/// </summary>
public sealed class ShellCultureViewModel : ViewModelBase
{
    private readonly ICultureService _cultureService;
    private readonly Suspender _cultureSyncSuspender = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellCultureViewModel"/> class.
    /// </summary>
    /// <param name="cultureService">Application culture service.</param>
    /// <param name="supportedCultures">
    /// Cultures offered in the selector. When omitted, <see cref="SupportedCultures.French"/> and
    /// <see cref="SupportedCultures.English"/> are used. Register a custom factory in DI to define
    /// the list for your host application.
    /// </param>
    public ShellCultureViewModel(
        ICultureService cultureService,
        IEnumerable<CultureInfo>? supportedCultures = null)
    {
        _cultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));

        foreach (var culture in supportedCultures ?? [SupportedCultures.French, SupportedCultures.English])
            Cultures.Add(culture);

        var initialCulture = ResolveSupportedCulture(_cultureService.CurrentCulture);
        if (initialCulture is not null)
            SelectedCulture = initialCulture;

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
            if (_cultureSyncSuspender.IsSuspended)
            {
                SetProperty(ref field, value);
                return;
            }

            if (!SetProperty(ref field, value))
                return;

            var culture = value ?? CultureInfo.InstalledUICulture;
            _cultureService.SetCulture(culture);
        }
    }

    private void OnCultureServiceCultureChanged(object? sender, CultureChangedEventArgs e)
    {
        var culture = ResolveSupportedCulture(_cultureService.CurrentCulture);
        if (culture is null || Equals(SelectedCulture, culture))
            return;

        using (_cultureSyncSuspender.Suspend())
            SelectedCulture = culture;
    }

    private CultureInfo? ResolveSupportedCulture(CultureInfo culture)
    {
        var current = culture;
        while (!current.Equals(CultureInfo.InvariantCulture))
        {
            var match = Cultures.FirstOrDefault(c => c.Equals(current));
            if (match is not null)
                return match;

            current = current.Parent;
        }

        return Cultures.FirstOrDefault();
    }
}
