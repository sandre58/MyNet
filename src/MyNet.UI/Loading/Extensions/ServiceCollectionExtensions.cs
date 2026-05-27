// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130
namespace MyNet.UI.Loading;
#pragma warning restore IDE0130

/// <summary>
/// Registers application-wide busy loading services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="BusyService"/> as <see cref="IBusyService"/>.
    /// View models use their own local <see cref="BusyService"/> from <see cref="ViewModels.ViewModelBase"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddBusy(this IServiceCollection services)
    {
        services.TryAddSingleton<IBusyService, BusyService>();

        return services;
    }
}
