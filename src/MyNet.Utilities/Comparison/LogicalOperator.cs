// -----------------------------------------------------------------------
// <copyright file="LogicalOperator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Represents logical operators used to combine comparison predicates.
/// </summary>
public enum LogicalOperator
{
    /// <summary>
    /// Logical AND operation.
    /// </summary>
    And,

    /// <summary>
    /// Logical OR operation.
    /// </summary>
    Or
}
