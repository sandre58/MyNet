// -----------------------------------------------------------------------
// <copyright file="IThemeService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Theming;

/// <summary>
/// Defines the contract for a service that manages themes in the application.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets the current theme applied to the application.
    /// </summary>
    Theme CurrentTheme { get; }

    /// <summary>
    /// Applies the specified theme configuration.
    /// </summary>
    /// <param name="theme">The theme to apply.</param>
    void ApplyTheme(Theme theme);

    /// <summary>
    /// Applies the specified base theme to the application.
    /// </summary>
    /// <param name="baseTheme">The base theme to apply.</param>
    void ApplyBaseTheme(IThemeBase baseTheme);

    /// <summary>
    /// Applies the specified primary color and optional foreground color to the application.
    /// </summary>
    /// <param name="color">The primary color to apply.</param>
    /// <param name="foreground">The optional foreground color to apply.</param>
    void ApplyPrimary(string color, string? foreground = null);

    /// <summary>
    /// Applies the specified accent color and optional foreground color to the application.
    /// </summary>
    /// <param name="color">The accent color to apply.</param>
    /// <param name="foreground">The optional foreground color to apply.</param>
    void ApplyAccent(string color, string? foreground = null);

    /// <summary>
    /// Updates the current theme by applying the specified update action to it.
    /// </summary>
    /// <param name="update">The update action to apply.</param>
    void UpdateTheme(Action<Theme> update);

    /// <summary>
    /// Adds a base theme extension to the service.
    /// </summary>
    /// <param name="extension">The base theme extension to add.</param>
    /// <returns>The theme service instance for chaining.</returns>
    IThemeService AddBaseExtension(IThemeExtension extension);

    /// <summary>
    /// Adds a primary color extension to the service.
    /// </summary>
    /// <param name="extension">The primary color extension to add.</param>
    /// <returns>The theme service instance for chaining.</returns>
    IThemeService AddPrimaryExtension(IThemeExtension extension);

    /// <summary>
    /// Adds an accent color extension to the service.
    /// </summary>
    /// <param name="extension">The accent color extension to add.</param>
    /// <returns>The theme service instance for chaining.</returns>
    IThemeService AddAccentExtension(IThemeExtension extension);

    /// <summary>
    /// Occurs when the theme is changed.
    /// </summary>
    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}
