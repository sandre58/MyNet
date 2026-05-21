// -----------------------------------------------------------------------
// <copyright file="NullableComparableFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for nullable comparable properties (int?, DateTime?, double?, etc.).
/// Supports range filtering (from/to) and various comparison operators including IsBetween.
/// Null property values are excluded from match unless operator is specifically checking for null.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <typeparam name="TValue">The type of the nullable comparable value (must be a struct implementing IComparable).</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the nullable comparable property to filter on.</param>
/// <param name="comparableOperator">The comparison operator to use (default: EqualsTo).</param>
public class NullableComparableFilterViewModel<T, TValue>(
    string propertyName,
    Expression<Func<T, TValue?>> property,
    ComplexComparableOperator comparableOperator = ComplexComparableOperator.EqualsTo)
    : FilterConditionViewModel<T>(propertyName)
    where TValue : struct, IComparable<TValue>
{
    /// <summary>
    /// Gets the expression representing the nullable comparable property to filter on.
    /// </summary>
    public Expression<Func<T, TValue?>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the comparison operator used for matching.
    /// </summary>
    public ComplexComparableOperator Operator { get; set; } = comparableOperator;

    /// <summary>
    /// Gets or sets the lower bound value for comparison.
    /// </summary>
    public TValue? From { get; set; }

    /// <summary>
    /// Gets or sets the upper bound value for comparison.
    /// </summary>
    public TValue? To { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value for the filter range.
    /// </summary>
    public TValue? Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value for the filter range.
    /// </summary>
    public TValue? Maximum { get; set; }

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// </summary>
    public override bool IsEmpty => (From is null || From.Equals(default(TValue))) && (To is null || To.Equals(default(TValue)));

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;

        // x.Property.HasValue
        var hasValue = Expression.Property(propertyBody, nameof(Nullable<>.HasValue));

        // x.Property.Value
        var propertyValue = Expression.Property(propertyBody, nameof(Nullable<>.Value));

        Expression comparison = Operator switch
        {
            ComplexComparableOperator.EqualsTo when From is not null =>
                Expression.Equal(propertyValue, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.NotEqualsTo when From is not null =>
                Expression.NotEqual(propertyValue, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.LessThan when To is not null =>
                Expression.LessThan(propertyValue, Expression.Constant(To.Value, typeof(TValue))),

            ComplexComparableOperator.GreaterThan when From is not null =>
                Expression.GreaterThan(propertyValue, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.LessEqualThan when To is not null =>
                Expression.LessThanOrEqual(propertyValue, Expression.Constant(To.Value, typeof(TValue))),

            ComplexComparableOperator.GreaterEqualThan when From is not null =>
                Expression.GreaterThanOrEqual(propertyValue, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.IsBetween when From is not null && To is not null =>
                Expression.AndAlso(
                    Expression.GreaterThanOrEqual(propertyValue, Expression.Constant(From.Value, typeof(TValue))),
                    Expression.LessThanOrEqual(propertyValue, Expression.Constant(To.Value, typeof(TValue)))),

            ComplexComparableOperator.IsNotBetween when From is not null && To is not null =>
                Expression.OrElse(
                    Expression.LessThan(propertyValue, Expression.Constant(From.Value, typeof(TValue))),
                    Expression.GreaterThan(propertyValue, Expression.Constant(To.Value, typeof(TValue)))),

            _ => Expression.Constant(true)
        };

        // Combine: x.Property.HasValue && <comparison>
        var body = Expression.AndAlso(hasValue, comparison);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Resets the filter to its default state by clearing From and To values.
    /// </summary>
    public override void Reset()
    {
        From = null;
        To = null;
    }
}
