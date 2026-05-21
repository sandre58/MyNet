// -----------------------------------------------------------------------
// <copyright file="PluginsProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MyNet.Utilities.Plugins;

/// <summary>
/// A scoped, caching facade over <see cref="PluginService"/> that targets a fixed
/// <see cref="Root"/> plugins directory.
/// <para>
/// All public members are thread-safe.
/// </para>
/// </summary>
/// <param name="root">
/// The root directory that contains plugin subdirectories.
/// See <see cref="PluginService"/> for the expected directory/DLL naming convention.
/// </param>
public class PluginsProvider(string root)
{
    /// <summary>
    /// Thread-safe cache of discovered plugin types, keyed by the contract type.
    /// Using <see cref="ConcurrentDictionary{TKey,TValue}"/> avoids the heavyweight
    /// expiry machinery of <c>CacheStorage</c>, which is unnecessary here because
    /// plugin types never expire during a process lifetime.
    /// </summary>
    private readonly ConcurrentDictionary<Type, IReadOnlyList<Type>> _cache = new();

    /// <summary>
    /// Gets the root plugins directory that this provider is scoped to.
    /// </summary>
    public string Root { get; } = root;

    /// <summary>
    /// Returns all concrete types found in the plugins under <see cref="Root"/> that
    /// implement or inherit <typeparamref name="T"/>.
    /// Results are cached after the first call so subsequent invocations are O(1).
    /// </summary>
    /// <typeparam name="T">The contract type (base class or interface) to look for.</typeparam>
    /// <returns>
    /// A read-only list of matching concrete types; never <c>null</c>, possibly empty.
    /// </returns>
    public IReadOnlyList<Type> GetTypes<T>() =>
        _cache.GetOrAdd(typeof(T), _ => [.. PluginService.GetTypes<T>(Root)]);

    /// <summary>
    /// Returns the <see cref="Type"/> of the plugin that implements <typeparamref name="T"/>,
    /// optionally filtered by <paramref name="assemblyName"/>.
    /// </summary>
    /// <typeparam name="T">The contract type (base class or interface) to look for.</typeparam>
    /// <param name="assemblyName">
    /// When provided, only types whose declaring assembly name matches
    /// (case-insensitive) are considered. When <c>null</c> or empty, the first
    /// discovered type is returned regardless of its assembly.
    /// </param>
    /// <returns>
    /// The matching <see cref="Type"/>, or <c>null</c> when no plugin satisfies the criteria.
    /// </returns>
    public Type? FindType<T>(string? assemblyName = null)
    {
        var types = GetTypes<T>();

        return !string.IsNullOrEmpty(assemblyName)
            ? types.FirstOrDefault(t => string.Equals(t.Assembly.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase))
            : types.FirstOrDefault();
    }

    /// <summary>
    /// Creates an instance of the plugin that implements <typeparamref name="T"/>,
    /// optionally filtered by <paramref name="assemblyName"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the created instance.</typeparam>
    /// <param name="assemblyName">
    /// When provided, limits the search to the plugin assembly with this name
    /// (case-insensitive). When <c>null</c> or empty, the first available plugin is used.
    /// </param>
    /// <param name="constructorParameters">
    /// Optional arguments forwarded verbatim to the plugin type's constructor via
    /// <see cref="Activator.CreateInstance(Type, object[])"/>.
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="T"/>, or <c>null</c> when no matching plugin
    /// was found or instantiation failed.
    /// </returns>
    public T? Create<T>(string? assemblyName = null, params object[] constructorParameters)
    {
        var type = FindType<T>(assemblyName);
        return type is null ? default : (T?)Activator.CreateInstance(type, constructorParameters);
    }
}
