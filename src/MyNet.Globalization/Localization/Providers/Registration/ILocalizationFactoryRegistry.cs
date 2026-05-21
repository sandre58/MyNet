// -----------------------------------------------------------------------
// <copyright file="ILocalizationFactoryRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyNet.Globalization.Localization.Providers.Factories;

namespace MyNet.Globalization.Localization.Providers.Registration;

/// <summary>
/// Defines a registry for localization service factories, allowing retrieval of factories based on the service type.
/// </summary>
public interface ILocalizationFactoryRegistry
{
    /// <summary>
    /// Attempts to retrieve a localization service factory for the specified service type.
    /// </summary>
    /// <param name="factory">The retrieved factory, if found; otherwise, null.</param>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <returns>True if a factory was found; otherwise, false.</returns>
    bool TryGetFactory<TService>([NotNullWhen(true)] out ILocalizationServiceFactory<TService>? factory)
        where TService : class, ICultureScoped;

    /// <summary>
    /// Gets a read-only dictionary of all registered factories, keyed by their service type. This allows for enumeration of available factories and their associated service types.
    /// </summary>
    IReadOnlyDictionary<Type, ILocalizationServiceFactory> Factories { get; }
}
