// -----------------------------------------------------------------------
// <copyright file="ExpressionEqualityComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq.Expressions;

namespace MyNet.Utilities.Comparers;

/// <summary>
/// An equality comparer for expressions that compares their string representations to determine equality.
/// </summary>
public static class ExpressionEqualityComparer
{
    /// <summary>
    /// Determines whether two expressions are equal by comparing their string representations.
    /// </summary>
    /// <param name="a">The first expression to compare.</param>
    /// <param name="b">The second expression to compare.</param>
    /// <returns>True if the expressions are equal; otherwise, false.</returns>
    public static bool Equals(Expression? a, Expression? b) => a?.ToString() == b?.ToString();
}
