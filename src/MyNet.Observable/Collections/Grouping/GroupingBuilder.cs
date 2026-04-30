// -----------------------------------------------------------------------
// <copyright file="GroupingBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Grouping;

/// <summary>
/// Provides a builder for constructing a list of grouping properties for type T, allowing to define multiple grouping levels.
/// </summary>
/// <typeparam name="T">The type of the elements to be grouped.</typeparam>
public class GroupingBuilder<T>
{
    private readonly List<IGroupingProperty<T>> _properties = [];

    /// <summary>
    /// Creates a new instance of the <see cref="GroupingBuilder{T}"/> class, providing a fluent API for constructing a list of grouping properties for type T. This method serves as the entry point for using the builder, allowing you to start defining your grouping configuration by chaining calls to the builder's methods.
    /// </summary>
    /// <returns>A new instance of the <see cref="GroupingBuilder{T}"/> class.</returns>
    public static GroupingBuilder<T> Create() => new();

    /// <summary>
    /// Adds a grouping property to the builder with the specified expression.
    /// </summary>
    /// <param name="expression">The expression used to select the property to group by.</param>
    /// <returns>The current instance of the <see cref="GroupingBuilder{T}"/> for method chaining.</returns>
    public GroupingBuilder<T> Add(Expression<Func<T, object?>> expression)
    {
        _properties.Add(new ExpressionGroupingProperty<T>(expression));
        return this;
    }

    /// <summary>
    /// Adds a grouping property to the builder with the specified expression. This method is a convenience method for adding properties that should be grouped in ascending order.
    /// </summary>
    /// <param name="expression">The expression used to select the property to group by.</param>
    /// <returns>The current instance of the <see cref="GroupingBuilder{T}"/> for method chaining.</returns>
    public GroupingBuilder<T> ThenBy(Expression<Func<T, object?>> expression)
    {
        Add(expression);
        return this;
    }

    /// <summary>
    /// Builds the list of grouping properties that have been added to the builder. This method returns a read-only list of <see cref="IGroupingProperty{T}"/> instances that represent the grouping configuration defined by the builder. Each grouping property in the list corresponds to an expression that was added using the builder's methods.
    /// </summary>
    /// <returns>A read-only list of <see cref="IGroupingProperty{T}"/> instances representing the grouping configuration.</returns>
    public IReadOnlyList<IGroupingProperty<T>> Build() => _properties;
}
