// -----------------------------------------------------------------------
// <copyright file="EmbeddedCountryFlagProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MyNet.Text;
using MyNet.Utilities.Geography;

namespace MyNet.Geography.Providers;

internal sealed class EmbeddedCountryFlagProvider() : EmbeddedCountryFlagProviderBase(typeof(EmbeddedCountryFlagProvider).Assembly);

/// <summary>
/// Provides country flags from embedded resources in an assembly. The expected resource naming convention is "fr-32.png" for the flag of France in 32x32 pixels, "us-64.png" for the flag of the United States in 64x64 pixels, etc. The provider builds an index of available flags at initialization for efficient retrieval.
/// </summary>
internal abstract class EmbeddedCountryFlagProviderBase(Assembly assembly) : ICountryFlagProvider
{
    // key = "fr-32"
    private readonly Dictionary<string, string> _index = BuildIndex(assembly);

    /// <summary>
    /// Builds an index of available flag resources in the assembly. The index maps a key (e.g., "fr-32") to the full resource name (e.g., "MyNet.Geography.Flags.fr-32.png") for efficient retrieval when requested.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded flag resources.</param>
    /// <returns>A dictionary mapping flag keys to resource names.</returns>
    private static Dictionary<string, string> BuildIndex(Assembly assembly)
        => assembly.GetManifestResourceNames()
            .Where(static n => n.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                static n => BuildKey(n),
                static n => n,
                StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Builds a key for the flag resource based on its name. The key is derived from the file name of the resource, excluding the extension, and is expected to follow the format "fr-32" for a flag of France in 32x32 pixels. The method converts the file name to lower case to ensure case-insensitive matching when retrieving flags.
    /// </summary>
    /// <param name="resourceName">The name of the resource.</param>
    /// <returns>The key for the flag resource.</returns>
    private static string BuildKey(string resourceName)
    {
        // Manifest resource names use dots as separators (not directory separators).
        // Example: MyNet.Geography.Flags.32x32.fr32.png => key: fr32
        var withoutExtension = Path.GetFileNameWithoutExtension(resourceName);
        var lastSeparator = withoutExtension.LastIndexOf('.');
        var file = lastSeparator >= 0 ? withoutExtension[(lastSeparator + 1)..] : withoutExtension;

        // expected: fr32, us64 etc.
        return file.ToLowerCase();
    }

    /// <summary>
    /// Opens a stream to the flag image for the specified country and size. The method constructs a key based on the country's alpha-2 code and the requested flag size (e.g., "fr-32" for France in 32x32 pixels) and looks up this key in the index built at initialization. If a matching resource is found, it returns a stream to that resource; otherwise, it returns null, indicating that the requested flag is not available.
    /// </summary>
    /// <param name="country">The country for which to retrieve the flag.</param>
    /// <param name="size">The size of the flag to retrieve.</param>
    /// <returns>A stream to the flag image, or null if the flag is not available.</returns>
    public Stream? Open(Country country, FlagSize size)
    {
        var key = $"{country.Alpha2}{(int)size}".ToLowerCase();

        return !_index.TryGetValue(key, out var resource) ? null : assembly.GetManifestResourceStream(resource);
    }

    /// <summary>
    /// Gets the flag image as a byte array for the specified country and size. This method internally calls the Open method to retrieve a stream to the flag image. If the stream is successfully opened, it reads the contents of the stream into a byte array and returns it. If the stream cannot be opened (e.g., if the requested flag is not available), it returns null.
    /// </summary>
    /// <param name="country">The country for which to retrieve the flag.</param>
    /// <param name="size">The size of the flag to retrieve.</param>
    /// <returns>A byte array containing the flag image, or null if the flag is not available.</returns>
    public byte[]? GetBytes(Country country, FlagSize size)
    {
        using var stream = Open(country, size);
        if (stream is null) return null;

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
