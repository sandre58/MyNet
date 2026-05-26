// -----------------------------------------------------------------------
// <copyright file="RegistryPath.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.IO.Registry;

/// <summary>
/// Represents a registry path, which is a string that specifies the location of a registry key in the Windows Registry. The registry path is used to access and manipulate registry keys and values. It typically consists of a root key (such as HKEY_CURRENT_USER or HKEY_LOCAL_MACHINE) followed by subkeys that define the hierarchy of the registry. The <see cref="RegistryPath"/> struct provides a convenient way to represent and work with registry paths in code, allowing for easy combination of parent and child keys and providing a clear representation of the registry path as a string.
/// </summary>
/// <param name="Parent">The parent registry path.</param>
/// <param name="Key">The child key to append to the parent path.</param>
public readonly record struct RegistryPath(string Parent, string Key)
{
    /// <summary>
    /// Combines a parent registry path with a child key to create a new registry path. This method takes a parent registry path and a child key, concatenates them with a backslash ("\") separator, and returns a new <see cref="RegistryPath"/> instance representing the combined path. This is useful for constructing registry paths dynamically by combining existing paths with new keys, allowing for easy navigation and manipulation of the Windows Registry in code.
    /// </summary>
    /// <param name="parent">The parent registry path.</param>
    /// <param name="key">The child key to append to the parent path.</param>
    /// <returns>A new <see cref="RegistryPath"/> instance representing the combined path.</returns>
    public static RegistryPath Combine(string parent, string key) => new(parent, key);

    /// <summary>
    /// Returns a string representation of the registry path in the format "Parent\Key". This method overrides the default ToString() implementation to provide a clear and concise representation of the registry path, making it easier to read and understand when working with registry paths in code. The resulting string can be used for logging, debugging, or any situation where a human-readable representation of the registry path is needed.
    /// </summary>
    /// <returns>A string representation of the registry path in the format "Parent\Key".</returns>
    public override string ToString() => $@"{Parent}\{Key}";
}
