// -----------------------------------------------------------------------
// <copyright file="BooleanFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for boolean properties.
/// Supports nullable boolean values with optional null value filtering.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the boolean property to filter on.</param>
/// <param name="allowNullValue">Whether null values are allowed. When true, Reset sets Value to null instead of false.</param>
public class BooleanFilterViewModel<T>(
    string propertyName,
    Expression<Func<T, bool?>> property,
    bool allowNullValue = false)
    : FilterConditionViewModel<T>(propertyName)
{
    /// <summary>
    /// Gets the expression representing the boolean property to filter on.
    /// </summary>
    public Expression<Func<T, bool?>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the boolean value to filter by.
    /// When null, the filter is considered empty (unless AllowNullValue is true).
    /// </summary>
    public bool? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether null values are allowed in the filter.
    /// When true, <see cref="Reset"/> sets Value to null instead of false.
    /// </summary>
    public bool AllowNullValue { get; set; } = allowNullValue;

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// A boolean filter is empty when <see cref="Value"/> is null.
    /// </summary>
    public override bool IsEmpty => Value is null;

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;
        var value = Expression.Constant(Value, typeof(bool?));

        var body = Expression.Equal(propertyBody, value);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Resets the filter to its default state.
    /// If <see cref="AllowNullValue"/> is true, sets Value to null; otherwise, sets it to null (empty filter).
    /// </summary>
    public override void Reset() => Value = null;
}
