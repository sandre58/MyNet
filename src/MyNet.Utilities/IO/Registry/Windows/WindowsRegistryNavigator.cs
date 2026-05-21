// -----------------------------------------------------------------------
// <copyright file="WindowsRegistryNavigator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace MyNet.Utilities.IO.Registry.Windows;

/// <summary>
/// Provides a registry navigator implementation for Windows using Microsoft.Win32.Registry.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsRegistryNavigator : IRegistryNavigator
{
    private readonly RegistryKey _root = Microsoft.Win32.Registry.CurrentUser;

    /// <summary>
    /// Gets the child registry paths of the specified parent registry path. This method opens the registry key corresponding to the parent path and retrieves its subkey names, combining them with the parent path to create a list of child registry paths. If the parent key does not exist, an empty list is returned.
    /// </summary>
    /// <param name="parent">The parent registry path.</param>
    /// <returns>An enumerable collection of child registry paths.</returns>
    public IEnumerable<RegistryPath> GetChildren(RegistryPath parent)
    {
        using var key = _root.OpenSubKey(parent.ToString());

        return key is null
            ? []
            : key.GetSubKeyNames()
            .Select(name => RegistryPath.Combine(parent.ToString(), name));
    }

    /// <summary>
    /// Counts the number of child registry keys under the specified parent registry path. This method opens the registry key corresponding to the parent path and retrieves its subkey count. If the parent key does not exist, it returns zero.
    /// </summary>
    /// <param name="parent">The parent registry path.</param>
    /// <returns>The number of child registry keys.</returns>
    public int Count(RegistryPath parent) => _root.OpenSubKey(parent.ToString())?.SubKeyCount ?? 0;
}
