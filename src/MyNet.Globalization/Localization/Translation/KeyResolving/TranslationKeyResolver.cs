// -----------------------------------------------------------------------
// <copyright file="TranslationKeyResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using MyNet.Globalization.Inflection;

namespace MyNet.Globalization.Localization.Translation.KeyResolving;

/// <summary>
/// Current implementation of <see cref="ITranslationKeyResolver"/>.
/// </summary>
public sealed class TranslationKeyResolver(IPluralizationService pluralizationService) : ITranslationKeyResolver
{
    public const string DefaultSuffix = "";
    public const string SymbolSuffix = nameof(DisplayStyle.Symbol);
    public const string AbbreviationSuffix = "Abbr";
    public const string ShortSuffix = nameof(DisplayStyle.Short);
    public const string NarrowSuffix = nameof(DisplayStyle.Narrow);

    public const string PluralSuffix = "Plural";
    public const string ZeroSuffix = "Zero";

    private static readonly Dictionary<DisplayStyle, string> StyleSuffix = new()
    {
        { DisplayStyle.Default, DefaultSuffix },
        { DisplayStyle.Symbol, SymbolSuffix },
        { DisplayStyle.Abbreviation, AbbreviationSuffix },
        { DisplayStyle.Short, ShortSuffix },
        { DisplayStyle.Narrow, NarrowSuffix }
    };

    private static readonly Dictionary<DisplayStyle, DisplayStyle[]> StyleSuffixes = new()
    {
        { DisplayStyle.Default, [DisplayStyle.Default] },
        { DisplayStyle.Symbol, [DisplayStyle.Symbol, DisplayStyle.Abbreviation] },
        { DisplayStyle.Abbreviation, [DisplayStyle.Abbreviation, DisplayStyle.Short, DisplayStyle.Narrow] },
        { DisplayStyle.Short, [DisplayStyle.Short, DisplayStyle.Narrow, DisplayStyle.Abbreviation] },
        { DisplayStyle.Narrow, [DisplayStyle.Narrow, DisplayStyle.Short, DisplayStyle.Abbreviation] }
    };

    /// <inheritdoc />
    public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseKey);

        var list = new List<TranslationKeyCandidate>();
        var seen = new HashSet<TranslationKeyCandidate>();

        var pluralSuffixes = options.Quantity.HasValue ? ResolvePluralSuffixes(options.Quantity.Value, culture) : [];
        var styles = StyleSuffixes[options.Style];

        // --------------------------------------------------
        // 1. MOST SPECIFIC: STYLE + PLURAL
        // --------------------------------------------------
        foreach (var plural in pluralSuffixes)
        {
            foreach (var style in styles)
                add(baseKey, style, plural);
        }

        // --------------------------------------------------
        // 2. PLURAL ONLY
        // --------------------------------------------------
        foreach (var plural in pluralSuffixes)
            add(baseKey, pluralSuffix: plural);

        // --------------------------------------------------
        // 3. STYLE ONLY
        // --------------------------------------------------
        foreach (var style in styles)
            add(baseKey, style);

        // --------------------------------------------------
        // 4. FALLBACK BASE KEY
        // --------------------------------------------------
        add(baseKey);

        return list;

        void add(string key, DisplayStyle? style = null, string? pluralSuffix = null)
        {
            var finalKey = BuildKey(key, style.HasValue ? StyleSuffix[style.Value] : null, pluralSuffix);

            var policy = GetPolicy(style, pluralSuffix);

            var candidate = new TranslationKeyCandidate(finalKey, policy);
            if (seen.Add(candidate))
                list.Add(candidate);
        }
    }

    /// <inheritdoc />
    public string ResolvePluralizedKey(string baseKey, decimal count, CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseKey);

        var suffix = ResolvePluralSuffixes(count, culture).FirstOrDefault();

        return BuildKey(baseKey, pluralSuffix: suffix);
    }

    /// <inheritdoc />
    public string ResolveStyledKey(string baseKey, DisplayStyle style, CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseKey);

        var suffix = ResolveStyleSuffixes(style).FirstOrDefault();

        return BuildKey(baseKey, styleSuffix: suffix);
    }

    /// <summary>
    /// Builds a translation key by combining the base key with the specified suffix. The resulting key is formed by concatenating the base key and the suffix, allowing for consistent generation of translation keys based on different styles or pluralization categories.
    /// </summary>
    /// <param name="baseKey">The base translation key.</param>
    /// <param name="styleSuffix">The optional style suffix to append to the base key.</param>
    /// <param name="pluralSuffix">The optional plural suffix to append to the base key.</param>
    /// <returns>The combined translation key.</returns>
    private static string BuildKey(string baseKey, string? styleSuffix = null, string? pluralSuffix = null) => string.Concat(baseKey, styleSuffix, pluralSuffix);

    /// <summary>
    /// Determines the translation fallback policy based on the presence of style and plural suffixes. The method evaluates the provided style and plural suffix to determine whether to allow inflection fallback when resolving translation keys. If the style is Symbol or Abbreviation, or if a plural suffix is present, the policy is set to Strict (no inflection fallback). For other styles without a plural suffix, the policy is set to Flexible (allowing inflection fallback). This logic ensures that more specific translation keys (e.g., those with symbols or plural forms) are resolved without fallback, while more general keys can benefit from inflection fallback when appropriate.
    /// </summary>
    /// <param name="style">The display style of the translation key.</param>
    /// <param name="pluralSuffix">The plural suffix of the translation key.</param>
    /// <returns>The determined translation fallback policy.</returns>
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement", Justification = "Readability")]
    private static TranslationFallbackPolicy GetPolicy(DisplayStyle? style, string? pluralSuffix)
    {
        // Symbol / Abbreviation => always strict
        if (style is DisplayStyle.Symbol or DisplayStyle.Abbreviation)
            return TranslationFallbackPolicy.Strict;

        // Plural/Zero => strict (grammatical decision already made)
        if (!string.IsNullOrWhiteSpace(pluralSuffix))
            return TranslationFallbackPolicy.Strict;

        // Current / Short / Narrow => flexible
        return TranslationFallbackPolicy.Flexible;
    }

    /// <summary>
    /// Resolves display style suffixes ordered by preference.
    /// </summary>
    private static IReadOnlyCollection<string> ResolveStyleSuffixes(DisplayStyle style) => [.. StyleSuffixes[style].Select(x => StyleSuffix[x])];

    /// <summary>
    /// Resolves plural suffixes for the given count and culture, ordered by preference. The method first determines the primary plural suffix based on the count and culture using the <see cref="IInflector"/> provider. If a primary suffix is found, it is returned first, followed by the default suffix. If no inflector is available for the culture, only the default suffix is returned.
    /// </summary>
    /// <param name="count">The count to determine the plural category.</param>
    /// <param name="culture">The culture to use for pluralization rules.</param>
    /// <returns>An enumerable of plural suffixes ordered by preference.</returns>
    private IReadOnlyCollection<string> ResolvePluralSuffixes(decimal count, CultureInfo culture) => pluralizationService.GetPluralCategory(count, culture) switch
    {
        PluralCategory.Zero => [ZeroSuffix],
        PluralCategory.Singular => [DefaultSuffix],
        _ => [PluralSuffix]
    };
}
