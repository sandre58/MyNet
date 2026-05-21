// -----------------------------------------------------------------------
// <copyright file="AlsoValidateAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Indicates that the specified property must be validated at the same time as this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class AlsoValidateAttribute(string propertyName) : Attribute
{
    /// <summary>
    /// Gets the name of the property that must be validated together with this property.
    /// </summary>
    public string PropertyName { get; } = propertyName;
}
