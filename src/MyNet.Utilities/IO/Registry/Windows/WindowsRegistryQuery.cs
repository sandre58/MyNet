// -----------------------------------------------------------------------
// <copyright file="WindowsRegistryQuery.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Versioning;
using Microsoft.Win32;

namespace MyNet.Utilities.IO.Registry.Windows;

/// <summary>
/// Implements a registry query using the Windows Registry API.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsRegistryQuery(IRegistryValueConverter converter) : IRegistryQuery
{
    private readonly RegistryKey _root = Microsoft.Win32.Registry.CurrentUser;

    /// <summary>
    /// Finds a registry key by searching for a specific value under a parent key.
    /// </summary>
    /// <param name="parent">The parent registry path.</param>
    /// <param name="valueKey">The name of the value to search for.</param>
    /// <param name="value">The value to match.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The registry path of the matching key, or null if not found.</returns>
    public RegistryPath? FindByValue<T>(RegistryPath parent, string valueKey, T value)
    {
        using var key = _root.OpenSubKey(parent.ToString());
        if (key is null)
            return null;

        foreach (var childName in key.GetSubKeyNames())
        {
            using var child = key.OpenSubKey(childName);

            var raw = child?.GetValue(valueKey);
            if (raw is null)
                continue;

            var converted = converter.ConvertFrom(raw, typeof(T));

            if (Equals(converted, value))
                return RegistryPath.Combine(parent.ToString(), childName);
        }

        return null;
    }
}
