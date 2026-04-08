// -----------------------------------------------------------------------
// <copyright file="CountryExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MyNet.Utilities.Geography.Extensions;

public static class CountryExtensions
{
    private static readonly Assembly Assembly = typeof(CountryExtensions).Assembly;

    private static readonly Lazy<IReadOnlyDictionary<string, string>> FlagResourceIndex = new(() =>
        Assembly.GetManifestResourceNames()
            .Where(static n => n.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                static n => n[n.LastIndexOf('.', n.Length - 5)..^4].TrimStart('.'),
                StringComparer.OrdinalIgnoreCase));

    public static byte[]? GetFlag(this Country country, FlagSize size = FlagSize.Pixel32)
    {
        var key = $"{country.Alpha2}{(int)size}";
        if (!FlagResourceIndex.Value.TryGetValue(key, out var resourceName)) return null;

        using var stream = Assembly.GetManifestResourceStream(resourceName);
        if (stream is null) return null;

        var bytes = new byte[stream.Length];
        stream.ReadExactly(bytes);
        return bytes;
    }

    public static string GetDisplayName(this Country country)
        => CountryResources.ResourceManager.GetString(country.Name, CultureInfo.CurrentCulture).OrEmpty();
}
