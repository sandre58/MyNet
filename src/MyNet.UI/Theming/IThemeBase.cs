// -----------------------------------------------------------------------
// <copyright file="IThemeBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Theming;

/// <summary>
/// Specifies the base theme type for the application.
/// </summary>
public interface IThemeBase
{
    /// <summary>
    /// Gets the name of the base theme (e.g. "Dark", "Light", "HighContrast").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the base theme is a dark theme.
    /// </summary>
    bool IsDark { get; }
}
