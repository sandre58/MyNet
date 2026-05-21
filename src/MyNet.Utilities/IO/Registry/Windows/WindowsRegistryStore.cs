// -----------------------------------------------------------------------
// <copyright file="WindowsRegistryStore.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Versioning;
using Microsoft.Win32;

namespace MyNet.Utilities.IO.Registry.Windows;

/// <summary>
/// Windows implementation of <see cref="IRegistryStore"/> using the Windows Registry.
/// </summary>
/// <param name="converter">The converter used to convert between .NET types and registry value types.</param>
[SupportedOSPlatform("windows")]
public sealed class WindowsRegistryStore(IRegistryValueConverter converter) : IRegistryStore
{
    private readonly RegistryKey _root = Microsoft.Win32.Registry.CurrentUser;

    /// <summary>
    /// Sets a value in the Windows Registry at the specified path and value key. The value is converted to a registry-compatible format using the provided <see cref="IRegistryValueConverter"/> before being stored in the registry. If the specified path does not exist, it will be created automatically. This method allows you to store various types of data in the registry, such as strings, integers, or custom objects, by converting them to a format that can be saved in the registry.
    /// </summary>
    /// <param name="path">The registry path where the value will be stored.</param>
    /// <param name="valueKey">The name of the value to store.</param>
    /// <param name="value">The value to store.</param>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    public void Set<T>(RegistryPath path, string valueKey, T value)
    {
        using var key = _root.CreateSubKey(path.ToString());
        key.SetValue(valueKey, converter.ConvertTo(value!));
    }

    /// <summary>
    /// Gets a value from the Windows Registry at the specified path and value key. The method attempts to open the registry key at the specified path and retrieve the value associated with the given value key. If the key or value does not exist, it returns the default value for the specified type. If the value is found, it uses the provided <see cref="IRegistryValueConverter"/> to convert the raw registry value back to the desired .NET type before returning it. This allows you to retrieve various types of data from the registry, such as strings, integers, or custom objects, by converting them from their registry format to a usable .NET type.
    /// </summary>
    /// <param name="path">The registry path where the value is stored.</param>
    /// <param name="valueKey">The name of the value to retrieve.</param>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <returns>The retrieved value, or the default value if the key or value does not exist.</returns>
    public T? Get<T>(RegistryPath path, string valueKey)
    {
        using var key = _root.OpenSubKey(path.ToString());

        var raw = key?.GetValue(valueKey);

        return raw is null ? default : (T)converter.ConvertFrom(raw, typeof(T));
    }

    /// <summary>
    /// Checks if a registry key exists at the specified path in the Windows Registry. The method attempts to open the registry key at the given path and returns true if the key exists (i.e., it can be opened successfully), or false if the key does not exist (i.e., it cannot be opened). This allows you to check for the existence of specific registry keys before attempting to read or write values, helping to prevent errors and ensure that your application interacts with the registry safely.
    /// </summary>
    /// <param name="path">The registry path to check for existence.</param>
    /// <returns>True if the registry key exists, false otherwise.</returns>
    public bool Exists(RegistryPath path) => _root.OpenSubKey(path.ToString()) is not null;

    /// <summary>
    /// Removes a registry key and its subkeys at the specified path in the Windows Registry. The method attempts to delete the registry key at the given path along with all its subkeys. If the key does not exist, no action is taken. This allows you to clean up registry entries created by your application or remove obsolete keys safely.
    /// </summary>
    /// <param name="path">The registry path of the key to remove.</param>
    public void Remove(RegistryPath path) => _root.DeleteSubKeyTree(path.ToString(), false);
}
