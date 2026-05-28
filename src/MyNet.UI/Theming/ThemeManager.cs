// -----------------------------------------------------------------------
// <copyright file="ThemeManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace MyNet.UI.Theming;

/// <summary>
/// Static bridge to <see cref="IThemeService"/> and <see cref="IThemeBaseRegistry"/> for legacy or view-layer code
/// that cannot receive dependencies. Prefer injecting <see cref="IThemeService"/> in new code.
/// Configure once at startup via <see cref="Configure"/> or <see cref="ServiceCollectionExtensions.UseThemeManager"/>.
/// </summary>
public static class ThemeManager
{
    private static readonly List<EventHandler<ThemeChangedEventArgs>> PendingThemeChangedHandlers = [];
    private static int _configured;
    private static IThemeService? _themeService;
    private static IThemeBaseRegistry? _themeBaseRegistry;

    /// <summary>
    /// Gets a value indicating whether <see cref="Configure"/> has been called.
    /// </summary>
    public static bool IsConfigured => Volatile.Read(ref _configured) == 1;

    /// <summary>
    /// Gets the current theme applied to the application.
    /// </summary>
    public static Theme? CurrentTheme => IsConfigured ? GetThemeService().CurrentTheme : null;

    /// <summary>
    /// Configures the static bridge with services resolved from dependency injection.
    /// </summary>
    /// <param name="themeService">The service used to manage themes.</param>
    /// <param name="themeBaseRegistry">The registry used to manage theme bases.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public static void Configure(IThemeService themeService, IThemeBaseRegistry themeBaseRegistry)
    {
        ArgumentNullException.ThrowIfNull(themeService);
        ArgumentNullException.ThrowIfNull(themeBaseRegistry);

        if (Interlocked.Exchange(ref _configured, 1) == 1)
            return;

        _themeService = themeService;
        _themeBaseRegistry = themeBaseRegistry;

        EventHandler<ThemeChangedEventArgs>[] pending;
        lock (PendingThemeChangedHandlers)
        {
            pending = [.. PendingThemeChangedHandlers];
            PendingThemeChangedHandlers.Clear();
        }

        foreach (var handler in pending)
            themeService.ThemeChanged += handler;
    }

    /// <summary>
    /// Configures the static bridge. Prefer <see cref="Configure"/>.
    /// </summary>
    [Obsolete("Use Configure(IThemeService, IThemeBaseRegistry) or IServiceProvider.UseThemeManager().")]
    public static void Initialize(IThemeService themeService, IThemeBaseRegistry themeBaseRegistry)
        => Configure(themeService, themeBaseRegistry);

    /// <summary>
    /// Occurs when the theme is changed.
    /// </summary>
    public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged
    {
        add
        {
            if (value is null)
            {
                return;
            }

            if (_themeService is not null)
            {
                _themeService.ThemeChanged += value;
            }
            else
            {
                lock (PendingThemeChangedHandlers)
                {
                    PendingThemeChangedHandlers.Add(value);
                }
            }
        }

        remove
        {
            if (value is null)
            {
                return;
            }

            if (_themeService is not null)
            {
                _themeService.ThemeChanged -= value;
            }
            else
            {
                lock (PendingThemeChangedHandlers)
                {
                    PendingThemeChangedHandlers.Remove(value);
                }
            }
        }
    }

    /// <summary>
    /// Gets the default dark base theme.
    /// </summary>
    public static IThemeBase Dark => GetThemeBaseRegistry().Dark;

    /// <summary>
    /// Gets the default light base theme.
    /// </summary>
    public static IThemeBase Light => GetThemeBaseRegistry().Light;

    /// <summary>
    /// Gets the default high contrast base theme.
    /// </summary>
    public static IThemeBase HighContrast => GetThemeBaseRegistry().HighContrast;

    /// <summary>
    /// Gets the collection of registered theme bases.
    /// </summary>
    public static IReadOnlyCollection<IThemeBase> AvailableBases => GetThemeBaseRegistry().AvailableBases;

    /// <summary>
    /// Gets the base theme with the specified name.
    /// </summary>
    /// <param name="name">The name of the base theme to retrieve.</param>
    /// <returns>The base theme with the specified name, or null if not found.</returns>
    public static IThemeBase? GetBase(string name) => GetThemeBaseRegistry().Get(name);

    /// <summary>
    /// Registers the specified theme base.
    /// </summary>
    /// <param name="themeBase">The theme base to register.</param>
    public static void Register(IThemeBase themeBase) => GetThemeBaseRegistry().Register(themeBase);

    /// <summary>
    /// Applies the specified base theme.
    /// </summary>
    /// <param name="themeBase">The base theme to apply.</param>
    public static void ApplyBase(IThemeBase themeBase) => GetThemeService().ApplyBaseTheme(themeBase);

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
    public static void ApplyPrimaryColor(string color, string? foreground)
        => GetThemeService().ApplyPrimary(color, foreground);

    /// <summary>
    /// Applies the specified accent color.
    /// </summary>
    /// <param name="color">The accent color to apply.</param>
    public static void ApplyAccentColor(string color) => ApplyAccentColor(color, null);

    /// <summary>
    /// Applies the specified accent color and foreground color.
    /// </summary>
    /// <param name="color">The accent color to apply.</param>
    /// <param name="foreground">The foreground color for the accent color.</param>
    public static void ApplyAccentColor(string color, string? foreground)
        => GetThemeService().ApplyAccent(color, foreground);

    /// <summary>
    /// Applies the specified theme configuration.
    /// </summary>
    /// <param name="theme">The theme to apply.</param>
    public static void ApplyTheme(Theme theme) => GetThemeService().ApplyTheme(theme);

    /// <summary>
    /// Updates the current theme using the specified transform and applies the result.
    /// </summary>
    /// <param name="update">Transforms the current theme into the theme to apply.</param>
    public static void UpdateTheme(Func<Theme, Theme> update) => GetThemeService().UpdateTheme(update);

    private static IThemeService GetThemeService() => _themeService ?? throw new InvalidOperationException(
        "ThemeManager is not configured. Register IThemeService and IThemeBaseRegistry, then call ThemeManager.Configure or IServiceProvider.UseThemeManager() during application startup.");

    private static IThemeBaseRegistry GetThemeBaseRegistry() => _themeBaseRegistry ?? throw new InvalidOperationException(
        "ThemeManager is not configured. Register IThemeService and IThemeBaseRegistry, then call ThemeManager.Configure or IServiceProvider.UseThemeManager() during application startup.");

    /// <summary>
    /// Resets configuration. For unit tests only.
    /// </summary>
    internal static void ResetForTesting()
    {
        Interlocked.Exchange(ref _configured, 0);
        _themeService = null;
        _themeBaseRegistry = null;

        lock (PendingThemeChangedHandlers)
            PendingThemeChangedHandlers.Clear();
    }
}
