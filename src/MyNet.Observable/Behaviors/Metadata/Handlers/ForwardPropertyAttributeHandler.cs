// -----------------------------------------------------------------------
// <copyright file="ForwardPropertyAttributeHandler.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;
using MyNet.Observable.Behaviors.Metadata.Attributes;
using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors.Metadata.Handlers;

/// <summary>
/// Handles <see cref="ForwardPropertyAttribute"/> by adding forwarding behavior metadata.
/// </summary>
public sealed class ForwardPropertyAttributeHandler : IMetadataAttributeHandler
{
    /// <inheritdoc />
    public bool CanHandle(Attribute attribute) => attribute is ForwardPropertyAttribute;

    /// <inheritdoc />
    public void Apply(Attribute attribute, PropertyMetadata metadata, MemberInfo member)
    {
        if (member.DeclaringType is null)
            return;

        var forward = (ForwardPropertyAttribute)attribute;
        GeneratedPropertyBehaviorRegistry.RegisterForwardProperty(member.DeclaringType, member.Name, forward.ConcatenatePropertyName);
    }
}
