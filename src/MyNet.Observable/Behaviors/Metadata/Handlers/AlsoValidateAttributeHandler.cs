// -----------------------------------------------------------------------
// <copyright file="AlsoValidateAttributeHandler.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;
using MyNet.Observable.Behaviors.Metadata.Attributes;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors.Metadata.Handlers;

/// <summary>
/// Represents a handler for the <see cref="AlsoValidateAttribute"/> that processes the attribute and updates the associated metadata for a property. This handler checks if the attribute can be handled and applies the necessary changes to the property metadata based on the information provided by the <see cref="AlsoValidateAttribute"/>. Specifically, it adds the dependent property name specified in the attribute to the list of dependents in the <see cref="ValidationDependencyFeature"/> associated with the property metadata. This allows for proper validation dependencies to be established between properties in an application, ensuring that when one property is validated, any dependent properties are also validated as needed based on the configured metadata information for those properties in an application.
/// </summary>
public sealed class AlsoValidateAttributeHandler : IMetadataAttributeHandler
{
    /// <summary>
    /// Determines whether this handler can process the specified attribute. This method checks if the provided attribute is of type <see cref="AlsoValidateAttribute"/>. If the attribute is of the correct type, the method returns true, indicating that this handler can process the attribute. Otherwise, it returns false, indicating that this handler cannot process the attribute. This allows for proper routing of attributes to their corresponding handlers based on the type of the attribute in an application.
    /// </summary>
    /// <param name="attribute">The attribute to check.</param>
    /// <returns>True if the attribute can be handled; otherwise, false.</returns>
    public bool CanHandle(Attribute attribute) => attribute is AlsoValidateAttribute;

    /// <summary>
    /// Applies the specified attribute to the property metadata. This method takes the provided attribute, casts it to <see cref="AlsoValidateAttribute"/>, and then updates the property metadata by adding the dependent property name specified in the attribute to the list of dependents in the <see cref="ValidationDependencyFeature"/> associated with the property metadata. This establishes a validation dependency between properties, ensuring that when one property is validated, any dependent properties are also validated as needed based on the configured metadata information for those properties in an application.
    /// </summary>
    /// <param name="attribute">The attribute to apply.</param>
    /// <param name="metadata">The property metadata to update.</param>
    /// <param name="member">The member information associated with the property.</param>
    public void Apply(Attribute attribute, PropertyMetadata metadata, MemberInfo member)
    {
        var alsoValidateAttribute = (AlsoValidateAttribute)attribute;

        var feature = metadata.GetOrCreate<ValidationDependencyFeature>();
        feature.Dependents.Add(alsoValidateAttribute.PropertyName);
    }
}
