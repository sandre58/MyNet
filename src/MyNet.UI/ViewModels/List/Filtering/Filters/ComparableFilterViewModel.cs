// -----------------------------------------------------------------------
// <copyright file="ComparableFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Primitives;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for comparable (numeric, date, etc.) properties.
/// Supports range filtering (from/to) and various comparison operators including IsBetween.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <typeparam name="TValue">The type of the comparable value (must be a struct implementing IComparable).</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the comparable property to filter on.</param>
/// <param name="comparableOperator">The comparison operator to use (default: EqualsTo).</param>
public class ComparableFilterViewModel<T, TValue>(
    string propertyName,
    Expression<Func<T, TValue>> property,
    ComplexComparableOperator comparableOperator = ComplexComparableOperator.EqualsTo)
    : FilterConditionViewModel<T>(propertyName)
    where TValue : struct, IComparable<TValue>
{
    /// <summary>
    /// Gets the expression representing the comparable property to filter on.
    /// </summary>
    public Expression<Func<T, TValue>> Property { get; } = property;

    /// <summary>
    /// Gets or sets the comparison operator used for matching.
    /// </summary>
    public ComplexComparableOperator Operator { get; set => SetProperty(ref field, value); } = comparableOperator;

    /// <summary>
    /// Gets or sets the lower bound value for comparison.
    /// Used as the primary value for single-value operators (EqualsTo, LessThan, etc.).
    /// </summary>
    public TValue? From { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the upper bound value for comparison.
    /// Used together with <see cref="From"/> for range operators (IsBetween, IsNotBetween).
    /// </summary>
    public TValue? To { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the minimum allowed value for the filter range.
    /// </summary>
    public TValue? Minimum { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the maximum allowed value for the filter range.
    /// </summary>
    public TValue? Maximum { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets a value indicating whether this filter is in an empty state.
    /// A comparable filter is empty when both From and To are null or equal to default.
    /// </summary>
    public override bool IsEmpty => (From is null || From.Equals(default(TValue))) && (To is null || To.Equals(default(TValue)));

    /// <inheritdoc />
    protected override Expression<Func<T, bool>> BuildExpressionCore()
    {
        var parameter = Property.Parameters[0];
        var propertyBody = Property.Body;

        Expression body = Operator switch
        {
            ComplexComparableOperator.EqualsTo when From is not null =>
                Expression.Equal(propertyBody, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.NotEqualsTo when From is not null =>
                Expression.NotEqual(propertyBody, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.LessThan when To is not null =>
                Expression.LessThan(propertyBody, Expression.Constant(To.Value, typeof(TValue))),

            ComplexComparableOperator.GreaterThan when From is not null =>
                Expression.GreaterThan(propertyBody, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.LessEqualThan when To is not null =>
                Expression.LessThanOrEqual(propertyBody, Expression.Constant(To.Value, typeof(TValue))),

            ComplexComparableOperator.GreaterEqualThan when From is not null =>
                Expression.GreaterThanOrEqual(propertyBody, Expression.Constant(From.Value, typeof(TValue))),

            ComplexComparableOperator.IsBetween when From is not null && To is not null =>
                Expression.AndAlso(
                    Expression.GreaterThanOrEqual(propertyBody, Expression.Constant(From.Value, typeof(TValue))),
                    Expression.LessThanOrEqual(propertyBody, Expression.Constant(To.Value, typeof(TValue)))),

            ComplexComparableOperator.IsNotBetween when From is not null && To is not null =>
                Expression.OrElse(
                    Expression.LessThan(propertyBody, Expression.Constant(From.Value, typeof(TValue))),
                    Expression.GreaterThan(propertyBody, Expression.Constant(To.Value, typeof(TValue)))),

            _ => Expression.Constant(true)
        };

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
