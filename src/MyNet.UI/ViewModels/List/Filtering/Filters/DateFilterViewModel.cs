// -----------------------------------------------------------------------
// <copyright file="DateFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for DateTime properties.
/// Supports date range filtering (from/to) and various comparison operators.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
/// <param name="propertyName">The key that identifies this filter condition.</param>
/// <param name="property">The expression representing the DateTime property to filter on.</param>
/// <param name="comparableOperator">The comparison operator to use (default: IsBetween).</param>
public class DateFilterViewModel<T>(
    string propertyName,
    Expression<Func<T, DateTime>> property,
    ComplexComparableOperator comparableOperator = ComplexComparableOperator.IsBetween)
    : ComparableFilterViewModel<T, DateTime>(propertyName, property, comparableOperator);
