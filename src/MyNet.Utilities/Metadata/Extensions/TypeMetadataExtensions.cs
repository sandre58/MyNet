// -----------------------------------------------------------------------
// <copyright file="TypeMetadataExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MyNet.Utilities.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TypeMetadataExtensions
{
    extension(TypeMetadata metadata)
    {
        /// <summary>
        /// Tries to get the metadata for a property with the specified name. This method checks if there is metadata associated with the property name in the Properties dictionary of the type metadata. If such metadata exists, it is returned through the out parameter and the method returns true. If no metadata for the specified property name is found, the out parameter is set to null and the method returns false. This allows for convenient retrieval of property metadata while also providing a way to handle cases where no metadata is available for a given property name, enabling various runtime behaviors based on the presence or absence of configured metadata information for properties in an application.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve metadata for.</param>
        /// <param name="property">When this method returns, contains the metadata for the specified property, if found; otherwise, null.</param>
        /// <returns>true if the metadata for the specified property is found; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the metadata parameter is null.</exception>
        public bool TryGetProperty(string propertyName, [NotNullWhen(true)] out PropertyMetadata? property)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return metadata.Properties.TryGetValue(propertyName, out property);
        }

        /// <summary>
        /// Gets the metadata for a property with the specified name, or returns null if it does not exist. This method checks if there is metadata associated with the property name in the Properties dictionary of the type metadata. If such metadata exists, it is returned. If no metadata for the specified property name is found, null is returned. This allows for convenient retrieval of property metadata while also providing a way to handle cases where no metadata is available for a given property name, enabling various runtime behaviors based on the presence or absence of configured metadata information for properties in an application.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve metadata for.</param>
        /// <returns>The metadata for the specified property, if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the metadata parameter is null.</exception>
        public PropertyMetadata? GetPropertyOrDefault(string propertyName)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            metadata.Properties.TryGetValue(propertyName, out var property);

            return property;
        }

        /// <summary>
        /// Gets the names of the properties that have a feature of the specified type associated with them. This method iterates through the Properties dictionary of the type metadata and checks if each property's metadata has a feature of the specified type associated with it. If a property has the specified feature, its name is included in the resulting array. Additionally, an optional predicate can be provided to further filter the properties based on specific conditions related to the feature. This allows for convenient retrieval of property names that have certain features associated with them, enabling various runtime behaviors based on the presence or absence of specific features for properties in an application.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the properties based on the feature.</param>
        /// <typeparam name="TFeature">The type of the feature to check for.</typeparam>
        /// <returns>An array of property names that have the specified feature.</returns>
        public string[] WithFeature<TFeature>(Func<TFeature, bool>? predicate = null)
            where TFeature : class
            => [.. metadata.Properties.Where(p => p.Value.TryGetFeature<TFeature>(out var feature) && (predicate is null || predicate(feature)))
                .Select(p => p.Key)];

        /// <summary>
        /// Checks if there is a feature of the specified type associated with the property of the specified name. This method first checks if there is metadata for the property with the given name in the Properties dictionary of the type metadata. If such metadata exists, it then checks if there is a feature of the specified type associated with that property's metadata. If both conditions are met, the method returns true; otherwise, it returns false. This allows for convenient checking of the presence of specific features for properties based on their names, enabling various runtime behaviors based on the presence or absence of configured feature information for properties in an application.
        /// </summary>
        /// <param name="propertyName">The name of the property to check for the feature.</param>
        /// <param name="predicate">An optional predicate to filter the feature based on specific conditions.</param>
        /// <typeparam name="TFeature">The type of the feature to check for.</typeparam>
        /// <returns>true if the feature of the specified type is associated with the property; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the metadata parameter is null.</exception>
        public bool HasFeature<TFeature>(string propertyName, Func<TFeature, bool>? predicate = null)
            where TFeature : class
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return metadata.TryGetFeature<TFeature>(propertyName, out var feature) && (predicate is null || predicate(feature));
        }

        /// <summary>
        /// Tries to get the feature of the specified type associated with the property of the specified name. This method first checks if there is metadata for the property with the given name in the Properties dictionary of the type metadata. If such metadata exists, it then tries to get the feature of the specified type from that property's metadata. If both conditions are met and the feature is found, it is returned through the out parameter and the method returns true. If either condition is not met or the feature is not found, the out parameter is set to null and the method returns false. This allows for convenient retrieval of specific features for properties based on their names, enabling various runtime behaviors based on the presence or absence of configured feature information for properties in an application.
        /// </summary>
        /// <param name="propertyName">The name of the property to get the feature for.</param>
        /// <param name="feature">When this method returns, contains the feature of the specified type associated with the property, if found; otherwise, null.</param>
        /// <typeparam name="TFeature">The type of the feature to get.</typeparam>
        /// <returns>true if the feature of the specified type is associated with the property; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the metadata parameter is null.</exception>
        public bool TryGetFeature<TFeature>(string propertyName, [NotNullWhen(true)] out TFeature? feature)
            where TFeature : class
        {
            ArgumentNullException.ThrowIfNull(metadata);

            feature = null;

            return metadata.Properties.TryGetValue(propertyName, out var property) && property.TryGetFeature(out feature);
        }

        /// <summary>
        /// Gets the feature of the specified type associated with the property of the specified name, or returns null if it does not exist. This method first checks if there is metadata for the property with the given name in the Properties dictionary of the type metadata. If such metadata exists, it then tries to get the feature of the specified type from that property's metadata. If both conditions are met and the feature is found, it is returned; otherwise, null is returned. This allows for convenient retrieval of specific features for properties based on their names, enabling various runtime behaviors based on the presence or absence of configured feature information for properties in an application.
        /// </summary>
        /// <param name="propertyName">The name of the property to get the feature for.</param>
        /// <typeparam name="TFeature">The type of the feature to get.</typeparam>
        /// <returns>The feature of the specified type associated with the property, or null if it does not exist.</returns>
        public TFeature? GetFeatureOrDefault<TFeature>(string propertyName)
            where TFeature : class
            => metadata.TryGetFeature<TFeature>(propertyName, out var feature) ? feature : null;
    }
}
