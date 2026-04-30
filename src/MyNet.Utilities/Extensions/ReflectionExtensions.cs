// -----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case", Justification = "Extensions methods for reflection are more readable when they are called on the type or property they operate on, rather than on a separate class.")]
public static class ReflectionExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesCache = new();
    private static readonly ConcurrentDictionary<(Type, Type), PropertyInfo[]> AttributeCache = new();

    extension(Type type)
    {
        public IList<PropertyInfo> GetPublicProperties()
            => PropertiesCache.GetOrAdd(type, t => [.. t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead || x.CanWrite)]);

        public IList<PropertyInfo> GetPublicPropertiesWithAttribute<TAttribute>()
            where TAttribute : Attribute
            => AttributeCache.GetOrAdd((type, typeof(TAttribute)), _ => [.. type.GetPublicProperties().Where(p => p.IsDefined(typeof(TAttribute), true))]);

        public object? GetDefault()
        {
            var f = GetDefault<object?>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);
        }
    }

    extension(PropertyInfo property)
    {
        public bool HasAttribute<T>()
            where T : Attribute
            => property.IsDefined(typeof(T), true);

        public bool HasPublicSetterOrGetter()
        {
            var setMethod = property.GetSetMethod();
            var getMethod = property.GetGetMethod();
            return (setMethod?.IsPublic == true) || (getMethod?.IsPublic == true);
        }
    }

    public static IEnumerable<T?> GetValuesOfType<T>(this IEnumerable<PropertyInfo> properties, object obj)
        => properties.Where(x => typeof(T).IsAssignableFrom(x.PropertyType)).Select(x => (T?)x.GetValue(obj)).Where(x => x is not null);

    public static T? GetAttribute<T>(this Enum value)
        where T : Attribute
        => value.GetType().GetField(value.ToString())?.GetCustomAttributes<T>().FirstOrDefault();

    extension(object rootObject)
    {
        public object? GetDeepPropertyValue(string path) => rootObject.GetDeepPropertyValue(path.Split(["."], StringSplitOptions.RemoveEmptyEntries));

        public object? GetDeepPropertyValue(IList<string> propertyNames)
        {
            var result = rootObject;

            if (!propertyNames.Any()) return result;
            foreach (var propertyName in propertyNames)
            {
                var properties = result?.GetType().GetPublicProperties();

                var propertyInfo = properties?.FirstOrDefault(x => x.Name == propertyName);
                if (propertyInfo == null)
                {
                    return null;
                }

                result = propertyInfo.GetValue(result, null);
            }

            return result;
        }

        public T? GetDeepPropertyValue<T>(string path) => (T?)rootObject.GetDeepPropertyValue(path);

        public T? GetDeepPropertyValue<T>(IList<string> propertyNames) => (T?)rootObject.GetDeepPropertyValue(propertyNames);
    }

    /// <summary>
    /// Gets the member inheritance chain as a stack.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="expression">The member expression.</param>
    /// <returns>The inheritance chain for the given member expression as a stack.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stack<MemberInfo> GetMembers<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expression)
    {
        var stack = new Stack<MemberInfo>();

        var currentExpression = expression.Body;
        while (true)
        {
            var memberExpression = currentExpression?.GetMemberExpression();
            if (memberExpression == null)
            {
                break;
            }

            stack.Push(memberExpression.Member);
            currentExpression = memberExpression.Expression;
        }

        return stack;
    }

    public static string? GetPropertyName<T>(this Expression<Func<T>> propertyExpression) => ((propertyExpression.Body as MemberExpression)?.Member as PropertyInfo)?.Name;

    public static string? GetPropertyName<TSource, T>(this Expression<Func<TSource, T>> propertyAccessor)
        => (propertyAccessor.Body as MemberExpression ?? ((UnaryExpression)propertyAccessor.Body).Operand as MemberExpression)?.Member.Name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MemberExpression? GetMemberExpression(this Expression expression)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.Convert:
                {
                    var body = (UnaryExpression)expression;
                    return body.Operand as MemberExpression;
                }

            case ExpressionType.MemberAccess:
                return expression as MemberExpression;
        }

        return null;
    }

    private static T? GetDefault<T>() => default;
}
