// -----------------------------------------------------------------------
// <copyright file="ITranslator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Localization.Translation;

/// <summary>
/// Provides pure, stateless translation of resource keys.
/// The culture must always be explicitly supplied — this interface has no notion of a "current" culture.
/// For culture-contextual translation, use <see cref="ITranslationService"/> which wraps this interface with an <see cref="Culture.ICultureContext"/>.
/// </summary>
public interface ITranslator
{
    /// <summary>
    /// Translates a key for the given culture, searching all registered resources.
    /// Returns <paramref name="key"/> if no translation is found.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="options">The translation options.</param>
    /// <param name="culture">The target culture.</param>
    /// <returns>Translated value, or <paramref name="key"/> if not found.</returns>
    string Translate(string key, TranslationOptions options, CultureInfo culture);

    /// <summary>
    /// Translates a key for the given culture, restricting the search to a specific resource.
    /// Returns <paramref name="key"/> if no translation is found.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="options">The translation options.</param>
    /// <param name="culture">The target culture.</param>
    /// <param name="resourceKey">The resource key to search in. If null or empty, all registered resources are searched.</param>
    /// <returns>Translated value, or <paramref name="key"/> if not found.</returns>
    string Translate(string key, TranslationOptions options, CultureInfo culture, string resourceKey);
}
