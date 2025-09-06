// -----------------------------------------------------------------------
// <copyright file="IEnumeration.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities;

/// <summary>
/// Defines a minimal contract for enumeration-like types that expose a name, resource key and underlying value.
/// </summary>
public interface IEnumeration
{
    /// <summary>
    /// Gets the resource key used for localization of the enumeration item.
    /// </summary>
    string ResourceKey { get; }

    /// <summary>
    /// Gets the name of the enumeration item.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the underlying value of the enumeration item as an object.
    /// </summary>
    object Value { get; }
}
