// -----------------------------------------------------------------------
// <copyright file="ViewLocator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using MyNet.UI.Extensions;

namespace MyNet.UI.Locators;

/// <summary>
/// Default implementation of <see cref="IViewLocator"/> that manages view instance registration and retrieval.
/// Provides caching for singleton views to improve performance.
/// </summary>
public class ViewLocator : IViewLocator
{
    // Thread-safe dictionary for singleton views (marked with [IsSingleton])
    private readonly ConcurrentDictionary<Type, Lazy<object>> _singletonInstances = new();

    // Optional: Cache for views without attributes (weak references to allow GC)
    private readonly ConcurrentDictionary<Type, WeakReference<object>> _sessionCache = new();

    /// <inheritdoc/>
    public void Register(Type type, Func<object> createInstance)
    {
        // Only register if marked as singleton
        if (type.IsRegisteredAsSingleton())
        {
            // Use GetOrAdd to handle race conditions
            _singletonInstances.GetOrAdd(type, _ => new Lazy<object>(createInstance));
        }
    }

    /// <inheritdoc/>
    public object Get(Type viewType)
    {
        // 1. Check if it's a singleton view
        if (viewType.IsRegisteredAsSingleton())
        {
            return _singletonInstances.GetOrAdd(
            viewType,
            _ => new Lazy<object>(() => CreateViewInstance(viewType))).Value;
        }

        // 2. Check if it's explicitly transient
        if (viewType.IsRegisteredAsTransient())
        {
            // Always create a new instance
            return CreateViewInstance(viewType);
        }

        // 3. Default behavior: Use session cache with weak references
        // This allows reuse while not preventing garbage collection
        if (_sessionCache.TryGetValue(viewType, out var weakRef) && weakRef.TryGetTarget(out var cachedView))
        {
            return cachedView;
        }

        // Create new instance
        var newInstance = CreateViewInstance(viewType);

        // Cache with weak reference (can be GC'd if memory is needed)
        _sessionCache.AddOrUpdate(
            viewType,
            new WeakReference<object>(newInstance),
            (_, _) => new WeakReference<object>(newInstance));

        return newInstance;
    }

    /// <summary>
    /// Creates a view instance using Activator.
    /// </summary>
    private static object CreateViewInstance(Type viewType) => Activator.CreateInstance(viewType)!;

    /// <summary>
    /// Clears the session cache. Singleton cache is preserved.
    /// Useful after major UI state changes or memory pressure.
    /// </summary>
    public void ClearSessionCache() => _sessionCache.Clear();

    /// <summary>
    /// Gets the number of cached singleton view instances.
    /// </summary>
    public int SingletonCacheSize => _singletonInstances.Count;

    /// <summary>
    /// Gets the number of active session cache entries.
    /// </summary>
    public int SessionCacheSize => _sessionCache.Count;
}
