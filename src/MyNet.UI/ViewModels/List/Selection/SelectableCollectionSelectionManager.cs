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
public sealed class SelectableCollectionSelectionManager<T> : ISelectionManager<T>
    where T : notnull
{
    private readonly SelectableCollection<T> _selection;
    private readonly bool _ownsSelectable;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableCollectionSelectionManager{T}"/> class.
    /// Creates a new selection manager with its own internal selectable collection (caller owns disposal of the collection itself).
    /// </summary>
    /// <param name="collection">The extended collection to be managed.</param>
    /// <param name="mode">The selection mode to be used.</param>
    /// <exception cref="ArgumentNullException">Thrown if the collection is null.</exception>
    public SelectableCollectionSelectionManager(ExtendedCollection<T> collection, SelectionMode mode = SelectionMode.Multiple)
    {
        ArgumentNullException.ThrowIfNull(collection);

        _selection = new(collection, mode, disposeCollection: false);
        _ownsSelectable = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableCollectionSelectionManager{T}"/> class.
    /// Uses an existing selectable collection (caller owns disposal of the collection itself).
    /// </summary>
    /// <param name="selectable">The existing selectable collection to be managed.</param>
    /// <param name="disposeSelectable">A value indicating whether the selectable collection should be disposed when the selection manager is disposed.</param>
    /// <exception cref="ArgumentNullException">Thrown if the selectable collection is null.</exception>
    public SelectableCollectionSelectionManager(SelectableCollection<T> selectable, bool disposeSelectable = true)
    {
        _selection = selectable ?? throw new ArgumentNullException(nameof(selectable));
        _ownsSelectable = disposeSelectable;
    }

    /// <inheritdoc />
    public event EventHandler? SelectionChanged;

    /// <inheritdoc />
    public SelectionMode Mode => _selection.Mode;

    /// <inheritdoc />
    public IReadOnlyList<T> SelectedItems => [.. _selection.SelectedItems];

    /// <inheritdoc />
    public int SelectedCount => _selection.SelectedCount;

    /// <inheritdoc />
    public bool IsSelected(T item) => _selection.IsSelected(item);

    /// <inheritdoc />
    public void Select(T item)
    {
        _selection.Select(item);
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Unselect(T item)
    {
        _selection.Unselect(item);
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
    public void Dispose()
    {
        if (_ownsSelectable)
            _selection.Dispose();
    }
}
