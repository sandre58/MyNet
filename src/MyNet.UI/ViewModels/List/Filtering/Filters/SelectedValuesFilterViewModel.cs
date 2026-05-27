// -----------------------------------------------------------------------
// <copyright file="SelectedValuesFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for selecting multiple values from a list of available values.
/// Matches items where the property value is contained in the selected values list.
/// Supports Is/IsNot operators.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <typeparam name="TValue">The type of the property value to filter on.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the property to filter on.</param>
/// <param name="availableValues">The list of available values for selection.</param>
/// <param name="operatorMode">The binary operator to use (default: Is).</param>
public class SelectedValuesFilterViewModel<T, TValue>(
    string propertyName,
    Expression<Func<T, TValue?>> property,
    IEnumerable<TValue> availableValues,
    BinaryOperator operatorMode = BinaryOperator.Is)
    : FilterConditionViewModel<T>(propertyName)
{
    /// <summary>
    /// Gets the expression representing the property to filter on.
    /// </summary>
    public Expression<Func<T, TValue?>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the binary operator used for matching (Is or IsNot).
    /// </summary>
    public BinaryOperator Operator { get; set; } = operatorMode;

    /// <summary>
    /// Gets or sets the selected values to match against.
    /// When null or empty, the filter is considered empty.
    /// </summary>
    public TValue[]? Values { get; set; }

    /// <summary>
    /// Gets or sets the available values for selection.
    /// </summary>
    public IEnumerable<TValue> AvailableValues { get; set; } = availableValues;

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// A selected values filter is empty when <see cref="Values"/> is null or empty.
    /// </summary>
    public override bool IsEmpty => Values is null || Values.Length == 0;

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;

        // Build: Values.Any(v => EqualityComparer<TValue>.Default.Equals(v, x.Property))
        // Simplified approach: use a captured HashSet for O(1) lookups
        var values = new HashSet<TValue>(Values ?? []);
        var valuesConstant = Expression.Constant(values, typeof(HashSet<TValue>));
        var containsMethod = typeof(HashSet<TValue>).GetMethod(nameof(HashSet<TValue>.Contains))!;
        var containsCall = Expression.Call(valuesConstant, containsMethod, Expression.Convert(propertyBody, typeof(TValue)));

        Expression body = Operator switch
        {
            BinaryOperator.Is => containsCall,
            BinaryOperator.IsNot => Expression.Not(containsCall),
            _ => throw new NotSupportedException($"Operator {Operator} is not supported")
        };

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Resets the filter to its default state by clearing the values.
    /// </summary>
    public override void Reset() => Values = null;
}
