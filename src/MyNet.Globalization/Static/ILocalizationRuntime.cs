// -----------------------------------------------------------------------
// <copyright file="ILocalizationRuntime.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Translation;

namespace MyNet.Globalization.Static;

/// <summary>
/// Defines the interface for a localization runtime, which provides access to the current culture, translation services, pluralization services, and localization provider contexts. This interface allows for retrieving localized resources and performing localization-related operations based on the current culture and context.
/// </summary>
public interface ILocalizationRuntime
{
    /// <summary>
    /// Gets the current culture of the localization runtime, which is used to determine the appropriate localized resources and formatting for the current context.
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Gets the translation service, which provides methods for retrieving localized strings based on translation keys and the current culture. This service is responsible for handling the translation logic and returning the appropriate localized values for the given keys.
    /// </summary>
    ITranslationService TranslationService { get; }

    /// <summary>
    /// Gets the pluralization service, which provides methods for determining the correct plural form of a word based on the current culture and context. This service is responsible for handling pluralization rules and returning the appropriate pluralized form of a word.
    /// </summary>
    IPluralizationService PluralizationService { get; }

    /// <summary>
    /// Gets the localization provider context for a specific culture, which allows for retrieving localized resources and performing localization-related operations based on the specified culture.
    /// </summary>
    /// <param name="culture">The culture for which to retrieve the localization provider context.</param>
    /// <returns>The localization provider context for the specified culture.</returns>
    ILocalizationServiceContext ForCulture(CultureInfo? culture = null);
}
