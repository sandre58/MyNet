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
    /// <summary>
    /// Configures the <see cref="ThemeManager"/> static bridge from the built service provider.
    /// Call after registering <see cref="IThemeService"/> and <see cref="IThemeBaseRegistry"/> in the host (WPF or Avalonia client).
    /// </summary>
    /// <param name="serviceProvider">Built application service provider.</param>
    /// <returns>The same service provider for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required theming services are not registered.</exception>
    public static IServiceProvider UseThemeManager(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        ThemeManager.Configure(
            serviceProvider.GetRequiredService<IThemeService>(),
            serviceProvider.GetRequiredService<IThemeBaseRegistry>());

        return serviceProvider;
    }
}
