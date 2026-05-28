// -----------------------------------------------------------------------
// <copyright file="DisplayViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using MyNet.UI.Resources;
using MyNet.UI.Theming;

namespace MyNet.UI.Legacy.ViewModels.Shell;

public class DisplayViewModel : NavigableWorkspaceViewModel
{
    private readonly IThemeService _themeService;

    protected override string CreateTitle() => UiResources.Display;

    public ObservableCollection<IThemeBase> AvailableBases { get; } = [];

    public IThemeBase? ThemeBase { get; set; }

    public string? PrimaryColor { get; set; }

    public string? AccentColor { get; set; }

    public string? PrimaryForegroundColor { get; set; }

    public string? AccentForegroundColor { get; set; }

    public bool AutoPrimaryForegroundColor { get; set; }

    public bool AutoAccentForegroundColor { get; set; }

    public float ContrastRatio { get; set; }

    public bool EnableContrast { get; set; }

    public DisplayViewModel(IThemeService themeService, IThemeBaseRegistry themeBaseRegistry)
    {
        _themeService = themeService;
        AvailableBases.AddRange(themeBaseRegistry.AvailableBases);
        UpdateFromTheme(_themeService.CurrentTheme);
        _themeService.ThemeChanged += ThemeService_ThemeChanged;
    }

    private void UpdateFromTheme(Theme theme)
    {
        if (ThemeBase != theme.Base) ThemeBase = theme.Base;
        if (PrimaryColor != theme.PrimaryColor) PrimaryColor = theme.PrimaryColor;
        if (AccentColor != theme.AccentColor) AccentColor = theme.AccentColor;
        if (PrimaryForegroundColor != theme.PrimaryForegroundColor) PrimaryForegroundColor = theme.PrimaryForegroundColor;
        if (AccentForegroundColor != theme.AccentForegroundColor) AccentForegroundColor = theme.AccentForegroundColor;
    }

    private void ThemeService_ThemeChanged(object? sender, ThemeChangedEventArgs e) => UpdateFromTheme(e.CurrentTheme);

    protected void OnThemeBaseChanged()
        => (ThemeBase is not null && ThemeBase != _themeService.CurrentTheme.Base).IfTrue(() => _themeService.ApplyBaseTheme(ThemeBase!));

    protected void OnPrimaryColorChanged()
        => (!string.IsNullOrEmpty(PrimaryColor) && PrimaryColor != _themeService.CurrentTheme.PrimaryColor)
            .IfTrue(() => _themeService.ApplyPrimary(PrimaryColor!, AutoPrimaryForegroundColor ? null : PrimaryForegroundColor));

    protected void OnAccentColorChanged()
        => (!string.IsNullOrEmpty(AccentColor) && AccentColor != _themeService.CurrentTheme.AccentColor)
            .IfTrue(() => _themeService.ApplyAccent(AccentColor!, AutoAccentForegroundColor ? null : AccentForegroundColor));

    protected void OnPrimaryForegroundColorChanged()
        => (!string.IsNullOrEmpty(PrimaryForegroundColor) && PrimaryForegroundColor != _themeService.CurrentTheme.PrimaryForegroundColor)
            .IfTrue(() => _themeService.ApplyPrimary(PrimaryColor!, AutoPrimaryForegroundColor ? null : PrimaryForegroundColor));

    protected void OnAccentForegroundColorChanged()
        => (!string.IsNullOrEmpty(AccentForegroundColor) && AccentForegroundColor != _themeService.CurrentTheme.AccentForegroundColor)
            .IfTrue(() => _themeService.ApplyAccent(AccentColor!, AutoAccentForegroundColor ? null : AccentForegroundColor));

    protected void OnAutoPrimaryForegroundColorChanged()
        => _themeService.ApplyPrimary(PrimaryColor!, AutoPrimaryForegroundColor ? null : PrimaryForegroundColor);

    protected void OnAutoAccentForegroundColorChanged()
        => _themeService.ApplyAccent(AccentColor!, AutoAccentForegroundColor ? null : AccentForegroundColor);

    protected override void Cleanup()
    {
        _themeService.ThemeChanged -= ThemeService_ThemeChanged;
        base.Cleanup();
    }
}
