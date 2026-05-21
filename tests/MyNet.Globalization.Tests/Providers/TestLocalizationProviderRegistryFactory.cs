// -----------------------------------------------------------------------
// <copyright file="TestLocalizationProviderRegistryFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Globalization.Localization.Providers.Factories;
using MyNet.Globalization.Localization.Providers.Registration;

namespace MyNet.Globalization.Tests.Providers;

internal static class TestLocalizationProviderRegistryFactory
{
    public static LocalizationFactoryRegistry Create(params object[] factories)
    {
        ArgumentNullException.ThrowIfNull(factories);

        var registrations = new Dictionary<Type, ILocalizationServiceFactory>();

        foreach (var factory in factories)
        {
            ArgumentNullException.ThrowIfNull(factory);

            var providerFactoryInterfaces = factory.GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ILocalizationServiceFactory<>))
                .ToArray();

            if (providerFactoryInterfaces.Length == 0)
            {
                throw new ArgumentException($"Factory type '{factory.GetType()}' does not implement ILocalizationServiceFactory<TProvider>.", nameof(factories));
            }

            if (factory is not ILocalizationServiceFactory nonGenericFactory)
            {
                throw new ArgumentException($"Factory type '{factory.GetType()}' does not implement ILocalizationServiceFactory.", nameof(factories));
            }

            foreach (var providerFactoryInterface in providerFactoryInterfaces)
            {
                var providerType = providerFactoryInterface.GetGenericArguments()[0];
                registrations[providerType] = nonGenericFactory;
            }
        }

        return new(registrations);
    }
}
