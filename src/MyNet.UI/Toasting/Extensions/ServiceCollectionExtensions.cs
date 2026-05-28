// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.UI.Notifications;
using MyNet.UI.Threading;
using MyNet.UI.Toasting.Filters;
using MyNet.UI.Toasting.Settings;

#pragma warning disable IDE0130
namespace MyNet.UI.Toasting;
#pragma warning restore IDE0130

/// <summary>
/// Extension methods for registering toast services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers toast services with default implementations.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configureOptions">Optional callback used to customize toast manager options.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddToasting(
        this IServiceCollection services,
        Action<ToastManagerOptions>? configureOptions = null)
    {
        var options = new ToastManagerOptions();
        configureOptions?.Invoke(options);

        services.AddSchedulerProvider();
        services.TryAddSingleton<INotificationService>(_ => new NotificationService([]));
        services.TryAddSingleton<IToastFilter, AllToastsFilter>();
        services.TryAddSingleton<IToastFactory, DefaultToastFactory>();
        services.TryAddSingleton(options);
        services.TryAddSingleton<IToastManager, ToastManager>();

        return services;
    }
}
