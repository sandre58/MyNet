// -----------------------------------------------------------------------
// <copyright file="LocalizationRuntime.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Translation;

namespace MyNet.Globalization.Facade;

/// <summary>
/// Represents the runtime environment for localization, providing access to the current culture, translation services, pluralization services, and localization provider contexts. This class serves as a central point for managing localization-related operations and retrieving localized resources based on the current culture and context. It allows for retrieving localized strings, determining plural forms, and accessing localization providers for specific cultures, enabling applications to adapt their content and behavior according to the user's language and cultural preferences.
/// </summary>
/// <param name="cultureContext">The culture context that provides information about the current culture.</param>
/// <param name="translationService">The translation service used for retrieving localized strings.</param>
/// <param name="pluralizationService">The pluralization service used for determining the correct plural forms of words.</param>
/// <param name="resolver">The localization provider resolver used for accessing localization providers for specific cultures.</param>
public sealed class LocalizationRuntime(
    ICultureContext cultureContext,
    ITranslationService translationService,
    IPluralizationService pluralizationService,
    ILocalizationServiceResolver resolver)
    : ILocalizationRuntime
{
    /// <inheritdoc/>
    public CultureInfo CurrentCulture => cultureContext.CurrentCulture;

    /// <inheritdoc/>
    public ITranslationService TranslationService => translationService;

    /// <inheritdoc/>
    public IPluralizationService PluralizationService => pluralizationService;

    /// <inheritdoc/>
    public ILocalizationServiceContext ForCulture(CultureInfo? culture = null) => resolver.ForCulture(culture ?? CurrentCulture);
}
