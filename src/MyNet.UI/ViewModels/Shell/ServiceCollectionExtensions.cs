// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.Preferences;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// DI registration helpers for shell view models and services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers shell services and panel view models.
    /// Call after <c>AddDialogs()</c> so <see cref="ShellDrawerCoordinator"/> can subscribe to content dialog events.
    /// </summary>
    public static IServiceCollection AddShell(this IServiceCollection services)
    {
        services.TryAddSingleton<IShellHostProvider, ShellHostProvider>();
        services.TryAddSingleton<IShellService, ShellService>();
        services.TryAddTransient<ShellNotificationsViewModel>();
        services.TryAddTransient<ShellCultureViewModel>();
        services.TryAddTransient<ShellThemeViewModel>();
        services.TryAddTransient<FileMenuViewModel>();
        services.TryAddTransient<SplashScreenViewModel>();

        services.AddPreferencesViewModels();

        services.TryAddSingleton<ITaskbarProgressSource, TaskbarProgressSource>();
        services.TryAddSingleton<BusyTaskbarCoordinator>(sp =>
            new BusyTaskbarCoordinator(
                sp.GetRequiredService<IBusyService>(),
                sp.GetRequiredService<ITaskbarProgressSource>()));

        services.TryAddSingleton<ShellNotificationsDrawerCoordinator>();
        services.TryAddSingleton<ShellDrawerCoordinator>(sp =>
        {
            _ = sp.GetRequiredService<IShellHostProvider>();
            return new ShellDrawerCoordinator(
                sp.GetRequiredService<MyNet.UI.Dialogs.ContentDialogs.IContentDialogService>(),
                sp.GetRequiredService<IShellHostProvider>());
        });

        return services;
    }
}
