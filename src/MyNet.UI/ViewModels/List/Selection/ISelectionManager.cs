// -----------------------------------------------------------------------
// <copyright file="ISelectionManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Observable.Collections.Selection;

namespace MyNet.UI.ViewModels.List.Selection;

/// <summary>
/// Provides an abstraction for list selection management.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface ISelectionManager<T> : IDisposable
    where T : notnull
{
    /// <summary>
    /// Occurs when the selection changes.
    /// </summary>
    event EventHandler? SelectionChanged;

    /// <summary>
    /// Gets the selection mode enforced by the selection engine. This property indicates whether the selection engine allows for single selection (only one item can be selected at a time) or multiple selection (multiple items can be selected simultaneously). The selection mode determines how the selection state is managed and updated when items are selected, deselected, or toggled, ensuring that the defined selection rules are consistently applied throughout the lifecycle of the selection engine.
    /// </summary>
    SelectionMode Mode { get; }

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    IReadOnlyList<T> SelectedItems { get; }

    /// <summary>
    /// Gets the selected item count.
    /// </summary>
    int SelectedCount { get; }

    /// <summary>
    /// Selects an item.
    /// </summary>
    void Select(T item);

    /// <summary>
    /// Toggles an item selection.
    /// </summary>
    void Toggle(T item);

    /// <summary>
    /// Clears the selection.
    /// </summary>
    void ClearSelection();

    /// <summary>
    /// Replaces the selection.
    /// </summary>
    void SetSelection(IEnumerable<T> items);
}
