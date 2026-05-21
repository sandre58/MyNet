// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.UI.Locators.Conventions;
using MyNet.UI.Locators.Factories;

namespace MyNet.UI.Locators;

/// <summary>
/// Extension methods for registering the Locator system into a <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Locator system to the DI container, including default naming conventions, resolver, locators and factory.
    /// </summary>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers all default Locator services into the DI container: <see cref="SuffixConvention"/>, <see cref="NamespaceConvention"/>,
        /// <see cref="ITypeResolver"/>, <see cref="IViewLocator"/>, <see cref="IViewModelLocator"/> and <see cref="IViewFactory"/>.
        /// <para/>
        /// The optional <paramref name="configureResolver"/> callback is called after the resolver has been built, allowing manual
        /// type registrations (e.g. <c>resolver.Register(typeof(MyViewModel), typeof(MyView))</c>).
        /// </summary>
        /// <param name="configureResolver">Optional callback to register manual mappings on the resolver.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddViewLocators(Action<ITypeResolver>? configureResolver = null)
        {
            // --- Naming conventions (all registered as ITypeNamingConvention → collected by TypeResolver via IEnumerable<T>) ---
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention, SuffixConvention>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention, NamespaceConvention>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention, ParentNamespaceConvention>());

            // --- Resolver ---
            services.TryAddSingleton<ITypeResolver>(sp =>
            {
                var conventions = sp.GetServices<ITypeNamingConvention>();
                ITypeResolver resolver = new TypeResolver(conventions);
                configureResolver?.Invoke(resolver);
                return resolver;
            });

            // --- Locators ---
            // ViewLocator: DI-first, fallback to Activator.CreateInstance for views not registered in DI.
            services.TryAddSingleton<IViewLocator, ViewLocator>();

            // ViewModelLocator: strict DI only — view models must be explicitly registered.
            services.TryAddSingleton<IViewModelLocator, ViewModelLocator>();

            // --- Factory ---
            services.TryAddSingleton<IViewFactory, ViewFactory>();

            return services;
        }

        /// <summary>
        /// Adds a single extra naming convention to the resolver chain.
        /// Call before <see cref="ServiceCollectionExtensions.AddViewLocators"/> if you need custom conventions evaluated first, or after for lower-priority fallbacks.
        /// </summary>
        /// <typeparam name="TConvention">The convention implementation type.</typeparam>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddViewNamingConvention<TConvention>()
            where TConvention : class, ITypeNamingConvention
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention, TConvention>());
            return services;
        }

        /// <summary>
        /// Adds <see cref="AssemblyRootConvention"/> to the resolver chain with a configurable sub-namespace.
        /// Useful for applications where views live under <c>{AssemblyName}.&lt;subNamespace&gt;.{Name}View</c>.
        /// </summary>
        /// <param name="subNamespace">The sub-namespace segment for views (default: <c>"UI.Views"</c>).</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddAssemblyRootConvention(string subNamespace = "UI.Views")
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention>(new AssemblyRootConvention(subNamespace)));
            return services;
        }
    }
}
