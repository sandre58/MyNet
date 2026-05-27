// -----------------------------------------------------------------------
// <copyright file="GuidFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Primitives;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for Guid properties.
/// Supports Is/IsNot operators for exact matching.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the Guid property to filter on.</param>
/// <param name="operatorMode">The binary operator to use (default: Is).</param>
public class GuidFilterViewModel<T>(
    string propertyName,
    Expression<Func<T, Guid>> property,
    BinaryOperator operatorMode = BinaryOperator.Is)
    : FilterConditionViewModel<T>(propertyName)
{
    /// <summary>
    /// Gets the expression representing the Guid property to filter on.
    /// </summary>
    public Expression<Func<T, Guid>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the binary operator used for matching (Is or IsNot).
    /// </summary>
    public BinaryOperator Operator { get; set => SetProperty(ref field, value); } = operatorMode;

    /// <summary>
    /// Gets or sets the Guid value to match against.
    /// When null, the filter is considered empty.
    /// </summary>
    public Guid? Value
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// </summary>
    public override bool IsEmpty => Value is null;

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;
        var value = Expression.Constant(Value!.Value, typeof(Guid));

        Expression body = Operator switch
        {
            BinaryOperator.Is => Expression.Equal(propertyBody, value),
            BinaryOperator.IsNot => Expression.NotEqual(propertyBody, value),
            _ => throw new NotSupportedException($"Operator {Operator} is not supported")
        };

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Resets the filter to its default state by clearing the value.
    /// </summary>
    public override void Reset() => Value = null;
}
