// -----------------------------------------------------------------------
// <copyright file="BinaryOperator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
