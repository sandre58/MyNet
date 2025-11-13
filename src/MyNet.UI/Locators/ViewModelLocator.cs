// -----------------------------------------------------------------------
// <copyright file="ViewModelLocator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;

namespace MyNet.UI.Locators;

/// <summary>
/// Default implementation of <see cref="IViewModelLocator"/> that retrieves view model instances using a service provider.
/// Includes caching for singleton instances to improve performance.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
/// </remarks>
/// <param name="serviceProvider">The service provider used to resolve view model instances.</param>
public class ViewModelLocator(IServiceProvider serviceProvider) : IViewModelLocator
{
    // Cache for singleton ViewModels (marked with [IsSingleton])
    private readonly ConcurrentDictionary<Type, object> _singletonCache = new();

    // Optional: Cache for transient ViewModels that should be reused during a navigation session
    // This is useful for pages that are expensive to create but should be fresh on each app restart
    private readonly ConcurrentDictionary<Type, WeakReference<object>> _sessionCache = new();

    /// <inheritdoc/>
    public object Get(Type viewModelType)
    {
        // 1. Try to get from service provider first (respects DI configuration)
        var instance = serviceProvider.GetService(viewModelType);

        if (instance is not null)
            return instance;

        // 2. Check if we have a cached singleton instance
        if (_singletonCache.TryGetValue(viewModelType, out var cachedInstance))
            return cachedInstance;

        // 3. Check session cache (weak references to avoid memory leaks)
        if (_sessionCache.TryGetValue(viewModelType, out var weakRef) && weakRef.TryGetTarget(out var sessionInstance))
        {
            // Check if it's marked as transient - if so, session cache shouldn't be used
            if (!Attribute.IsDefined(viewModelType, typeof(Attributes.IsTransientAttribute)))
                return sessionInstance;
        }

        // 4. Create new instance
        instance = Activator.CreateInstance(viewModelType)!;

        // 5. Determine caching strategy
        var isSingleton = Attribute.IsDefined(viewModelType, typeof(Attributes.IsSingletonAttribute));
        var isTransient = Attribute.IsDefined(viewModelType, typeof(Attributes.IsTransientAttribute));

        if (isSingleton)
        {
            // Strong reference - kept until app closes
            _singletonCache.TryAdd(viewModelType, instance);
        }
        else if (!isTransient)
        {
            // Weak reference - can be garbage collected if memory is needed
            // Good for pages that are expensive to create but not critical to keep
            _sessionCache.AddOrUpdate(
                viewModelType,
                new WeakReference<object>(instance),
                (_, _) => new WeakReference<object>(instance));
        }

        // If IsTransient, don't cache at all
        return instance;
    }

    /// <summary>
    /// Clears the session cache. Useful after major application state changes.
    /// Singleton cache is preserved.
    /// </summary>
    public void ClearSessionCache() => _sessionCache.Clear();

    /// <summary>
    /// Gets the number of cached singleton instances.
    /// </summary>
    public int SingletonCacheSize => _singletonCache.Count;

    /// <summary>
    /// Gets the number of active session cache entries.
    /// </summary>
    public int SessionCacheSize => _sessionCache.Count;
}
