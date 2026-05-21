// -----------------------------------------------------------------------
// <copyright file="IgnoreModificationTrackingAttributeHandler.cs" company="Stéphane ANDRE">
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
/// Provides a handler for the IgnoreModificationTrackingAttribute, which is used to configure a property to ignore modification tracking. When this attribute is applied to a property, the handler will set the Ignore property of the ModificationTrackingFeature to true for that property, indicating that changes to the property should not be tracked or trigger any modification-related behaviors in the application. This is particularly useful for properties that do not require tracking of changes, allowing for improved performance and reduced overhead when working with modification tracking in an application.
/// </summary>
public sealed class IgnoreModificationTrackingAttributeHandler : IMetadataAttributeHandler
{
    /// <summary>
    /// Determines whether this handler can process the specified attribute. This method checks if the provided attribute is of type <see cref="IgnoreModificationTrackingAttribute"/>, which indicates that it is intended to configure a property to ignore modification tracking. If the attribute is of the correct type, the method returns true, indicating that this handler can process it; otherwise, it returns false. This allows for proper routing of attributes to their corresponding handlers based on the attribute type in an application that uses metadata for configuration and behavior management.
    /// </summary>
    /// <param name="attribute">The attribute to check.</param>
    /// <returns>True if the handler can process the attribute; otherwise, false.</returns>
    public bool CanHandle(Attribute attribute) => attribute is IgnoreModificationTrackingAttribute;

    /// <summary>
    /// Applies the effects of the <see cref="IgnoreModificationTrackingAttribute"/> to the property metadata. This method sets the Ignore property of the associated <see cref="ModificationTrackingFeature"/> to true, indicating that changes to the property should not be tracked or trigger any modification-related behaviors.
    /// </summary>
    /// <param name="attribute">The attribute to apply.</param>
    /// <param name="metadata">The property metadata to update.</param>
    /// <param name="member">The member information of the property.</param>
    public void Apply(Attribute attribute, PropertyMetadata metadata, MemberInfo member) => metadata.GetOrCreate<ModificationTrackingFeature>().Ignore = true;
}
