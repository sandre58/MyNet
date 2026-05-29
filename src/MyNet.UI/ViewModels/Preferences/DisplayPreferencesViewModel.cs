// -----------------------------------------------------------------------
// <copyright file="DisplayPreferencesViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive.Disposables;
using MyNet.Primitives;
using MyNet.UI.Resources;
using MyNet.UI.Theming;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Preferences;

/// <summary>
/// Preferences page for theme and display settings.
/// </summary>
public sealed class DisplayPreferencesViewModel : WorkspaceViewModel
{
    private readonly IThemeService _themeService;
    private bool _syncingFromTheme;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayPreferencesViewModel"/> class.
    /// </summary>
    public DisplayPreferencesViewModel(IThemeService themeService, IThemeBaseRegistry themeBaseRegistry)
    {
        ArgumentNullException.ThrowIfNull(themeService);
        ArgumentNullException.ThrowIfNull(themeBaseRegistry);

        _themeService = themeService;

        foreach (var themeBase in themeBaseRegistry.AvailableBases)
            AvailableBases.Add(themeBase);

        UpdateFromTheme(_themeService.CurrentTheme);
        _themeService.ThemeChanged += OnThemeServiceThemeChanged;

        Disposables.Add(Disposable.Create(() => _themeService.ThemeChanged -= OnThemeServiceThemeChanged));
    }

    /// <summary>
    /// Gets the available theme bases.
    /// </summary>
    public ObservableCollection<IThemeBase> AvailableBases { get; } = [];

    /// <summary>
    /// Gets or sets the selected theme base.
    /// </summary>
    public IThemeBase? ThemeBase
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _syncingFromTheme)
                return;

            (value is not null && value != _themeService.CurrentTheme.Base)
                .IfTrue(() => _themeService.ApplyBaseTheme(value!));
        }
    }

    /// <summary>
    /// Gets or sets the primary color.
    /// </summary>
    public string? PrimaryColor
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _syncingFromTheme)
                return;

            (!string.IsNullOrEmpty(value) && value != _themeService.CurrentTheme.PrimaryColor)
                .IfTrue(() => _themeService.ApplyPrimary(value!, AutoPrimaryForegroundColor ? null : PrimaryForegroundColor));
        }
    }

    /// <summary>
    /// Gets or sets the accent color.
    /// </summary>
    public string? AccentColor
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _syncingFromTheme)
                return;

            (!string.IsNullOrEmpty(value) && value != _themeService.CurrentTheme.AccentColor)
                .IfTrue(() => _themeService.ApplyAccent(value!, AutoAccentForegroundColor ? null : AccentForegroundColor));
        }
    }

    /// <summary>
    /// Gets or sets the primary foreground color.
    /// </summary>
    public string? PrimaryForegroundColor
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _syncingFromTheme)
                return;

            (!string.IsNullOrEmpty(value) && value != _themeService.CurrentTheme.PrimaryForegroundColor)
                .IfTrue(() => _themeService.ApplyPrimary(PrimaryColor!, AutoPrimaryForegroundColor ? null : value));
        }
    }

    /// <summary>
    /// Gets or sets the accent foreground color.
    /// </summary>
    public string? AccentForegroundColor
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _syncingFromTheme)
                return;

            (!string.IsNullOrEmpty(value) && value != _themeService.CurrentTheme.AccentForegroundColor)
                .IfTrue(() => _themeService.ApplyAccent(AccentColor!, AutoAccentForegroundColor ? null : value));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the primary foreground is computed automatically.
    /// </summary>
    public bool AutoPrimaryForegroundColor
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _syncingFromTheme)
                return;

            _themeService.ApplyPrimary(PrimaryColor!, value ? null : PrimaryForegroundColor);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the accent foreground is computed automatically.
    /// </summary>
    public bool AutoAccentForegroundColor
    {
        get;
        set
        {
            if (!SetProperty(ref field, value) || _syncingFromTheme)
                return;

            _themeService.ApplyAccent(AccentColor!, value ? null : AccentForegroundColor);
        }
    }

    /// <summary>
    /// Gets or sets the contrast ratio (UI binding).
    /// </summary>
    public float ContrastRatio { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets a value indicating whether contrast enforcement is enabled.
    /// </summary>
    public bool EnableContrast { get; set => SetProperty(ref field, value); }

    /// <inheritdoc />
    protected override string? CreateTitle(CultureInfo culture) => UiResources.Display;

    private void OnThemeServiceThemeChanged(object? sender, ThemeChangedEventArgs e) => UpdateFromTheme(e.CurrentTheme);

    private void UpdateFromTheme(Theme theme)
    {
        _syncingFromTheme = true;
        try
        {
            if (ThemeBase != theme.Base)
                ThemeBase = theme.Base;

            if (PrimaryColor != theme.PrimaryColor)
                PrimaryColor = theme.PrimaryColor;

            if (AccentColor != theme.AccentColor)
                AccentColor = theme.AccentColor;

            if (PrimaryForegroundColor != theme.PrimaryForegroundColor)
                PrimaryForegroundColor = theme.PrimaryForegroundColor;

            if (AccentForegroundColor != theme.AccentForegroundColor)
                AccentForegroundColor = theme.AccentForegroundColor;
        }
        finally
        {
            _syncingFromTheme = false;
        }
    }
}
