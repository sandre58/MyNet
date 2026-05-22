// -----------------------------------------------------------------------
// <copyright file="PropertyMetadata.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyNet.Utilities.Metadata;

/// <summary>
/// Represents metadata associated with a property. This class is used to store and manage metadata for a specific property, allowing for the organization and retrieval of property-specific metadata information. The metadata can include various attributes, configurations, or any other relevant information that needs to be associated with a property. This class provides methods to set, get, and create metadata for the property, enabling flexible and extensible metadata management for properties in an application.
/// </summary>
public sealed class PropertyMetadata
{
    private readonly ConcurrentDictionary<Type, object> _features = new();

    /// <summary>
    /// Sets the feature for the specified type. This method allows you to associate a specific piece of metadata (referred to as a "feature") with the property represented by this instance of <see cref="PropertyMetadata"/>. The feature is stored in a dictionary, where the key is the type of the feature and the value is the feature object itself. By calling this method, you can add or update feature information for the property, which can then be retrieved later using the corresponding get methods. This provides a flexible way to manage and organize metadata features for properties in an application, allowing for various runtime behaviors based on the configured feature information for the property.
    /// </summary>
    /// <param name="feature">The feature to set for the specified type.</param>
    /// <typeparam name="TFeature">The type of the feature.</typeparam>
    public void SetFeature<TFeature>(TFeature feature)
        where TFeature : class => _features[typeof(TFeature)] = feature;

    /// <summary>
    /// Tries to get the feature of the specified type. This method checks if there is a feature associated with the property represented by this instance of <see cref="PropertyMetadata"/> for the specified feature type. If such a feature exists, it is returned through the out parameter and the method returns true. If no feature of the specified type is found, the out parameter is set to null and the method returns false. This allows for safe retrieval of feature information for a property, enabling various runtime behaviors based on the presence or absence of specific features for the property.
    /// </summary>
    /// <param name="feature">When this method returns, contains the feature of the specified type if it exists; otherwise, null.</param>
    /// <typeparam name="TFeature">The type of the feature to retrieve.</typeparam>
    /// <returns>true if the feature of the specified type exists; otherwise, false.</returns>
    public bool TryGetFeature<TFeature>([NotNullWhen(true)] out TFeature? feature)
        where TFeature : class
    {
        if (_features.TryGetValue(typeof(TFeature), out var value))
        {
            feature = (TFeature)value;
            return true;
        }

        feature = null;
        return false;
    }

    /// <summary>
    /// Gets the feature of the specified type, or creates a new instance if it does not exist. This method checks if there is a feature associated with the property represented by this instance of <see cref="PropertyMetadata"/> for the specified feature type. If such a feature exists, it is returned. If no feature of the specified type is found, a new instance of the feature type is created, associated with the property, and then returned. This allows for convenient retrieval of feature information for a property, ensuring that there is always a valid instance of the specified feature type available for use in various runtime behaviors based on the configured feature information for the property.
    /// </summary>
    /// <typeparam name="TFeature">The type of the feature to retrieve or create.</typeparam>
    /// <returns>The existing or newly created feature of the specified type.</returns>
    public TFeature GetOrCreate<TFeature>()
        where TFeature : class, new()
        => (TFeature)_features.GetOrAdd(typeof(TFeature), static _ => new TFeature());

    /// <summary>
    /// Returns an enumerable collection of key-value pairs representing the features associated with this property metadata. Each key in the collection is a Type that represents the type of the feature, and each value is the corresponding feature object associated with that type. This method allows you to retrieve all the features that have been set for this property metadata, enabling you to inspect or utilize the features as needed in various runtime behaviors based on the configured feature information for the property.
    /// </summary>
    /// <returns>An enumerable collection of key-value pairs representing the features associated with this property metadata.</returns>
    internal IEnumerable<KeyValuePair<Type, object>> FeaturesSnapshot() => _features;
}
