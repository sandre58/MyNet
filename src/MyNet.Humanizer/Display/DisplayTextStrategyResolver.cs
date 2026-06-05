// -----------------------------------------------------------------------
// <copyright file="DisplayTextStrategyResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MyNet.Humanizer.Display.Registration;

namespace MyNet.Humanizer.Display;

/// <summary>
/// Resolves display name providers using deterministic hierarchical fallback.
/// Resolution order is: exact type, interfaces, base types, then object.
/// </summary>
public sealed class DisplayTextStrategyResolver(IDisplayTextStrategyRegistry registry) : IDisplayTextStrategyResolver
{
    private readonly ConcurrentDictionary<Type, IDisplayTextStrategy?> _cache = new();

    /// <inheritdoc/>
    public bool TryGet<T>([NotNullWhen(true)] out IDisplayTextStrategy<T>? strategy)
        where T : notnull
    {
        if (_cache.GetOrAdd(typeof(T), Resolve) is { } result)
        {
            strategy = new TypedDisplayTextStrategyAdapter<T>(result);
            return true;
        }

        strategy = null;
        return false;
    }

    /// <inheritdoc/>
    public IDisplayTextStrategy<T> GetRequired<T>()
        where T : notnull
        => TryGet<T>(out var s)
            ? s
            : throw new InvalidOperationException($"No display text strategy registered for '{typeof(T).FullName}'.");

    /// <inheritdoc/>
    public IDisplayTextStrategy GetRequiredForType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return _cache.GetOrAdd(type, Resolve)
            ?? throw new InvalidOperationException($"No display text strategy registered for '{type.FullName}'.");
    }

    /// <summary>
    /// Resolves a display name provider for the requested type using hierarchical fallback:
    /// 1. Exact match: provider registered for the requested type.
    /// 2. Assignable fallback: provider registered for an interface or base type assignable from the requested type.
    /// 3. Enum fallback: provider registered for Enum if the requested type is an enum (important case).
    /// 4. Object fallback: provider registered for object.
    /// </summary>
    /// <param name="requestedType">The type for which to resolve a display name provider.</param>
    /// <returns>The resolved display name provider, or null if none is found.</returns>
    private IDisplayTextStrategy? Resolve(Type requestedType)
    {
        // 1. exact match
        if (registry.TryGetStrategy(requestedType, out var exact))
            return exact;

        // 2. assignable fallback (interfaces / base types)
        var fallback = registry
            .Strategies
            .FirstOrDefault(x => x.Key.IsAssignableFrom(requestedType));

        if (fallback.Value is not null)
            return fallback.Value;

        // 3. Enum fallback (important case)
        if (requestedType.IsEnum && registry.TryGetStrategy(typeof(Enum), out var enumStrategy))
        {
            return enumStrategy;
        }

        // 4. object fallback (optional global default)
        return registry.TryGetStrategy(typeof(object), out var objectStrategy) ? objectStrategy : null;
    }
}
