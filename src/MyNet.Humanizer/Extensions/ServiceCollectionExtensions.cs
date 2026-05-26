// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Display;
using MyNet.Humanizer.Display.Registration;
using MyNet.Humanizer.Display.Strategies;
using MyNet.Humanizer.Facade;
using MyNet.Humanizer.Formatting.Addresses;
using MyNet.Humanizer.Formatting.Addresses.Cultures;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Humanizer.Ordinalizing;
using MyNet.Humanizer.Ordinalizing.Cultures;
using MyNet.Humanizer.Resources;
using MyNet.Humanizer.Temporal;
using MyNet.Primitives;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Dependency injection extensions for registering humanizer providers.
/// Call this after AddLocalization().
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds humanizer services to the service collection.
        /// This method registers humanizer resources and providers into the localization system.
        /// </summary>
        /// <param name="configure">An action to configure the time localization options.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddHumanizer(Action<TimeLocalizationOptions>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            var options = new TimeLocalizationOptions();
            configure?.Invoke(options);

            // Ensure Localization infrastructure is loaded before registering humanizer services, as some providers depend on localization services.
            services.AddLocalization();

            // List formatters
            services.AddLocalizationService<IListFormatter>((sp, culture) => new ListFormatter(sp.GetRequiredService<ITranslationService>(), culture));

            // DateTime providers
            // Uses LocalizationServiceContext to resolve IListFormatter for the same culture without repeating it.
            services.AddLocalizationService<ITimeHumanizer>((sp, culture) => new TimeHumanizer(
                sp.GetRequiredService<ITranslationService>(),
                sp.GetRequiredService<ICultureScopedServiceSource<IListFormatter>>(),
                culture,
                options));

            // Ordinalizers
            services.AddLocalizationService<IOrdinalizer>((_, _) => Ordinalizers.Invariant);
            services.ConfigureLocalizationService<IOrdinalizer>(builder => builder
                .RegisterCulture(SupportedCultures.English, () => Ordinalizers.English)
                .RegisterCulture(SupportedCultures.French, () => Ordinalizers.French));

            // Address formatters
            services.AddLocalizationService<IAddressFormatter>((_, _) => AddressFormatters.Invariant);
            services.ConfigureLocalizationService<IAddressFormatter>(builder => builder
                .RegisterCulture(SupportedCultures.English, () => AddressFormatters.English)
                .RegisterCulture(SupportedCultures.French, () => AddressFormatters.French));

            // Resources
            services.AddTranslationResource(nameof(DateTimeResources), DateTimeResources.ResourceManager);
            services.AddTranslationResource(nameof(EnumResources), EnumResources.ResourceManager);
            services.AddTranslationResource(nameof(ListResources), ListResources.ResourceManager);

            // DisplayText providers
            services.AddDisplayTextInfrastructure();
            services.AddDisplayTextStrategy<Enum, EnumDisplayTextStrategy>();
            services.AddDisplayTextStrategy<ISmartEnum, SmartEnumDisplayTextStrategy>();

            return services;
        }

        /// <summary>
        /// Adds a display text strategy for the specified type and strategy implementation. This method registers the strategy as a singleton in the service collection, and also registers a corresponding strategy registration to associate the strategy with the target type. The strategy will then be available for resolution when retrieving display text for values of the specified type.
        /// </summary>
        /// <typeparam name="T">The target type for which the display text strategy is registered.</typeparam>
        /// <typeparam name="TStrategy">The display text strategy implementation.</typeparam>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddDisplayTextStrategy<T, TStrategy>()
            where T : notnull
            where TStrategy : class, IDisplayTextStrategy<T>
        {
            services.AddLocalization();
            services.AddDisplayTextInfrastructure();

            services.TryAddSingleton<TStrategy>();

            // Register the non-generic marker so the registry can discover all factories.
            // We use AddSingleton (not TryAddEnumerable) because TryAddEnumerable requires
            // a concrete implementation type for deduplication, which factory-delegate descriptors lack.
            services.TryAddSingleton<IDisplayTextStrategy<T>, TStrategy>();
            services.AddSingleton<IDisplayTextStrategyRegistration>(sp => new DisplayTextStrategyRegistration<T>(sp.GetRequiredService<TStrategy>()));

            return services;
        }

        /// <summary>
        /// Adds the necessary infrastructure for display text providers, including the registry and resolver.
        /// </summary>
        private void AddDisplayTextInfrastructure()
        {
            services.TryAddSingleton<IDisplayTextStrategyRegistry>(sp =>
            {
                var registrations = sp.GetServices<IDisplayTextStrategyRegistration>();

                var dictionary = registrations.ToDictionary(x => x.TargetType, x => x.Strategy);

                return new DisplayTextStrategyRegistry(dictionary);
            });
            services.TryAddSingleton<IDisplayTextStrategyResolver, DisplayTextStrategyResolver>();
            services.TryAddSingleton<IDisplayTextService, DisplayTextService>();
        }
    }

    extension(IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Configures the static Humanizer class to use the IDisplayTextService from the service provider. This method should be called after building the service provider to ensure that the Humanizer class is properly configured to retrieve display text using the registered display text strategies and services.
        /// </summary>
        /// <returns>The updated service provider.</returns>
        public IServiceProvider UseDisplayText()
        {
            TextHumanizer.Configure(serviceProvider.GetRequiredService<IDisplayTextService>());

            return serviceProvider;
        }
    }
}
