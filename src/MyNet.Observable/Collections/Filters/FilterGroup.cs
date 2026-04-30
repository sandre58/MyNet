// -----------------------------------------------------------------------
// <copyright file="FilterGroup.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MyNet.Utilities.Comparison;

namespace MyNet.Observable.Collections.Filters;

/// <summary>
/// Represents a group of filter nodes combined with a logical operator (AND/OR).
/// </summary>
/// <param name="operator">The logical operator to combine the filter nodes.</param>
/// <param name="children">The child filter nodes to be combined.</param>
/// <typeparam name="T">The type of items to be filtered.</typeparam>
public sealed class FilterGroup<T>(LogicalOperator @operator, IEnumerable<IFilter<T>> children) : IFilter<T>
{
    /// <summary>
    /// Gets the logical operator used to combine the child filter nodes.
    /// </summary>
    public LogicalOperator Operator { get; } = @operator;

    /// <summary>
    /// Gets the child filter nodes that are combined using the specified logical operator.
    /// </summary>
    public IReadOnlyList<IFilter<T>> Children { get; } = [.. children];

    /// <summary>
    /// Converts the filter group into a single expression that can be used to evaluate whether an item of type T matches the combined filter criteria. The resulting expression will represent the logical combination of all child filter nodes according to the specified operator (AND/OR). If there are no child nodes, the expression will always return true, effectively matching all items.
    /// </summary>
    /// <returns>An expression representing the combined filter criteria.</returns>
    /// <exception cref="NotSupportedException">Thrown when an unsupported logical operator is encountered.</exception>
    public Expression<Func<T, bool>> ProvideExpression()
    {
        if (Children.Count == 0)
            return _ => true;

        var param = Expression.Parameter(typeof(T), "x");

        var expressions = Children
            .Select(c => ReplaceParameter(c.ProvideExpression(), param))
            .ToList();

        var body = expressions[0];

        for (var i = 1; i < expressions.Count; i++)
        {
            body = Operator switch
            {
                LogicalOperator.And => Expression.AndAlso(body, expressions[i]),
                LogicalOperator.Or => Expression.OrElse(body, expressions[i]),
                _ => throw new NotSupportedException()
            };
        }

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    /// <summary>
    /// Replaces the parameter in the given expression with a new parameter. This is necessary to ensure that all expressions in the filter group use the same parameter when combining them into a single expression. The method uses an expression visitor to traverse the expression tree and replace occurrences of the old parameter with the new parameter, allowing for seamless combination of multiple filter expressions into one.
    /// </summary>
    /// <param name="expression">The expression in which to replace the parameter.</param>
    /// <param name="newParam">The new parameter to use in the expression.</param>
    /// <returns>The expression with the parameter replaced.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the expression body is null.</exception>
    private static Expression ReplaceParameter(Expression<Func<T, bool>> expression, ParameterExpression newParam)
    {
        var visitor = new ReplaceParameterVisitor(expression.Parameters[0], newParam);
        return visitor.Visit(expression.Body) ?? throw new InvalidOperationException("Expression body cannot be null.");
    }

    /// <summary>
    /// An expression visitor that replaces a specified parameter in an expression tree with a new parameter. This visitor is used to ensure that all expressions in a filter group use the same parameter when combining them into a single expression. By visiting each node in the expression tree, the visitor checks for occurrences of the old parameter and replaces them with the new parameter, allowing for consistent parameter usage across all combined expressions.
    /// </summary>
    /// <param name="oldParam">The parameter to be replaced.</param>
    /// <param name="newParam">The parameter to replace with.</param>
    private sealed class ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
    {
        /// <summary>
        /// Visits the <see cref="ParameterExpression"/> nodes in the expression tree and replaces occurrences of the old parameter with the new parameter. If the current node is the old parameter, it returns the new parameter; otherwise, it continues visiting the expression tree as usual. This method ensures that all expressions in a filter group use the same parameter when they are combined into a single expression, allowing for seamless integration of multiple filter expressions.
        /// </summary>
        /// <param name="node">The parameter expression node to visit.</param>
        /// <returns>The modified expression node.</returns>
        protected override Expression VisitParameter(ParameterExpression node) => node == oldParam ? newParam : base.VisitParameter(node);
    }
}
