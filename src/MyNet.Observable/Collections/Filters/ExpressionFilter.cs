// -----------------------------------------------------------------------
// <copyright file="ExpressionFilter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Filters;

/// <summary>
/// Represents a filter that uses an expression to define the filtering criteria. The filter is based on a property of type TProperty of the items being filtered, and a predicate function that determines whether an item matches the filter criteria based on the value of that property.
/// </summary>
/// <typeparam name="T">The type of items to be filtered.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ExpressionFilter{T}"/> class with the specified expression and predicate. The filter will apply the predicate to the value of the property specified by the expression for each item of type T. The property name is extracted from the expression and used to identify the filter when applied to collections.
/// </remarks>
/// <param name="expression">The expression that specifies the property to filter on.</param>
public class ExpressionFilter<T>(Expression<Func<T, bool>> expression) : IFilter<T>
{
    /// <summary>
    /// Gets the name of the property being filtered, which is extracted from the body of the expression. The property name is used to identify the filter when applied to collections, allowing for dynamic filtering based on the specified property.
    /// </summary>
    /// <returns>The expression that specifies the property to filter on.</returns>
    public Expression<Func<T, bool>> ProvideExpression() => expression;
}
