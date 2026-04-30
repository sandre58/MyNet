// -----------------------------------------------------------------------
// <copyright file="IFilterNodeViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Observable.Collections.Filters;
using MyNet.UI.ViewModels.Common;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Defines a view model for a filter node in a filter tree, which can be active or inactive and can build an <see cref="IFilter{T}"/> based on its state and configuration.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
public interface IFilterNodeViewModel<T> : IActivable
{
    /// <summary>
    /// Gets a value indicating whether this filter node is read-only.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Builds the expression representing this node.
    /// Returns null if the node is not active or empty.
    /// </summary>
    Expression<Func<T, bool>>? BuildExpression();

    /// <summary>
    /// Resets the filter condition to its default state.
    /// </summary>
    void Reset();
}
