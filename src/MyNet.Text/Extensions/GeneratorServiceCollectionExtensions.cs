// -----------------------------------------------------------------------
// <copyright file="GeneratorServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyNet.Generator;
using MyNet.Generator.Facade;
using MyNet.Text.Randomize;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class GeneratorServiceCollectionExtensions
{
    /// <summary>
    /// Registers random generator services in DI and aligns static facades with the same singleton instance.
    /// </summary>
    public static IServiceCollection AddRandomGenerator(this IServiceCollection services)
    {
        services.TryAddSingleton<IRandomSource, SystemRandomSource>();
        services.TryAddSingleton<IRandomGenerator, DefaultRandomGenerator>();
        services.TryAddSingleton<ITextRandomGenerator>(TextRandomGenerator.Current);

        services.TryAddSingleton(provider =>
        {
            RandomGenerator.Current = provider.GetRequiredService<IRandomGenerator>();
            return RandomGenerator.Current;
        });

        return services;
    }
}
