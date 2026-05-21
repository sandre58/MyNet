// -----------------------------------------------------------------------
// <copyright file="IVirtualizedListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Defines a list view model exposing a virtualized visible window.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IVirtualizedListViewModel<T> : IListViewModel<T>
{
    /// <summary>
    /// Gets or sets the zero-based index of the first visible item.
    /// </summary>
    int VisibleStartIndex { get; set; }

    /// <summary>
    /// Gets or sets the number of visible items.
    /// </summary>
    int VisibleCount { get; set; }

    /// <summary>
    /// Gets the current visible window over <see cref="IListViewModel{T}.Items"/>.
    /// </summary>
    IReadOnlyList<T> VisibleItems { get; }
}
