// -----------------------------------------------------------------------
// <copyright file="RegistryEntry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.IO.Registry;

/// <summary>
/// Represents an entry in the registry, associating a registry path with a specific item of type T.
/// </summary>
/// <typeparam name="T">The type of the item stored in the registry entry.</typeparam>
public sealed class RegistryEntry<T>(RegistryPath path, T item)
{
    /// <summary>
    /// Gets the registry path associated with this entry. The registry path represents the location in the registry where the item is stored. This property is read-only and is initialized through the constructor.
    /// </summary>
    public RegistryPath Path { get; } = path;

    /// <summary>
    /// Gets the item stored in this registry entry. This property is read-only and is initialized through the constructor.
    /// </summary>
    public T Item { get; } = item;
}
