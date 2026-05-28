// -----------------------------------------------------------------------
// <copyright file="IThemeBaseRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.UI.Theming;

/// <summary>
/// Registry of theme bases available to the application (light, dark, high contrast, custom variants).
/// </summary>
public interface IThemeBaseRegistry
{
    /// <summary>
    /// Gets the default light base theme.
    /// </summary>
    IThemeBase Light { get; }

    /// <summary>
    /// Gets the default dark base theme.
    /// </summary>
    IThemeBase Dark { get; }

    /// <summary>
    /// Gets the default high contrast base theme.
    /// </summary>
    IThemeBase HighContrast { get; }

    /// <summary>
    /// Gets the theme bases registered in this registry.
    /// </summary>
    IReadOnlyCollection<IThemeBase> AvailableBases { get; }

    /// <summary>
    /// Registers a theme base for discovery and application.
    /// </summary>
    /// <param name="themeBase">The theme base to register.</param>
    void Register(IThemeBase themeBase);

    /// <summary>
    /// Retrieves a registered theme base by name.
    /// </summary>
    /// <param name="name">Name of the theme base.</param>
    /// <returns>The theme base if found; otherwise, null.</returns>
    IThemeBase? Get(string name);
}
