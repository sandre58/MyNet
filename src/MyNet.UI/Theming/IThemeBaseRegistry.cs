// -----------------------------------------------------------------------
// <copyright file="IThemeBaseRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.UI.Theming;

public interface IThemeBaseRegistry
{
    /// <summary>
    /// Gets the default light base theme. This is the base theme that will be used when the application is in light mode.
    /// </summary>
    IThemeBase Light { get; }

    /// <summary>
    /// Gets the default dark base theme. This is the base theme that will be used when the application is in dark mode.
    /// </summary>
    IThemeBase Dark { get; }

    /// <summary>
    /// Gets the default high contrast base theme. This is the base theme that will be used when the application is in high contrast mode.
    /// </summary>
    IThemeBase HighContrast { get; }

    /// <summary>
    /// Gets the collection of available themes supported by the application.
    /// </summary>
    /// <remarks>The collection is read-only and reflects the themes currently accessible for use. The
    /// contents may change if themes are added or removed at runtime, depending on the application's
    /// implementation.</remarks>
    IReadOnlyCollection<IThemeBase> Availables { get; }

    /// <summary>
    /// Registers a base theme. This method should be called during the application startup to register all the available base themes.
    /// </summary>
    /// <param name="themeBase">The base theme to register.</param>
    void Register(IThemeBase themeBase);

    /// <summary>
    /// Retrieves a base theme by its name. This method can be used to get a specific base theme that has been registered. If the base theme with the specified name does not exist, it returns null.
    /// </summary>
    /// <param name="name">Name of the base theme to retrieve.</param>
    /// <returns>The base theme if found; otherwise, null.</returns>
    IThemeBase? Get(string name);
}
