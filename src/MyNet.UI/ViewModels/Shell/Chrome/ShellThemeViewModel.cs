// -----------------------------------------------------------------------
// <copyright file="ShellThemeViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Theming;
using MyNet.Utilities.Suspending;

namespace MyNet.UI.ViewModels.Shell.Chrome;

/// <summary>
/// Shell chrome for quick light/dark theme base switching.
/// Detailed theme colors remain in preferences display settings.
/// </summary>
public sealed class ShellThemeViewModel : ViewModelBase
{
    private readonly IThemeService _themeService;
    private readonly IThemeBaseRegistry _themeBaseRegistry;
    private readonly Suspender _themeSyncSuspender = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellThemeViewModel"/> class.
    /// </summary>
    public ShellThemeViewModel(
        IThemeService themeService,
        IThemeBaseRegistry themeBaseRegistry,
        ICommandFactory? commandFactory = null)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _themeBaseRegistry = themeBaseRegistry ?? throw new ArgumentNullException(nameof(themeBaseRegistry));

        var commands = commandFactory ?? RelayCommandFactory.Default;
        IsDarkCommand = commands.Create(() => IsDark = true);
        IsLightCommand = commands.Create(() => IsDark = false);
        ChangeThemeCommand = commands.CreateRequired<IThemeBase>(themeBase => _themeService.ApplyBaseTheme(themeBase));

        IsDark = _themeService.CurrentTheme.Base.IsDark;

        _themeService.ThemeChanged += OnThemeServiceThemeChanged;
        Disposables.Add(Disposable.Create(() => _themeService.ThemeChanged -= OnThemeServiceThemeChanged));
    }

    public IThemeBase CurrentTheme => _themeService.CurrentTheme.Base;

    /// <summary>
    /// Gets or sets a value indicating whether the dark theme base is active.
    /// </summary>
    public bool IsDark
    {
        get;
        set
        {
            if (_themeSyncSuspender.IsSuspended)
            {
                SetProperty(ref field, value);
                return;
            }

            if (!SetProperty(ref field, value))
                return;

            _themeService.ApplyBaseTheme(value ? _themeBaseRegistry.Dark : _themeBaseRegistry.Light);
        }
    }

    /// <summary>
    /// Gets the command that switches to the dark theme base.
    /// </summary>
    public ICommand IsDarkCommand { get; }

    /// <summary>
    /// Gets the command that switches to the light theme base.
    /// </summary>
    public ICommand IsLightCommand { get; }

    /// <summary>
    /// Gets the command that toggles between light and dark theme bases.
    /// </summary>
    public ICommand ChangeThemeCommand { get; }

    private void OnThemeServiceThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        var isDark = e.CurrentTheme.Base.IsDark;
        if (IsDark == isDark)
            return;

        using (_themeSyncSuspender.Suspend())
            IsDark = isDark;

        NotifyPropertyChanged(nameof(CurrentTheme));
    }
}
