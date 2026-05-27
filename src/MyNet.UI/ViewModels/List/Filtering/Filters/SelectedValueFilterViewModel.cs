// -----------------------------------------------------------------------
// <copyright file="SelectedValueFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for selecting a single value from a list of available values.
/// Supports Is/IsNot operators with equality comparison.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <typeparam name="TValue">The type of the property value to filter on.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the property to filter on.</param>
/// <param name="availableValues">The list of available values for selection.</param>
/// <param name="operatorMode">The binary operator to use (default: Is).</param>
public class SelectedValueFilterViewModel<T, TValue>(
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
    /// Gets or sets the selected value to match against.
    /// When null, the filter is considered empty.
    /// </summary>
    public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets the available values for selection.
    /// </summary>
    public IEnumerable<TValue> AvailableValues { get; set; } = availableValues;

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// A selected value filter is empty when <see cref="Value"/> is null.
    /// </summary>
    public override bool IsEmpty => Value is null;

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;

        // Use EqualityComparer<TValue>.Default.Equals for robust comparison
        var equalsMethod = typeof(EqualityComparer<TValue>).GetProperty(nameof(EqualityComparer<TValue>.Default))!;
        var defaultComparer = Expression.Property(null, equalsMethod);
        var equalsCall = Expression.Call(
            defaultComparer,
            typeof(EqualityComparer<TValue>).GetMethod(nameof(EqualityComparer<TValue>.Equals), [typeof(TValue), typeof(TValue)])!,
            Expression.Convert(propertyBody, typeof(TValue)),
            Expression.Constant(Value, typeof(TValue)));

        Expression body = Operator switch
        {
            BinaryOperator.Is => equalsCall,
            BinaryOperator.IsNot => Expression.Not(equalsCall),
            _ => throw new NotSupportedException($"Operator {Operator} is not supported")
        };

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Resets the filter to its default state by clearing the value.
    /// </summary>
    public override void Reset() => Value = default;
}
