// -----------------------------------------------------------------------
// <copyright file="SelectableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Concurrency;
using DynamicData;
using DynamicData.Binding;
using MyNet.Observable.Collections;
using MyNet.Observable.Extensions;
using MyNet.UI.Selection.Models;
using MyNet.Utilities;
using MyNet.Utilities.Deferring;

namespace MyNet.UI.Selection;

/// <summary>
/// Represents a collection of selectable items, supporting single or multiple selection modes.
/// Provides selection logic, events, and access to selected items and wrappers.
/// Optimized for performance with efficient batch operations and minimal allocations.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class SelectableCollection<T> : ExtendedWrapperCollection<T, SelectedWrapper<T>>
    where T : notnull
{
    private readonly Deferrer _selectionChangedDeferrer;
    private readonly ReadOnlyObservableCollection<SelectedWrapper<T>> _selectedWrappers;
    private readonly IObservable<IChangeSet<SelectedWrapper<T>>> _observableSelectedWrappers;
    private SelectionMode _selectionMode;

    /// <summary>
    /// Gets or sets the selection mode (single or multiple).
    /// </summary>
    public SelectionMode SelectionMode
    {
        get => _selectionMode;
        set
        {
            if (_selectionMode == value)
                return;

            _selectionMode = value;

            // Optimize: When switching to Single mode, keep only the first selected item
            if (_selectionMode == SelectionMode.Single && _selectedWrappers.Count > 1)
            {
                using (_selectionChangedDeferrer.Defer())
                {
                    var first = _selectedWrappers[0];
                    foreach (var wrapper in _selectedWrappers)
                    {
                        if (!ReferenceEquals(wrapper, first))
                            wrapper.IsSelected = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the collection of selected wrappers.
    /// </summary>
    public ReadOnlyObservableCollection<SelectedWrapper<T>> SelectedWrappers => _selectedWrappers;

    /// <summary>
    /// Gets the collection of selected items.
    /// Optimized to avoid LINQ allocations.
    /// </summary>
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery", Justification = "Avoid LINQ Select allocation in hot path")]
    public IEnumerable<T> SelectedItems
    {
        get
        {
            // Optimize: Direct iteration without LINQ
            foreach (var wrapper in _selectedWrappers)
                yield return wrapper.Item;
        }
    }

    /// <summary>
    /// Gets the number of selected items.
    /// Optimized for quick access without enumerating SelectedItems.
    /// </summary>
    public int SelectedCount => _selectedWrappers.Count;

    /// <summary>
    /// Occurs when the selection changes.
    /// </summary>
    public event EventHandler? SelectionChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableCollection{T}"/> class with a source list.
    /// </summary>
    public SelectableCollection(SourceList<T> sourceList,
                                bool isReadOnly,
                                SelectionMode selectionMode = SelectionMode.Multiple,
                                IScheduler? scheduler = null,
                                Func<T, SelectedWrapper<T>>? createWrapper = null)
        : base(sourceList, isReadOnly, scheduler, createWrapper ?? (x => new(x)))
    {
        _selectionChangedDeferrer = new Deferrer(() => SelectionChanged?.Invoke(this, EventArgs.Empty));
        _selectionMode = selectionMode;

        var obs = ConnectWrappersSource();

        Disposables.AddRange(
        [
            obs.AutoRefresh(x => x.IsSelected)
                .Filter(y => y is { IsSelectable: true, IsSelected: true })
                .ObserveOnOptional(scheduler)
                .Bind(out _selectedWrappers)
                .Subscribe(),
            obs.WhenPropertyChanged(x => x.IsSelected).Subscribe(x => UpdateSelection(x.Sender)),
            _selectedWrappers.ToObservableChangeSet().Subscribe(_ => _selectionChangedDeferrer.DeferOrExecute())
        ]);

        _observableSelectedWrappers = _selectedWrappers.ToObservableChangeSet();
    }

    /// <summary>
    /// Returns an observable for changes to the selected wrappers.
    /// </summary>
    public IObservable<IChangeSet<SelectedWrapper<T>>> ConnectSelectedWrappers() => _observableSelectedWrappers;

    #region Selection

    /// <summary>
    /// Changes the selection state of the specified item.
    /// </summary>
    /// <param name="item">The item to change selection state for.</param>
    /// <param name="value">True to select; false to unselect.</param>
    public virtual void ChangeSelectState(T item, bool value)
    {
        var wrapper = GetOrCreate(item);

        if (!wrapper.IsSelectable)
            return;

        // Optimize: Early exit if already in desired state
        if (wrapper.IsSelected == value)
            return;

        // Optimize: For Single mode with selection, deselect others efficiently
        if (_selectionMode == SelectionMode.Single && value)
        {
            using (_selectionChangedDeferrer.Defer())
            {
                // Deselect all currently selected wrappers
                foreach (var selectedWrapper in _selectedWrappers.ToArray())
                {
                    if (!ReferenceEquals(selectedWrapper, wrapper))
                        selectedWrapper.IsSelected = false;
                }

                wrapper.IsSelected = true;
            }
        }
        else
        {
            wrapper.IsSelected = value;
        }
    }

    /// <summary>
    /// Selects the specified item.
    /// </summary>
    /// <param name="item">The item to select.</param>
    public void Select(T item) => ChangeSelectState(item, true);

    /// <summary>
    /// Selects the specified items.
    /// Optimized for batch operations.
    /// </summary>
    /// <param name="items">The items to select.</param>
    public void Select(IEnumerable<T> items)
    {
        using (_selectionChangedDeferrer.Defer())
        {
            if (_selectionMode == SelectionMode.Single)
            {
                // Optimize: For single mode, just select the first valid item
                var first = items.FirstOrDefault();
                if (first is not null)
                    ChangeSelectState(first, true);
            }
            else
            {
                // Optimize: Batch select for Multiple mode
                foreach (var item in items)
                {
                    var wrapper = GetOrCreate(item);
                    if (wrapper.IsSelectable && !wrapper.IsSelected)
                        wrapper.IsSelected = true;
                }
            }
        }
    }

    /// <summary>
    /// Selects or unselects all items in the collection.
    /// Optimized to avoid unnecessary state changes.
    /// </summary>
    /// <param name="value">True to select all; false to unselect all.</param>
    public void SelectAll(bool value)
    {
        using (_selectionChangedDeferrer.Defer())
        {
            if (_selectionMode == SelectionMode.Single && value)
            {
                // Optimize: For single mode, only select first selectable item
                var first = WrappersSource.FirstOrDefault(w => w.IsSelectable);
                if (first is not null)
                {
                    // Clear all first
                    foreach (var wrapper in _selectedWrappers.ToArray())
                        wrapper.IsSelected = false;

                    first.IsSelected = true;
                }
            }
            else
            {
                // Optimize: Only change wrappers that need state change
                foreach (var wrapper in WrappersSource)
                {
                    if (wrapper.IsSelectable && wrapper.IsSelected != value)
                        wrapper.IsSelected = value;
                }
            }
        }
    }

    /// <summary>
    /// Unselects the specified item.
    /// </summary>
    /// <param name="item">The item to unselect.</param>
    public virtual void Unselect(T item) => ChangeSelectState(item, false);

    /// <summary>
    /// Unselects the specified items.
    /// Optimized for batch operations.
    /// </summary>
    /// <param name="items">The items to unselect.</param>
    public virtual void Unselect(IEnumerable<T> items)
    {
        using (_selectionChangedDeferrer.Defer())
        {
            // Optimize: Only unselect items that are currently selected
            foreach (var item in items)
            {
                var wrapper = GetOrCreate(item);
                if (wrapper.IsSelected)
                    wrapper.IsSelected = false;
            }
        }
    }

    /// <summary>
    /// Clears the selection of all items in the collection.
    /// Optimized to only change selected wrappers.
    /// </summary>
    public virtual void ClearSelection()
    {
        // Optimize: Only iterate over selected wrappers, not all wrappers
        if (_selectedWrappers.Count == 0)
            return;

        using (_selectionChangedDeferrer.Defer())
        {
            // ToArray to avoid collection modification during iteration
            foreach (var wrapper in _selectedWrappers.ToArray())
                wrapper.IsSelected = false;
        }
    }

    /// <summary>
    /// Sets the selection to the specified items, clearing previous selection.
    /// Optimized to minimize notifications.
    /// </summary>
    /// <param name="items">The items to select.</param>
    public void SetSelection(IEnumerable<T> items)
    {
        using (_selectionChangedDeferrer.Defer())
        {
            var itemsArray = items.ToArray();

            // Optimize: Create a HashSet for O(1) lookup
            var itemsToSelect = new HashSet<T>(itemsArray);

            // Deselect items not in the new selection
            foreach (var wrapper in _selectedWrappers.ToArray())
            {
                if (!itemsToSelect.Contains(wrapper.Item))
                    wrapper.IsSelected = false;
            }

            // Select new items
            if (_selectionMode == SelectionMode.Single)
            {
                var first = itemsArray.FirstOrDefault();
                if (first is not null)
                {
                    var wrapper = GetOrCreate(first);
                    if (wrapper.IsSelectable)
                        wrapper.IsSelected = true;
                }
            }
            else
            {
                foreach (var item in itemsArray)
                {
                    var wrapper = GetOrCreate(item);
                    if (wrapper.IsSelectable && !wrapper.IsSelected)
                        wrapper.IsSelected = true;
                }
            }
        }
    }

    /// <summary>
    /// Toggles the selection state of the specified item.
    /// </summary>
    /// <param name="item">The item to toggle.</param>
    public void ToggleSelection(T item)
    {
        var wrapper = GetOrCreate(item);
        if (wrapper.IsSelectable)
            ChangeSelectState(item, !wrapper.IsSelected);
    }

    /// <summary>
    /// Checks if the specified item is selected.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is selected; otherwise, false.</returns>
    public bool IsSelected(T item)
    {
        var wrapper = GetOrCreate(item);
        return wrapper.IsSelected;
    }

    /// <summary>
    /// Checks if the specified item is selectable.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is selectable; otherwise, false.</returns>
    public bool IsSelectable(T item)
    {
        var wrapper = GetOrCreate(item);
        return wrapper.IsSelectable;
    }

    protected virtual void UpdateSelection(SelectedWrapper<T> wrapper)
    {
        // This method is called when a wrapper's IsSelected property changes
        // In Single mode, we need to deselect other items when one is selected
        // This is already handled in ChangeSelectState, so this method can remain empty
        // or be used for additional logic if needed in derived classes
    }

    #endregion Selection
}
