// -----------------------------------------------------------------------
// <copyright file="InflectorExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Inflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Globalization.Static;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for inflection and text manipulation.
/// </summary>
public static class InflectorExtensions
{
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    extension(string word)
    {
        /// <summary>
        /// Inflects the word based on the provided quantity and culture. Returns the singular or plural form of the word according to the quantity and culture-specific rules. This method uses the culture-specific inflector to determine the correct form of the word based on whether the quantity is considered plural or singular in that culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="quantity">The quantity used to determine the correct form of the word.</param>
        /// <param name="culture">The culture used to determine the correct form of the word.</param>
        /// <returns>The inflected form of the word.</returns>
        public string Inflect(int quantity, CultureInfo? culture = null) => word.Inflect((decimal)quantity, culture);

        /// <summary>
        /// Inflects the word based on the provided quantity and culture. Returns the singular or plural form of the word according to the quantity and culture-specific rules. This method uses the culture-specific inflector to determine the correct form of the word based on whether the quantity is considered plural or singular in that culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="quantity">The quantity used to determine the correct form of the word.</param>
        /// <param name="culture">The culture used to determine the correct form of the word.</param>
        /// <returns>The inflected form of the word.</returns>
        public string Inflect(double quantity, CultureInfo? culture = null) => word.Inflect((decimal)quantity, culture);

        /// <summary>
        /// Inflects the word based on the provided quantity and culture. Returns the singular or plural form of the word according to the quantity and culture-specific rules. This method uses the culture-specific inflector to determine the correct form of the word based on whether the quantity is considered plural or singular in that culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="quantity">The quantity used to determine the correct form of the word.</param>
        /// <param name="culture">The culture used to determine the correct form of the word.</param>
        /// <returns>The inflected form of the word.</returns>
        public string Inflect(decimal quantity, CultureInfo? culture = null) => !quantity.IsPlural(culture) ? word.Singularize(inputIsKnownToBePlural: false) : word.Pluralize(inputIsKnownToBeSingular: false);

        /// <summary>
        /// Pluralizes the provided input considering irregular words and culture-specific rules.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="inputIsKnownToBeSingular">
        /// When <c>true</c> (default), always pluralizes.
        /// When <c>false</c>, returns the word unchanged if it appears to already be in plural form
        /// (detected via singularize→re-pluralize round-trip).
        /// </param>
        /// <param name="culture">Target culture used to resolve the inflector provider.</param>
        public string Pluralize(bool inputIsKnownToBeSingular = true, CultureInfo? culture = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(word);

            if (!inputIsKnownToBeSingular)
            {
                var asPlural = Localizer.Pluralization.Pluralize(word, culture);
                if (asPlural == word)
                    return word;

                var asSingular = Localizer.Pluralization.Singularize(word, culture);

                // Keep irregular plurals (people, children, mice, ...) unchanged when plurality is unknown.
                return asSingular != word && Localizer.Pluralization.Pluralize(asSingular, culture) == word && !string.LooksLikeSimpleSuffixPlural(word, asSingular)
                    ? word
                    : asPlural;
            }

            return Localizer.Pluralization.Pluralize(word, culture);
        }

        /// <summary>
        /// Singularizes the provided input considering irregular words and culture-specific rules.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="inputIsKnownToBePlural">
        /// When <c>true</c> (default), always singularizes.
        /// When <c>false</c>, returns the word unchanged if it appears to already be in singular form.
        /// </param>
        /// <param name="skipSimpleWords">
        /// When <c>true</c>, words of 3 characters or fewer are returned unchanged to avoid misidentification.
        /// </param>
        /// <param name="culture">Target culture used to resolve the inflector provider.</param>
        public string Singularize(bool inputIsKnownToBePlural = true, bool skipSimpleWords = false, CultureInfo? culture = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(word);

            if (skipSimpleWords && word.Length <= 3)
                return word;

            if (!inputIsKnownToBePlural)
            {
                var asSingular = Localizer.Pluralization.Singularize(word, culture);

                if (asSingular == word)
                    return word;

                var asPlural = Localizer.Pluralization.Pluralize(word, culture);

                // Guard against false positives for singular words ending with 's' (bus, status, analysis, ...).
                return asPlural != word && Localizer.Pluralization.Singularize(asPlural, culture) == word && string.LooksLikeSimpleSuffixPlural(word, asSingular)
                    ? word
                    : asSingular;
            }

            return Localizer.Pluralization.Singularize(word, culture);
        }

        private static bool LooksLikeSimpleSuffixPlural(string input, string singularCandidate)
            => input.Equals(singularCandidate + "s", Comparison)
               || input.Equals(singularCandidate + "es", Comparison)
               || (singularCandidate.Length > 1
                   && singularCandidate.EndsWith('y')
                   && input.Equals(singularCandidate[..^1] + "ies", Comparison));
    }

    extension(decimal count)
    {
        /// <summary>
        /// Gets the plural category for a decimal quantity using the culture-specific provider.
        /// Falls back to a simple invariant rule when no provider is registered.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public PluralCategory GetPluralCategory(CultureInfo? culture = null) => Localizer.Pluralization.GetPluralCategory(count, culture);

        /// <summary>
        /// Returns <c>true</c> when <paramref name="count"/> requires the plural form.
        /// Zero is treated as plural (e.g. "0 dogs" in English), only 1 is singular.
        /// Falls back to <c>count != 1</c> when no inflector is registered.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public bool IsPlural(CultureInfo? culture = null) => Localizer.Pluralization.IsPlural(count, culture);
    }

    extension(int count)
    {
        /// <summary>
        /// Gets the plural category for an integer quantity using the culture-specific provider.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public PluralCategory GetPluralCategory(CultureInfo? culture = null) => ((decimal)count).GetPluralCategory(culture);

        /// <summary>
        /// Returns <c>true</c> when <paramref name="count"/> requires the plural form.
        /// Zero is treated as plural. Falls back to <c>count != 1</c> when no inflector is registered.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public bool IsPlural(CultureInfo? culture = null) => ((decimal)count).IsPlural(culture);
    }

    extension(long count)
    {
        /// <summary>
        /// Gets the plural category for a long quantity using the culture-specific provider.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public PluralCategory GetPluralCategory(CultureInfo? culture = null) => ((decimal)count).GetPluralCategory(culture);

        /// <summary>
        /// Returns <c>true</c> when <paramref name="count"/> requires the plural form.
        /// Zero is treated as plural. Falls back to <c>count != 1</c> when no inflector is registered.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public bool IsPlural(CultureInfo? culture = null) => ((decimal)count).IsPlural(culture);
    }

    extension(double count)
    {
        /// <summary>
        /// Gets the plural category for a double quantity using the culture-specific provider.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public PluralCategory GetPluralCategory(CultureInfo? culture = null) => ((decimal)count).GetPluralCategory(culture);

        /// <summary>
        /// Returns <c>true</c> when <paramref name="count"/> requires the plural form.
        /// Zero is treated as plural. Falls back to <c>count != 1</c> when no inflector is registered.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        public bool IsPlural(CultureInfo? culture = null) => ((decimal)count).IsPlural(culture);
    }
}
