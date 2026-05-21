// -----------------------------------------------------------------------
// <copyright file="IRegistryService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Utilities.IO.Registry;

/// <summary>
/// Defines a generic abstraction over the Windows Registry that supports typed read/write operations
/// using reflecting property serialization.
/// </summary>
public interface IRegistryService
{
    /// <summary>
    /// Creates or updates all public writable properties of <paramref name="entry"/>'s item
    /// at the entry's full registry path.
    /// </summary>
    /// <typeparam name="T">Type of the item to persist.</typeparam>
    /// <param name="entry">The registry entry to store.</param>
    void AddOrUpdate<T>(RegistryEntry<T> entry);

    /// <summary>
    /// Reads a single entry from the registry by its full path (<c>Parent\Key</c>).
    /// </summary>
    /// <typeparam name="T">Type to deserialize the entry into.</typeparam>
    /// <param name="path">Full registry path.</param>
    /// <returns>The deserialized entry, or <c>null</c> if the key does not exist.</returns>
    RegistryEntry<T>? Get<T>(RegistryPath path)
        where T : new();

    /// <summary>
    /// Reads all direct sub-key entries under <paramref name="parent"/>, skipping any that cannot be
    /// deserialized.
    /// </summary>
    /// <typeparam name="T">Type to deserialize each entry into.</typeparam>
    /// <param name="parent">Parent registry path.</param>
    /// <returns>A sequence of deserialized entries.</returns>
    IEnumerable<RegistryEntry<T>> GetAll<T>(RegistryPath parent)
        where T : new();

    /// <summary>
    /// Removes the key at the given full <paramref name="path"/>.
    /// No exception is thrown when the key does not exist.
    /// </summary>
    void Remove(RegistryPath path);
}
