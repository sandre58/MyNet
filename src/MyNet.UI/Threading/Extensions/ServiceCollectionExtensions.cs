// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130
namespace MyNet.UI.Threading;
#pragma warning restore IDE0130

/// <summary>
/// Extension methods for registering threading services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default <see cref="ISchedulerProvider"/> when none is already registered.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddSchedulerProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<ISchedulerProvider, DefaultSchedulerProvider>();
        return services;
    }
}
