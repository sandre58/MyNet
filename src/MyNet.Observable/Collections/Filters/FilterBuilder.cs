// -----------------------------------------------------------------------
// <copyright file="FilterBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MyNet.Utilities.Comparison;

namespace MyNet.Observable.Collections.Filters;

/// <summary>
/// Provides a fluent API for building complex filter expressions for collections of type T.
/// </summary>
/// <typeparam name="T">The type of items to be filtered.</typeparam>
public sealed class FilterBuilder<T>
{
    private readonly List<(LogicalOperator Operator, IFilter<T> Node)> _nodes = [];

    /// <summary>
    /// Creates a new instance of the <see cref="FilterBuilder{T}"/> class, providing a fluent API for constructing complex filter expressions for collections of type T.
    /// </summary>
    /// <returns>A new instance of <see cref="FilterBuilder{T}"/>.</returns>
    public static FilterBuilder<T> Create() => new();

    /// <summary>
    /// Adds a filter expression to the builder using the specified predicate. The predicate is a lambda expression that defines the filtering logic for items of type T. This method allows you to build complex filter expressions by chaining multiple calls to <see cref="Where(Expression{Func{T, bool}})"/> and combining them with logical operators using the <see cref="And(Expression{Func{T, bool}})"/> and <see cref="Or(Expression{Func{T, bool}})"/> methods.
    /// </summary>
    /// <param name="expr">A lambda expression that defines the filtering logic for items of type T.</param>
    /// <returns>The current instance of <see cref="FilterBuilder{T}"/>.</returns>
    public FilterBuilder<T> Where(Expression<Func<T, bool>> expr) => And(expr);

    /// <summary>
    /// Adds a grouped filter expression to the builder using the specified group function. The group function is a lambda that takes a new instance of <see cref="FilterBuilder{T}"/> and returns it after adding filter expressions to it. This allows you to create nested filter groups that can be combined with logical operators using the <see cref="And(Func{FilterBuilder{T}, FilterBuilder{T}})"/> and <see cref="Or(Func{FilterBuilder{T}, FilterBuilder{T}})"/> methods, enabling the construction of complex filter expressions with multiple levels of grouping and logical combinations.
    /// </summary>
    /// <param name="group">A lambda function that defines a group of filter expressions.</param>
    /// <returns>The current instance of <see cref="FilterBuilder{T}"/>.</returns>
    public FilterBuilder<T> Where(Func<FilterBuilder<T>, FilterBuilder<T>> group) => And(group);

    /// <summary>
    /// Adds a filter expression to the builder using the logical AND operator. The provided expression is combined with any existing expressions in the builder using the AND operator, allowing you to build complex filter expressions that require multiple conditions to be met. This method can be chained with other calls to <see cref="And(Expression{Func{T, bool}})"/> and <see cref="Or(Expression{Func{T, bool}})"/> to create more intricate filter logic.
    /// </summary>
    /// <param name="expr">A lambda expression that defines the filtering logic for items of type T.</param>
    /// <returns>The current instance of <see cref="FilterBuilder{T}"/>.</returns>
    public FilterBuilder<T> And(Expression<Func<T, bool>> expr)
    {
        AddNode(LogicalOperator.And, new ExpressionFilter<T>(expr));
        return this;
    }

    /// <summary>
    /// Adds a grouped filter expression to the builder using the logical AND operator. The provided group function is used to create a nested filter group, which is then combined with any existing expressions in the builder using the AND operator. This allows you to build complex filter expressions with multiple levels of grouping and logical combinations, enabling more sophisticated filtering logic for collections of type T.
    /// </summary>
    /// <param name="group">A lambda function that defines a group of filter expressions.</param>
    /// <returns>The current instance of <see cref="FilterBuilder{T}"/>.</returns>
    public FilterBuilder<T> And(Func<FilterBuilder<T>, FilterBuilder<T>> group) => AddGroup(LogicalOperator.And, group);

    /// <summary>
    /// Adds a grouped filter expression to the builder using the logical OR operator. The provided group function is used to create a nested filter group, which is then combined with any existing expressions in the builder using the OR operator. This allows you to build complex filter expressions with multiple levels of grouping and logical combinations, enabling more sophisticated filtering logic for collections of type T.
    /// </summary>
    /// <param name="expression">A lambda function that defines a group of filter expressions.</param>
    /// <returns>The current instance of <see cref="FilterBuilder{T}"/>.</returns>
    public FilterBuilder<T> Or(Expression<Func<T, bool>> expression)
    {
        AddNode(LogicalOperator.Or, new ExpressionFilter<T>(expression));
        return this;
    }

    /// <summary>
    /// Adds a grouped filter expression to the builder using the logical OR operator. The provided group function is used to create a nested filter group, which is then combined with any existing expressions in the builder using the OR operator. This allows you to build complex filter expressions with multiple levels of grouping and logical combinations, enabling more sophisticated filtering logic for collections of type T.
    /// </summary>
    /// <param name="group">A lambda function that defines a group of filter expressions.</param>
    /// <returns>The current instance of <see cref="FilterBuilder{T}"/>.</returns>
    public FilterBuilder<T> Or(Func<FilterBuilder<T>, FilterBuilder<T>> group) => AddGroup(LogicalOperator.Or, group);

    /// <summary>
    /// Adds a grouped filter expression to the builder using the specified logical operator. The provided group function is used to create a nested filter group, which is then combined with any existing expressions in the builder using the specified logical operator (AND or OR). This method allows for flexible construction of complex filter expressions with multiple levels of grouping and logical combinations, enabling sophisticated filtering logic for collections of type T.
    /// </summary>
    /// <param name="op">The logical operator to use when combining the group with existing expressions.</param>
    /// <param name="builder">A lambda function that defines a group of filter expressions.</param>
    /// <returns>The current instance of <see cref="FilterBuilder{T}"/>.</returns>
    private FilterBuilder<T> AddGroup(LogicalOperator op, Func<FilterBuilder<T>, FilterBuilder<T>> builder)
    {
        var inner = builder(new());
        var node = inner.Build();

        if (node is not null)
            AddNode(op, node);

        return this;
    }

    /// <summary>
    /// Adds a filter node to the builder with the specified logical operator. The node represents a filter expression or a group of filter expressions that will be combined with existing expressions in the builder using the specified logical operator (AND or OR). This method is used internally to manage the construction of the filter expression tree as nodes are added to the builder, allowing for the creation of complex filter logic for collections of type T.
    /// </summary>
    /// <param name="op">The logical operator to use when combining the node with existing expressions.</param>
    /// <param name="node">The filter node to add to the builder.</param>
    private void AddNode(LogicalOperator op, IFilter<T> node) => _nodes.Add((op, node));

    /// <summary>
    /// Builds the filter expression tree based on the nodes added to the builder. If no nodes have been added, it returns a filter that always returns true. If only one node has been added, it returns that node directly. If multiple nodes have been added, it constructs a filter group that combines all nodes according to their specified logical operators, resulting in a single filter node that represents the entire filter expression tree built by the builder.
    /// </summary>
    /// <returns>The root filter node representing the entire filter expression tree.</returns>
    public IFilter<T>? Build() =>
        _nodes.Count switch
        {
            0 => null,
            1 => _nodes[0].Node,
            _ => BuildTree()
        };

    /// <summary>
    /// Builds the filter expression tree by combining all nodes in the builder according to their specified logical operators. The method iterates through the list of nodes, starting with the first node as the current root, and combines it with each subsequent node using the logical operator specified for that node. This results in a single filter node that represents the entire filter expression tree built by the builder, allowing for complex combinations of filter expressions based on the logical operators used when adding nodes to the builder.
    /// </summary>
    /// <returns>The root filter node representing the entire filter expression tree.</returns>
    private IFilter<T> BuildTree()
    {
        // Group sequentially respecting operators
        var current = _nodes[0].Node;

        for (var i = 1; i < _nodes.Count; i++)
        {
            var (op, node) = _nodes[i];

            current = new FilterGroup<T>(op, [current, node]);
        }

        return current;
    }
}
