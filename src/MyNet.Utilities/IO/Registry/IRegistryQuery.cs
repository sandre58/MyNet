// -----------------------------------------------------------------------
// <copyright file="IRegistryQuery.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.IO.Registry;

/// <summary>
/// Interface for querying the Windows Registry, allowing retrieval of registry paths based on specific values.
/// </summary>
public interface IRegistryQuery
{
    /// <summary>
    /// Finds a registry path under the specified parent path that contains a value with the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the value to search for.</typeparam>
    /// <param name="parent">The parent registry path to search under.</param>
    /// <param name="valueKey">The key of the value to search for.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>The registry path that contains the specified value, or null if not found.</returns>
    RegistryPath? FindByValue<T>(RegistryPath parent, string valueKey, T value);
}
