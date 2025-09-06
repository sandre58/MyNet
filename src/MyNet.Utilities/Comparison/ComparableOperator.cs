// -----------------------------------------------------------------------
// <copyright file="ComparableOperator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Comparison;

/// <summary>
/// Represents comparison operators for comparable values.
/// </summary>
public enum ComparableOperator
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
    GreaterEqualThan
}
