// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130
namespace MyNet.UI.Theming;
#pragma warning restore IDE0130

/// <summary>
/// Dependency injection helpers for theming.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Configures the <see cref="ThemeManager"/> static bridge from the built service provider.
        /// Call after registering <see cref="IThemeService"/> and <see cref="IThemeBaseRegistry"/> in the host (WPF or Avalonia client).
        /// </summary>
        /// <returns>The same service provider for chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when required theming services are not registered.</exception>
        public IServiceProvider UseThemeManager()
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            ThemeManager.Configure(
                serviceProvider.GetRequiredService<IThemeService>(),
                serviceProvider.GetRequiredService<IThemeBaseRegistry>());

            return serviceProvider;
        }

        /// <summary>
        /// Configures <see cref="ThemeManager"/> when both <see cref="IThemeService"/> and <see cref="IThemeBaseRegistry"/> are registered; otherwise no-op.
        /// Used by <see cref="MyNet.UI.ServiceCollectionExtensions.UseUi"/>.
        /// </summary>
        /// <returns>The same service provider for chaining.</returns>
        public IServiceProvider UseThemeManagerIfAvailable()
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            var themeService = serviceProvider.GetService<IThemeService>();
            var themeRegistry = serviceProvider.GetService<IThemeBaseRegistry>();
            if (themeService is not null && themeRegistry is not null)
                ThemeManager.Configure(themeService, themeRegistry);

            return serviceProvider;
        }
    }
}
