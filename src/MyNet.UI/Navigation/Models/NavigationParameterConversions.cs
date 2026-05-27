// -----------------------------------------------------------------------
// <copyright file="NavigationParameterConversions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Provides utility methods for converting raw navigation parameter values to strongly typed values, supporting various types including enums and GUIDs.
/// </summary>
internal static class NavigationParameterConversions
{
    /// <summary>
    /// Attempts to convert a raw navigation parameter value to a strongly typed value of type T. Supports direct assignment, nullability, enums, GUIDs, and other convertible types. Returns true if the conversion is successful; otherwise, false.
    /// </summary>
    /// <param name="rawValue">The raw navigation parameter value.</param>
    /// <param name="value">The strongly typed value if the conversion is successful; otherwise, the default value of type T.</param>
    /// <typeparam name="T">The target type to convert to.</typeparam>
    /// <returns>True if the conversion is successful; otherwise, false.</returns>
    public static bool TryConvert<T>(object? rawValue, out T? value)
    {
        switch (rawValue)
        {
            case T typed:
                value = typed;
                return true;
            case null:
                value = default;
                return true;
        }

        var targetType = typeof(T);
        var nullableUnderlying = Nullable.GetUnderlyingType(targetType);
        if (nullableUnderlying is not null)
            targetType = nullableUnderlying;

        try
        {
            var converted = targetType switch
            {
                _ when targetType.IsEnum => ConvertToEnum(rawValue, targetType),
                _ when targetType == typeof(Guid) && rawValue is string guidText => Guid.Parse(guidText),
                _ when rawValue is string text => Convert.ChangeType(text, targetType, CultureInfo.InvariantCulture),
                _ => Convert.ChangeType(rawValue, targetType, CultureInfo.InvariantCulture),
            };

            value = (T)converted;
            return true;
        }
        catch (FormatException)
        {
            value = default;
            return false;
        }
        catch (InvalidCastException)
        {
            value = default;
            return false;
        }
        catch (OverflowException)
        {
            value = default;
            return false;
        }
    }

    private static object ConvertToEnum(object rawValue, Type enumType) => rawValue switch
    {
        string text => Enum.Parse(enumType, text, ignoreCase: true),
        _ => Enum.ToObject(enumType, Convert.ChangeType(rawValue, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture)!),
    };
}
