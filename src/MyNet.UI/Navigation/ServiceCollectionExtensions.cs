// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MyNet.UI.Navigation;

/// <summary>
/// Extension methods for registering navigation services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the modern navigation stack with its default implementations.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.TryAddSingleton<INavigationJournal, NavigationJournal>();
        services.TryAddSingleton<INavigationLifecycle, NavigationLifecycle>();
        services.TryAddSingleton<INavigationService, NavigationService>();
        services.TryAddSingleton<INavigationClient, NavigationClient>();

        return services;
    }
}
