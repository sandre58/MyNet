// -----------------------------------------------------------------------
// <copyright file="GroupingViewModelBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MyNet.Observable.Collections.Grouping;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Fluent builder used to create <see cref="GroupingViewModel{T}"/> instances.
/// </summary>
/// <typeparam name="T">The type of the items to group.</typeparam>
public sealed class GroupingViewModelBuilder<T>
{
    private readonly List<GroupingPropertyViewModelBuilder<T>> _propertyBuilders = [];

    /// <summary>
    /// Creates and immediately builds a grouping view model from a configuration action.
    /// </summary>
    public static GroupingViewModel<T> Create(Action<GroupingViewModelBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new GroupingViewModelBuilder<T>();
        configure(builder);
        return builder.Build();
    }

    /// <summary>
    /// Adds a grouping property by using a dedicated property builder.
    /// </summary>
    public GroupingViewModelBuilder<T> AddProperty(Expression<Func<T, object?>> expression, Action<GroupingPropertyViewModelBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new GroupingPropertyViewModelBuilder<T>(expression);
        configure(builder);
        _propertyBuilders.Add(builder);
        return this;
    }

    /// <summary>
    /// Creates the configured <see cref="GroupingViewModel{T}"/>.
    /// </summary>
    public GroupingViewModel<T> Build()
    {
        var results = new List<GroupingPropertyBuildResult>();
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var def in _propertyBuilders.Select(builder => builder.Build()))
        {
            if (!keys.Add(def.Key))
                throw new InvalidOperationException($"Duplicate grouping key '{def.Key}'.");

            var vm = new GroupingPropertyViewModel<T>(
                def.Expression,
                def.Key,
                def.DisplayName);

            IGroupingProperty<T>? defaultGroup = null;

            if (def.DefaultState is not null)
            {
                defaultGroup = new ExpressionGroupingProperty<T>(def.Expression);
            }

            results.Add(new(vm, defaultGroup));
        }

        var defaultGrouping = results
            .Select(x => x.DefaultGrouping)
            .Where(x => x is not null)
            .Cast<IGroupingProperty<T>>()
            .ToList();

        var viewModels = results.ConvertAll(x => x.ViewModel);

        return new(viewModels, defaultGrouping);
    }

    /// <summary>
    /// Represents the result of building a grouping property, containing both the view model and the default grouping configuration if specified. This record is used internally during the build process to collect the necessary information for constructing the final <see cref="GroupingViewModel{T}"/> instance, allowing to keep track of both the view model and its associated default grouping property in a structured way.
    /// </summary>
    /// <param name="ViewModel">The view model of the grouping property.</param>
    /// <param name="DefaultGrouping">The default grouping configuration, if specified.</param>
    private sealed record GroupingPropertyBuildResult(IGroupingPropertyViewModel<T> ViewModel, IGroupingProperty<T>? DefaultGrouping);
}
