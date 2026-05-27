// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130
namespace MyNet.UI.Navigation;
#pragma warning restore IDE0130

/// <summary>
/// Extension methods for registering navigation services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the modern navigation stack with its default implementations.
        /// </summary>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddNavigation()
        {
            services.TryAddSingleton<INavigationJournal, NavigationJournal>();
            services.TryAddSingleton<INavigationLifecycle, NavigationLifecycle>();
            services.TryAddSingleton<INavigationService, NavigationService>();
            services.TryAddSingleton<INavigationClient, NavigationClient>();

            return services;
        }

        /// <summary>
        /// Registers a navigation guard implementation.
        /// Guards run in registration order; the first guard returning <see langword="false"/> cancels navigation.
        /// </summary>
        /// <typeparam name="TGuard">Guard implementation type.</typeparam>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddNavigationGuard<TGuard>()
            where TGuard : class, INavigationGuard
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<INavigationGuard, TGuard>());
            return services;
        }

        /// <summary>
        /// Registers a navigation guard instance.
        /// Guards run in registration order; the first guard returning <see langword="false"/> cancels navigation.
        /// </summary>
        /// <typeparam name="TGuard">Guard contract type.</typeparam>
        /// <param name="guard">Guard instance.</param>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddNavigationGuard<TGuard>(TGuard guard)
            where TGuard : class, INavigationGuard
        {
            ArgumentNullException.ThrowIfNull(guard);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<INavigationGuard>(guard));
            return services;
        }

        /// <summary>
        /// Registers a navigation middleware implementation.
        /// Middleware runs in registration order as the outermost layer first (similar to ASP.NET Core).
        /// </summary>
        /// <typeparam name="TMiddleware">Middleware implementation type.</typeparam>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddNavigationMiddleware<TMiddleware>()
            where TMiddleware : class, INavigationMiddleware
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<INavigationMiddleware, TMiddleware>());
            return services;
        }

        /// <summary>
        /// Registers a navigation middleware instance.
        /// Middleware runs in registration order as the outermost layer first (similar to ASP.NET Core).
        /// </summary>
        /// <typeparam name="TMiddleware">Middleware contract type.</typeparam>
        /// <param name="middleware">Middleware instance.</param>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddNavigationMiddleware<TMiddleware>(TMiddleware middleware)
            where TMiddleware : class, INavigationMiddleware
        {
            ArgumentNullException.ThrowIfNull(middleware);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<INavigationMiddleware>(middleware));
            return services;
        }
    }
}
