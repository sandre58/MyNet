// -----------------------------------------------------------------------
// <copyright file="GeneratedMetadataBootstrapInvoker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyNet.Metadata;

/// <summary>
/// Invokes assembly-generated <c>ObservableMetadataBootstrap.Ensure</c> on first metadata access (lazy configuration).
/// </summary>
internal static class GeneratedMetadataBootstrapInvoker
{
    private const string BootstrapTypeFullName = "MyNet.Metadata.Generated.ObservableMetadataBootstrap";

    private static readonly ConcurrentDictionary<Assembly, Action<Type>?> Invokers = new();

    /// <summary>
    /// Applies generated metadata configuration for <paramref name="type"/> when present in its assembly.
    /// </summary>
    public static void Ensure(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var invoker = Invokers.GetOrAdd(type.Assembly, CreateInvoker);
        invoker?.Invoke(type);
    }

    private static Action<Type>? CreateInvoker(Assembly assembly)
    {
        var bootstrapType = assembly.GetType(BootstrapTypeFullName, throwOnError: false);

        var ensureMethod = bootstrapType?.GetMethod(
            "Ensure",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(Type)],
            modifiers: null);

        return ensureMethod is null ? null : (type => ensureMethod.Invoke(null, [type]));
    }
}
