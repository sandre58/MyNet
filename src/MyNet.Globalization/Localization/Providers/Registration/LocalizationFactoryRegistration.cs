// -----------------------------------------------------------------------
// <copyright file="LocalizationFactoryRegistration.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Localization.Providers.Factories;

namespace MyNet.Globalization.Localization.Providers.Registration;

/// <summary>
/// Defines a localization service configuration that uses a delegate to configure the service factory builder. This allows for flexible and concise configuration of localization services without the need for creating separate classes for each service configuration.
/// </summary>
/// <param name="configure">The delegate used to configure the service factory builder.</param>
/// <param name="priority">The priority of the service configuration. Higher values are applied later and can override previous registrations.</param>
/// <typeparam name="TService">The type of the localization service.</typeparam>
public sealed class LocalizationFactoryRegistration<TService>(Action<LocalizationServiceFactoryBuilder<TService>> configure, int priority = 0) : ILocalizationFactoryRegistration<TService>
    where TService : class, ICultureScoped
{
    /// <summary>
    /// Gets the priority of the service configuration.
    /// Configurations are applied in ascending order; higher values are therefore applied later and can override previous registrations.
    /// </summary>
    public int Priority { get; } = priority;

    /// <summary>
    /// Configures the localization service factory builder using the provided delegate. This method is called by the localization service registry when registering the service, allowing the delegate to set up the necessary configuration for the service factory.
    /// </summary>
    /// <param name="builder">The service factory builder to configure.</param>
    public void Configure(LocalizationServiceFactoryBuilder<TService> builder) => configure(builder);
}
