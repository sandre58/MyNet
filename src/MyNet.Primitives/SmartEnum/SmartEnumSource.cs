// -----------------------------------------------------------------------
// <copyright file="SmartEnumSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Discovers all instances declared on a smart enum type.
/// </summary>
public static class SmartEnumSource
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<ISmartEnum>> Cache = new();

    /// <summary>
    /// Returns all smart enum instances for the specified type.
    /// </summary>
    /// <param name="smartEnumType">The smart enum type.</param>
    /// <returns>All discovered smart enum instances.</returns>
    public static IReadOnlyList<ISmartEnum> GetAll(Type smartEnumType)
    {
        ArgumentNullException.ThrowIfNull(smartEnumType);

        return !typeof(ISmartEnum).IsAssignableFrom(smartEnumType)
            ? throw new ArgumentException($"Type '{smartEnumType.FullName}' does not implement {nameof(ISmartEnum)}.", nameof(smartEnumType))
            : Cache.GetOrAdd(smartEnumType, Discover);
    }

    /// <summary>
    /// Returns all smart enum instances for the specified type.
    /// </summary>
    /// <typeparam name="TSmartEnum">The smart enum type.</typeparam>
    /// <returns>All discovered smart enum instances.</returns>
    public static IReadOnlyList<TSmartEnum> GetAll<TSmartEnum>()
        where TSmartEnum : class, ISmartEnum
        => [.. GetAll(typeof(TSmartEnum)).Cast<TSmartEnum>()];

    private static IReadOnlyList<ISmartEnum> Discover(Type smartEnumType)
    {
        var smartEnumBase = FindSmartEnumBaseType(smartEnumType);
        var allProperty = smartEnumBase?.GetProperty("All", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        return allProperty?.GetValue(null) is IEnumerable values
            ? [.. values.Cast<ISmartEnum>()]
            : [.. smartEnumType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(field => smartEnumType.IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(null))
                .OfType<ISmartEnum>()];
    }

    private static Type? FindSmartEnumBaseType(Type smartEnumType)
    {
        for (var type = smartEnumType; type is not null; type = type.BaseType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SmartEnum<,>))
            {
                return type;
            }
        }

        return null;
    }
}
