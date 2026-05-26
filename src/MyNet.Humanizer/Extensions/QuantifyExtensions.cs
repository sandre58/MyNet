// -----------------------------------------------------------------------
// <copyright file="QuantifyExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Primitives;
using MyNet.Text.Templating;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extensions for formatting a <see cref="string"/> word as a quantity.
/// </summary>
public static class QuantifyExtensions
{
    extension(string input)
    {
        /// <summary>
        /// Prefixes the provided word with the number and accordingly pluralizes or singularizes the word.
        /// </summary>
        /// <param name="quantity">The quantity of the word.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <example>
        /// "request".Quantify(0) => "0 requests"
        /// "request".Quantify(10000, format: "N0") => "10,000 requests"
        /// "request".Quantify(1, format: "N0") => "1 request".
        /// </example>
        public string Quantify(int quantity, string? format = null, CultureInfo? culture = null) => input.Quantify((decimal)quantity, format, culture: culture);

        /// <summary>
        /// Prefixes the provided word with the number and accordingly pluralizes or singularizes the word.
        /// </summary>
        /// <param name="quantity">The quantity of the word.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <example>
        /// "request".Quantify(0) => "0 requests"
        /// "request".Quantify(10000, format: "N0") => "10,000 requests"
        /// "request".Quantify(1, format: "N0") => "1 request".
        /// </example>
        public string Quantify(double quantity, string? format = null, CultureInfo? culture = null) => input.Quantify((decimal)quantity, format, culture: culture);

        /// <summary>
        /// Prefixes the provided word with the number and accordingly pluralizes or singularizes the word.
        /// </summary>
        /// <param name="quantity">The quantity of the word.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted quantity string.</returns>
        public string Quantify(decimal quantity, string? format = null, CultureInfo? culture = null)
            => TextTemplating.Create(x => x.PrefixWithQuantity(quantity, format)).Apply(input, culture.OrCurrent());
    }

    extension(int quantity)
    {
        /// <summary>
        /// Prefixes the provided word with the number and accordingly pluralizes or singularizes the word.
        /// </summary>
        /// <param name="word">The word to be formatted.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted quantity string.</returns>
        public string Quantify(string word, string? format = null, CultureInfo? culture = null) => word.Quantify(quantity, format, culture);
    }

    extension(double quantity)
    {
        /// <summary>
        /// Prefixes the provided word with the number and accordingly pluralizes or singularizes the word.
        /// </summary>
        /// <param name="word">The word to be formatted.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted quantity string.</returns>
        public string Quantify(string word, string? format = null, CultureInfo? culture = null) => word.Quantify(quantity, format, culture);
    }

    extension(decimal quantity)
    {
        /// <summary>
        /// Prefixes the provided word with the number and accordingly pluralizes or singularizes the word.
        /// </summary>
        /// <param name="word">The word to be formatted.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted quantity string.</returns>
        public string Quantify(string word, string? format = null, CultureInfo? culture = null) => word.Quantify(quantity, format, culture);
    }
}
