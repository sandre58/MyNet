// -----------------------------------------------------------------------
// <copyright file="IgnoreMemberAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Attributes;

/// <summary>
/// Indicates that a property or field should be ignored by consumers that respect this attribute.
/// </summary>
/// <remarks>
/// This attribute can be applied to properties and fields to signal that they should be
/// excluded from processing by serializers, mappers, comparison utilities or other
/// components that honor this marker. It does not enforce any behavior by itself; the
/// behavior depends on the consumer implementation.
/// </remarks>
/// <example>
/// <code>
/// public class User
/// {
///     public string Name { get; set; }
///
///     [IgnoreMember]
///     public string Password { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class IgnoreMemberAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoreMemberAttribute"/> class.
    /// </summary>
    public IgnoreMemberAttribute()
    {
    }
}
