// -----------------------------------------------------------------------
// <copyright file="SortingViewModelBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MyNet.Observable.Collections.Sorting;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Fluent builder used to create <see cref="SortingViewModel{T}"/> instances.
/// </summary>
/// <typeparam name="T">The type of the items to sort.</typeparam>
public sealed class SortingViewModelBuilder<T>
{
    private readonly List<SortingPropertyViewModelBuilder<T>> _propertyBuilders = [];

    /// <summary>
    /// Creates and immediately builds a sorting view model from a configuration action.
    /// </summary>
    public static SortingViewModel<T> Create(Action<SortingViewModelBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new SortingViewModelBuilder<T>();
        configure(builder);
        return builder.Build();
    }

    /// <summary>
    /// Adds a sorting property by using a dedicated property builder.
    /// </summary>
    public SortingViewModelBuilder<T> AddProperty(Expression<Func<T, object?>> expression, Action<SortingPropertyViewModelBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new SortingPropertyViewModelBuilder<T>(expression);
        configure(builder);
        _propertyBuilders.Add(builder);
        return this;
    }

    /// <summary>
    /// Creates the configured <see cref="SortingViewModel{T}"/>.
    /// </summary>
    public SortingViewModel<T> Build()
    {
        var results = new List<SortingPropertyBuildResult>();
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var def in _propertyBuilders.Select(builder => builder.Build()))
        {
            if (!keys.Add(def.Key))
                throw new InvalidOperationException($"Duplicate sorting key '{def.Key}'.");

            var vm = new SortingPropertyViewModel<T>(
                def.Expression,
                def.Key,
                def.DisplayName);

            ISortingProperty<T>? defaultSort = null;

            if (def.DefaultState is not null)
            {
                defaultSort = new ExpressionSortingProperty<T>(
                    def.Expression,
                    def.DefaultState.Direction);
            }

            results.Add(new(vm, defaultSort));
        }

        var defaultSorting = results
            .Select(x => x.DefaultSorting)
            .Where(x => x is not null)
            .Cast<ISortingProperty<T>>()
            .ToList();

        var viewModels = results.ConvertAll(x => x.ViewModel);

        return new(viewModels, defaultSorting);
    }

    /// <summary>
    /// Represents the result of building a sorting property, containing both the view model and the default sorting configuration if specified. This record is used internally during the build process to collect the necessary information for constructing the final <see cref="SortingViewModel{T}"/> instance, allowing to keep track of both the view model and its associated default sorting property in a structured way.
    /// </summary>
    /// <param name="ViewModel">The view model of the sorting property.</param>
    /// <param name="DefaultSorting">The default sorting configuration, if specified.</param>
    private sealed record SortingPropertyBuildResult(ISortingPropertyViewModel<T> ViewModel, ISortingProperty<T>? DefaultSorting);
}
