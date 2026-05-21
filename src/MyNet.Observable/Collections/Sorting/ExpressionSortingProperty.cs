// -----------------------------------------------------------------------
// <copyright file="ExpressionSortingProperty.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Sorting;

/// <summary>
/// Represents a sorting property defined by an expression, along with the sorting direction.
/// </summary>
/// <param name="expression">The expression that defines the sorting property.</param>
/// <param name="direction">The direction in which to sort the items.</param>
/// <typeparam name="T">The type of the items to be sorted.</typeparam>
public sealed class ExpressionSortingProperty<T>(Expression<Func<T, object?>> expression, ListSortDirection direction = ListSortDirection.Ascending) : ISortingProperty<T>
{
    /// <summary>
    /// Gets the expression that defines the sorting property. This expression is used to determine the value of the property by which the items will be sorted. The expression should return a value that can be compared for sorting purposes, such as a property or a computed value based on the properties of T.
    /// </summary>
    public Expression<Func<T, object?>> Expression { get; } = expression;

    /// <summary>
    /// Gets the direction in which to sort the items based on this sorting property. The direction can be either ascending or descending, and it determines how the items will be ordered when sorted by this property. Ascending means that items will be ordered from smallest to largest based on the value of the sorting property, while descending means that items will be ordered from largest to smallest.
    /// </summary>
    public ListSortDirection Direction { get; } = direction;

    /// <summary>
    /// Provides the expression that defines the sorting property. This method is part of the <see cref="ISortingProperty{T}"/> interface and allows consumers to retrieve the expression used for sorting. The expression can be used to extract the value from items of type T that will be used for comparison during sorting operations.
    /// </summary>
    /// <returns>The expression that defines the sorting property.</returns>
    public Expression<Func<T, object?>> ProvideExpression() => Expression;
}
