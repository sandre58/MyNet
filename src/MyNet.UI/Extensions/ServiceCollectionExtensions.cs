// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization;
using MyNet.Humanizer;
using MyNet.Observable.Validation;
using MyNet.UI.Extensions;
using MyNet.UI.Resources;
using MyNet.UI.Theming;

#pragma warning disable IDE0130
namespace MyNet.UI;
#pragma warning restore IDE0130

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers core UI services shared across host applications, including globalization, localization, inflection, humanization, busy indicators, navigation, view locators, dialogs, notifications, toasting, and shell support.
        /// </summary>
        /// <param name="supportedCultures">The cultures supported by the UI.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddUi(IEnumerable<CultureInfo>? supportedCultures = null) => services.AddUi(b => b.WithSupportedCultures(supportedCultures));

        /// <summary>
        /// Registers core UI services and applies optional configuration (dialogs, locators, navigation, preferences).
        /// </summary>
        /// <param name="configure">Callback to customize registrations on a <see cref="UiBuilder"/>.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddUi(Action<UiBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new UiBuilder(services);
            configure(builder);
            builder.Apply();

            return services;
        }

        /// <summary>
        /// Contributes UI translation resources to the catalog.
        /// </summary>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddUiTranslations()
        {
            services.AddTranslationResource(nameof(UiResources), UiResources.ResourceManager);
            services.AddTranslationResource(nameof(MessageResources), MessageResources.ResourceManager);
            services.AddTranslationResource(nameof(FormatResources), FormatResources.ResourceManager);

            return services;
        }
    }

    extension(IServiceProvider services)
    {
        /// <summary>
        /// Configures shared UI static facades (globalization, localization, humanizer, validation).
        /// Configures <see cref="ThemeManager"/> only when <see cref="IThemeService"/> and <see cref="IThemeBaseRegistry"/> are registered in DI.
        /// </summary>
        /// <returns>The updated service provider.</returns>
        public IServiceProvider UseUi()
        {
            services.UseGlobalization();
            services.UseLocalization();
            services.UseDisplayText();
            services.UseThemeManagerIfAvailable();
            ValidationLocalization.Configure();

            return services;
        }
    }
}
