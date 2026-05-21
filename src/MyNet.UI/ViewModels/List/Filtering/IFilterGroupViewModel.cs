// -----------------------------------------------------------------------
// <copyright file="IFilterGroupViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Defines a view model for a group of filter nodes, which can contain child filter nodes and a logical operator (AND/OR) to combine them.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
public interface IFilterGroupViewModel<T> : IFilterNodeViewModel<T>
{
    /// <summary>
    /// Gets or sets the logical operator used to combine the child filters.
    /// </summary>
    LogicalOperator Operator { get; set; }

    /// <summary>
    /// Gets a read-only collection of child filter nodes contained in this group.
    /// </summary>
    ReadOnlyObservableCollection<IFilterNodeViewModel<T>> Children { get; }

    /// <summary>
    /// Adds a child filter node to this group. The child can be either a simple filter or another group of filters.
    /// </summary>
    /// <param name="child">The child filter node to add.</param>
    void Add(IFilterNodeViewModel<T> child);

    /// <summary>
    /// Removes a child filter node from this group.
    /// </summary>
    /// <param name="child">The child filter node to remove.</param>
    void Remove(IFilterNodeViewModel<T> child);

    /// <summary>
    /// Clears all child filter nodes from this group, leaving it empty.
    /// </summary>
    void Clear();
}
