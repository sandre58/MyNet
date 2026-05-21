// -----------------------------------------------------------------------
// <copyright file="ITranslationKeyResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;

namespace MyNet.Globalization.Localization.Translation.KeyResolving;

/// <summary>
/// Resolves candidate translation keys based on translation options.
/// </summary>
public interface ITranslationKeyResolver
{
    /// <summary>
    /// Resolves a list of candidate translation keys based on the base key, translation options, and culture.
    /// </summary>
    /// <param name="baseKey">The base translation key.</param>
    /// <param name="options">Translation options.</param>
    /// <param name="culture">Culture used for pluralization rules.</param>
    /// <returns>A collection of <see cref="TranslationKeyCandidate"/> representing the resolved keys and inflection fallback information.</returns>
    IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture);

    /// <summary>
    /// Resolves a pluralized translation key based on the count and culture.
    /// </summary>
    /// <param name="baseKey">The base translation key.</param>
    /// <param name="count">The count used for pluralization.</param>
    /// <param name="culture">The culture used for pluralization rules.</param>
    /// <returns>The pluralized translation key.</returns>
    string ResolvePluralizedKey(string baseKey, decimal count, CultureInfo culture);

    /// <summary>
    /// Resolves a styled translation key based on the display style.
    /// </summary>
    /// <param name="baseKey">The base translation key.</param>
    /// <param name="style">The display style.</param>
    /// <param name="culture">The culture used for pluralization rules.</param>
    /// <returns>The styled translation key.</returns>
    string ResolveStyledKey(string baseKey, DisplayStyle style, CultureInfo culture);
}

/// <summary>
/// Represents a candidate translation key along with its associated fallback policy. This record encapsulates the translation key and the policy that indicates whether inflection fallback is allowed when resolving this key. It is used by the <see cref="ITranslationKeyResolver"/> to provide information about each candidate key, allowing translation services to determine how to handle cases where a direct translation is not found and whether to apply inflection rules as a fallback mechanism based on the specified policy.
/// </summary>
/// <param name="Key">The translation key.</param>
/// <param name="Policy">The fallback policy associated with the translation key.</param>
public sealed record TranslationKeyCandidate(string Key, TranslationFallbackPolicy Policy);

/// <summary>
/// Represents the policy for translation fallback, specifically whether inflection fallback is allowed when resolving translation keys. This record encapsulates the fallback policy, allowing translation key resolvers and translators to determine how to handle cases where a direct translation is not found and whether to apply inflection rules as a fallback mechanism based on the specified policy.
/// </summary>
public sealed record TranslationFallbackPolicy
{
    /// <summary>
    /// Gets a value indicating whether to allow inflection fallback when resolving translation keys. If true, inflection fallback will be applied when no direct translation is found. If false, inflection fallback will not be used, and only direct translations will be considered. This property allows for flexible handling of translation key resolution based on the desired fallback behavior.
    /// </summary>
    public bool AllowInflectionFallback { get; init; }

    /// <summary>
    /// Gets a translation fallback policy that does not allow inflection fallback. When this policy is used, the translation key resolver will only consider direct translations and will not apply any inflection rules as a fallback mechanism. This is useful in scenarios where strict translation matching is desired without any fallback to inflected forms of the translation key.
    /// </summary>
    public static TranslationFallbackPolicy Strict => new() { AllowInflectionFallback = false };

    /// <summary>
    /// Gets a translation fallback policy that allows inflection fallback. When this policy is used, the translation key resolver will apply inflection rules as a fallback mechanism when no direct translation is found. This allows for more flexible translation key resolution, enabling the use of inflected forms of the translation key as potential matches when direct translations are not available.
    /// </summary>
    public static TranslationFallbackPolicy Flexible => new() { AllowInflectionFallback = true };
}
