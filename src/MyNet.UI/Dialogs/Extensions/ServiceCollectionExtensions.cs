// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.FileDialogs;
using MyNet.UI.Dialogs.MessageBox;

#pragma warning disable IDE0130
namespace MyNet.UI.Dialogs;
#pragma warning restore IDE0130

/// <summary>
/// Extension methods for registering dialog services into a <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the dialog stack with default headless implementations suitable for tests and non-UI hosts.
        /// </summary>
        /// <param name="configure">Optional callback to register platform strategies or presenters.</param>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddDialogs(Action<DialogServiceBuilder>? configure = null)
        {
            var builder = new DialogServiceBuilder(services);
            configure?.Invoke(builder);

            services.TryAddSingleton<IContentDialogService, ContentDialogService>();
            services.TryAddSingleton<IMessageBoxFactory, DefaultMessageBoxFactory>();
            services.TryAddSingleton<IMessageBoxService, MessageBoxService>();
            services.TryAddSingleton<IFileDialogService, CancelledFileDialogService>();
            services.TryAddSingleton<IDialogService, DialogService>();

            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IDialogStrategy, HeadlessDialogStrategy>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IDialogStrategy, PresenterDialogStrategy>());

            return services;
        }

        /// <summary>
        /// Registers a custom <see cref="IDialogStrategy"/> implementation (e.g. WPF window host).
        /// </summary>
        /// <typeparam name="TStrategy">Strategy implementation type.</typeparam>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddDialogStrategy<TStrategy>()
            where TStrategy : class, IDialogStrategy
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDialogStrategy, TStrategy>());
            return services;
        }

        /// <summary>
        /// Registers a custom <see cref="IDialogStrategy"/> instance.
        /// </summary>
        /// <typeparam name="TStrategy">Strategy contract type.</typeparam>
        /// <param name="strategy">Strategy instance.</param>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddDialogStrategy<TStrategy>(TStrategy strategy)
            where TStrategy : class, IDialogStrategy
        {
            ArgumentNullException.ThrowIfNull(strategy);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDialogStrategy>(strategy));
            return services;
        }

        /// <summary>
        /// Registers a platform <see cref="IDialogPresenter"/> and enables <see cref="PresenterDialogStrategy"/>.
        /// </summary>
        /// <typeparam name="TPresenter">Presenter implementation type.</typeparam>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddDialogPresenter<TPresenter>()
            where TPresenter : class, IDialogPresenter
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDialogPresenter, TPresenter>());
            return services;
        }

        /// <summary>
        /// Registers a platform <see cref="IDialogPresenter"/> instance.
        /// </summary>
        /// <typeparam name="TPresenter">Presenter contract type.</typeparam>
        /// <param name="presenter">Presenter instance.</param>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddDialogPresenter<TPresenter>(TPresenter presenter)
            where TPresenter : class, IDialogPresenter
        {
            ArgumentNullException.ThrowIfNull(presenter);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDialogPresenter>(presenter));
            return services;
        }

        /// <summary>
        /// Replaces the default headless <see cref="IFileDialogService"/> with a platform implementation.
        /// </summary>
        /// <typeparam name="TFileDialogService">File dialog service type.</typeparam>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddFileDialogService<TFileDialogService>()
            where TFileDialogService : class, IFileDialogService
        {
            services.Replace(ServiceDescriptor.Singleton<IFileDialogService, TFileDialogService>());
            return services;
        }

        /// <summary>
        /// Replaces the default <see cref="IMessageBoxFactory"/> (e.g. for custom styling).
        /// </summary>
        /// <typeparam name="TFactory">Factory implementation type.</typeparam>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddMessageBoxFactory<TFactory>()
            where TFactory : class, IMessageBoxFactory
        {
            services.Replace(ServiceDescriptor.Singleton<IMessageBoxFactory, TFactory>());
            return services;
        }

        /// <summary>
        /// Supplies a shared <see cref="ICommandFactory"/> for message box view models.
        /// </summary>
        /// <param name="commandFactory">Command factory instance.</param>
        /// <returns>The same service collection for chaining.</returns>
        public IServiceCollection AddMessageBoxCommandFactory(ICommandFactory commandFactory)
        {
            ArgumentNullException.ThrowIfNull(commandFactory);
            services.Replace(ServiceDescriptor.Singleton<IMessageBoxFactory>(
                new DefaultMessageBoxFactory(commandFactory)));
            return services;
        }
    }
}

/// <summary>
/// Configures optional dialog registrations inside <see cref="ServiceCollectionExtensions.AddDialogs"/>.
/// </summary>
public sealed class DialogServiceBuilder(IServiceCollection services)
{
    /// <summary>
    /// Gets the underlying service collection.
    /// </summary>
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Registers a platform presenter on the underlying service collection.
    /// </summary>
    public DialogServiceBuilder AddPresenter<TPresenter>()
        where TPresenter : class, IDialogPresenter
    {
        Services.AddDialogPresenter<TPresenter>();
        return this;
    }
}
