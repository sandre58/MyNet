// -----------------------------------------------------------------------
// <copyright file="PropertyCache.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyNet.Utilities.Reflection;

/// <summary>
/// Provides cached reflection queries for properties.
/// </summary>
public static class PropertyCache
{
    private static readonly ConcurrentDictionary<(Type Type, string Key), object> Cache = new();

    /// <summary>
    /// Creates a property query for the specified type.
    /// </summary>
    /// <param name="type">The target type.</param>
    /// <returns>A new <see cref="PropertyQuery"/> instance.</returns>
    public static PropertyQuery For(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return new(type);
    }

    /// <summary>
    /// Creates a property query for the specified generic type parameter. This method is a convenient overload of the For(Type) method, allowing you to specify the target type as a generic type parameter instead of passing a Type object. It simply calls the For(Type) method with the Type of the generic parameter, providing a more concise syntax for creating property queries for specific types.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <returns>A new <see cref="PropertyQuery"/> instance.</returns>
    public static PropertyQuery For<T>() => For(typeof(T));

    internal static TValue GetOrCreate<TValue>(
        Type type,
        string key,
        Func<TValue> factory)
        where TValue : class
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(factory);

        return (TValue)Cache.GetOrAdd(
            (type, key),
            static (_, state) => state(),
            factory);
    }
}

/// <summary>
/// Represents a cached property query.
/// </summary>
public sealed class PropertyQuery
{
    private readonly Type _type;
    private readonly List<Func<PropertyInfo, bool>> _predicates = [];

    internal PropertyQuery(Type type) => _type = type;

    /// <summary>
    /// Adds a custom filter predicate.
    /// </summary>
    /// <param name="predicate">The predicate to add.</param>
    /// <returns>The current query.</returns>
    public PropertyQuery Where(Func<PropertyInfo, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        _predicates.Add(predicate);

        return this;
    }

    /// <summary>
    /// Includes only properties marked with the specified attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="inherit">
    /// Indicates whether inherited attributes should be considered.
    /// </param>
    /// <returns>The current query.</returns>
    public PropertyQuery WhereAttribute<TAttribute>(bool inherit = true)
        where TAttribute : Attribute
        => Where(property => property.IsDefined(typeof(TAttribute), inherit));

    /// <summary>
    /// Excludes properties marked with the specified attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="inherit">
    /// Indicates whether inherited attributes should be considered.
    /// </param>
    /// <returns>The current query.</returns>
    public PropertyQuery ExcludeAttribute<TAttribute>(bool inherit = true)
        where TAttribute : Attribute
        => Where(property => !property.IsDefined(typeof(TAttribute), inherit));

    /// <summary>
    /// Excludes the specified property names.
    /// </summary>
    /// <param name="propertyNames">The property names to exclude.</param>
    /// <returns>The current query.</returns>
    public PropertyQuery ExcludeNames(params IEnumerable<string> propertyNames)
    {
        ArgumentNullException.ThrowIfNull(propertyNames);

        var names = new HashSet<string>(propertyNames, StringComparer.Ordinal);

        return Where(property => !names.Contains(property.Name));
    }

    /// <summary>
    /// Includes only readable properties.
    /// </summary>
    /// <returns>The current query.</returns>
    public PropertyQuery Readable() => Where(static property => property.CanRead);

    /// <summary>
    /// Includes only writable properties.
    /// </summary>
    /// <returns>The current query.</returns>
    public PropertyQuery Writable() => Where(static property => property.CanWrite);

    /// <summary>
    /// Excludes indexer properties.
    /// </summary>
    /// <returns>The current query.</returns>
    public PropertyQuery WithoutIndexers() => Where(static property => property.GetIndexParameters().Length == 0);

    /// <summary>
    /// Returns the matching properties.
    /// </summary>
    /// <returns>The matching properties.</returns>
    public PropertyInfo[] ToArray()
    {
        var cacheKey = BuildCacheKey("properties");

        return PropertyCache.GetOrCreate(_type, cacheKey, Execute);
    }

    /// <summary>
    /// Returns the matching property names.
    /// </summary>
    /// <returns>The matching property names.</returns>
    public string[] ToNames()
    {
        var cacheKey = BuildCacheKey("names");

        return PropertyCache.GetOrCreate(
            _type,
            cacheKey,
            () =>
            {
                var properties = Execute();

                var result = new string[properties.Length];

                for (var i = 0; i < properties.Length; i++)
                {
                    result[i] = properties[i].Name;
                }

                return result;
            });
    }

    /// <summary>
    /// Executes the property query by retrieving the public properties of the target type and applying all the specified predicates to filter the properties. If no predicates are defined, it returns all public properties. Otherwise, it iterates through the properties and includes only those that satisfy all the predicates, returning the filtered list of properties as an array. This method is used internally to perform the actual reflection query and is cached for performance optimization.
    /// </summary>
    /// <returns>The filtered array of <see cref="PropertyInfo"/> objects.</returns>
    private PropertyInfo[] Execute()
    {
        var properties = _type.GetPublicProperties();

        if (_predicates.Count == 0)
        {
            return properties;
        }

        var result = new List<PropertyInfo>(properties.Length);
        result.AddRange(from property in properties let include = _predicates.All(predicate => predicate(property)) where include select property);

        return [.. result];
    }

    /// <summary>
    /// Builds a cache key based on the specified suffix and the method names of the predicates. The cache key is constructed by concatenating the suffix with the method names of the predicates, separated by a colon and a pipe character. This allows for unique cache keys based on the combination of filters applied to the property query, ensuring that different queries with different filters will have distinct cache entries.
    /// </summary>
    /// <param name="suffix">The suffix to use for the cache key.</param>
    /// <returns>The constructed cache key.</returns>
    private string BuildCacheKey(string suffix) => $"{suffix}:{string.Join("|", _predicates.Select(static x => x.Method.Name))}";
}
