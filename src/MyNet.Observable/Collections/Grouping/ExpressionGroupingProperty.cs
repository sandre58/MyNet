// -----------------------------------------------------------------------
// <copyright file="ExpressionGroupingProperty.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Grouping;

/// <summary>
/// Represents a grouping property defined by an expression for items of type T. This class implements the <see cref="IGroupingProperty{T}"/> interface and provides a way to specify how items should be grouped based on a property or computed value defined by the provided expression. The grouping property can be used in collection views or other data structures that support grouping to organize items based on the specified criteria.
/// </summary>
/// <param name="expression">The expression that defines the grouping property.</param>
/// <typeparam name="T">The type of the items to be grouped.</typeparam>
public sealed class ExpressionGroupingProperty<T>(Expression<Func<T, object?>> expression) : IGroupingProperty<T>
{
    /// <summary>
    /// Gets the expression that defines the grouping property. This expression is used to determine the value of the property by which the items will be grouped. The expression should return a value that can be compared for grouping purposes, such as a property or a computed value based on the properties of T.
    /// </summary>
    public Expression<Func<T, object?>> Expression { get; } = expression;

    /// <summary>
    /// Provides the expression that defines the grouping property. This method is part of the <see cref="IGroupingProperty{T}"/> interface and allows consumers to retrieve the expression used for grouping. The expression can be used to extract the value from items of type T that will be used for comparison during grouping operations.
    /// </summary>
    /// <returns>The expression that defines the grouping property.</returns>
    public Expression<Func<T, object?>> ProvideExpression() => Expression;
}
