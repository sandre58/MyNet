// -----------------------------------------------------------------------
// <copyright file="LocalizationServiceFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Globalization.Localization.Policies;
using MyNet.Primitives.Comparers;

namespace MyNet.Globalization.Localization.Providers.Factories;

/// <summary>
/// A localization service factory that maps specific cultures to their corresponding service factories,
/// allowing for culture-specific localization services with configurable <see cref="ICultureFallbackPolicy"/>.
/// </summary>
/// <param name="factories">A dictionary mapping cultures to their corresponding service factories.</param>
/// <param name="defaultFactory">The default service factory to use when no specific factory is found for a culture.</param>
/// <param name="fallbackPolicy">
/// The culture fallback policy. Defaults to <see cref="CultureFallbackPolicies.ParentCulture"/>.
/// Use <see cref="CultureFallbackPolicies.None"/> to disable fallback.
/// </param>
/// <typeparam name="TService">The type of the localization service.</typeparam>
public sealed class LocalizationServiceFactory<TService>(
    IDictionary<CultureInfo, Func<TService>> factories,
    Func<CultureInfo, TService> defaultFactory,
    ICultureFallbackPolicy? fallbackPolicy = null)
    : ILocalizationServiceFactory<TService>
    where TService : class, ICultureScoped
{
    private readonly FrozenDictionary<CultureInfo, Func<TService>> _cultureFactories = factories.ToFrozenDictionary(CultureInfoNameComparer.Instance);
    private readonly Func<CultureInfo, TService> _defaultFactory = defaultFactory ?? throw new ArgumentNullException(nameof(defaultFactory));
    private readonly ICultureFallbackPolicy _fallbackPolicy = fallbackPolicy ?? CultureFallbackPolicies.ParentCulture;

    /// <inheritdoc />
    public Type TargetType => typeof(TService);

    /// <inheritdoc />
    public TService Create(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var current = culture;

        while (true)
        {
            if (_cultureFactories.TryGetValue(current, out var factory))
                return factory();

            var fallback = _fallbackPolicy.GetFallback(current);
            if (fallback is null)
                break;

            current = fallback;
        }

        return _defaultFactory(culture);
    }
}
