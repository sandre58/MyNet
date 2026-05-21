// -----------------------------------------------------------------------
// <copyright file="PluginService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MyNet.Utilities.Plugins;

/// <summary>
/// Low-level, stateless helpers for discovering, loading, and instantiating plugin types.
/// <para>
/// Convention: a plugin lives in a subdirectory whose name matches the DLL file it contains.
/// For example, a plugin directory <c>plugins/MyPlugin/</c> must contain <c>MyPlugin.dll</c>.
/// </para>
/// <para>
/// Loaded assemblies are cached by their full path so the same DLL is never loaded into
/// more than one <see cref="PluginLoadContext"/>, preserving type identity across calls.
/// </para>
/// </summary>
public static class PluginService
{
    /// <summary>
    /// Process-wide cache of successfully loaded plugin assemblies, keyed by the
    /// normalised (lower-case, full) DLL path. Prevents loading the same file into
    /// multiple <see cref="PluginLoadContext"/> instances, which would make types from
    /// different contexts incompatible with each other even when they come from the same DLL.
    /// </summary>
    private static readonly ConcurrentDictionary<string, Assembly> AssemblyCache =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Scans every subdirectory of <paramref name="pluginsDirectory"/> for a conventionally
    /// named DLL and returns all concrete (non-abstract, non-interface) types that
    /// implement or inherit <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The contract type (base class or interface) to look for.</typeparam>
    /// <param name="pluginsDirectory">
    /// The root directory whose immediate subdirectories are treated as plugin packages.
    /// </param>
    /// <returns>
    /// A flat, deduplicated sequence of matching concrete types across all loaded plugins.
    /// Returns an empty sequence when the directory does not exist or contains no valid plugins.
    /// </returns>
    public static IEnumerable<Type> GetTypes<T>(string pluginsDirectory)
    {
        if (!Directory.Exists(pluginsDirectory)) return [];

        var result = new List<Type>();

        foreach (var subdirectory in new DirectoryInfo(pluginsDirectory).GetDirectories())
        {
            var dllPath = Path.Combine(subdirectory.FullName, $"{subdirectory.Name}.dll");

            if (!File.Exists(dllPath)) continue;

            var assembly = LoadAssembly(dllPath);
            if (assembly is null) continue;

            var matchingTypes = assembly.GetTypes()
                                        .Where(t => t is { IsAbstract: false, IsInterface: false }
                                                    && t.IsAssignableTo(typeof(T)));
            result.AddRange(matchingTypes);
        }

        return result;
    }

    /// <summary>
    /// Loads the assembly located at <paramref name="pluginPath"/> and returns the first
    /// concrete type that implements or inherits <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The contract type (base class or interface) to look for.</typeparam>
    /// <param name="pluginPath">The full path to the plugin DLL file.</param>
    /// <returns>
    /// The first matching concrete <see cref="Type"/>, or <c>null</c> when the file does
    /// not exist, cannot be loaded, or contains no suitable type.
    /// </returns>
    public static Type? FindType<T>(string pluginPath)
    {
        if (string.IsNullOrEmpty(pluginPath)) return null;

        var assembly = LoadAssembly(pluginPath);
        return assembly?.GetTypes()
                        .FirstOrDefault(t => t is { IsAbstract: false, IsInterface: false }
                                             && t.IsAssignableTo(typeof(T)));
    }

    /// <summary>
    /// Loads the assembly at <paramref name="pluginPath"/>, locates the first concrete type
    /// implementing <typeparamref name="T"/>, and creates an instance of it using
    /// <see cref="Activator.CreateInstance(Type, object[])"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the created instance.</typeparam>
    /// <param name="pluginPath">The full path to the plugin DLL file.</param>
    /// <param name="constructorParameters">
    /// Optional arguments forwarded to the constructor of the discovered type.
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="T"/>, or <c>null</c> if no matching type
    /// was found or instantiation failed.
    /// </returns>
    public static T? CreateInstance<T>(string pluginPath, params object[] constructorParameters)
    {
        if (string.IsNullOrEmpty(pluginPath)) return default;

        var type = FindType<T>(pluginPath);
        return type is null ? default : (T?)Activator.CreateInstance(type, constructorParameters);
    }

    /// <summary>
    /// Returns the cached <see cref="Assembly"/> for <paramref name="dllPath"/>, loading it
    /// on first access via a dedicated <see cref="PluginLoadContext"/>.
    /// </summary>
    /// <param name="dllPath">The full, absolute path to the DLL file.</param>
    /// <returns>
    /// The loaded <see cref="Assembly"/>, or <c>null</c> when the file is missing or the
    /// load operation throws (the exception is swallowed and the entry is not cached so
    /// that a corrected DLL can be picked up on the next call).
    /// </returns>
    private static Assembly? LoadAssembly(string dllPath)
    {
        if (!File.Exists(dllPath)) return null;

        if (AssemblyCache.TryGetValue(dllPath, out var cached)) return cached;

        try
        {
            var assemblyName = Path.GetFileNameWithoutExtension(dllPath);
            var loadContext = new PluginLoadContext(dllPath);
            var assembly = loadContext.LoadFromAssemblyName(new(assemblyName));

            // Only cache on success so a broken DLL can be replaced and retried.
            AssemblyCache.TryAdd(dllPath, assembly);
            return assembly;
        }
        catch (Exception ex) when (ex is FileLoadException or BadImageFormatException or FileNotFoundException)
        {
            // The DLL exists on disk but could not be loaded (wrong architecture, missing
            // dependencies, corrupted file …). Log-worthy but not fatal for the host.
            return null;
        }
    }
}
