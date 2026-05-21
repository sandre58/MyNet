// -----------------------------------------------------------------------
// <copyright file="IRegistryValueConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Win32;

namespace MyNet.Utilities.IO.Registry;

/// <summary>
/// Interface for converting between .NET types and registry value types.
/// </summary>
public interface IRegistryValueConverter
{
    /// <summary>
    /// Gets the registry value kind for the specified .NET type.
    /// </summary>
    /// <param name="type">The .NET type.</param>
    /// <returns>The corresponding registry value kind.</returns>
    RegistryValueKind GetKind(Type type);

    /// <summary>
    /// Converts a .NET value to a registry-compatible value.
    /// </summary>
    /// <param name="value">The .NET value to convert.</param>
    /// <returns>The converted registry value.</returns>
    object ConvertTo(object value);

    /// <summary>
    /// Converts a registry value to a .NET value of the specified type.
    /// </summary>
    /// <param name="value">The registry value to convert.</param>
    /// <param name="targetType">The target .NET type.</param>
    /// <returns>The converted .NET value.</returns>
    object ConvertFrom(object value, Type targetType);
}
