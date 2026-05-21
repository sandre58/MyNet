// -----------------------------------------------------------------------
// <copyright file="CollectionContainsFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for collection properties.
/// Checks whether a collection property contains any/all of the specified values.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <typeparam name="TElement">The type of elements in the collection property.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the collection property to filter on.</param>
/// <param name="availableValues">The available values for selection.</param>
/// <param name="operatorMode">The logical operator to combine checks (And = all must match, Or = any must match).</param>
public class CollectionContainsFilterViewModel<T, TElement>(
    string propertyName,
    Expression<Func<T, IEnumerable<TElement>>> property,
    IEnumerable<TElement> availableValues,
    LogicalOperator operatorMode = LogicalOperator.Or)
    : FilterConditionViewModel<T>(propertyName)
{
    /// <summary>
    /// Gets the expression representing the collection property to filter on.
    /// </summary>
    public Expression<Func<T, IEnumerable<TElement>>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the logical operator (And = all values must be present, Or = any value must be present).
    /// </summary>
    public LogicalOperator Operator { get; set; } = operatorMode;

    /// <summary>
    /// Gets or sets the values to search for in the collection.
    /// When null or empty, the filter is considered empty.
    /// </summary>
    public TElement[]? Values { get; set; }

    /// <summary>
    /// Gets or sets the available values for selection.
    /// </summary>
    public IEnumerable<TElement> AvailableValues { get; set; } = availableValues;

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// </summary>
    public override bool IsEmpty => Values is null || Values.Length == 0;

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;

        var values = Values?.ToList() ?? [];

        // Build individual Contains checks for each value
        var containsMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TElement));

        Expression? combined = null;

        foreach (var value in values)
        {
            var containsCall = Expression.Call(containsMethod, propertyBody, Expression.Constant(value, typeof(TElement)));

            combined = combined is null
                ? containsCall
                : Operator == LogicalOperator.And
                    ? Expression.AndAlso(combined, containsCall)
                    : Expression.OrElse(combined, containsCall);
        }

        combined ??= Expression.Constant(true);

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    /// <summary>
    /// Resets the filter to its default state by clearing the values.
    /// </summary>
    public override void Reset() => Values = null;
}
