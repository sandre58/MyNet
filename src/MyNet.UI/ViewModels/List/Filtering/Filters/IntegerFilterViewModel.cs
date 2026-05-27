// -----------------------------------------------------------------------
// <copyright file="IntegerFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for integer properties.
/// Supports range filtering (from/to) and various comparison operators (less than, greater than, between, etc.).
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the integer property to filter on.</param>
/// <param name="comparableOperator">The comparison operator to use (default: EqualsTo).</param>
public class IntegerFilterViewModel<T>(
    string propertyName,
    Expression<Func<T, int>> property,
    ComplexComparableOperator comparableOperator = ComplexComparableOperator.EqualsTo)
    : ComparableFilterViewModel<T, int>(propertyName, property, comparableOperator);
