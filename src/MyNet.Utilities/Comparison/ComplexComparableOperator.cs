// -----------------------------------------------------------------------
// <copyright file="ComplexComparableOperator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Comparison;

/// <summary>
/// Represents comparison operators for comparable values including range operators.
/// </summary>
public enum ComplexComparableOperator
{
    /// <summary>
    /// Indicates equality.
    /// </summary>
    EqualsTo,

    /// <summary>
    /// Indicates inequality.
    /// </summary>
    NotEqualsTo,

    /// <summary>
    /// Indicates less-than comparison.
    /// </summary>
    LessThan,

    /// <summary>
    /// Indicates greater-than comparison.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Indicates less-than-or-equal comparison.
    /// </summary>
    LessEqualThan,

    /// <summary>
    /// Indicates greater-than-or-equal comparison.
    /// </summary>
    GreaterEqualThan,

    /// <summary>
    /// Indicates that a value falls between two bounds (inclusive or per consumer semantics).
    /// </summary>
    IsBetween,

    /// <summary>
    /// Indicates that a value does not fall between two bounds.
    /// </summary>
    IsNotBetween
}
