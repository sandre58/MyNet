// -----------------------------------------------------------------------
// <copyright file="CultureExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Localization.Translation;
using MyNet.Primitives;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Globalization.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CultureExtensions
{
    extension(CultureInfo? culture)
    {
        /// <summary>
        /// Returns the current culture from the globalization services if <paramref name="culture"/> is null, otherwise returns <paramref name="culture"/>. This method allows for a fallback to the globalization services' current culture when no specific culture is provided, ensuring that a valid culture is always returned for localization operations.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="GlobalizationServices"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <returns>The current culture from the globalization services if <paramref name="culture"/> is null, otherwise the provided culture.</returns>
        public CultureInfo OrContext() => culture.Or(GlobalizationServices.Current.CurrentCulture);
    }

    extension(CultureInfo culture)
    {
        /// <summary>
        /// Translates the key using the current culture.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="key">The key to translate.</param>
        /// <returns>The translated string.</returns>
        public string Translate(string key) => Localizer.Translation.Translate(key, TranslationOptionsPresets.Default, culture);

        /// <summary>
        /// Translates the key from a specific resource using the current culture. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="key">The key to translate.</param>
        /// <param name="resourceKey">The resource key to use for translation.</param>
        /// <returns>The translated string.</returns>
        public string Translate(string key, string resourceKey) => Localizer.Translation.Translate(key, TranslationOptionsPresets.Default, resourceKey,  culture);

        /// <summary>
        /// Translates the key using the current culture with specified translation options. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="key">The key to translate.</param>
        /// <param name="configure">A delegate to configure the translation options.</param>
        /// <returns>The translated string.</returns>
        public string Translate(string key, Action<TranslationOptionsBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new TranslationOptionsBuilder();

            configure(builder);

            return Localizer.Translation.Translate(key, builder.Build(), culture);
        }

        /// <summary>
        /// Translates the key from a specific resource using the current culture. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="key">The key to translate.</param>
        /// <param name="configure">A delegate to configure the translation options.</param>
        /// <param name="resourceKey">The resource key to use for translation.</param>
        /// <returns>The translated string.</returns>
        public string Translate(string key, Action<TranslationOptionsBuilder> configure, string resourceKey)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new TranslationOptionsBuilder();

            configure(builder);

            return Localizer.Translation.Translate(key, builder.Build(), resourceKey, culture);
        }

        /// <summary>
        /// Translates the key using the current culture with specified translation options. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="key">The key to translate.</param>
        /// <param name="options">The translation options to use.</param>
        /// <returns>The translated string.</returns>
        public string Translate(string key, TranslationOptions options) => Localizer.Translation.Translate(key, options, culture);

        /// <summary>
        /// Translates the key from a specific resource using the current culture. When null, the current culture is used.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience facade that uses <see cref="Localizer"/>.
        /// Prefer explicit DI overloads when services are available.
        /// </remarks>
        /// <param name="key">The key to translate.</param>
        /// <param name="options">The translation options to use.</param>
        /// <param name="resourceKey">The resource key to use for translation.</param>
        /// <returns>The translated string.</returns>
        public string Translate(string key, TranslationOptions options, string resourceKey) => Localizer.Translation.Translate(key, options, resourceKey, culture);
    }
}
