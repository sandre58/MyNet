// -----------------------------------------------------------------------
// <copyright file="NotifyPropertyChangedSourceSnapshot.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Cached compiled getters used when attaching to a wrapper source (attach path only).
/// </summary>
internal static class NotifyPropertyChangedSourceSnapshot
{
    private static readonly ConcurrentDictionary<Type, PropertyGetter[]> GetterCache = new();

    /// <summary>
    /// Enumerates public readable instance property values on the source object.
    /// </summary>
    public static IEnumerable<(string PropertyName, object? Value)> EnumerateValues(object source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return enumerateValues();

        IEnumerable<(string PropertyName, object? Value)> enumerateValues()
        {
            var getters = GetterCache.GetOrAdd(source.GetType(), CreateGetters);

            foreach (var entry in getters)
                yield return (entry.Name, entry.Getter(source));
        }
    }

    private static PropertyGetter[] CreateGetters(Type type)
    {
        var list = (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public) where property.CanRead && property.GetIndexParameters().Length == 0 let getter = CompileGetter(type, property) select new PropertyGetter(property.Name, getter)).ToList();

        return [.. list];
    }

    private static Func<object, object?> CompileGetter(Type type, PropertyInfo property)
    {
        var parameter = Expression.Parameter(typeof(object));

        var cast = Expression.Convert(parameter, type);

        var propertyAccess = Expression.Property(cast, property);

        var convert = Expression.Convert(propertyAccess, typeof(object));

        return Expression.Lambda<Func<object, object?>>(convert, parameter).Compile();
    }

    private sealed class PropertyGetter(string name, Func<object, object?> getter)
    {
        public string Name { get; } = name;

        public Func<object, object?> Getter { get; } = getter;
    }
}
