// -----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyNet.Utilities.Metadata.Attributes;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ReflectionExtensions
{
    extension(MemberInfo memberInfo)
    {
        /// <summary>
        /// Gets the attribute of the specified type applied to the member.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="inherit">
        /// Indicates whether to search the member's inheritance chain.
        /// </param>
        /// <returns>
        /// The attribute instance if found; otherwise <c>null</c>.
        /// </returns>
        public TAttribute? GetAttribute<TAttribute>(bool inherit = true)
            where TAttribute : Attribute
            => memberInfo.GetCustomAttribute<TAttribute>(inherit);
    }

    /// <summary>
    /// Gets the public instance properties of the specified type, using a cache to improve performance on repeated calls.
    /// </summary>
    extension(Type type)
    {
        /// <summary>
        /// Gets the public instance properties of the specified type, using a cache to improve performance on repeated calls.
        /// </summary>
        public PropertyInfo[] GetPublicProperties() => [.. GetCachedPublicProperties(type)];

        /// <summary>
        /// Gets the public instance properties of the specified type that are decorated with the specified attribute, using a cache to improve performance on repeated calls.
        /// </summary>
        public PropertyInfo[] GetPublicPropertiesWithAttribute<TAttribute>()
            where TAttribute : Attribute
            => [.. GetCachedPublicPropertiesWithAttribute(type, typeof(TAttribute))];

        /// <summary>
        /// Gets a dictionary mapping property names to their corresponding PropertyInfo objects for the specified type, using a cache to improve performance on repeated calls.
        /// </summary>
        public Dictionary<string, PropertyInfo> GetPropertyMap() => new(GetCachedPropertyMap(type), StringComparer.Ordinal);

        /// <summary>
        /// Gets the default value for the specified type. For reference types, this will be null; for value types, this will be the result of Activator.CreateInstance(type).
        /// </summary>
        /// <returns>The default value for the specified type.</returns>
        public object? GetDefault() => type.IsValueType ? Activator.CreateInstance(type) : null;

        /// <summary>
        /// Determines whether the specified type is a simple type. Simple types include primitive types, enums, strings, decimals, DateTime, DateTimeOffset, TimeSpan, Guid, and Uri. The method also handles nullable types by checking the underlying type. This is used to avoid traversing simple types during the validation process, as they do not need to be validated like complex objects.
        /// </summary>
        /// <returns>True if the type is a simple type; otherwise, false.</returns>
        public bool IsSimple()
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid) ||
                type == typeof(Uri);
        }

        /// <summary>
        /// Gets the attribute of the specified type applied to the type.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="inherit">
        /// Indicates whether to search the inheritance chain.
        /// </param>
        /// <returns>
        /// The attribute instance if found; otherwise <c>null</c>.
        /// </returns>
        public TAttribute? GetAttribute<TAttribute>(bool inherit = true)
            where TAttribute : Attribute
            => type.GetCustomAttribute<TAttribute>(inherit);
    }

    /// <summary>
    /// Extension methods for PropertyInfo to check for attributes and accessors.
    /// </summary>
    extension(PropertyInfo property)
    {
        /// <summary>
        /// Determines whether the property is decorated with the specified attribute, checking the inheritance chain if necessary.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to check for.</typeparam>
        /// <returns>True if the property is decorated with the specified attribute; otherwise, false.</returns>
        public bool HasAttribute<TAttribute>()
            where TAttribute : Attribute
            => property.IsDefined(typeof(TAttribute), inherit: true);

        /// <summary>
        /// Determines whether the property has a public getter or setter.
        /// </summary>
        /// <returns>True if the property has a public getter or setter; otherwise, false.</returns>
        public bool HasPublicGetterOrSetter()
            => property.GetMethod?.IsPublic == true ||
               property.SetMethod?.IsPublic == true;

        /// <summary>
        /// Gets the attribute of the specified type applied to the property.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="inherit">
        /// Indicates whether to search the inheritance chain.
        /// </param>
        /// <returns>
        /// The attribute instance if found; otherwise <c>null</c>.
        /// </returns>
        public TAttribute? GetAttribute<TAttribute>(bool inherit = true)
            where TAttribute : Attribute
            => property.GetCustomAttribute<TAttribute>(inherit);

        /// <summary>
        /// Gets the symbol associated with the enum value by retrieving the SymbolAttribute applied to it. Returns null if the SymbolAttribute is not found.
        /// </summary>
        /// <returns>The symbol associated with the enum value, or null if the SymbolAttribute is not found.</returns>
        public string? GetSymbol() => property.GetAttribute<SymbolAttribute>()?.Value;
    }

    extension(Enum value)
    {
        /// <summary>
        /// Gets the specified attribute from an enum value, checking the inheritance chain if necessary. Returns null if the attribute is not found.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to get.</typeparam>
        /// <returns>The attribute of the specified type, or null if not found.</returns>
        public TAttribute? GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            var member = value.GetType().GetField(value.ToString());

            return member?.GetAttribute<TAttribute>();
        }

        /// <summary>
        /// Gets the symbol associated with the enum value by retrieving the SymbolAttribute applied to it. Returns null if the SymbolAttribute is not found.
        /// </summary>
        /// <returns>The symbol associated with the enum value, or null if the SymbolAttribute is not found.</returns>
        public string? GetSymbol() => value.GetAttribute<SymbolAttribute>()?.Value;
    }

    /// <summary>
    /// Gets the value of a nested property specified by a dot-separated path, using reflection to traverse the object graph. Returns null if any part of the path is invalid or if the final value is null.
    /// </summary>
    extension(object root)
    {
        /// <summary>
        /// Gets the value of a nested property specified by a dot-separated path, using reflection to traverse the object graph. Returns null if any part of the path is invalid or if the final value is null.
        /// </summary>
        /// <param name="path">The dot-separated path of the nested property.</param>
        /// <returns>The value of the nested property, or null if any part of the path is invalid or if the final value is null.</returns>
        public object? GetDeepPropertyValue(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            return root.GetDeepPropertyValue(parts);
        }

        /// <summary>
        /// Gets the value of a nested property specified by a list of property names, using reflection to traverse the object graph. Returns null if any part of the path is invalid or if the final value is null.
        /// </summary>
        /// <param name="path">The list of property names representing the path of the nested property.</param>
        /// <returns>The value of the nested property, or null if any part of the path is invalid or if the final value is null.</returns>
        public object? GetDeepPropertyValue(IReadOnlyList<string> path)
        {
            var current = root;

            for (var i = 0; i < path.Count && current is not null; i++)
            {
                var type = current.GetType();
                var map = GetCachedPropertyMap(type);

                if (!map.TryGetValue(path[i], out var property))
                    return null;

                current = property.GetValue(current);
            }

            return current;
        }

        /// <summary>
        /// Gets the value of a nested property specified by a dot-separated path, using reflection to traverse the object graph and casting the final value to the specified type. Returns null if any part of the path is invalid, if the final value is null, or if the final value cannot be cast to the specified type.
        /// </summary>
        /// <param name="path">The dot-separated path of the nested property.</param>
        /// <typeparam name="T">The type to cast the final value to.</typeparam>
        /// <returns>The value of the nested property cast to the specified type, or null if any part of the path is invalid, if the final value is null, or if the final value cannot be cast to the specified type.</returns>
        public T? GetDeepPropertyValue<T>(string path) => (T?)root.GetDeepPropertyValue(path);
    }

    extension(IEnumerable<PropertyInfo> properties)
    {
        /// <summary>
        /// Gets the values of the properties of the specified type from the given instance, using reflection to check each property's type and retrieve its value. Returns an enumerable of the property values that match the specified type, or an empty enumerable if no properties match or if the instance is null.
        /// </summary>
        /// <param name="instance">The instance from which to retrieve the property values.</param>
        /// <typeparam name="T">The type of the property values to retrieve.</typeparam>
        /// <returns>An enumerable of the property values that match the specified type, or an empty enumerable if no properties match or if the instance is null.</returns>
        public IEnumerable<T?> GetValuesOfType<T>(object? instance)
        {
            ArgumentNullException.ThrowIfNull(properties);

            return getValuesOfType();

            IEnumerable<T?> getValuesOfType()
            {
                if (instance is null)
                    yield break;

                foreach (var property in properties)
                {
                    if (typeof(T).IsAssignableFrom(property.PropertyType))
                    {
                        var value = property.GetValue(instance);
                        if (value is T typed)
                            yield return typed;
                    }
                }
            }
        }
    }

    #region Property caches

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PublicPropertiesCache = new();
    private static readonly ConcurrentDictionary<(Type Type, Type Attribute), PropertyInfo[]> AttributeCache = new();
    private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> PropertyMapCache = new();

    private static PropertyInfo[] GetCachedPublicProperties(Type type)
        => PublicPropertiesCache.GetOrAdd(type, static t =>
            t.GetProperties(BindingFlags.Instance | BindingFlags.Public));

    private static PropertyInfo[] GetCachedPublicPropertiesWithAttribute(Type type, Type attributeType)
    {
        var key = (type, attributeType);

        return AttributeCache.GetOrAdd(key, static k =>
        {
            var (t, attrType) = k;

            var properties = GetCachedPublicProperties(t);

            return [.. properties.Where(p => p.IsDefined(attrType, inherit: true))];
        });
    }

    private static Dictionary<string, PropertyInfo> GetCachedPropertyMap(Type type)
        => PropertyMapCache.GetOrAdd(type, static t =>
        {
            var dict = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);

            foreach (var prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                dict[prop.Name] = prop;
            }

            return dict;
        });

    #endregion
}
