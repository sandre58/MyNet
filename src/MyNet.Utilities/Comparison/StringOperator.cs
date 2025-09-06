// -----------------------------------------------------------------------
// <copyright file="StringOperator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Comparison;

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
