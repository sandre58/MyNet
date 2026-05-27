// -----------------------------------------------------------------------
// <copyright file="EnumMultipleFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for enum properties (multiple values selection).
/// Matches items where the property value is contained in the selected values list.
/// Supports Is/IsNot operators.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <typeparam name="TEnum">The enum type to filter on.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the enum property to filter on.</param>
/// <param name="operatorMode">The binary operator to use (default: Is).</param>
public class EnumMultipleFilterViewModel<T, TEnum>(
    string propertyName,
    Expression<Func<T, TEnum>> property,
    BinaryOperator operatorMode = BinaryOperator.Is)
    : FilterConditionViewModel<T>(propertyName)
    where TEnum : struct, Enum
{
    /// <summary>
    /// Gets the expression representing the enum property to filter on.
    /// </summary>
    public Expression<Func<T, TEnum>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the binary operator used for matching (Is or IsNot).
    /// </summary>
    public BinaryOperator Operator { get; set; } = operatorMode;

    /// <summary>
    /// Gets or sets the selected enum values to match against.
    /// When null or empty, the filter is considered empty.
    /// </summary>
    public TEnum[]? Values { get; set; }

    /// <summary>
    /// Gets the available enum values for selection.
    /// </summary>
    public IReadOnlyList<TEnum> AvailableValues { get; } = Enum.GetValues<TEnum>().ToList().AsReadOnly();

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// An enum multiple filter is empty when <see cref="Values"/> is null or empty.
    /// </summary>
    public override bool IsEmpty => Values is null || Values.Length == 0;

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;

        // Build: Values.Contains(x.Property)
        var valuesConstant = Expression.Constant(Values, typeof(ICollection<TEnum>));
        var containsMethod = typeof(ICollection<TEnum>).GetMethod(nameof(ICollection<TEnum>.Contains))!;
        var containsCall = Expression.Call(valuesConstant, containsMethod, propertyBody);

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
