// -----------------------------------------------------------------------
// <copyright file="SortingBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Sorting;

/// <summary>
/// Provides a builder for constructing a list of sorting properties for type T, allowing to define multiple sorting levels with specified directions.
/// </summary>
/// <typeparam name="T">The type of the elements to be sorted.</typeparam>
public class SortingBuilder<T>
{
    private readonly List<ISortingProperty<T>> _properties = [];

    /// <summary>
    /// Creates a new instance of the <see cref="SortingBuilder{T}"/> class, providing a fluent API for constructing a list of sorting properties for type T. This method serves as the entry point for using the builder, allowing you to start defining your sorting configuration by chaining calls to the builder's methods.
    /// </summary>
    /// <returns>A new instance of the <see cref="SortingBuilder{T}"/> class.</returns>
    public static SortingBuilder<T> Create() => new();

    /// <summary>
    /// Adds a sorting property to the builder with the specified expression and sort direction.
    /// </summary>
    /// <param name="expression">The expression used to select the property to sort by.</param>
    /// <param name="direction">The direction in which to sort the property.</param>
    /// <returns>The current instance of the <see cref="SortingBuilder{T}"/> for method chaining.</returns>
    public SortingBuilder<T> Add(Expression<Func<T, object?>> expression, ListSortDirection direction)
    {
        _properties.Add(new ExpressionSortingProperty<T>(expression, direction));
        return this;
    }

    /// <summary>
    /// Adds a sorting property to the builder with the specified expression and ascending sort direction. This method is a convenience method for adding properties that should be sorted in ascending order.
    /// </summary>
    /// <param name="expression">The expression used to select the property to sort by.</param>
    /// <returns>The current instance of the <see cref="SortingBuilder{T}"/> for method chaining.</returns>
    public SortingBuilder<T> ThenBy(Expression<Func<T, object?>> expression)
    {
        Add(expression, ListSortDirection.Ascending);
        return this;
    }

    /// <summary>
    /// Adds a sorting property to the builder with the specified expression and descending sort direction. This method is a convenience method for adding properties that should be sorted in descending order.
    /// </summary>
    /// <param name="expression">The expression used to select the property to sort by.</param>
    /// <returns>The current instance of the <see cref="SortingBuilder{T}"/> for method chaining.</returns>
    public SortingBuilder<T> ThenByDescending(Expression<Func<T, object?>> expression)
    {
        Add(expression, ListSortDirection.Descending);
        return this;
    }

    /// <summary>
    /// Builds the list of sorting properties that have been added to the builder. This method returns a read-only list of <see cref="ISortingProperty{T}"/> instances that represent the sorting configuration defined by the builder. Each sorting property in the list corresponds to an expression and sort direction that was added using the builder's methods.
    /// </summary>
    /// <returns>A read-only list of <see cref="ISortingProperty{T}"/> instances representing the sorting configuration.</returns>
    public IReadOnlyList<ISortingProperty<T>> Build() => _properties;
}
