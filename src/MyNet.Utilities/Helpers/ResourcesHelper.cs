// -----------------------------------------------------------------------
// <copyright file="ResourcesHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;

namespace MyNet.Utilities.Helpers;

/// <summary>
/// Helper class for working with embedded resources in assemblies.
/// </summary>
public static class ResourcesHelper
{
    /// <summary>
    /// Opens an embedded resource stream from the specified assembly based on a resource name suffix.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resource.</param>
    /// <param name="resourceNameSuffix">The suffix of the resource name to match.</param>
    /// <param name="comparison">The string comparison type to use when matching the resource name.</param>
    /// <returns>A stream representing the embedded resource, or null if not found.</returns>
    public static Stream OpenEmbeddedResource(
        Assembly assembly,
        string resourceNameSuffix,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceNameSuffix);

        string? match = null;

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.EndsWith(resourceNameSuffix, comparison))
            {
                continue;
            }

            if (match is not null)
            {
                throw new InvalidOperationException(
                    $"Multiple embedded resources match '{resourceNameSuffix}'.");
            }

            match = resourceName;
        }

        return match is null
            ? throw new InvalidOperationException(
                $"Embedded resource '{resourceNameSuffix}' not found.")
            : assembly.GetManifestResourceStream(match)!;
    }
}
