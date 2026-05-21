// -----------------------------------------------------------------------
// <copyright file="TypeMetadata.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Utilities.Metadata;

/// <summary>
/// Represents metadata associated with a type. This class is used to store and manage metadata for a specific type, allowing for the organization and retrieval of type-specific metadata information. The metadata can include various attributes, configurations, or any other relevant information that needs to be associated with a type. This class provides methods to access property metadata for the type, enabling flexible and extensible metadata management for types in an application. By using this class, developers can easily define and manage metadata for types, allowing for various runtime behaviors based on the configured metadata information for the type and its properties in an application.
/// </summary>
public sealed class TypeMetadata
{
    /// <summary>
    /// Gets the dictionary of property metadata for the type. This dictionary allows for the storage and retrieval of metadata information associated with each property of the type. The keys in the dictionary are the names of the properties, and the values are instances of <see cref="PropertyMetadata"/> that contain the metadata information for each respective property. This structure enables organized management of property-specific metadata within the context of the type's overall metadata, allowing for various runtime behaviors based on the configured metadata information for each property in an application.
    /// </summary>
    public Dictionary<string, PropertyMetadata> Properties { get; } = [];

    /// <summary>
    /// Gets the metadata for the specified property name. This method checks if there is metadata associated with the property name in the Properties dictionary. If such metadata exists, it is returned. If no metadata for the specified property name is found, a new instance of <see cref="PropertyMetadata"/> is created, added to the Properties dictionary under the specified property name, and then returned. This allows for convenient retrieval of property metadata, ensuring that there is always a valid instance of <see cref="PropertyMetadata"/> available for any property name that is accessed, enabling various runtime behaviors based on the configured metadata information for properties in an application.
    /// </summary>
    /// <param name="name">The name of the property for which to retrieve metadata.</param>
    /// <returns>The <see cref="PropertyMetadata"/> instance associated with the specified property name.</returns>
    public PropertyMetadata GetProperty(string name)
    {
        if (!Properties.TryGetValue(name, out var meta))
        {
            meta = new();
            Properties[name] = meta;
        }

        return meta;
    }
}
