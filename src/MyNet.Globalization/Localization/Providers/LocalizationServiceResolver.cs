// -----------------------------------------------------------------------
// <copyright file="LocalizationServiceResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using MyNet.Globalization.Localization.Providers.Registration;

namespace MyNet.Globalization.Localization.Providers;

/// <summary>
/// Resolves localization providers based on their type and culture, with caching and cycle detection to prevent circular dependencies during resolution.
/// </summary>
/// <param name="registry">The localization provider registry.</param>
public sealed class LocalizationServiceResolver(ILocalizationFactoryRegistry registry) : ILocalizationServiceResolver
{
    private static readonly AsyncLocal<(Type Type, string CultureName)[]?> ResolutionPath = new();
    private readonly ConcurrentDictionary<(Type, string), Lazy<object?>> _cache = new();

    /// <inheritdoc />
    public bool TryGet<TService>(CultureInfo culture, [NotNullWhen(true)] out TService? provider)
        where TService : class, ICultureScoped
    {
        ArgumentNullException.ThrowIfNull(culture);

        var cacheKey = (typeof(TService), culture.Name);

        if (IsCycle(cacheKey))
            throw CreateCycleException(cacheKey);

        var lazyProvider = _cache.GetOrAdd(cacheKey,
            _ => new(() => ResolveWithCycleCheck<TService>(culture), LazyThreadSafetyMode.ExecutionAndPublication));

        provider = lazyProvider.Value as TService;
        return lazyProvider.Value is not null;
    }

    /// <inheritdoc />
    public TService GetRequired<TService>(CultureInfo culture)
        where TService : class, ICultureScoped
        => !TryGet<TService>(culture, out var provider)
            ? throw new InvalidOperationException($"Required localization provider of type {typeof(TService).Name} for culture {culture.Name} not found.")
            : provider;

    /// <summary>
    /// Creates an exception indicating a circular dependency in localization provider resolution, including the resolution path for easier debugging.
    /// </summary>
    /// <param name="requested">The requested provider and culture that caused the cycle.</param>
    /// <returns>An <see cref="InvalidOperationException"/> describing the circular dependency.</returns>
    private static InvalidOperationException CreateCycleException((Type Type, string CultureName) requested)
    {
        var path = ResolutionPath.Value ?? [];
        var chain = string.Join(" -> ", path.Select(x => $"{x.Type.Name}({x.CultureName})"));
        var requestedNode = $"{requested.Type.Name}({requested.CultureName})";
        var message = string.IsNullOrWhiteSpace(chain)
            ? $"Circular localization provider dependency detected for {requestedNode}."
            : $"Circular localization provider dependency detected: {chain} -> {requestedNode}.";

        return new(message);
    }

    /// <summary>
    /// Checks if the given provider and culture combination is already in the current resolution path, indicating a circular dependency.
    /// </summary>
    /// <param name="key">The provider and culture combination to check.</param>
    /// <returns>True if a circular dependency is detected; otherwise, false.</returns>
    private static bool IsCycle((Type Type, string CultureName) key)
    {
        var path = ResolutionPath.Value;
        return path?.Contains(key) == true;
    }

    private TService? ResolveWithCycleCheck<TService>(CultureInfo culture)
        where TService : class, ICultureScoped
    {
        var key = (typeof(TService), culture.Name);
        var path = ResolutionPath.Value ?? [];

        if (path.Contains(key))
            throw CreateCycleException(key);

        ResolutionPath.Value = [.. path, key];

        try
        {
            return Resolve<TService>(culture);
        }
        finally
        {
            ResolutionPath.Value = path.Length == 0 ? null : path;
        }
    }

    /// <summary>
    /// Resolves the provider of type <typeparamref name="TService"/> for the given culture by querying the registry.
    /// </summary>
    /// <typeparam name="TService">The type of the provider to resolve.</typeparam>
    /// <param name="culture">The culture for which to resolve the provider.</param>
    /// <returns>The resolved provider instance, or null if not found.</returns>
    private TService? Resolve<TService>(CultureInfo culture)
        where TService : class, ICultureScoped
        => !registry.TryGetFactory<TService>(out var factory) ? null : factory.Create(culture);

    /// <inheritdoc />
    public ILocalizationServiceContext ForCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return new LocalizationServiceContext(this, culture);
    }
}
