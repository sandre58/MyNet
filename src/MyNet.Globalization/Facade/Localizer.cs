// -----------------------------------------------------------------------
// <copyright file="Localizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Localization.Policies;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Providers.Factories;
using MyNet.Globalization.Localization.Providers.Registration;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.Catalog;
using MyNet.Globalization.Localization.Translation.KeyResolving;

namespace MyNet.Globalization.Facade;

/// <summary>
/// Provides static access to the localization stack.
/// This static context is a convenience facade for legacy/non-DI scenarios.
/// Prefer explicit DI service usage in new code.
/// </summary>
public static class Localizer
{
    private static ILocalizationRuntime _runtime = CreateDefaultRuntime();
    private static int _configured;

    /// <summary>
    /// Configures the localization services by providing an <see cref="IServiceProvider"/>.
    /// Call this once during application startup after building the DI container.
    /// </summary>
    public static void Configure(ILocalizationRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);

        if (Interlocked.Exchange(ref _configured, 1) == 1)
            return;

        _runtime = runtime;
    }

    public static CultureInfo CurrentCulture => _runtime.CurrentCulture;

    /// <summary>Gets the culture-aware translation service (uses application culture automatically).</summary>
    public static ITranslationService Translation => _runtime.TranslationService;

    /// <summary>Gets the culture-aware pluralization service (uses application culture automatically).</summary>
    public static IPluralizationService Pluralization => _runtime.PluralizationService;

    /// <summary>
    /// Gets a localization provider context for the specified culture. If no culture is provided, uses the current application culture.
    /// </summary>
    /// <param name="culture">The culture for which to get the provider context. If null, the current culture is used.</param>
    /// <returns>The localization provider context for the specified culture.</returns>
    public static ILocalizationServiceContext ForCulture(CultureInfo? culture = null) => _runtime.ForCulture(culture);

    /// <summary>
    /// Creates a default localization runtime with no translations or providers, used when DI is not configured. This allows the static API to be used without throwing exceptions, but will return empty results for all translation and pluralization requests.
    /// </summary>
    /// <returns>The default localization runtime.</returns>
    private static LocalizationRuntime CreateDefaultRuntime()
    {
        TranslationCatalog defaultCatalog = new([]);
        ThreadCultureContext defaultCultureContext = new();
        LocalizationFactoryRegistry defaultFactoryRegistry = new(new Dictionary<Type, ILocalizationServiceFactory>());
        LocalizationServiceResolver defaultServiceResolver = new(defaultFactoryRegistry);
        PluralizationService defaultPluralizationService = new(defaultServiceResolver, defaultCultureContext);
        TranslationKeyResolver defaultTranslationKeyResolver = new(defaultPluralizationService);
        Translator defaultTranslator = new(defaultCatalog, defaultTranslationKeyResolver, defaultPluralizationService, CultureFallbackPolicies.ParentCulture);
        TranslationService defaultTranslationService = new(defaultTranslator, defaultCultureContext);

        return new(
            defaultCultureContext,
            defaultTranslationService,
            defaultPluralizationService,
            defaultServiceResolver);
    }
}
