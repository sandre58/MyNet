// -----------------------------------------------------------------------
// <copyright file="UpdateOnTimeZoneChangedAttributeHandler.cs" company="Stéphane ANDRE">
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
/// Handles the <see cref="UpdateOnTimeZoneChangedAttribute"/> by setting the UpdateOnTimeZoneChanged property of the associated <see cref="TimeZoneChangedEvent"/> to true, indicating that the property should be updated when the timeZone changes. This handler is responsible for applying the effects of the UpdateOnTimeZoneChangedAttribute to the property metadata during metadata construction in an application, ensuring that properties marked with this attribute will react appropriately to time zone changes by updating their values as needed.
/// </summary>
public sealed class UpdateOnTimeZoneChangedAttributeHandler : IMetadataAttributeHandler
{
    /// <summary>
    /// Determines whether this handler can process the specified attribute. This method checks if the provided attribute is of type <see cref="UpdateOnTimeZoneChangedAttribute"/>. If the attribute is of the correct type, the method returns true, indicating that this handler can process it. Otherwise, it returns false. This allows the metadata system to determine which handler should be used for processing specific attributes based on their types, ensuring that the appropriate logic is applied to the property metadata when attributes are encountered during metadata construction in an application.
    /// </summary>
    /// <param name="attribute">The attribute to check.</param>
    /// <returns>True if the handler can process the attribute; otherwise, false.</returns>
    public bool CanHandle(Attribute attribute) => attribute is UpdateOnTimeZoneChangedAttribute;

    /// <summary>
    /// Applies the effects of the specified attribute to the property metadata. This method is called when the metadata system encounters an attribute that this handler can process (as determined by the CanHandle method). In this implementation, it sets the UpdateOnTimeZoneChanged property of the associated <see cref="TimeZoneChangedEvent"/> to true, indicating that the property should be updated when the time zone changes. This ensures that properties marked with the <see cref="UpdateOnTimeZoneChangedAttribute"/> will react appropriately to time zone changes by updating their values as needed during runtime in an application. The method takes three parameters: the attribute being processed, the property metadata to which the attribute is applied, and the member information for the property being processed, allowing for context-aware application of the attribute's effects on the property metadata during metadata construction in an application.
    /// </summary>
    /// <param name="attribute">The attribute to apply.</param>
    /// <param name="metadata">The property metadata to update.</param>
    /// <param name="member">The member information of the property.</param>
    public void Apply(Attribute attribute, PropertyMetadata metadata, MemberInfo member) => metadata.ReactTo<TimeZoneChangedEvent>();
}
