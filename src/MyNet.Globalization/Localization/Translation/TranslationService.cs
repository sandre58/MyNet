// -----------------------------------------------------------------------
// <copyright file="TranslationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Culture;

namespace MyNet.Globalization.Localization.Translation;

/// <summary>
/// Current implementation of <see cref="ITranslationService"/> that uses an <see cref="ITranslator"/> to perform translations based on the current culture provided by an <see cref="ICultureContext"/>.
/// This service abstracts the translation logic and culture management, allowing for easy localization of applications without needing to directly manage culture information in the translation calls.
/// </summary>
/// <param name="translator">The translator to use for performing translations.</param>
/// <param name="cultureContext">The culture context to use for determining the current culture.</param>
public sealed class TranslationService(ITranslator translator, ICultureContext cultureContext) : ITranslationService
{
    /// <inheritdoc />
    public string Translate(string key, TranslationOptions options, CultureInfo? culture = null) => translator.Translate(key, options, culture ?? cultureContext.CurrentCulture);

    /// <inheritdoc />
    public string Translate(string key, TranslationOptions options, string resourceKey, CultureInfo? culture = null) => translator.Translate(key, options, culture ?? cultureContext.CurrentCulture, resourceKey);
}
