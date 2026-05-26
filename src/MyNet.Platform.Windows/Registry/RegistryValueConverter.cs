// -----------------------------------------------------------------------
// <copyright file="RegistryValueConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Versioning;
using Microsoft.Win32;
using MyNet.IO.Registry;

namespace MyNet.Platform.Windows.Registry;

/// <summary>
/// Provides functionality to convert .NET types to and from their corresponding registry value representations. This class implements the <see cref="IRegistryValueConverter"/> interface, allowing for consistent conversion of various data types when working with the Windows Registry. It supports a range of common types, including strings, numeric types, booleans, byte arrays, and enums, ensuring that values can be accurately stored and retrieved from the registry in a format that is compatible with the expected registry value kinds.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class RegistryValueConverter : IRegistryValueConverter
{
    private static readonly Dictionary<Type, RegistryValueKind> TypeMappings = new()
    {
        [typeof(string)] = RegistryValueKind.String,
        [typeof(short)] = RegistryValueKind.DWord,
        [typeof(ushort)] = RegistryValueKind.DWord,
        [typeof(int)] = RegistryValueKind.DWord,
        [typeof(uint)] = RegistryValueKind.DWord,
        [typeof(long)] = RegistryValueKind.QWord,
        [typeof(ulong)] = RegistryValueKind.QWord,
        [typeof(bool)] = RegistryValueKind.DWord,
        [typeof(byte[])] = RegistryValueKind.Binary,
        [typeof(string[])] = RegistryValueKind.MultiString
    };

    /// <summary>
    /// Gets the corresponding <see cref="RegistryValueKind"/> for a given .NET type. This method checks if the provided type is an enum and returns <see cref="RegistryValueKind.String"/> for enums, as they are stored as strings in the registry. For other types, it looks up the type in the predefined <see cref="TypeMappings"/> dictionary to determine the appropriate registry value kind. If the type is not found in the dictionary, it defaults to <see cref="RegistryValueKind.String"/>, ensuring that unsupported types are still handled gracefully when interacting with the registry.
    /// </summary>
    /// <param name="type">The .NET type for which to get the corresponding registry value kind.</param>
    /// <returns>The <see cref="RegistryValueKind"/> that corresponds to the specified .NET type.</returns>
    public RegistryValueKind GetKind(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsEnum ? RegistryValueKind.String : TypeMappings.GetValueOrDefault(type, RegistryValueKind.String);
    }

    /// <summary>
    /// Converts a .NET value to its corresponding registry value representation. This method takes an object as input and determines its type to perform the appropriate conversion. If the value is an enum, it converts it to its string representation. For boolean values, it converts them to 1 (true) or 0 (false). For DateTime and DateTimeOffset values, it converts them to their ISO 8601 string representation using the "O" format specifier. For Guid values, it converts them to their standard string representation. For other types, it returns the value as is, relying on the registry's ability to handle the underlying type based on the determined registry value kind.
    /// </summary>
    /// <param name="value">The .NET value to convert to a registry value representation.</param>
    /// <returns>The registry value representation of the specified .NET value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided value is null.</exception>
    public object ConvertTo(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var type = value.GetType();

        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsEnum
            ? value.ToString()!
            : type == typeof(bool)
            ? (bool)value ? 1 : 0
            : type == typeof(DateTime)
            ? ((DateTime)value).ToString("O", CultureInfo.InvariantCulture)
            : type == typeof(DateTimeOffset)
            ? ((DateTimeOffset)value).ToString("O", CultureInfo.InvariantCulture)
            : type == typeof(Guid) ? value.ToString()! : value;
    }

    /// <summary>
    /// Converts a registry value representation back to its corresponding .NET type. This method takes an object representing the registry value and a target .NET type to convert to. It first checks if the target type is nullable and retrieves the underlying type if it is. Then, it performs the appropriate conversion based on the effective type. If the effective type is an enum, it parses the string representation back to the enum value. For boolean values, it converts integers (1 or 0) back to their boolean representation. For DateTime and DateTimeOffset values, it parses the ISO 8601 string representation back to their respective types. For Guid values, it parses the string representation back to a Guid object. For other types, it uses the standard Convert.ChangeType method to perform the conversion, ensuring that values are accurately reconstructed from their registry representations.
    /// </summary>
    /// <param name="value">The registry value representation to convert.</param>
    /// <param name="targetType">The target .NET type to convert to.</param>
    /// <returns>The .NET value corresponding to the specified registry value representation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided value is null.</exception>
    public object ConvertFrom(object value, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(value);

        var underlyingType = Nullable.GetUnderlyingType(targetType);
        var effectiveType = underlyingType ?? targetType;

        return effectiveType.IsEnum
            ? Enum.Parse(effectiveType, value.ToString()!, true)
            : effectiveType == typeof(bool)
            ? Convert.ToBoolean(value, CultureInfo.InvariantCulture)
            : effectiveType == typeof(DateTime)
            ? DateTime.Parse(
                value.ToString()!,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind)
            : effectiveType == typeof(DateTimeOffset)
            ? DateTimeOffset.Parse(
                value.ToString()!,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind)
            : effectiveType == typeof(Guid)
            ? Guid.Parse(value.ToString()!)
            : Convert.ChangeType(
            value,
            effectiveType,
            CultureInfo.InvariantCulture);
    }
}
