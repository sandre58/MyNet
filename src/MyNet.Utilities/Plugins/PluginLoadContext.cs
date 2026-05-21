// -----------------------------------------------------------------------
// <copyright file="PluginLoadContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.Loader;

namespace MyNet.Utilities.Plugins;

/// <summary>
/// An isolated <see cref="AssemblyLoadContext"/> used to load a single plugin assembly
/// and its dependencies without polluting the default load context.
/// Each plugin subdirectory gets its own <see cref="PluginLoadContext"/> so that
/// different plugins can reference different versions of the same dependency.
/// </summary>
/// <param name="pluginPath">
/// The full path to the plugin DLL. The <see cref="AssemblyDependencyResolver"/>
/// uses this path to locate sibling dependency assemblies.
/// </param>
internal sealed class PluginLoadContext(string pluginPath) : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

    /// <summary>
    /// Resolves the managed assembly by probing the plugin's own dependency graph first.
    /// Returns <c>null</c> to fall back to the default context when the assembly is not
    /// part of the plugin (e.g., shared framework assemblies).
    /// </summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    /// <summary>
    /// Resolves a native (unmanaged) library by probing the plugin's dependency graph.
    /// Returns <see cref="nint.Zero"/> to fall back to the OS search when not found.
    /// </summary>
    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : nint.Zero;
    }
}
