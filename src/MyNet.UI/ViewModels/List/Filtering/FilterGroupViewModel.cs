// -----------------------------------------------------------------------
// <copyright file="FilterGroupViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using MyNet.Observable;
using MyNet.Utilities;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Implements a view model for a group of filter nodes, which can contain child filter nodes and a logical operator (AND/OR) to combine them.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
public class FilterGroupViewModel<T> : EditableObject, IFilterGroupViewModel<T>
{
    private readonly ObservableCollection<IFilterNodeViewModel<T>> _children = [];

    /// <summary>
    /// Gets or sets the logical operator used to combine the child filters. The default operator is AND.
    /// </summary>
    public LogicalOperator Operator { get; set; } = LogicalOperator.And;

    /// <summary>
    /// Gets a value indicating whether this filter condition is read-only.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the filter group is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets a read-only collection of child filter nodes contained in this group. The children can be either simple filters or other groups of filters.
    /// </summary>
    public ReadOnlyObservableCollection<IFilterNodeViewModel<T>> Children { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterGroupViewModel{T}"/> class, creating an empty group of filters with the default logical operator (AND).
    /// </summary>
    /// <param name="isReadOnly">A value indicating whether this filter group is read-only.</param>
    public FilterGroupViewModel(bool isReadOnly = false)
    {
        IsReadOnly = isReadOnly;
        Children = new(_children);
    }

    /// <summary>
    /// Adds a child filter node to this group. The child can be either a simple filter or another group of filters. The new child will be combined with the existing children using the current logical operator.
    /// </summary>
    /// <param name="child">The child filter node to add.</param>
    public void Add(IFilterNodeViewModel<T> child) => _children.Add(child);

    /// <summary>
    /// Removes a child filter node from this group.
    /// </summary>
    /// <param name="child">The child filter node to remove.</param>
    public void Remove(IFilterNodeViewModel<T> child) => _children.Remove(child);

    /// <summary>
    /// Clears all child filter nodes from this group, leaving it empty. After calling this method, the filter group will not contain any child filters and will not affect the filtering results until new child filters are added.
    /// </summary>
    public void Clear() => _children.Clear();

    /// <summary>
    /// Resets the filter condition to its default state. This method should be implemented by derived classes to clear any criteria or values associated with the filter condition, effectively making it empty and not affecting the filtering results until new criteria are defined.
    /// </summary>
    public void Reset() => _children.ForEach(c => c.Reset());

    /// <summary>
    /// Builds the expression representing this filter group by combining the expressions of its active child nodes using the specified logical operator. If there are no active child nodes, it returns an expression that always evaluates to true. If the operator is not supported, it throws a NotSupportedException.
    /// </summary>
    /// <returns>The combined expression representing the filter group.</returns>
    /// <exception cref="NotSupportedException">Thrown when the logical operator is not supported.</exception>
    public Expression<Func<T, bool>>? BuildExpression()
    {
        if (!IsEnabled)
            return null;

        var expressions = _children
            .Select(c => c.BuildExpression())
            .NotNull()
            .ToList();

        if (expressions.Count == 0)
            return null;

        var param = Expression.Parameter(typeof(T), "x");

        var bodies = expressions.ConvertAll(e => ReplaceParameter(e, param))
            ;

        var body = bodies[0];

        for (var i = 1; i < bodies.Count; i++)
        {
            body = Operator switch
            {
                LogicalOperator.And => Expression.AndAlso(body, bodies[i]),
                LogicalOperator.Or => Expression.OrElse(body, bodies[i]),
                _ => throw new NotSupportedException()
            };
        }

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    /// <summary>
    /// Replaces the parameter in the given expression with a new parameter. This is necessary to combine multiple expressions that may have different parameters into a single expression with a common parameter. The method uses an expression visitor to traverse the expression tree and replace occurrences of the old parameter with the new parameter.
    /// </summary>
    /// <param name="expr">The expression in which to replace the parameter.</param>
    /// <param name="param">The new parameter to use in the expression.</param>
    /// <returns>The expression with the parameter replaced.</returns>
    private static Expression ReplaceParameter(Expression<Func<T, bool>> expr, ParameterExpression param)
    {
        var visitor = new ReplaceVisitor(expr.Parameters[0], param);
        return visitor.Visit(expr.Body);
    }

    /// <summary>
    /// An expression visitor that replaces occurrences of a specific parameter with a new parameter in an expression tree. This is used to ensure that all expressions combined in the filter group use the same parameter, allowing them to be combined correctly with logical operators.
    /// </summary>
    /// <param name="oldParam">The parameter to be replaced.</param>
    /// <param name="newParam">The new parameter to replace the old parameter with.</param>
    private sealed class ReplaceVisitor(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == oldParam ? newParam : base.VisitParameter(node);
    }
}
