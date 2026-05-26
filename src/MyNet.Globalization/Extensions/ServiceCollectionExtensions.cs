// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;
using MyNet.Globalization.Events;
using MyNet.Globalization.Facade;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Inflection.Cultures;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Providers.Factories;
using MyNet.Globalization.Localization.Providers.Registration;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.Catalog;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using MyNet.Globalization.Localization.Translation.KeyResolving;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Globalization;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds globalization services (culture, time zone, events) to the service collection.
        /// Registers <see cref="ICultureService"/> as the application-level <see cref="ICultureContext"/>.
        /// </summary>
        /// <param name="configure">An optional action to configure the <see cref="GlobalizationOptions"/>.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddGlobalization(Action<GlobalizationOptions>? configure = null)
        {
            var options = new GlobalizationOptions();
            configure?.Invoke(options);

            services.TryAddSingleton(options);

            // Core services
            services.TryAddSingleton<ICultureService, CultureService>();
            services.TryAddSingleton<ITimeZoneService, TimeZoneService>();
            services.TryAddSingleton<IGlobalizationService, GlobalizationService>();

            // Events
            services.TryAddSingleton<IGlobalizationEvents, GlobalizationEvents>();

            return services;
        }

        /// <summary>
        /// Adds localization services (translation pipeline, provider resolver) to the service collection.
        /// Requires that an <see cref="ICultureContext"/> has already been registered —
        /// either via <c>AddGlobalization()</c> (which registers the application-level <see cref="ICultureService"/>)
        /// or by registering a custom <see cref="ICultureContext"/> implementation.
        /// When neither is present, falls back to reading <see cref="System.Globalization.CultureInfo.CurrentCulture"/>
        /// of the current thread via <c>ThreadCultureContext</c>.
        /// </summary>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddLocalization(Action<LocalizationServiceFactoryBuilder<IInflector>>? configure = null)
        {
            // ------------------------------------------------------------------
            // Culture context
            // Prefer the application-level ICultureService (registered by AddGlobalization).
            // Fall back to ThreadCultureContext only when neither is available.
            // ------------------------------------------------------------------
            services.TryAddSingleton<ICultureContext>(sp => sp.GetService<ICultureService>() ?? (ICultureContext)new ThreadCultureContext());

            // ------------------------------------------------------------------
            // Translation catalog
            // ------------------------------------------------------------------
            services.TryAddSingleton<ITranslationCatalog>(sp =>
            {
                var builder = new TranslationCatalogBuilder();

                foreach (var contribution in sp.GetServices<ITranslationCatalogContribution>().OrderBy(x => x.Priority))
                {
                    contribution.Apply(builder);
                }

                return builder.Build();
            });

            // ------------------------------------------------------------------
            // Localization provider registry (uses IServiceProvider to resolve factories on demand)
            // ------------------------------------------------------------------
            services.TryAddSingleton<ILocalizationFactoryRegistry>(sp =>
            {
                var factories = sp.GetServices<ILocalizationServiceFactory>().ToDictionary(x => x.TargetType, x => x);

                return new LocalizationFactoryRegistry(factories);
            });

            // ------------------------------------------------------------------
            // Localization provider resolver
            // ------------------------------------------------------------------
            services.TryAddSingleton<ILocalizationServiceResolver, LocalizationServiceResolver>();
            services.TryAddSingleton(typeof(ICultureScopedServiceSource<>), typeof(CultureScopedServiceSource<>));

            // ------------------------------------------------------------------
            // Translation pipeline
            // ------------------------------------------------------------------
            services.TryAddSingleton<ITranslationKeyProvider, TranslationKeyProvider>();
            services.TryAddSingleton<ITranslationKeyResolver, TranslationKeyResolver>();

            // --- Pure, stateless translator ---
            services.TryAddSingleton<ITranslator>(sp =>
                new Translator(sp.GetRequiredService<ITranslationCatalog>(),
                    sp.GetRequiredService<ITranslationKeyResolver>(),
                    sp.GetRequiredService<IPluralizationService>(),
                    sp.GetService<GlobalizationOptions>()?.CultureFallbackPolicy));

            // --- Contextual translation service ---
            services.TryAddSingleton<ITranslationService, TranslationService>();

            // Static facade initialization
            services.TryAddSingleton<ILocalizationRuntime, LocalizationRuntime>();

            // ------------------------------------------------------------------
            // Inflection providers
            // ------------------------------------------------------------------
            services.AddInflection();

            if (configure is not null)
                services.ConfigureLocalizationService(configure);

            return services;
        }

        /// <summary>
        /// Adds the default inflection provider to the service collection with built-in support for English and French.
        /// Additional cultures can be registered via <c>ConfigureLocalizationService</c>.
        /// </summary>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddInflection()
        {
            services.TryAddSingleton<IPluralizationService, PluralizationService>();
            services.AddLocalizationService<IInflector>((_, _) => Inflectors.Invariant);

            services.ConfigureLocalizationService<IInflector>(builder => builder
                .RegisterCulture(SupportedCultures.English, () => Inflectors.English)
                .RegisterCulture(SupportedCultures.French, () => Inflectors.French));

            return services;
        }

        /// <summary>
        /// Registers a culture-aware localization provider of type <typeparamref name="TService"/>.
        /// The <paramref name="defaultFactory"/> is invoked for cultures that have no explicit registration.
        /// Use <c>ConfigureLocalizationService</c> to register culture-specific overrides.
        /// </summary>
        /// <param name="defaultFactory">Factory invoked with (IServiceProvider, CultureInfo) to create the default provider.</param>
        /// <typeparam name="TService">The culture-aware provider type to register.</typeparam>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddLocalizationService<TService>(Func<IServiceProvider, CultureInfo, TService> defaultFactory)
            where TService : class, ICultureScoped
        {
            // Guard against duplicate registrations (TryAddEnumerable with factory delegates
            // cannot distinguish entries by implementation type and would throw ArgumentException).
            if (services.Any(d => d.ServiceType == typeof(ILocalizationServiceFactory<TService>)))
                return services;

            services.AddSingleton<ILocalizationServiceFactory<TService>>(sp =>
            {
                var builder = new LocalizationServiceFactoryBuilder<TService>(x => defaultFactory(sp, x));

                var services = sp.GetServices<ILocalizationFactoryRegistration<TService>>()
                    .OrderBy(x => x.Priority).ToList();

                services.ForEach(cfg => cfg.Configure(builder));

                return builder.Build();
            });

            // Register the non-generic marker so the registry can discover all factories.
            // We use AddSingleton (not TryAddEnumerable) because TryAddEnumerable requires
            // a concrete implementation type for deduplication, which factory-delegate descriptors lack.
            services.AddSingleton<ILocalizationServiceFactory>(sp => sp.GetRequiredService<ILocalizationServiceFactory<TService>>());

            return services;
        }

        /// <summary>
        /// Registers culture-specific overrides for a previously added <typeparamref name="TService"/>.
        /// Multiple calls are allowed; registrations are applied in ascending priority order.
        /// </summary>
        /// <param name="configure">Action that registers culture-specific factories on the builder.</param>
        /// <param name="priority">Priority of this registration. Higher values are applied later (override earlier ones).</param>
        /// <typeparam name="TService">The culture-aware provider type to configure.</typeparam>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection ConfigureLocalizationService<TService>(Action<LocalizationServiceFactoryBuilder<TService>> configure, int priority = 0)
            where TService : class, ICultureScoped
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILocalizationFactoryRegistration<TService>>(new LocalizationFactoryRegistration<TService>(configure, priority)));

            return services;
        }

        /// <summary>
        /// Contributes a translation resource manager to the catalog at startup.
        /// </summary>
        /// <param name="resourceKey">Unique key identifying the resource (e.g., "DateTimeResources").</param>
        /// <param name="resourceManager">The resource manager instance.</param>
        /// <param name="priority">Contribution priority. Higher values are applied later.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddTranslationResource(string resourceKey, ResourceManager resourceManager, int priority = 0)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);
            ArgumentNullException.ThrowIfNull(resourceManager);

            // Multiple resources share the same contribution implementation type,
            // so we must not use TryAddEnumerable here (it deduplicates by implementation type).
            services.AddSingleton<ITranslationCatalogContribution>(
                new TranslationCatalogContribution(registry => registry.Register(resourceKey, resourceManager), priority));

            return services;
        }
    }

    extension(IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Initializes the globalization static facade by configuring it with the registered <see cref="IGlobalizationService"/> implementation.
        /// </summary>
        /// <returns>The updated service provider.</returns>
        public IServiceProvider UseGlobalization()
        {
            GlobalizationServices.Configure(serviceProvider.GetRequiredService<IGlobalizationService>());

            return serviceProvider;
        }

        /// <summary>
        /// Initializes the localization static facade by configuring it with the registered <see cref="ILocalizationRuntime"/> implementation.
        /// </summary>
        /// <returns>The updated service provider.</returns>
        public IServiceProvider UseLocalization()
        {
            Localizer.Configure(serviceProvider.GetRequiredService<ILocalizationRuntime>());

            return serviceProvider;
        }
    }
}
