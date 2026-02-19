// -----------------------------------------------------------------------
// <copyright file="ThemeManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.UI.Theming;

/// <summary>
/// Provides a global access point for managing and applying themes in the application.
/// </summary>
public static class ThemeManager
{
    private static IThemeService? _themeService;
    private static IThemeBaseRegistry? _themeBaseRegistry;

    /// <summary>
    /// Gets the current theme applied to the application.
    /// </summary>
    public static Theme? CurrentTheme => _themeService?.CurrentTheme;

    /// <summary>
    /// Initializes the <see cref="ThemeManager"/> with the specified <see cref="IThemeService"/>.
    /// </summary>
    /// <param name="themeService">The service used to manage themes.</param>
    /// <param name="themeBaseRegistry">The registry used to manage theme bases.</param>
    public static void Initialize(IThemeService themeService, IThemeBaseRegistry themeBaseRegistry)
    {
        _themeService = themeService;
        _themeBaseRegistry = themeBaseRegistry;
    }

    /// <summary>
    /// Occurs when the theme is changed.
    /// </summary>
    public static event EventHandler<ThemeChangedEventArgs> ThemeChanged
    {
        add
        {
            if (_themeService is not null)
                _themeService.ThemeChanged += value;
        }

        remove
        {
            if (_themeService is not null)
                _themeService.ThemeChanged -= value;
        }
    }

    /// <summary>
    /// Gets the default dark base theme. This is the base theme that will be used when the application is in dark mode.
    /// </summary>
    public static IThemeBase? Dark => _themeBaseRegistry?.Dark;

    /// <summary>
    /// Gets the default light base theme. This is the base theme that will be used when the application is in light mode.
    /// </summary>
    public static IThemeBase? Light => _themeBaseRegistry?.Light;

    /// <summary>
    /// Gets the collection of available themes supported by the application.
    /// </summary>
    public static IReadOnlyCollection<IThemeBase>? AvailableBases => _themeBaseRegistry?.Availables;

    /// <summary>
    /// Gets the base theme with the specified name.
    /// </summary>
    /// <param name="name">The name of the base theme to retrieve.</param>
    /// <returns>The base theme with the specified name, or null if not found.</returns>
    public static IThemeBase? GetBase(string name) => _themeBaseRegistry?.Get(name);

    /// <summary>
    /// Registers the specified theme with the theme registry.
    /// </summary>
    /// <param name="themeBase">The theme instance to register. Cannot be null.</param>
    public static void Register(IThemeBase themeBase) => _themeBaseRegistry?.Register(themeBase);

    /// <summary>
    /// Applies the specified base theme.
    /// </summary>
    /// <param name="themeBase">The base theme to apply.</param>
    public static void ApplyBase(IThemeBase themeBase) => _themeService?.ApplyBaseTheme(themeBase);

    /// <summary>
    /// Applies the specified primary color.
    /// </summary>
    /// <param name="color">The primary color to apply.</param>
    public static void ApplyPrimaryColor(string color) => ApplyPrimaryColor(color, null);

    /// <summary>
    /// Applies the specified primary color and foreground color.
    /// </summary>
    /// <param name="color">The primary color to apply.</param>
    /// <param name="foreground">The foreground color for the primary color.</param>
    public static void ApplyPrimaryColor(string color, string? foreground) => _themeService?.ApplyPrimary(color, foreground);

    /// <summary>
    /// Applies the specified accent color.
    /// </summary>
    /// <param name="color">The accent color to apply.</param>
    public static void ApplyAccentColor(string color) => _themeService?.ApplyAccent(color, null);

    /// <summary>
    /// Applies the specified accent color and foreground color.
    /// </summary>
    /// <param name="color">The accent color to apply.</param>
    /// <param name="foreground">The foreground color for the accent color.</param>
    public static void ApplyAccentColor(string color, string? foreground) => _themeService?.ApplyAccent(color, foreground);

    /// <summary>
    /// Applies the specified theme configuration.
    /// </summary>
    /// <param name="theme">The theme to apply.</param>
    public static void ApplyTheme(Theme theme) => _themeService?.ApplyTheme(theme);

    /// <summary>
    /// Updates the current theme by applying the specified update action to it.
    /// </summary>
    /// <param name="update">The update action to apply.</param>
    public static void UpdateTheme(Action<Theme> update) => _themeService?.UpdateTheme(update);
}
