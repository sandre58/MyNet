// -----------------------------------------------------------------------
// <copyright file="ITranslationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Localization.Translation;

public interface ITranslationService
{
    /// <summary>
    /// Translates a key using the current culture.
    /// </summary>
    /// <param name="key">Translation key.</param>
    /// <param name="options">Translation options.</param>
    /// <param name="culture">Optional culture to override the current culture.</param>
    /// <returns>Translated value or the key itself if not found.</returns>
    string Translate(string key, TranslationOptions options, CultureInfo? culture = null);

    /// <summary>
    /// Translates a key from a specific resource source.
    /// </summary>
    /// <param name="key">Translation key.</param>
    /// <param name="options">Translation options.</param>
    /// <param name="resourceKey">Resource source key.</param>
    /// <param name="culture">Optional culture to override the current culture.</param>
    /// <returns>Translated value or the key itself if not found.</returns>
    string Translate(string key, TranslationOptions options, string resourceKey, CultureInfo? culture = null);
}
