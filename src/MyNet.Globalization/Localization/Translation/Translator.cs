// -----------------------------------------------------------------------
// <copyright file="Translator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Linq;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Localization.Policies;
using MyNet.Globalization.Localization.Translation.Catalog;
using MyNet.Globalization.Localization.Translation.KeyResolving;
using MyNet.Utilities.Text.Templating;

namespace MyNet.Globalization.Localization.Translation;

/// <summary>
/// Pure, stateless implementation of <see cref="ITranslator"/> backed by an <see cref="ITranslationCatalog"/>.
/// This class has no dependency on any "current culture" context — the caller always provides the culture explicitly.
/// Culture fallback is applied according to the configured <see cref="ICultureFallbackPolicy"/>:
///   by default, walks up the hierarchy: requested culture → parent culture → invariant culture.
/// </summary>
public sealed class Translator(
    ITranslationCatalog catalog,
    ITranslationKeyResolver keyResolver,
    IPluralizationService pluralizationService,
    ICultureFallbackPolicy? fallbackPolicy = null) : ITranslator
{
    private readonly ICultureFallbackPolicy _fallbackPolicy = fallbackPolicy ?? CultureFallbackPolicies.ParentCulture;

    /// <summary>
    /// Initializes a new instance of the <see cref="Translator"/> class with default parent-culture fallback.
    /// </summary>
    public Translator(ITranslationCatalog catalog, ITranslationKeyResolver keyResolver, IPluralizationService pluralizationService)
        : this(catalog, keyResolver, pluralizationService, CultureFallbackPolicies.ParentCulture)
    {
    }

    /// <inheritdoc />
    public string Translate(string key, TranslationOptions options, CultureInfo culture) => TranslateInternal(key, options, culture);

    /// <inheritdoc />
    public string Translate(string key, TranslationOptions options, CultureInfo culture, string resourceKey) => TranslateInternal(key, options, culture, resourceKey);

    /// <summary>
    /// Internal translation method that performs the actual lookup and fallback logic.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="options">The translation options.</param>
    /// <param name="culture">The target culture.</param>
    /// <param name="resourceKey">The resource key to search in. If null or empty, all registered resources are searched.</param>
    /// <returns>The translated string if found; otherwise, an empty string.</returns>
    private string TranslateInternal(string key, TranslationOptions options, CultureInfo culture, string? resourceKey = null)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        var effectiveCulture = culture;

        while (true)
        {
            var candidateKeys = keyResolver.Resolve(key, options, effectiveCulture);
            foreach (var candidateKey in candidateKeys)
            {
                var translation = TryTranslate(candidateKey.Key, effectiveCulture, resourceKey);

                if (!string.IsNullOrWhiteSpace(translation))
                {
                    return PostProcessing(translation, candidateKey.Policy, options, effectiveCulture);
                }
            }

            var fallback = _fallbackPolicy.GetFallback(effectiveCulture);
            if (fallback is null)
                break;

            effectiveCulture = fallback;
        }

        return options.UseKeyAsFallback ? key : string.Empty;
    }

    /// <summary>
    /// Performs post-processing on the translated value, such as formatting the count if applicable. If the translation contains a "{0}" placeholder, it will be replaced with the formatted count. If it contains a "#" character, it will be replaced with the formatted count as well. The formatting of the count can be culture-specific based on the provided culture and options.
    /// </summary>
    /// <param name="value">The translation value.</param>
    /// <param name="policy">The translation fallback policy.</param>
    /// <param name="options">The translation options.</param>
    /// <param name="culture">The culture to use for formatting.</param>
    /// <returns>The translation value with post-processing applied.</returns>
    private string PostProcessing(string value, TranslationFallbackPolicy policy, TranslationOptions options, CultureInfo culture)
    {
        var translation = options.UseInflectionFallback && policy.AllowInflectionFallback && options.Quantity.HasValue
            ? ApplyInflectionFallback(value, options.Quantity.Value, culture)
            : value;

        return new TemplateTransform(new()
        {
            Quantity = options.Quantity,
            Arguments = options.Arguments,
            QuantityFormat = options.QuantityFormat,
            QuantityRenderingMode = options.QuantityRenderingMode,
            QuantitySeparator = options.QuantitySeparator
        }).Apply(translation, culture);
    }

    /// <summary>
    /// Attempts to look up the translation for the given key and culture, optionally restricted to a specific resource.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="culture">The target culture.</param>
    /// <param name="resourceKey">The resource key to search in. If null or empty, all registered resources are searched.</param>
    /// <returns>The translated string if found; otherwise, null.</returns>
    private string? TryTranslate(string key, CultureInfo culture, string? resourceKey)
        => string.IsNullOrWhiteSpace(resourceKey)
            ? catalog.Resources.Values.Select(resource => resource.GetString(key, culture)).FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
            : catalog.Resources.TryGetValue(resourceKey, out var rm)
                ? rm.GetString(key, culture)
                : null;

    /// <summary>
    /// Applies inflection fallback to the given value based on the count and culture using the provided inflector.
    /// </summary>
    /// <param name="value">The value to apply inflection fallback to.</param>
    /// <param name="count">The count used to determine pluralization.</param>
    /// <param name="culture">The culture to use for inflection.</param>
    /// <returns>The value with inflection fallback applied.</returns>
    private string ApplyInflectionFallback(string value, decimal count, CultureInfo culture)
    {
        var isPlural = pluralizationService.IsPlural(count, culture);

        return isPlural ? pluralizationService.Pluralize(value, culture) : pluralizationService.Singularize(value, culture);
    }
}
