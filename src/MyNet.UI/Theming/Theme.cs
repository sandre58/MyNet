// -----------------------------------------------------------------------
// <copyright file="Theme.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Theming;

/// <summary>
/// Represents a theme configuration for the application, including base theme and color settings.
/// </summary>
public sealed class Theme
{
    /// <summary>
    /// Gets the base theme (e.g. Dark, Light, HighContrast).
    /// </summary>
    public IThemeBase? Base { get; }

    /// <summary>
    /// Gets the primary color of the theme.
    /// </summary>
    public string? PrimaryColor { get; }

    /// <summary>
    /// Gets the foreground color for the primary color.
    /// </summary>
    public string? PrimaryForegroundColor { get; }

    /// <summary>
    /// Gets the accent color of the theme.
    /// </summary>
    public string? AccentColor { get; }

    /// <summary>
    /// Gets the foreground color for the accent color.
    /// </summary>
    public string? AccentForegroundColor { get; }
}
