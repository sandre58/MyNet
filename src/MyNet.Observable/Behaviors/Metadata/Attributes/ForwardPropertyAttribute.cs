// -----------------------------------------------------------------------
// <copyright file="ForwardPropertyAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Marks a property as a wrapper whose PropertyChanged notifications should be relayed
/// to the owner when a <see cref="PropertyChangedForwardingBehavior"/> is attached.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ForwardPropertyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether forwarded notifications should be emitted as
    /// "Wrapper.Property" instead of only "Property".
    /// </summary>
    public bool ConcatenatePropertyName { get; set; }
}
