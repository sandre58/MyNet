// -----------------------------------------------------------------------
// <copyright file="StringOperator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Represents string comparison operators used by string-based predicates.
/// </summary>
public enum StringOperator
{
    /// <summary>
    /// Indicates exact match.
    /// </summary>
    Is,

    /// <summary>
    /// Indicates negated exact match.
    /// </summary>
    IsNot,

    /// <summary>
    /// Indicates the value starts with the specified substring.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Indicates the value ends with the specified substring.
    /// </summary>
    EndsWith,

    /// <summary>
    /// Indicates the value contains the specified substring.
    /// </summary>
    Contains
}
