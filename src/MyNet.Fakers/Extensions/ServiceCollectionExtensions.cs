// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.Fakers.Contacts;
using MyNet.Fakers.Geography;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using MyNet.Fakers.Media;
using MyNet.Fakers.Text;
using MyNet.Globalization;
using MyNet.Text;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Fakers;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// <para>
        /// Adds the MyNet.Fakers framework with all dependencies configured.
        /// This method handles:
        /// - Random number generation
        /// - Localization infrastructure
        /// - Provider registration for all cultures
        /// - All faker implementations
        /// - Configuration options.
        /// </para>
        /// <para>Call this once during application startup. It integrates with <see cref="Globalization.ServiceCollectionExtensions.AddLocalization"/> if called.</para>
        /// </summary>
        /// <returns>The updated IServiceCollection instance.</returns>
        public IServiceCollection AddFakers()
        {
            // Ensure localization infrastructure is registered (with fallback to defaults if not already present)
            services.AddLocalization();

            // Random number generation
            services.AddRandomGenerator();

            // Contacts
            services.TryAddSingleton<IMailFaker, MailFaker>();
            services.TryAddSingleton<IPhoneFaker, PhoneFaker>();
            services.AddLocalizationService<IPhoneFakerProvider>((_, x) => ResxPhoneFakerProvider.Create(x));

            // Geography
            services.TryAddSingleton<ICountryFaker, CountryFaker>();
            services.TryAddSingleton<IStreetFaker, StreetFaker>();
            services.TryAddSingleton<IAddressFaker, AddressFaker>();
            services.AddLocalizationService<IAddressFakerProvider>((_, x) => ResxAddressFakerProvider.Create(x));

            // Identity
            services.TryAddSingleton<IIdentityFaker, IdentityFaker>();
            services.TryAddSingleton<INameFaker, NameFaker>();
            services.AddLocalizationService<INameFakerProvider>((_, x) => ResxNameFakerProvider.Create(x));

            // Internet
            services.TryAddSingleton<IDomainFaker, DomainFaker>();
            services.AddLocalizationService<IDomainFakerProvider>((_, x) => ResxDomainFakerProvider.Create(x));

            // Media
            services.TryAddSingleton<IColorFaker, ColorFaker>();

            // Text
            services.TryAddSingleton<ITextFaker, TextFaker>();

            // Main IFakeDataGenerator facade
            services.TryAddSingleton<IFakeDataGenerator, FakeDataGenerator>();

            return services;
        }
    }

    extension(IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Configures the static <see cref="Static.Faker"/> class to use the <see cref="IFakeDataGenerator"/> instance from the service provider. This allows for convenient static access to faker methods without needing to inject dependencies in every class. Call this once after building the service provider, typically during application startup.
        /// </summary>
        /// <returns>The updated service provider.</returns>
        public IServiceProvider UseFakers()
        {
            Static.Faker.Configure(serviceProvider.GetRequiredService<IFakeDataGenerator>());

            return serviceProvider;
        }
    }
}
