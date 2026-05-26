// -----------------------------------------------------------------------
// <copyright file="LocalizationServiceFactoryBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Globalization.Localization.Policies;
using MyNet.Primitives.Comparers;

namespace MyNet.Globalization.Localization.Providers.Factories;

/// <summary>
/// Mutable builder used to configure a culture map service factory.
/// </summary>
/// <param name="defaultFactory">The default service factory.</param>
/// <typeparam name="TService">The service type.</typeparam>
public sealed class LocalizationServiceFactoryBuilder<TService>(Func<CultureInfo, TService> defaultFactory)
    where TService : class, ICultureScoped
{
    private readonly Dictionary<CultureInfo, Func<TService>> _cultureFactories = new(CultureInfoNameComparer.Instance);
    private Func<CultureInfo, TService> _defaultFactory = defaultFactory;
    private ICultureFallbackPolicy _fallbackPolicy = CultureFallbackPolicies.ParentCulture;

    /// <summary>
    /// Sets the culture fallback policy applied when no factory is found for an exact culture.
    /// Defaults to <see cref="CultureFallbackPolicies.ParentCulture"/> (walks up the culture hierarchy).
    /// Use <see cref="CultureFallbackPolicies.None"/> to disable fallback.
    /// </summary>
    public void SetFallbackPolicy(ICultureFallbackPolicy value) => _fallbackPolicy = value;

    /// <summary>
    /// Sets the default provider factory.
    /// </summary>
    public LocalizationServiceFactoryBuilder<TService> SetDefault(Func<CultureInfo, TService> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _defaultFactory = factory;

        return this;
    }

    /// <summary>
    /// Registers a provider factory for an exact culture.
    /// </summary>
    public LocalizationServiceFactoryBuilder<TService> RegisterCulture(CultureInfo culture, Func<TService> factory, CultureRegistrationBehavior behavior = CultureRegistrationBehavior.Replace)
    {
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(factory);

        if (_cultureFactories.ContainsKey(culture))
        {
            switch (behavior)
            {
                case CultureRegistrationBehavior.SkipIfExists:
                    return this;
                case CultureRegistrationBehavior.ThrowIfExists:
                    throw new InvalidOperationException($"A provider factory is already registered for culture '{culture.Name}'.");
                case CultureRegistrationBehavior.Replace:
                default:
                    break;
            }
        }

        _cultureFactories[culture] = factory;

        return this;
    }

    /// <summary>
    /// Registers a provider factory for the specified culture name.
    /// </summary>
    public LocalizationServiceFactoryBuilder<TService> RegisterCulture(string cultureName, Func<TService> factory, CultureRegistrationBehavior behavior = CultureRegistrationBehavior.Replace)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cultureName);

        return RegisterCulture(CultureInfo.GetCultureInfo(cultureName), factory, behavior);
    }

    /// <summary>
    /// Builds the immutable provider factory.
    /// </summary>
    public ILocalizationServiceFactory<TService> Build() => new LocalizationServiceFactory<TService>(_cultureFactories, _defaultFactory, _fallbackPolicy);
}
