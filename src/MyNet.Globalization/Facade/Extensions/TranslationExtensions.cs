// -----------------------------------------------------------------------
// <copyright file="TranslationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Globalization.Localization.Translation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Globalization.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TranslationExtensions
{
    extension(string key)
    {
        /// <summary>
        /// Translates the key using the current culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="culture">The culture to use for translation. If null, the current culture is used.</param>
        /// <returns>The translated string.</returns>
        public string Translate(CultureInfo? culture = null) => key.Translate(TranslationOptionsPresets.Default, culture);

        /// <summary>
        /// Translates the key from a specific resource using the current culture. When null, the current culture is used.
        /// </summary>
        /// <param name="resourceKey">The resource key to use for translation.</param>
        /// <param name="culture">The culture to use for translation. If null, the current culture is used.</param>
        /// <returns>The translated string.</returns>
        public string Translate(string resourceKey, CultureInfo? culture = null) => key.Translate(TranslationOptionsPresets.Default, resourceKey, culture);

        /// <summary>
        /// Translates the key using the current culture with specified translation options. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="configure">A delegate to configure the translation options.</param>
        /// <param name="culture">The culture to use for translation. If null, the current culture is used.</param>
        /// <returns>The translated string.</returns>
        public string Translate(Action<TranslationOptionsBuilder> configure, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new TranslationOptionsBuilder();

            configure(builder);

            return key.Translate(builder.Build(), culture);
        }

        /// <summary>
        /// Translates the key from a specific resource using the current culture. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="configure">A delegate to configure the translation options.</param>
        /// <param name="resourceKey">The resource key to use for translation.</param>
        /// <param name="culture">The culture to use for translation. If null, the current culture is used.</param>
        /// <returns>The translated string.</returns>
        public string Translate(Action<TranslationOptionsBuilder> configure, string resourceKey, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new TranslationOptionsBuilder();

            configure(builder);

            return key.Translate(builder.Build(), resourceKey, culture);
        }

        /// <summary>
        /// Translates the key using the current culture with specified translation options. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="options">The translation options to use.</param>
        /// <param name="culture">The culture to use for translation. If null, the current culture is used.</param>
        /// <returns>The translated string.</returns>
        public string Translate(TranslationOptions options, CultureInfo? culture = null) => Localizer.Translation.Translate(key, options, culture);

        /// <summary>
        /// Translates the key from a specific resource using the current culture. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="options">The translation options to use.</param>
        /// <param name="resourceKey">The resource key to use for translation.</param>
        /// <param name="culture">The culture to use for translation. If null, the current culture is used.</param>
        /// <returns>The translated string.</returns>
        public string Translate(TranslationOptions options, string resourceKey, CultureInfo? culture = null)
            => culture == null
                ? Localizer.Translation.Translate(key, options, resourceKey)
                : Localizer.Translation.Translate(key, options, resourceKey, culture);

        /// <summary>
        /// Translates the key as a date pattern for the specified culture. It first attempts to retrieve the date pattern from the culture's DateTimeFormat properties using reflection. If the property corresponding to the key exists, its value is returned as a string. If the property does not exist, it falls back to translating the key using the localization context's translator for the specified culture. An ArgumentException is thrown if the key is null or empty.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="culture">The culture to use for translation. If null, the current culture is used.</param>
        /// <returns>The translated date pattern.</returns>
        public string TranslateDatePattern(CultureInfo? culture = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            var effectiveCulture = culture.OrContext();
            var format = effectiveCulture.DateTimeFormat;
            var prop = format.GetType().GetProperty(key);
            return prop is not null ? prop.GetValue(format)?.ToString() ?? string.Empty : key.Translate(culture);
        }

        /// <summary>
        /// Translates using the specified display style and culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="style">The display style.</param>
        /// <param name="culture">The target culture.</param>
        /// <returns>The translated string.</returns>
        public string Translate(DisplayStyle style, CultureInfo? culture = null) => key.Translate(x => x.WithStyle(style), culture);

        /// <summary>
        /// Translates using pluralization count and culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="count">The pluralization count.</param>
        /// <param name="culture">The target culture.</param>
        /// <returns>The translated string.</returns>
        public string Translate(decimal count, CultureInfo culture) => key.Translate(x => x.WithQuantity(count), culture);

        /// <summary>
        /// Translates using pluralization count, display style and culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="count">The pluralization count.</param>
        /// <param name="style">The display style.</param>
        /// <param name="culture">The target culture.</param>
        /// <returns>The translated string.</returns>
        public string Translate(decimal count, DisplayStyle style, CultureInfo culture) => key.Translate(x => x.WithStyle(style).WithQuantity(count), culture);

        /// <summary>
        /// Translates using custom rendering arguments.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="arguments">The rendering arguments.</param>
        /// <returns>The translated string.</returns>
        public string Translate(params KeyValuePair<string, object?>[] arguments) => key.Translate(x => x.WithArguments(arguments));

        /// <summary>
        /// Translates when <paramref name="key"/> is set; otherwise returns <paramref name="fallbackText"/> or empty.
        /// </summary>
        public string TranslateOr(
            string? fallbackText = null,
            string? resourceFilename = null,
            CultureInfo? culture = null)
            => !string.IsNullOrEmpty(key)
                ? string.IsNullOrEmpty(resourceFilename)
                    ? key.Translate(culture)
                    : key.Translate(resourceFilename, culture)
                : fallbackText ?? string.Empty;
    }
}
