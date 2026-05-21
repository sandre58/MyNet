// -----------------------------------------------------------------------
// <copyright file="IMetadataAttributeHandler.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyNet.Utilities.Metadata;

/// <summary>
/// Defines an interface for handling metadata attributes. This interface provides methods to determine if a specific attribute can be handled and to apply the attribute's effects to the property metadata. Implementations of this interface can be used to process custom attributes and modify the associated property metadata accordingly, allowing for flexible and extensible metadata management based on the attributes applied to properties in an application.
/// </summary>
public interface IMetadataAttributeHandler
{
    /// <summary>
    /// Determines whether the specified attribute can be handled by this handler. This method checks if the given attribute is of a type that this handler is designed to process. If the attribute can be handled, the method returns true; otherwise, it returns false. This allows for selective processing of attributes based on their types, enabling different handlers to manage different sets of attributes and apply their effects to property metadata as needed in an application.
    /// </summary>
    /// <param name="attribute">The attribute to check.</param>
    /// <returns>True if the attribute can be handled; otherwise, false.</returns>
    bool CanHandle(Attribute attribute);

    /// <summary>
    /// Applies the effects of the specified attribute to the given property metadata. This method is called when the attribute is determined to be handled by this handler. It allows the handler to modify the property metadata based on the attribute's information, enabling dynamic and flexible metadata management.
    /// </summary>
    /// <param name="attribute">The attribute to apply.</param>
    /// <param name="metadata">The property metadata to modify.</param>
    /// <param name="member">The member information associated with the property.</param>
    void Apply(Attribute attribute, PropertyMetadata metadata, MemberInfo member);
}
