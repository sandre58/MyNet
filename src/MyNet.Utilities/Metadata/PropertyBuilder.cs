// -----------------------------------------------------------------------
// <copyright file="PropertyBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Metadata;

/// <summary>
/// Provides a builder for configuring metadata for a specific property of a type. This class allows for the fluent configuration of metadata for a property, enabling developers to easily define and manage metadata information associated with a specific property of a type. The Feature method is used to specify a feature or attribute that should be associated with the property, allowing for various runtime behaviors based on the configured metadata information for that property in an application. This builder pattern allows for a clear and concise way to define metadata for properties in a type-safe manner, improving code readability and maintainability when working with metadata in an application.
/// </summary>
/// <param name="metadata">The metadata associated with the property being configured.</param>
/// <typeparam name="T">The type that owns the property being configured.</typeparam>
/// <typeparam name="TProperty">The type of the property being configured.</typeparam>
public sealed class PropertyBuilder<T, TProperty>(PropertyMetadata metadata)
{
    /// <summary>
    /// Gets the underlying property metadata being configured.
    /// </summary>
    public PropertyMetadata Metadata => metadata;

    /// <summary>
    /// Specifies a feature or attribute that should be associated with the property being configured. This method allows you to define additional metadata information for the property by associating a specific feature or attribute with it. The configure parameter is an action that takes an instance of the feature type, allowing you to configure the properties or settings of the feature as needed. By calling this method, you can enhance the metadata for the property with additional information that can be used at runtime to influence behavior based on the configured metadata information for that property in an application.
    /// </summary>
    /// <param name="configure">An action that configures the feature for the property.</param>
    /// <typeparam name="TFeature">The type of the feature to associate with the property.</typeparam>
    /// <returns>The current <see cref="PropertyBuilder{T, TProperty}"/> instance for fluent configuration.</returns>
    public PropertyBuilder<T, TProperty> Feature<TFeature>(Action<TFeature> configure)
        where TFeature : class, new()
    {
        var feature = metadata.GetOrCreate<TFeature>();
        configure(feature);
        return this;
    }
}
