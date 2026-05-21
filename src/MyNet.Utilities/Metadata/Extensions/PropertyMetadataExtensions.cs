// -----------------------------------------------------------------------
// <copyright file="PropertyMetadataExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class PropertyMetadataExtensions
{
    extension(PropertyMetadata metadata)
    {
        /// <summary>
        /// Checks if the property metadata has a specific feature associated with it. This method allows you to determine if a particular feature or attribute is present in the metadata for the property. By specifying the type of the feature you want to check for, you can easily verify if that feature is associated with the property metadata, enabling you to make decisions based on the presence or absence of specific features in the metadata for that property in an application.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the feature based on specific conditions.</param>
        /// <typeparam name="TFeature">The type of the feature to check for.</typeparam>
        /// <returns>true if the feature of the specified type is associated with the property; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the metadata parameter is null.</exception>
        public bool HasFeature<TFeature>(Func<TFeature, bool>? predicate = null)
            where TFeature : class
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return metadata.TryGetFeature<TFeature>(out var feature) && (predicate == null || predicate(feature));
        }

        /// <summary>
        /// Gets the feature of the specified type associated with the property metadata, or returns null if it does not exist. This method allows you to retrieve a specific feature or attribute from the property metadata. By specifying the type of the feature you want to retrieve, you can obtain the associated feature if it exists, or receive null if there is no such feature associated with the property metadata. This provides a convenient way to access specific features in the metadata for a property, enabling various runtime behaviors based on the presence or absence of specific features in an application.
        /// </summary>
        /// <typeparam name="TFeature">The type of the feature to get.</typeparam>
        /// <returns>The feature of the specified type associated with the property, or null if it does not exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the metadata parameter is null.</exception>
        public TFeature? GetFeatureOrDefault<TFeature>()
            where TFeature : class
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return metadata.TryGetFeature<TFeature>(out var feature)
                ? feature
                : null;
        }
    }
}
