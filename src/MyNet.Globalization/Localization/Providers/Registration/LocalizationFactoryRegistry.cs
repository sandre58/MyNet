// -----------------------------------------------------------------------
// <copyright file="LocalizationFactoryRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyNet.Globalization.Localization.Providers.Factories;

namespace MyNet.Globalization.Localization.Providers.Registration;

/// <summary>
/// Current implementation of <see cref="ILocalizationFactoryRegistry"/> that stores factories in a frozen dictionary for efficient retrieval.
/// </summary>
/// <param name="factories">The factories to be registered.</param>
public sealed class LocalizationFactoryRegistry(IReadOnlyDictionary<Type, ILocalizationServiceFactory> factories) : ILocalizationFactoryRegistry
{
    private readonly FrozenDictionary<Type, ILocalizationServiceFactory> _factories = factories.ToFrozenDictionary();

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, ILocalizationServiceFactory> Factories => _factories;

    /// <inheritdoc />
    public bool TryGetFactory<TService>([NotNullWhen(true)] out ILocalizationServiceFactory<TService>? factory)
        where TService : class, ICultureScoped
    {
        if (_factories.TryGetValue(typeof(TService), out var obj) && obj is ILocalizationServiceFactory<TService> typedFactory)
        {
            factory = typedFactory;
            return true;
        }

        factory = null;
        return false;
    }
}
