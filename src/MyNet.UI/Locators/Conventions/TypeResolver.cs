// -----------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Implements a type resolver that uses a collection of naming conventions to resolve types, with support for manual registrations and caching for performance.
/// </summary>
/// <param name="conventions">The collection of naming conventions to use for type resolution.</param>
public sealed class TypeResolver(IEnumerable<ITypeNamingConvention> conventions) : ITypeResolver
{
    private readonly IReadOnlyList<ITypeNamingConvention> _conventions = [.. conventions];
    private readonly ConcurrentDictionary<Type, Type?> _cache = new();
    private readonly ConcurrentDictionary<Type, Type> _manual = new();

    /// <summary>
    /// Registers a mapping between a source type and a target type. This allows you to override the default resolution logic provided by the naming conventions. When resolving, the resolver will first check for any manually registered mappings before falling back to the conventions.
    /// </summary>
    /// <param name="source">The source type to map from.</param>
    /// <param name="target">The target type to map to.</param>
    public void Register(Type source, Type target)
    {
        _manual[source] = target;
        _cache.TryRemove(source, out _);
    }

    /// <summary>
    /// Resolves the target type for the given source type. The resolver first checks for any manually registered mappings, and if none are found, it iterates through the naming conventions to find a match. The result is cached for future lookups to improve performance. If no mapping is found, it returns null.
    /// </summary>
    /// <param name="source">The source type to resolve.</param>
    /// <returns>The resolved target type, or null if no mapping is found.</returns>
    public Type? Resolve(Type source) => _cache.GetOrAdd(source, ResolveInternal);

    /// <summary>
    /// Clears the convention-based resolution cache. Manually registered mappings are preserved and will still override conventions on the next resolution. Use this to force re-evaluation after dynamically loading assemblies or adding new conventions at runtime.
    /// </summary>
    public void ClearCache() => _cache.Clear();

    /// <summary>
    /// Performs the actual resolution logic for a given source type. This method first checks if there is a manually registered mapping for the source type. If a manual mapping exists, it returns that. If not, it iterates through the registered naming conventions and attempts to resolve the type using each convention until it finds a match. If no conventions can resolve the type, it returns null.
    /// </summary>
    /// <param name="source">The source type to resolve.</param>
    /// <returns>The resolved target type, or null if no mapping is found.</returns>
    private Type? ResolveInternal(Type source) => _manual.TryGetValue(source, out var manual) ? manual : _conventions.Select(convention => convention.Resolve(source)).OfType<Type>().FirstOrDefault();
}
