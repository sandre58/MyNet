// -----------------------------------------------------------------------
// <copyright file="PropertyAccessorCache.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace MyNet.Utilities.Caching;

/// <summary>
/// Caches compiled property accessors for sorting properties to improve performance when sorting by property names.
/// </summary>
/// <typeparam name="T">Type of object.</typeparam>
public static class PropertyAccessorCache<T>
{
    private static readonly ConcurrentDictionary<string, Func<T, object?>> Cache = new();

    /// <summary>
    /// Gets a compiled property accessor for the specified property name, using a cache to improve performance. If the accessor does not exist in the cache, it is created and added to the cache before being returned.
    /// </summary>
    /// <param name="propertyName">The name of the property for which to get the accessor.</param>
    /// <returns>A function that accesses the specified property.</returns>
    public static Func<T, object?> Get(string propertyName) => Cache.GetOrAdd(propertyName, Create);

    /// <summary>
    /// Creates a compiled property accessor for the specified property name, using expression trees to generate a function that accesses the property. The property name can include nested properties separated by dots (e.g., "Address.City"). The generated function is compiled and returned as a delegate.
    /// </summary>
    /// <param name="propertyName">The name of the property for which to create the accessor.</param>
    /// <returns>A function that accesses the specified property.</returns>
    private static Func<T, object?> Create(string propertyName)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var body = propertyName.Split('.').Aggregate<string?, Expression>(param, (current, member) => Expression.PropertyOrField(current, member.OrEmpty()));

        var convert = Expression.Convert(body, typeof(object));

        return Expression.Lambda<Func<T, object?>>(convert, param).Compile();
    }
}
