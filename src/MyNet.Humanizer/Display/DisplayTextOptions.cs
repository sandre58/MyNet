// -----------------------------------------------------------------------
// <copyright file="DisplayTextOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Localization.Translation;

namespace MyNet.Humanizer.Display;

/// <summary>
/// Options for display name generation.
/// </summary>
public class DisplayTextOptions
{
    /// <summary>
    /// Gets the default display name options.
    /// </summary>
    public static DisplayTextOptions Default { get; } = new();

    /// <summary>
    /// Gets the display style to use when generating the display name.
    /// </summary>
    public DisplayStyle Style { get; init; }
}
