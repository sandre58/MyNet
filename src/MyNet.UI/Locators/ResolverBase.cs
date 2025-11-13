// -----------------------------------------------------------------------
// <copyright file="ResolverBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities.Helpers;

namespace MyNet.UI.Locators;

/// <summary>
/// Base class for all resolvers. Implements shared logic for type resolution using naming conventions and cache.
/// Optimized with concurrent dictionary and reduced reflection calls.
/// </summary>
public abstract class ResolverBase : IResolver
{
    // Thread-safe cache without locks
    private readonly ConcurrentDictionary<string, string> _resolvedCache = new();

    // Cache for failed resolutions to avoid repeated expensive lookups
    private readonly ConcurrentDictionary<string, bool> _failedResolutionCache = new();

    // Cache for Type objects to avoid repeated Type.GetType() calls
    private readonly ConcurrentDictionary<string, Type?> _typeCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ResolverBase"/> class.
    /// </summary>
    protected ResolverBase() => NamingConventions = [.. GetDefaultNamingConventionsInternal()];

    /// <summary>
    /// Gets the naming conventions to use to locate types.
    /// </summary>
    public IList<string> NamingConventions { get; }

    /// <summary>
    /// Gets cache statistics for monitoring.
    /// </summary>
    public (int Resolved, int Failed, int Types) CacheStats => (_resolvedCache.Count, _failedResolutionCache.Count, _typeCache.Count);

    private IEnumerable<string> GetDefaultNamingConventionsInternal() => GetDefaultNamingConventions();

    /// <summary>
    /// Registers the specified type in the local cache.
    /// </summary>
    protected void Register(string valueToResolve, string resolvedValue)
    {
        if (string.IsNullOrWhiteSpace(valueToResolve))
            throw new ArgumentException("Value to resolve cannot be null or whitespace.", nameof(valueToResolve));
        if (string.IsNullOrWhiteSpace(resolvedValue))
            throw new ArgumentException("Resolved value cannot be null or whitespace.", nameof(resolvedValue));

        _resolvedCache[valueToResolve] = resolvedValue;
        _failedResolutionCache.TryRemove(valueToResolve, out _);
    }

    /// <summary>
    /// Resolves the specified values with caching and optimization.
    /// </summary>
    protected virtual IEnumerable<string> ResolveValues(string valueToResolve)
    {
        if (string.IsNullOrWhiteSpace(valueToResolve))
            throw new ArgumentException("Value to resolve cannot be null or whitespace.", nameof(valueToResolve));

        // Fast path: Check cache first
        if (_resolvedCache.TryGetValue(valueToResolve, out var cachedValue) && !string.IsNullOrEmpty(cachedValue))
        {
            return [cachedValue];
        }

        // Check if we've already failed to resolve this
        if (_failedResolutionCache.TryGetValue(valueToResolve, out _))
        {
            return [];
        }

        var assembly = TypeHelper.GetAssemblyName(valueToResolve);
        var typeToResolveName = TypeHelper.GetTypeName(valueToResolve);

        Type? resolvedType = null;

        // Try conventions until one succeeds
        foreach (var namingConvention in NamingConventions)
        {
            var resolvedTypeName = ResolveNamingConvention(assembly, typeToResolveName, namingConvention);
            resolvedType = GetTypeFromCache(resolvedTypeName);

            if (resolvedType is not null)
                break;
        }

        if (resolvedType?.AssemblyQualifiedName is null)
        {
            _failedResolutionCache[valueToResolve] = true;
            return [];
        }

        var fullResolvedTypeName = TypeHelper.GetTypeNameWithAssembly(resolvedType.AssemblyQualifiedName);

        if (string.IsNullOrEmpty(fullResolvedTypeName))
        {
            _failedResolutionCache[valueToResolve] = true;
            return [];
        }

        _resolvedCache[valueToResolve] = fullResolvedTypeName;
        return [fullResolvedTypeName];
    }

    /// <summary>
    /// Gets a Type from cache or resolves it.
    /// </summary>
    private Type? GetTypeFromCache(string typeName) => string.IsNullOrEmpty(typeName) ? null : _typeCache.GetOrAdd(typeName, TypeHelper.GetTypeFrom);

    protected virtual string? Resolve(string valueToResolve)
    {
        var values = ResolveValues(valueToResolve);
        return values.LastOrDefault();
    }

    protected string? GetItemFromCache(string valueToResolve) => string.IsNullOrWhiteSpace(valueToResolve)
            ? throw new ArgumentException("Value to resolve cannot be null or whitespace.", nameof(valueToResolve))
            : _resolvedCache.TryGetValue(valueToResolve, out var value) ? value : null;

    protected void AddItemToCache(string valueToResolve, string resolvedValue) => _resolvedCache[valueToResolve] = resolvedValue;

    public void ClearCache()
    {
        _resolvedCache.Clear();
        _failedResolutionCache.Clear();
        _typeCache.Clear();
    }

    protected abstract string ResolveNamingConvention(string assembly, string typeToResolveName, string namingConvention);

    protected abstract IEnumerable<string> GetDefaultNamingConventions();
}
