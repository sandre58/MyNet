// -----------------------------------------------------------------------
// <copyright file="MetadataRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyNet.Utilities.Metadata;

/// <summary>
/// Provides a central registry for managing metadata associated with types. This static class allows for the configuration and retrieval of metadata for specific types, enabling developers to define and manage metadata information in a centralized manner. The For method is used to configure metadata for a specific type, returning a builder that allows for fluent configuration of the metadata. The Get method retrieves the metadata associated with a given type, allowing for access to the configured metadata information at runtime. By using this class, developers can easily manage and access metadata for types in an application, enabling various runtime behaviors based on the configured metadata information for those types and their properties in an application.
/// </summary>
public static class MetadataRegistry
{
    private static readonly ConcurrentDictionary<Type, TypeMetadata> Cache = [];

    /// <summary>
    /// Configures metadata for a specific type. This method takes a generic type parameter T, which represents the type for which metadata is being configured. If there is no existing metadata for the specified type in the cache, a new instance of <see cref="TypeMetadata"/> is created and added to the cache for that type. The method then returns a <see cref="MetadataBuilder{T}"/> that can be used to further configure the metadata for the specified type, allowing for various runtime behaviors based on the configured metadata information for that type and its properties in an application.
    /// </summary>
    /// <typeparam name="T">The type for which metadata is being configured.</typeparam>
    /// <returns>A <see cref="MetadataBuilder{T}"/> that can be used to further configure the metadata for the specified type.</returns>
    public static MetadataBuilder<T> For<T>()
    {
        var metadata = Get(typeof(T));
        return new(metadata);
    }

    /// <summary>
    /// Retrieves the metadata associated with a given type. This method takes a Type object as a parameter and returns the corresponding <see cref="TypeMetadata"/> instance from the cache. If there is no metadata associated with the specified type in the cache, this method will throw a KeyNotFoundException. This allows for access to the configured metadata information for a type at runtime, enabling various runtime behaviors based on the configured metadata information for that type and its properties in an application.
    /// </summary>
    /// <param name="type">The type for which metadata is being retrieved.</param>
    /// <returns>The <see cref="TypeMetadata"/> associated with the specified type.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if there is no metadata associated with the specified type in the cache.</exception>
    public static TypeMetadata Get(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        GeneratedMetadataBootstrapInvoker.Ensure(type);

        return GetOrCreate(type);
    }

    /// <summary>
    /// Retrieves the metadata associated with a given type, or creates a new instance if it does not exist. This method takes a Type object as a parameter and returns the corresponding <see cref="TypeMetadata"/> instance from the cache. If there is no metadata associated with the specified type in the cache, a new instance of <see cref="TypeMetadata"/> is created, added to the cache for that type, and then returned. This allows for convenient retrieval of metadata information for a type at runtime, ensuring that there is always a valid instance of <see cref="TypeMetadata"/> available for any type that is accessed, enabling various runtime behaviors based on the configured metadata information for that type and its properties in an application.
    /// </summary>
    /// <param name="type">The type for which metadata is being retrieved or created.</param>
    /// <returns>The <see cref="TypeMetadata"/> associated with the specified type.</returns>
    private static TypeMetadata GetOrCreate(Type type) => Cache.GetOrAdd(type, static _ => new());
}
