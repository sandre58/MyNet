// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using MyNet.Geography.Localization.Providers;
using MyNet.Humanizer;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Geography;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the embedded country flag provider to the service collection, allowing retrieval of country flags from embedded resources.
    /// </summary>
    /// <param name="services">The service collection to which the provider will be added.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGeographyLocalization(this IServiceCollection services)
    {
        services.AddDisplayTextStrategy<Country, CountryDisplayTextStrategy>();

        return services;
    }
}
