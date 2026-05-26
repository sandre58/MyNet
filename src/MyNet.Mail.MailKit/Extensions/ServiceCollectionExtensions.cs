// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Mail.MailKit;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Dependency injection extensions for MailKit mail services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers <see cref="MailKitServiceFactory"/> as <see cref="IMailServiceFactory"/>.
        /// </summary>
        public IServiceCollection AddMailKitMailService()
        {
            services.TryAddSingleton<IMailServiceFactory, MailKitServiceFactory>();
            return services;
        }
    }
}
