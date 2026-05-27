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

#pragma warning disable IDE0130
namespace MyNet.UI.Locators;
#pragma warning restore IDE0130

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
        /// Registers Locator services: <see cref="SuffixConvention"/> (default), <see cref="ITypeResolver"/>,
        /// <see cref="IViewLocator"/>, <see cref="IViewModelLocator"/> and <see cref="IViewFactory"/>.
        /// <para/>
        /// Additional layout strategies are opt-in: <see cref="AddNamespaceConvention"/>,
        /// <see cref="AddParentNamespaceConvention"/>, <see cref="AddAssemblyRootConvention"/>.
        /// <para/>
        /// The optional <paramref name="configureResolver"/> callback is called after the resolver has been built, allowing manual
        /// type registrations (e.g. <c>resolver.Register(typeof(MyViewModel), typeof(MyView))</c>).
        /// </summary>
        /// <param name="configureResolver">Optional callback to register manual mappings on the resolver.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddViewLocators(Action<ITypeResolver>? configureResolver = null)
        {
            // Default: SuffixConvention handles ViewModels/Views segment swap and multiple view suffixes.
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention, SuffixConvention>());

            // --- Resolver ---
            services.TryAddSingleton<ITypeResolver>(sp =>
            {
                var conventions = sp.GetServices<ITypeNamingConvention>();
                ITypeResolver resolver = new TypeResolver(conventions);
                configureResolver?.Invoke(resolver);
                return resolver;
            });

            // ViewLocator: DI-first, fallback to Activator.CreateInstance for views not registered in DI.
            services.TryAddSingleton<IViewLocator, ViewLocator>();

            // ViewModelLocator: strict DI only — view models must be explicitly registered.
            services.TryAddSingleton<IViewModelLocator, ViewModelLocator>();

            services.TryAddSingleton<IViewFactory, ViewFactory>();

            return services;
        }

        /// <summary>
        /// Adds a single extra naming convention to the resolver chain.
        /// Call before <see cref="AddViewLocators"/> for higher priority, or after for lower priority.
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
        /// Adds <see cref="NamespaceConvention"/> (ViewModels/Views segment swap, <c>*View</c> suffix only).
        /// </summary>
        public IServiceCollection AddNamespaceConvention()
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention, NamespaceConvention>());
            return services;
        }

        /// <summary>
        /// Adds <see cref="ParentNamespaceConvention"/> (views under <c>{Parent}.Views</c>).
        /// </summary>
        public IServiceCollection AddParentNamespaceConvention()
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention, ParentNamespaceConvention>());
            return services;
        }

        /// <summary>
        /// Adds <see cref="AssemblyRootConvention"/> with a configurable sub-namespace.
        /// Useful when views live under <c>{AssemblyName}.&lt;subNamespace&gt;.{Name}View</c>.
        /// </summary>
        /// <param name="subNamespace">The sub-namespace segment for views (default: <c>"UI.Views"</c>).</param>
        public IServiceCollection AddAssemblyRootConvention(string subNamespace = "UI.Views")
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITypeNamingConvention>(new AssemblyRootConvention(subNamespace)));
            return services;
        }
    }
}
