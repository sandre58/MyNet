// -----------------------------------------------------------------------
// <copyright file="ResourceDatasetLoader.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Globalization;
using System.Resources;

namespace MyNet.Fakers.Localization;

/// <summary>
/// Helper class for loading and caching resource datasets from semicolon-separated strings.
/// </summary>
internal static class ResourceDatasetLoader
{
    private static readonly ConcurrentDictionary<ResourceKey, ImmutableArray<string>> Cache = [];

    /// <summary>
    /// Loads a localized string list from a RESX resource.
    /// </summary>
    /// <param name="resourceManager">The resource manager to load from.</param>
    /// <param name="key">The resource key.</param>
    /// <param name="culture">The target culture.</param>
    /// <returns>A cached read-only string list.</returns>
    public static ImmutableArray<string> LoadList(ResourceManager resourceManager, string key, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(resourceManager);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(culture);

        var cacheKey = new ResourceKey(resourceManager, key, culture.Name);

        return Cache.GetOrAdd(cacheKey, static x =>
        {
            var value = x.ResourceManager.GetString(x.Key, CultureInfo.GetCultureInfo(x.Culture));

            return string.IsNullOrWhiteSpace(value)
                ? []
                : [..value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
        });
    }

    /// <summary>
    /// Represents a unique key for caching resource datasets, based on the resource type, key, and culture.
    /// </summary>
    /// <param name="ResourceManager">The resource manager.</param>
    /// <param name="Key">The resource key.</param>
    /// <param name="Culture">The target culture.</param>
    private sealed record ResourceKey(ResourceManager ResourceManager, string Key, string Culture);
}
