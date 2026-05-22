// -----------------------------------------------------------------------
// <copyright file="ObservableObjectPropertyAccess.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace MyNet.Observable;

/// <summary>
/// Cached compiled property getters for <see cref="ObservableObject"/> instances (startup / attach paths only).
/// </summary>
internal static class ObservableObjectPropertyAccess
{
    private static readonly ConcurrentDictionary<(Type Type, string PropertyName), Func<ObservableObject, object?>> GetterCache = new();

    /// <summary>
    /// Gets the current value of a public instance property by name.
    /// </summary>
    public static object? GetPropertyValue(ObservableObject instance, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var key = (instance.GetType(), propertyName);

        var getter = GetterCache.GetOrAdd(key, static x => CreateGetter(x.Type, x.PropertyName));

        return getter(instance);
    }

    private static Func<ObservableObject, object?> CreateGetter(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        if (property?.CanRead != true || property.GetIndexParameters().Length != 0)
            return static _ => UnknownValue.Instance;

        var parameter = Expression.Parameter(typeof(ObservableObject));

        var cast = Expression.Convert(parameter, type);

        var propertyAccess = Expression.Property(cast, property);

        var convert = Expression.Convert(propertyAccess, typeof(object));

        return Expression.Lambda<Func<ObservableObject, object?>>(convert, parameter).Compile();
    }
}
