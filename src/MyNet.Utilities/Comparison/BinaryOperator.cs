// -----------------------------------------------------------------------
// <copyright file="BinaryOperator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Comparison;

/// <summary>
/// Represents a binary comparison operator for boolean-like comparisons.
/// </summary>
public enum BinaryOperator
{
    /// <summary>
    /// Indicates equality or positive match.
    /// </summary>
    Is,

    /// <summary>
    /// Indicates inequality or negative match.
    /// </summary>
    IsNot
}
