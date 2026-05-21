// -----------------------------------------------------------------------
// <copyright file="UpdateOnCultureChangedAttributeHandler.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;
using MyNet.Observable.Behaviors.Metadata.Attributes;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors.Metadata.Handlers;

/// <summary>
/// Represents a metadata attribute handler for the <see cref="UpdateOnCultureChangedAttribute"/>. This handler is responsible for processing the <see cref="UpdateOnCultureChangedAttribute"/> and applying its effects to the property metadata. When the <see cref="UpdateOnCultureChangedAttribute"/> is applied to a property, this handler will set the UpdateOnCultureChanged property of the associated <see cref="CultureFeature"/> to true, indicating that the property should be updated when the culture changes. This allows for dynamic updates of properties based on culture changes in an application, enabling localization and internationalization features for properties that are marked with this attribute. By using this handler, developers can easily configure properties to respond to culture changes without needing to manually implement the logic for handling culture changes in their code, improving maintainability and reducing boilerplate code when working with culture-sensitive properties in an application.
/// </summary>
public sealed class UpdateOnCultureChangedAttributeHandler : IMetadataAttributeHandler
{
    /// <summary>
    /// Determines whether this handler can process the specified attribute. This method checks if the provided attribute is of type <see cref="UpdateOnCultureChangedAttribute"/>. If the attribute is of the correct type, the method returns true, indicating that this handler can process it. Otherwise, it returns false. This allows the metadata system to determine which handler should be used for processing specific attributes based on their types, ensuring that the appropriate logic is applied to the property metadata when attributes are encountered during metadata construction in an application.
    /// </summary>
    /// <param name="attribute">The attribute to check.</param>
    /// <returns>True if the handler can process the attribute; otherwise, false.</returns>
    public bool CanHandle(Attribute attribute) => attribute is UpdateOnCultureChangedAttribute;

    /// <summary>
    /// Applies the effects of the <see cref="UpdateOnCultureChangedAttribute"/> to the property metadata. This method sets the UpdateOnCultureChanged property of the associated <see cref="CultureFeature"/> to true, indicating that the property should be updated when the culture changes.
    /// </summary>
    /// <param name="attribute">The attribute to apply.</param>
    /// <param name="metadata">The property metadata to update.</param>
    /// <param name="member">The member information of the property.</param>
    public void Apply(Attribute attribute, PropertyMetadata metadata, MemberInfo member) => metadata.ReactTo<CultureChangedEvent>();
}
