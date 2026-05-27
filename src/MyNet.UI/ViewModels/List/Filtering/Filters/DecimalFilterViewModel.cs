// -----------------------------------------------------------------------
// <copyright file="DecimalFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Primitives;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for decimal properties.
/// Supports range filtering (from/to) and various comparison operators.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the decimal property to filter on.</param>
/// <param name="comparableOperator">The comparison operator to use (default: EqualsTo).</param>
public class DecimalFilterViewModel<T>(
    string propertyName,
    Expression<Func<T, decimal>> property,
    ComplexComparableOperator comparableOperator = ComplexComparableOperator.EqualsTo)
    : ComparableFilterViewModel<T, decimal>(propertyName, property, comparableOperator);
