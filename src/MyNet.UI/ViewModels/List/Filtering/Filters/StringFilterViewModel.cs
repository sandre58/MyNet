// -----------------------------------------------------------------------
// <copyright file="StringFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

public class StringFilterViewModel<T>(
    string propertyName,
    Expression<Func<T, string?>> property,
    StringOperator filterMode = StringOperator.Contains,
    bool caseSensitive = false)
    : FilterConditionViewModel<T>(propertyName)
{
    /// <summary>
    /// Gets the expression representing the string property to filter on.
    /// </summary>
    public Expression<Func<T, string?>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the string comparison operator used for matching.
    /// </summary>
    public StringOperator Operator { get; set; } = filterMode;

    /// <summary>
    /// Gets or sets the search value to match against.
    /// When null or empty, the filter matches all items (empty filter).
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the string comparison should be case-sensitive.
    /// When false (default), comparisons are case-insensitive.
    /// </summary>
    public bool CaseSensitive { get; set; } = caseSensitive;

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// A string filter is empty when <see cref="Value"/> is null or empty.
    /// </summary>
    public override bool IsEmpty => string.IsNullOrEmpty(Value);

    /// <summary>
    /// Builds the expression representing this string filter condition based on the specified operator, value, and case sensitivity.
    /// </summary>
    /// <returns>An expression representing the filter condition.</returns>
    /// <exception cref="NotSupportedException">Thrown when the specified operator is not supported.</exception>
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var property = Property.Body;

        // Ensure it's string
        var propertyAsString = property.Type == typeof(string)
            ? property
            : Expression.Call(property, nameof(ToString), Type.EmptyTypes);

        var value = Expression.Constant(Value, typeof(string));

        var left = propertyAsString;
        Expression right = value;

        // Handle case sensitivity
        if (!CaseSensitive)
        {
            var toUpper = typeof(string).GetMethod(nameof(string.ToUpper), Type.EmptyTypes)!;

            left = Expression.Condition(
                Expression.Equal(propertyAsString, Expression.Constant(null, typeof(string))),
                Expression.Constant(null, typeof(string)),
                Expression.Call(propertyAsString, toUpper));

            right = Expression.Call(value, toUpper);
        }

        // Handle null property
        var notNull = Expression.NotEqual(propertyAsString, Expression.Constant(null, typeof(string)));

        Expression body = Operator switch
        {
            StringOperator.Contains =>
                Expression.AndAlso(
                    notNull,
                    Expression.Call(left, nameof(string.Contains), Type.EmptyTypes, right)),

            StringOperator.StartsWith =>
                Expression.AndAlso(
                    notNull,
                    Expression.Call(left, nameof(string.StartsWith), Type.EmptyTypes, right)),

            StringOperator.EndsWith =>
                Expression.AndAlso(
                    notNull,
                    Expression.Call(left, nameof(string.EndsWith), Type.EmptyTypes, right)),

            StringOperator.Is =>
                Expression.Equal(left, right),

            StringOperator.IsNot =>
                Expression.NotEqual(left, right),

            _ => throw new NotSupportedException($"Operator {Operator} is not supported")
        };

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Resets the filter to its default state by clearing the value.
    /// </summary>
    public override void Reset() => Value = null;
}
