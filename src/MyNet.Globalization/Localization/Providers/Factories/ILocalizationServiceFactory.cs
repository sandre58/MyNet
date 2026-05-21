// -----------------------------------------------------------------------
// <copyright file="ILocalizationServiceFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Globalization.Localization.Providers.Factories;

/// <summary>
/// Factory abstraction responsible for choosing and creating a provider for a requested culture.
/// </summary>
/// <typeparam name="TProvider">Provider type.</typeparam>
public interface ILocalizationServiceFactory<out TProvider> : ILocalizationServiceFactory
    where TProvider : class, ICultureScoped
{
    /// <summary>
    /// Creates the provider matching the requested culture.
    /// </summary>
    /// <param name="culture">Requested culture.</param>
    /// <returns>The provider instance.</returns>
    TProvider Create(CultureInfo culture);
}

/// <summary>
/// Marker interface for localization provider factories, allowing non-generic registration of factories in DI container.
/// </summary>
public interface ILocalizationServiceFactory
{
    /// <summary>
    /// Gets the target service type of the factory. This property indicates the type of service that the factory is responsible for creating, allowing for proper registration and retrieval of factories based on the service type in the localization framework.
    /// </summary>
    Type TargetType { get; }
}
