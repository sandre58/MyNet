// -----------------------------------------------------------------------
// <copyright file="ILocalizationFactoryRegistration.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Localization.Providers.Factories;

namespace MyNet.Globalization.Localization.Providers.Registration;

/// <summary>
/// Defines a contract for configuring a localization service within the localization service factory builder. Implementations of this interface can be used to set up and services the behavior of localization providers, such as specifying resource paths, caching strategies, or culture-specific settings.
/// </summary>
/// <typeparam name="TService">The type of the localization service.</typeparam>
public interface ILocalizationFactoryRegistration<TService>
    where TService : class, ICultureScoped
{
    /// <summary>
    /// Gets the priority of the localization service configuration.
    /// Configurations are applied in ascending order; higher values are therefore applied later and can override previous registrations.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Configures the localization service using the provided builder. This method is called during the registration process of the localization service, allowing you to set up necessary configurations and dependencies for the service to function correctly within the localization framework.
    /// </summary>
    /// <param name="builder">The builder used to configure the localization service.</param>
    void Configure(LocalizationServiceFactoryBuilder<TService> builder);
}
