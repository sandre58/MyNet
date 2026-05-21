// -----------------------------------------------------------------------
// <copyright file="MetadataAttributeBootstrapper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyNet.Utilities.Metadata;

/// <summary>
/// Provides a bootstrapper for applying metadata attributes to types and their properties. This class allows for the registration of handlers that can process specific attributes and apply the corresponding metadata to the properties of types. The Apply method can be used to scan assemblies, types, or individual properties for attributes and invoke the registered handlers to configure the metadata accordingly. This bootstrapper facilitates the dynamic configuration of metadata based on attributes, enabling various runtime behaviors based on the configured metadata information for types and their properties in an application.
/// </summary>
public static class MetadataAttributeBootstrapper
{
    private static readonly List<IMetadataAttributeHandler> Handlers = [];

    /// <summary>
    /// Registers a metadata attribute handler. This method allows you to add a handler that can process specific attributes and apply the corresponding metadata to the properties of types. The handler must implement the <see cref="IMetadataAttributeHandler"/> interface, which defines the methods for determining if the handler can process a given attribute and for applying the metadata based on the attribute. By registering handlers, you can enable the dynamic configuration of metadata based on attributes, allowing for various runtime behaviors based on the configured metadata information for types and their properties in an application.
    /// </summary>
    /// <param name="handler">The metadata attribute handler to register.</param>
    /// <exception cref="ArgumentNullException">Thrown if the handler is null.</exception>
    public static void Register(IMetadataAttributeHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        Handlers.Add(handler);
    }

    /// <summary>
    /// Applies metadata attributes from the specified assemblies. This method scans the provided assemblies for types and their properties, looking for attributes that can be processed by the registered handlers. For each property that has attributes, the method invokes the appropriate handlers to apply the corresponding metadata based on the attributes found. This allows for the dynamic configuration of metadata based on attributes across multiple assemblies, enabling various runtime behaviors based on the configured metadata information for types and their properties in an application.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for types and their properties.</param>
    public static void Apply(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            Apply(assembly);
        }
    }

    /// <summary>
    /// Applies metadata attributes from the specified assembly. This method scans the provided assembly for types and their properties, looking for attributes that can be processed by the registered handlers. For each property that has attributes, the method invokes the appropriate handlers to apply the corresponding metadata based on the attributes found. This allows for the dynamic configuration of metadata based on attributes within a specific assembly, enabling various runtime behaviors based on the configured metadata information for types and their properties in an application.
    /// </summary>
    /// <param name="assembly">The assembly to scan for types and their properties.</param>
    /// <exception cref="ArgumentNullException">Thrown if the assembly is null.</exception>
    public static void Apply(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var type in assembly.GetTypes())
        {
            Apply(type);
        }
    }

    /// <summary>
    /// Applies metadata attributes from the specified type. This method scans the provided type for its properties, looking for attributes that can be processed by the registered handlers. For each property that has attributes, the method invokes the appropriate handlers to apply the corresponding metadata based on the attributes found. This allows for the dynamic configuration of metadata based on attributes for a specific type, enabling various runtime behaviors based on the configured metadata information for the properties of that type in an application.
    /// </summary>
    /// <param name="type">The type to scan for properties and their attributes.</param>
    /// <exception cref="ArgumentNullException">Thrown if the type is null.</exception>
    public static void Apply(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var typeMetadata = MetadataRegistry.Get(type);

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var propertyMetadata = typeMetadata.GetProperty(property.Name);

            var attributes = property.GetCustomAttributes(true).OfType<Attribute>();

            foreach (var attribute in attributes)
            {
                foreach (var handler in Handlers.Where(handler => handler.CanHandle(attribute)))
                {
                    handler.Apply(attribute, propertyMetadata, property);
                }
            }
        }
    }
}
