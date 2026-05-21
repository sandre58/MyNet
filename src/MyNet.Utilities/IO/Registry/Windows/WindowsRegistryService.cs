// -----------------------------------------------------------------------
// <copyright file="WindowsRegistryService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace MyNet.Utilities.IO.Registry.Windows;

/// <summary>
/// Windows implementation of <see cref="IRegistryService"/>.
/// </summary>
/// <param name="store">The registry store used to interact with the Windows Registry.</param>
/// <param name="navigator">The registry navigator used to navigate the Windows Registry.</param>
/// <param name="converter">The converter used to translate registry values to CLR property types.</param>
[SupportedOSPlatform("windows")]
public sealed class WindowsRegistryService(IRegistryStore store, IRegistryNavigator navigator, IRegistryValueConverter converter) : IRegistryService
{
    /// <summary>
    /// Adds or updates a registry entry in the Windows Registry. The method takes a <see cref="RegistryEntry{T}"/> object, which contains the path and the item to be stored. The method iterates through the properties of the item and sets their values in the registry using the provided path. If a property value is null, it is skipped and not stored in the registry.
    /// </summary>
    /// <param name="entry">The registry entry to add or update.</param>
    /// <typeparam name="T">The type of the item in the registry entry.</typeparam>
    public void AddOrUpdate<T>(RegistryEntry<T> entry)
    {
        foreach (var prop in typeof(T).GetProperties().Where(p => p.CanWrite))
        {
            var value = prop.GetValue(entry.Item);

            if (value is null)
                continue;

            store.Set(entry.Path, prop.Name, value);
        }
    }

    /// <summary>
    /// Retrieves a registry entry from the Windows Registry based on the provided <see cref="RegistryPath"/>. The method creates a new instance of the specified type <typeparamref name="T"/> and populates its properties with values retrieved from the registry. If the registry key does not exist, the method returns null. Properties that cannot be written to or have null values in the registry are skipped during the population process.
    /// </summary>
    /// <param name="path">The registry path of the entry to retrieve.</param>
    /// <typeparam name="T">The type of the item in the registry entry.</typeparam>
    /// <returns>The retrieved registry entry, or null if the key does not exist.</returns>
    public RegistryEntry<T>? Get<T>(RegistryPath path)
        where T : new()
    {
        var instance = new T();

        using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path.ToString());
        if (key is null)
            return null;

        foreach (var prop in typeof(T).GetProperties().Where(p => p.CanWrite))
        {
            var raw = key.GetValue(prop.Name);
            if (raw is null)
                continue;

            var converted = converter.ConvertFrom(raw, prop.PropertyType);
            prop.SetValue(instance, converted);
        }

        return new(path, instance);
    }

    /// <summary>
    /// Retrieves all registry entries under a specified parent path in the Windows Registry. The method uses the provided <see cref="RegistryPath"/> to navigate to the parent key and retrieves all child keys. For each child key, it attempts to retrieve a registry entry of the specified type <typeparamref name="T"/> using the <see cref="Get{T}"/> method. The resulting collection contains all successfully retrieved registry entries, while any entries that could not be retrieved (e.g., due to missing keys or null values) are filtered out from the final result.
    /// </summary>
    /// <param name="parent">The parent registry path under which to retrieve entries.</param>
    /// <typeparam name="T">The type of the items in the registry entries.</typeparam>
    /// <returns>A collection of retrieved registry entries.</returns>
    public IEnumerable<RegistryEntry<T>> GetAll<T>(RegistryPath parent)
        where T : new() => navigator.GetChildren(parent)
            .Select(Get<T>)
            .Where(x => x is not null)!;

    /// <summary>
    /// Removes a registry entry from the Windows Registry based on the provided <see cref="RegistryPath"/>. The method uses the registry store to delete the key at the specified path, effectively removing the associated registry entry from the Windows Registry. If the key does not exist, the method does nothing and does not throw an exception.
    /// </summary>
    /// <param name="path">The registry path of the entry to remove.</param>
    public void Remove(RegistryPath path) => store.Remove(path);
}
