// -----------------------------------------------------------------------
// <copyright file="ThemeChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Theming;

/// <summary>
/// Provides data for the <see cref="IThemeService.ThemeChanged"/> event.
/// </summary>
/// <param name="currentTheme">The current theme after the change.</param>
public sealed class ThemeChangedEventArgs(Theme currentTheme) : EventArgs
{
    /// <summary>
    /// Gets the current theme after the change.
    /// </summary>
    public Theme CurrentTheme { get; } = currentTheme;
}
