// -----------------------------------------------------------------------
// <copyright file="LocalizationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace MyNet.Utilities.Localization;

public static class LocalizationService
{
    private static readonly Dictionary<CultureInfo, IDictionary<Type, object>> CultureProviders = [];
    private static readonly Dictionary<Type, object> DefaultProviders = [];

    private static readonly Lock LockCultureProviders = new();
    private static readonly Lock LockDefaultProviders = new();

    /// <summary>
    /// Registrer a provider for a specific culture.
    /// </summary>
    /// <typeparam name="TInterface">Provider interface.</typeparam>
    /// <typeparam name="TProvider">Provider implementation.</typeparam>
    public static void Register<TInterface, TProvider>(CultureInfo culture)
        where TProvider : TInterface
        =>
        Register<TInterface, TProvider>(culture, Activator.CreateInstance<TProvider>());

    /// <summary>
    /// Registrer a provider for a specific culture.
    /// </summary>
    /// <typeparam name="TInterface">Provider interface.</typeparam>
    /// <typeparam name="TProvider">Provider implementation.</typeparam>
    /// <param name="culture">Specific culture.</param>
    /// <param name="provider">Provider object.</param>
    public static void Register<TInterface, TProvider>(CultureInfo culture, TProvider provider)
        where TProvider : TInterface
    {
        lock (LockCultureProviders)
        {
            if (provider is null) return;
            if (!CultureProviders.ContainsKey(culture))
                CultureProviders.Add(culture, new Dictionary<Type, object>());

            if (!CultureProviders[culture].ContainsKey(typeof(TInterface)))
            {
                CultureProviders[culture].Add(typeof(TInterface), provider);
            }
            else
            {
                CultureProviders[culture][typeof(TInterface)] = provider;
            }
        }
    }

    /// <summary>
    /// Registrer a default provider for culture without specific provider.
    /// </summary>
    /// <typeparam name="TInterface">Provider interface.</typeparam>
    /// <typeparam name="TProvider">Provider implementation.</typeparam>
    public static void Register<TInterface, TProvider>()
        where TProvider : TInterface
        =>
        Register<TInterface, TProvider>(Activator.CreateInstance<TProvider>());

    /// <summary>
    /// Registrer a default provider for culture without specific provider.
    /// </summary>
    /// <typeparam name="TInterface">Provider interface.</typeparam>
    /// <typeparam name="TProvider">Provider implementation.</typeparam>
    /// <param name="provider">Provider object.</param>
    public static void Register<TInterface, TProvider>(TProvider provider)
        where TProvider : TInterface
    {
        lock (LockDefaultProviders)
        {
            if (provider is null) return;
            DefaultProviders[typeof(TInterface)] = provider;
        }
    }

    public static TProvider? Get<TProvider>(CultureInfo culture)
    {
        lock (LockDefaultProviders)
        {
            var provider = GetOrDefault<TProvider>(CultureProviders.GetOrDefault(culture, new Dictionary<Type, object>())!, typeof(TProvider));

            if (provider is not null || culture.IsNeutralCulture) return provider;

            // Prevent infinite recursion for invariant culture
            if (culture.Parent == culture)
                return GetOrDefault<TProvider>(DefaultProviders, typeof(TProvider));

            provider = Get<TProvider>(culture.Parent);
            return provider is not null ? provider : GetOrDefault<TProvider>(DefaultProviders, typeof(TProvider));
        }
    }

    public static TProvider? GetOrCurrent<TProvider>(CultureInfo? culture = null) =>
        Get<TProvider>(culture ?? CultureInfo.CurrentCulture);

    private static TProvider? GetOrDefault<TProvider>(IDictionary<Type, object> dictionary, Type type)
        => dictionary.TryGetValue(type, out var value) ? (TProvider?)value : default;
}
