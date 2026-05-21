// -----------------------------------------------------------------------
// <copyright file="SelectionEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using DynamicData;

namespace MyNet.Observable.Collections.Selection;

/// <summary>
/// Manages the selection state of a collection of items, enforcing selection rules based on the specified selection mode (single or multiple).
/// </summary>
/// <typeparam name="T">The type of items in the collection, which must implement INotifyPropertyChanged.</typeparam>
public sealed class SelectionEngine<T> : IDisposable
    where T : notnull
{
    private readonly HashSet<T> _items = [];
    private readonly HashSet<T> _selected = [];
    private readonly BehaviorSubject<IReadOnlyCollection<T>> _subject;
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionEngine{T}"/> class with the specified source of items, selection mode, and optional scheduler for observing changes.
    /// </summary>
    /// <param name="source">The source of items to be managed by the selection engine.</param>
    /// <param name="mode">The selection mode to be enforced by the selection engine (single or multiple).</param>
    public SelectionEngine(IObservable<IChangeSet<T>> source, SelectionMode mode)
    {
        Mode = mode;
        _subject = new([]);

        _disposables.Add(source.Subscribe(ApplySourceChanges));
    }

    /// <summary>
    /// Gets the selection mode enforced by the selection engine. This property indicates whether the selection engine allows for single selection (only one item can be selected at a time) or multiple selection (multiple items can be selected simultaneously). The selection mode determines how the selection state is managed and updated when items are selected, deselected, or toggled, ensuring that the defined selection rules are consistently applied throughout the lifecycle of the selection engine.
    /// </summary>
    public SelectionMode Mode { get; }

    /// <summary>
    /// Gets a read-only collection of the currently selected items. The collection is automatically updated as the selection state changes, and it reflects the current selection based on the enforced selection rules.
    /// </summary>
    public IReadOnlyCollection<T> SelectedItems => _selected;

    /// <summary>
    /// Gets the count of currently selected items. This property provides a quick way to determine how many items are currently selected in the collection, and it is updated in real-time as the selection state changes.
    /// </summary>
    public int Count => _selected.Count;

    /// <summary>
    /// Connects to the selection engine and returns an observable sequence of the currently selected items. This allows observers to subscribe to changes in the selection state and receive updates whenever the selection changes. The observable emits the current collection of selected items, which is automatically updated based on the selection rules defined by the current selection mode.
    /// </summary>
    /// <returns>An observable sequence of the currently selected items.</returns>
    public IObservable<IReadOnlyCollection<T>> Connect() => _subject;

    /// <summary>
    /// Selects the specified item according to the selection rules defined by the current selection mode. If the item is not selectable, this method does nothing. If the selection mode is set to single, this method will first clear any existing selection before selecting the specified item. If the selection mode is set to multiple, this method will simply select the specified item without affecting other selected items. This method provides a convenient way to programmatically change the selection state of items in the collection while ensuring that the defined selection rules are respected.
    /// </summary>
    /// <param name="item">The item to be selected.</param>
    public void Select(T item)
    {
        if (!_items.Contains(item))
            return;

        if (Mode == SelectionMode.Single)
            _selected.Clear();

        _selected.Add(item);
        Publish();
    }

    /// <summary>
    /// Deselects the specified item. If the item is not selectable, this method does nothing. This method provides a convenient way to programmatically change the selection state of items in the collection by deselecting them, regardless of the current selection mode. It simply sets the IsSelected property of the specified item to false, which will automatically update the selection state and trigger any necessary updates to the SelectedItems collection and related observables.
    /// </summary>
    /// <param name="item">The item to be deselected.</param>
    public void Unselect(T item)
    {
        _selected.Remove(item);
        Publish();
    }

    /// <summary>
    /// Toggles the selection state of the specified item. If the item is not selectable, this method does nothing. This method provides a convenient way to programmatically change the selection state of items in the collection by toggling their current state. If the item is currently selected, it will be deselected; if it is currently deselected, it will be selected. The method respects the selection rules defined by the current selection mode, ensuring that any necessary adjustments to other selected items are made when toggling the selection state of an item.
    /// </summary>
    /// <param name="item">The item whose selection state is to be toggled.</param>
    public void Toggle(T item)
    {
        if (!_items.Contains(item))
            return;

        if (Mode == SelectionMode.Single)
        {
            if (_selected.Contains(item))
            {
                _selected.Clear();
            }
            else
            {
                _selected.Clear();
                _selected.Add(item);
            }

            Publish();
            return;
        }

        if (!_selected.Add(item))
            _selected.Remove(item);

        Publish();
    }

    /// <summary>
    /// Clears all selections in the collection. This method deselects all currently selected items, regardless of the current selection mode. It iterates through the collection of selected items and sets their IsSelected property to false, effectively clearing the selection state of all items in the collection. This is a convenient way to reset the selection state when needed, such as when a user wants to start a new selection or when the context of the selection changes significantly.
    /// </summary>
    public void Clear()
    {
        _selected.Clear();
        Publish();
    }

    /// <summary>
    /// Sets the selection state of the specified items according to the selection rules defined by the current selection mode. This method takes an enumerable of items that should be selected and updates their selection state accordingly. If the selection mode is set to single, only the first item in the provided collection will be selected, and all other items will be deselected. If the selection mode is set to multiple, all items in the provided collection will be selected, and any items not included in the collection will be deselected. This method provides a convenient way to programmatically set the selection state of multiple items at once while ensuring that the defined selection rules are respected.
    /// </summary>
    /// <param name="items">The items whose selection state is to be set.</param>
    public void Set(IEnumerable<T> items)
    {
        _selected.Clear();

        if (Mode == SelectionMode.Single)
        {
            var first = items.FirstOrDefault(_items.Contains);
            if (first is not null)
                _selected.Add(first);

            Publish();
            return;
        }

        foreach (var item in items)
        {
            if (_items.Contains(item))
                _selected.Add(item);
        }

        Publish();
    }

    /// <summary>
    /// Determines whether the specified item is currently selected in the collection. This method checks if the given item is part of the current selection by verifying if it exists in the collection of selected items. It returns true if the item is selected, and false otherwise. This is a useful method for quickly checking the selection state of an item without needing to directly access the SelectedItems collection or subscribe to changes.
    /// </summary>
    /// <param name="item">The wrapper representing the item to check.</param>
    /// <returns>True if the item is selected; otherwise, false.</returns>
    public bool Contains(T item) => _selected.Contains(item);

    /// <summary>
    /// Publishes the current selection state to the observers. This method is called whenever there is a change in the selection state, such as when items are selected, deselected, or toggled. It uses the BehaviorSubject to emit the current collection of selected items to any subscribers that are observing changes to the selection state. This ensures that all observers receive the most up-to-date information about which items are currently selected in the collection.
    /// </summary>
    private void Publish() => _subject.OnNext([.. _selected]);

    /// <summary>
    /// Rebuilds the selection state based on the current items in the collection. This method is called when there are changes to the underlying collection of items, such as additions, removals, or updates. It ensures that the selection state remains consistent with the current set of items by re-evaluating which items are selected and updating the selection state accordingly. After rebuilding the selection state, it publishes the updated selection to observers to reflect any changes that may have occurred due to modifications in the underlying collection.
    /// </summary>
    private void ApplySourceChanges(IChangeSet<T> changes)
    {
        var selectionChanged = false;

        foreach (var change in changes)
        {
            switch (change.Reason)
            {
                case ListChangeReason.Add:
                    _items.Add(change.Item.Current);
                    break;
                case ListChangeReason.AddRange:
                    foreach (var item in change.Range)
                        _items.Add(item);

                    break;
                case ListChangeReason.Remove:
                    _items.Remove(change.Item.Current);
                    selectionChanged |= _selected.Remove(change.Item.Current);
                    break;
                case ListChangeReason.RemoveRange:
                    foreach (var item in change.Range)
                    {
                        _items.Remove(item);
                        selectionChanged |= _selected.Remove(item);
                    }

                    break;
                case ListChangeReason.Replace:
                    if (change.Item.Previous.HasValue)
                    {
                        var previous = change.Item.Previous.Value;
                        _items.Remove(previous);
                        selectionChanged |= _selected.Remove(previous);
                    }

                    _items.Add(change.Item.Current);
                    break;
                case ListChangeReason.Clear:
                    _items.Clear();
                    if (_selected.Count > 0)
                    {
                        _selected.Clear();
                        selectionChanged = true;
                    }

                    break;
                case ListChangeReason.Moved:
                case ListChangeReason.Refresh:
                default:
                    break;
            }
        }

        if (Mode == SelectionMode.Single && _selected.Count > 1)
        {
            var first = _selected.First();
            _selected.Clear();
            _selected.Add(first);
            selectionChanged = true;
        }

        if (selectionChanged)
            Publish();
    }

    /// <summary>
    /// Disposes of the resources used by the selection engine. This method should be called when the selection engine is no longer needed to ensure that all subscriptions and resources are properly cleaned up. It disposes of the internal CompositeDisposable, which in turn disposes of all subscriptions to observables and any other disposable resources used by the selection engine. This is important to prevent memory leaks and to ensure that any event handlers or subscriptions are properly released when the selection engine is disposed.
    /// </summary>
    public void Dispose()
    {
        _subject.Dispose();
        _disposables.Dispose();
    }
}
