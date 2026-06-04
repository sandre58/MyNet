// -----------------------------------------------------------------------
// <copyright file="FiltersViewModelBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Primitives;
using MyNet.UI.ViewModels.List.Filtering.Filters;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Fluent builder used to create <see cref="FiltersViewModel{T}"/> instances.
/// </summary>
/// <typeparam name="T">The type of items to filter.</typeparam>
public sealed class FiltersViewModelBuilder<T>
{
    private readonly FilterGroupViewModel<T> _group;

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersViewModelBuilder{T}"/> class with an empty root group.
    /// </summary>
    public FiltersViewModelBuilder()
        : this(new())
    {
    }

    private FiltersViewModelBuilder(FilterGroupViewModel<T> group) => _group = group;

    /// <summary>
    /// Creates and immediately builds a filters view model from a configuration action.
    /// </summary>
    public static FiltersViewModel<T> Create(Action<FiltersViewModelBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new FiltersViewModelBuilder<T>();
        configure(builder);
        return builder.Build();
    }

    /// <summary>
    /// Adds a child group combined with <see cref="LogicalOperator.Or"/>.
    /// </summary>
    public FiltersViewModelBuilder<T> OrGroup(Action<FiltersViewModelBuilder<T>> configure) =>
        AddGroup(LogicalOperator.Or, configure);

    /// <summary>
    /// Adds a child group combined with <see cref="LogicalOperator.And"/>.
    /// </summary>
    public FiltersViewModelBuilder<T> AndGroup(Action<FiltersViewModelBuilder<T>> configure) =>
        AddGroup(LogicalOperator.And, configure);

    /// <summary>
    /// Adds a string filter to the current group.
    /// </summary>
    public FiltersViewModelBuilder<T> AddStringFilter(
        string propertyName,
        Expression<Func<T, string?>> property,
        Action<StringFilterViewModel<T>>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(property);

        var filter = new StringFilterViewModel<T>(propertyName, property);
        configure?.Invoke(filter);
        _group.Add(filter);
        return this;
    }

    /// <summary>
    /// Creates the configured <see cref="FiltersViewModel{T}"/>.
    /// </summary>
    public FiltersViewModel<T> Build() => new(_group);

    private FiltersViewModelBuilder<T> AddGroup(LogicalOperator logicalOperator, Action<FiltersViewModelBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var group = new FilterGroupViewModel<T> { Operator = logicalOperator };
        configure(new(group));
        _group.Add(group);
        return this;
    }
}
