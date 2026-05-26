// -----------------------------------------------------------------------
// <copyright file="TypeMetadataExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#pragma warning disable IDE0130
namespace MyNet.Metadata;
#pragma warning restore IDE0130

/// <summary>
/// Query extensions for <see cref="TypeMetadata"/> (runtime consumption, not fluent authoring).
/// </summary>
public static class TypeMetadataExtensions
{
    extension(TypeMetadata metadata)
    {
        /// <summary>
        /// Gets the names of properties that have a feature of the specified type.
        /// </summary>
        public string[] WithFeature<TFeature>(Func<TFeature, bool>? predicate = null)
            where TFeature : class
            => [.. metadata.Properties.Where(p => p.Value.TryGetFeature<TFeature>(out var feature) && (predicate is null || predicate(feature)))
                .Select(p => p.Key)];

        /// <summary>
        /// Tries to get a feature for the specified property.
        /// </summary>
        public bool TryGetFeature<TFeature>(string propertyName, [NotNullWhen(true)] out TFeature? feature)
            where TFeature : class
        {
            feature = null;

            return metadata.Properties.TryGetValue(propertyName, out var property) && property.TryGetFeature(out feature);
        }

        /// <summary>
        /// Gets a feature for the specified property, or <c>null</c> when missing.
        /// </summary>
        public TFeature? GetFeatureOrDefault<TFeature>(string propertyName)
            where TFeature : class
            => metadata.TryGetFeature<TFeature>(propertyName, out var feature) ? feature : null;
    }
}
