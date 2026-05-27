// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.UI.Notifications.Processors;
using MyNet.UI.Threading;

#pragma warning disable IDE0130
namespace MyNet.UI.Notifications;
#pragma warning restore IDE0130

/// <summary>
/// Extension methods for registering notification services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the built-in notification service and manager.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configureProcessors">Optional callback to add processors applied before publication.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddNotifications(
        this IServiceCollection services,
        Action<IList<INotificationProcessor>>? configureProcessors = null)
    {
        var processors = new List<INotificationProcessor>();
        configureProcessors?.Invoke(processors);

        services.TryAddSingleton<ISchedulerProvider, DefaultSchedulerProvider>();
        services.TryAddSingleton<INotificationService>(_ => new NotificationService(processors));
        services.TryAddSingleton<INotificationsManager, NotificationsManager>();

        return services;
    }
}
