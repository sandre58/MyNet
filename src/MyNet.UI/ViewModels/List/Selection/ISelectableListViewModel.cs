// -----------------------------------------------------------------------
// <copyright file="ISelectableListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Observable.Collections.Selection;

namespace MyNet.UI.ViewModels.List.Selection;

/// <summary>
/// Defines a list view model with item selection backed by <see cref="SelectableCollection{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface ISelectableListViewModel<T> : IListViewModel<T>
    where T : notnull
{
    /// <summary>
    /// Gets the selection mode enforced by the selection engine.
    /// </summary>
    SelectionMode SelectionMode { get; }

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    IReadOnlyList<T> SelectedItems { get; }

    /// <summary>
    /// Gets the number of selected items.
    /// </summary>
    int SelectedCount { get; }

    /// <summary>
    /// Gets or sets the first selected item, if any.
    /// </summary>
    T? SelectedItem { get; set; }

    /// <summary>
    /// Determines whether the specified item is selected.
    /// </summary>
    bool IsSelected(T item);

    /// <summary>
    /// Selects the specified item.
    /// </summary>
    void Select(T item);

    /// <summary>
    /// Unselects the specified item.
    /// </summary>
    void Unselect(T item);

    /// <summary>
    /// Toggles the selection state of the specified item.
    /// </summary>
    void Toggle(T item);

    /// <summary>
    /// Clears the selection.
    /// </summary>
    void ClearSelection();

    /// <summary>
    /// Replaces the current selection.
    /// </summary>
    void SetSelection(IEnumerable<T> items);
}
