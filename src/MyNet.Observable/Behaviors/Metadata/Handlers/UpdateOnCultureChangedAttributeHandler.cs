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
/// Represents a metadata attribute handler for the <see cref="UpdateOnCultureChangedAttribute"/>. This handler is responsible for processing the <see cref="UpdateOnCultureChangedAttribute"/> and applying its effects to the property metadata. When the attribute is applied to a property, this handler marks the property as reacting to <see cref="CultureChangedEvent"/> so the property is refreshed when culture changes.
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
    /// Applies the effects of the <see cref="UpdateOnCultureChangedAttribute"/> to the property metadata by registering a reaction to <see cref="CultureChangedEvent"/>.
    /// </summary>
    /// <param name="attribute">The attribute to apply.</param>
    /// <param name="metadata">The property metadata to update.</param>
    /// <param name="member">The member information of the property.</param>
    public void Apply(Attribute attribute, PropertyMetadata metadata, MemberInfo member) => metadata.ReactTo<CultureChangedEvent>();
}
