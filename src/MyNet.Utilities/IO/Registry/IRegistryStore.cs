// -----------------------------------------------------------------------
// <copyright file="IRegistryStore.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.IO.Registry;

/// <summary>
/// Defines an interface for a registry store, which provides methods for setting, getting, checking existence, and removing values in a registry. The interface abstracts the underlying implementation of the registry, allowing for different implementations that can interact with various types of registries (e.g., Windows Registry, custom in-memory registry, etc.). The methods allow for storing and retrieving values of any type, as well as checking if a specific registry path exists and removing entries from the registry.
/// </summary>
public interface IRegistryStore
{
    /// <summary>
    /// Sets the value of a registry entry at the specified path and key.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="path">The registry path where the value is stored.</param>
    /// <param name="valueKey">The key of the value to set.</param>
    /// <param name="value">The value to set.</param>
    void Set<T>(RegistryPath path, string valueKey, T value);

    /// <summary>
    /// Gets the value of a registry entry at the specified path and key.
    /// </summary>
    /// <typeparam name="T">The type of the value to get.</typeparam>
    /// <param name="path">The registry path where the value is stored.</param>
    /// <param name="valueKey">The key of the value to get.</param>
    /// <returns>The value of the registry entry, or null if the entry does not exist.</returns>
    T? Get<T>(RegistryPath path, string valueKey);

    /// <summary>
    /// Checks if a registry entry exists at the specified path.
    /// </summary>
    /// <param name="path">The registry path to check.</param>
    /// <returns>True if the registry entry exists, false otherwise.</returns>
    bool Exists(RegistryPath path);

    /// <summary>
    /// Removes a registry entry at the specified path.
    /// </summary>
    /// <param name="path">The registry path of the entry to remove.</param>
    void Remove(RegistryPath path);
}
