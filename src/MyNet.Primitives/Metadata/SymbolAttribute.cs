// -----------------------------------------------------------------------
// <copyright file="SymbolAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Primitives.Metadata;

/// <summary>
/// Specifies a symbolic representation for a type member or property.
/// Typical examples include SI units ("kg", "cm"), currencies ("€"),
/// or technical shorthand not intended for localization.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class SymbolAttribute(string value) : Attribute
{
    /// <summary>
    /// Gets the symbolic representation.
    /// </summary>
    public string Value { get; } = value;
}
