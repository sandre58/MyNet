// -----------------------------------------------------------------------
// <copyright file="ISortingProperty.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Sorting;

/// <summary>
/// Defines a sorting property for type T, including the sort direction and the expression to access the property to sort by.
/// </summary>
/// <typeparam name="T">The type of items to be sorted.</typeparam>
public interface ISortingProperty<T>
{
    /// <summary>
    /// Gets the sort direction for this sorting property, indicating whether the sorting should be in ascending or descending order.
    /// </summary>
    ListSortDirection Direction { get; }

    /// <summary>
    /// Provides an expression that accesses the property of type T to be used for sorting.
    /// </summary>
    /// <returns>An expression that represents the property to sort by.</returns>
    Expression<Func<T, object?>> ProvideExpression();
}
