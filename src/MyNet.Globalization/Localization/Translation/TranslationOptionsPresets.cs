// -----------------------------------------------------------------------
// <copyright file="TranslationOptionsPresets.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Globalization.Localization.Translation;

/// <summary>
/// Provides predefined presets of <see cref="TranslationOptions"/> for common translation scenarios, such as default, abbreviated, and symbol styles.
/// </summary>
public static class TranslationOptionsPresets
{
    /// <summary>
    /// Gets the default translation options, which typically use the full display style for translations.
    /// </summary>
    public static TranslationOptions Default { get; } = new TranslationOptionsBuilder().Build();

    /// <summary>
    /// Gets the abbreviated translation options, which typically use a shortened display style for translations.
    /// </summary>
    public static TranslationOptions Abbreviated { get; } = new TranslationOptionsBuilder()
        .WithStyle(DisplayStyle.Abbreviation)
        .Build();

    /// <summary>
    /// Gets the symbol translation options, which typically use a symbolic display style for translations.
    /// </summary>
    public static TranslationOptions Symbol { get; } = new TranslationOptionsBuilder()
        .WithStyle(DisplayStyle.Symbol)
        .Build();
}
