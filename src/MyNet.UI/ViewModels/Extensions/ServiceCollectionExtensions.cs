// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Loading;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Preferences;
using MyNet.UI.ViewModels.Shell;
using MyNet.UI.ViewModels.Shell.About;
using MyNet.UI.ViewModels.Shell.Chrome;
using MyNet.UI.ViewModels.Shell.Drawers;
using MyNet.UI.ViewModels.Shell.FileMenu;
using MyNet.UI.ViewModels.Shell.Host;
using MyNet.UI.ViewModels.Shell.Notifications;
using MyNet.UI.ViewModels.Shell.Services;
using MyNet.UI.ViewModels.Shell.Startup;
using MyNet.UI.ViewModels.Shell.Taskbar;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels;
#pragma warning restore IDE0130

/// <summary>
/// DI registration helpers for view models.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers shell services and panel view models.
        /// Call after <c>AddDialogs()</c> so <see cref="ShellDrawerCoordinator"/> can subscribe to content dialog events.
        /// Register <see cref="ShellHostViewModel"/> in the host application (pass <see cref="FileMenuViewModel"/> when required).
        /// </summary>
        public IServiceCollection AddShell()
        {
            services.TryAddSingleton<IApplicationInfo, ApplicationInfo>();
            services.TryAddSingleton<IShellHostProvider, ShellHostProvider>();
            services.TryAddSingleton<ShellService>();
            services.TryAddSingleton<IShellService>(static sp => sp.GetRequiredService<ShellService>());
            services.TryAddSingleton<IShellDrawerService>(static sp => sp.GetRequiredService<ShellService>());
            services.TryAddSingleton<IShellFileMenuService>(static sp => sp.GetRequiredService<ShellService>());
            services.TryAddSingleton<IFileMenuViewModelFactory, FileMenuViewModelFactory>();

            services.TryAddTransient<ShellNotificationsViewModel>();
            services.TryAddTransient<ShellCultureViewModel>();
            services.TryAddTransient<ShellThemeViewModel>();
            services.TryAddTransient<SplashScreenViewModel>();
            services.TryAddTransient<AboutViewModel>();

            services.TryAddSingleton<ITaskbarProgressSource, TaskbarProgressSource>();
            services.TryAddSingleton<BusyTaskbarCoordinator>(static sp =>
                new(
                    sp.GetRequiredService<IBusyService>(),
                    sp.GetRequiredService<ITaskbarProgressSource>()));

            services.TryAddSingleton<ShellNotificationsDrawerCoordinator>();
            services.TryAddSingleton<ShellDrawerCoordinator>(static sp =>
            {
                _ = sp.GetRequiredService<IShellHostProvider>();
                return new(
                    sp.GetRequiredService<IContentDialogService>(),
                    sp.GetRequiredService<IShellHostProvider>());
            });

            return services;
        }

        /// <summary>
        /// Registers preferences page view models used by the shell preferences workspace.
        /// Register <see cref="PreferencesViewModel"/> in the host with the desired page list.
        /// </summary>
        public IServiceCollection AddShellPreferences()
        {
            services.TryAddTransient<DisplayPreferencesViewModel>();
            services.TryAddTransient<TimeAndLanguagePreferencesViewModel>();
            return services;
        }
    }
}
