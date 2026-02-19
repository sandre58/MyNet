// -----------------------------------------------------------------------
// <copyright file="Theme.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Theming;

/// <summary>
/// Represents a theme configuration for the application, including base theme and color settings.
/// </summary>
/// <param name="Base">Gets the base theme (e.g. Dark, Light, HighContrast).</param>
/// <param name="PrimaryColor">Gets the primary color used in the theme.</param>
/// <param name="AccentColor">Gets the accent color used in the theme.</param>
/// <param name="PrimaryForegroundColor">Gets the optional foreground color for the primary color. If not specified, it will be determined automatically based on the primary color.</param>
/// <param name="AccentForegroundColor">Gets the optional foreground color for the accent color. If not specified, it will be determined automatically based on the accent color.</param>
public sealed record Theme(
    IThemeBase Base,
    string PrimaryColor,
    string AccentColor,
    string? PrimaryForegroundColor = null,
    string? AccentForegroundColor = null);
