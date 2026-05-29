// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.Preferences;
using MyNet.UI.ViewModels.Shell;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels;
#pragma warning restore IDE0130

/// <summary>
/// DI registration helpers for shell view models.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers shell panel view models. Register <see cref="MainWindowViewModel"/> in the host application
        /// (pass <see cref="FileMenuViewModel"/> when a file menu is required).
        /// </summary>
        public IServiceCollection AddShellViewModels()
        {
            services.TryAddTransient<ShellNotificationsViewModel>();
            services.TryAddTransient<FileMenuViewModel>();
            return services;
        }

        /// <summary>
        /// Registers preferences page view models.
        /// Register <see cref="PreferencesViewModel"/> in the host with the desired page list.
        /// </summary>
        public IServiceCollection AddPreferencesViewModels()
        {
            services.TryAddTransient<DisplayPreferencesViewModel>();
            services.TryAddTransient<TimeAndLanguagePreferencesViewModel>();
            return services;
        }

        /// <summary>
        /// Registers shell services and panel view models.
        /// Call after <c>AddDialogs()</c> so <see cref="ShellDrawerCoordinator"/> can subscribe to content dialog events.
        /// </summary>
        public IServiceCollection AddShell()
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
                    sp.GetRequiredService<Dialogs.ContentDialogs.IContentDialogService>(),
                    sp.GetRequiredService<IShellHostProvider>());
            });

            return services;
        }
    }
}
