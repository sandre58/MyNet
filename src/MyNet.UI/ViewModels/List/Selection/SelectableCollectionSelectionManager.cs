// -----------------------------------------------------------------------
// <copyright file="SelectableCollectionSelectionManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Selection;

namespace MyNet.UI.ViewModels.List.Selection;

/// <summary>
/// Selection manager backed by <see cref="SelectableCollection{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="SelectableCollectionSelectionManager{T}"/> class.
/// </remarks>
public sealed class SelectableCollectionSelectionManager<T>(ExtendedCollection<T> collection, SelectionMode mode = SelectionMode.Multiple) : ISelectionManager<T>
    where T : notnull
{
    private readonly SelectableCollection<T> _selection = new(collection, mode);

    /// <inheritdoc />
    public event EventHandler? SelectionChanged;

    /// <summary>
    /// Gets the selection mode enforced by the selection engine. This property indicates whether the selection engine allows for single selection (only one item can be selected at a time) or multiple selection (multiple items can be selected simultaneously). The selection mode determines how the selection state is managed and updated when items are selected, deselected, or toggled, ensuring that the defined selection rules are consistently applied throughout the lifecycle of the selection engine.
    /// </summary>
    public SelectionMode Mode => _selection.Mode;

    // <inheritdoc />
    public IReadOnlyList<T> SelectedItems => [.. _selection.SelectedItems];

    /// <inheritdoc />
    public int SelectedCount => _selection.SelectedCount;

    /// <inheritdoc />
    public void Select(T item)
    {
        _selection.Select(item);
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Toggle(T item)
    {
        _selection.Toggle(item);
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void ClearSelection()
    {
        if (!_selection.SelectedItems.Any())
            return;

        _selection.ClearSelection();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void SetSelection(IEnumerable<T> items)
    {
        _selection.SetSelection(items);
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Dispose() => _selection.Dispose();
}
