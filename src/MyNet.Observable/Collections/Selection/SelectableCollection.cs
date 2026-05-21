// -----------------------------------------------------------------------
// <copyright file="SelectableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using DynamicData;
using MyNet.Observable.Collections.Sources;

namespace MyNet.Observable.Collections.Selection;

/// <summary>
/// Provides factory methods to create selectable collections from various sources, allowing items to be selected and unselected while maintaining the selection state in a reactive manner.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "This class serves as a factory for SelectableCollection<T> and the name reflects its purpose.")]
public static class SelectableCollection
{
    /// <summary>
    /// Creates a selectable collection from an enumerable.
    /// </summary>
    public static SelectableCollection<T> From<T>(
        IEnumerable<T> items,
        SelectionMode mode = SelectionMode.Multiple,
        IScheduler? scheduler = null)
        where T : notnull
    {
        var source = SourceEngine<T>.From(items, readOnly: false);

        return Create(source, mode, scheduler);
    }

    /// <summary>
    /// Creates a selectable collection from an observable source.
    /// </summary>
    public static SelectableCollection<T> FromObservable<T>(
        IObservable<IChangeSet<T>> source,
        SelectionMode mode = SelectionMode.Multiple,
        IScheduler? scheduler = null)
        where T : notnull
    {
        var engine = SourceEngine<T>.FromObservable(source);

        return Create(engine, mode, scheduler);
    }

    /// <summary>
    /// Creates an empty selectable collection.
    /// </summary>
    public static SelectableCollection<T> Empty<T>(
        SelectionMode mode = SelectionMode.Multiple,
        IScheduler? scheduler = null)
        where T : notnull
    {
        var source = SourceEngine<T>.Empty();

        return Create(source, mode, scheduler);
    }

    /// <summary>
    /// Core factory.
    /// </summary>
    public static SelectableCollection<T> Create<T>(
        SourceEngine<T> source,
        SelectionMode mode = SelectionMode.Multiple,
        IScheduler? scheduler = null)
        where T : notnull
    {
        var wrappers = new ExtendedCollection<T>(
            source,
            scheduler: scheduler);

        return new(wrappers, mode);
    }
}

/// <summary>
/// Represents a collection of items that can be selected, providing methods to manage the selection state of the items.
/// </summary>
/// <param name="collection">The collection of wrappers for the items.</param>
/// <param name="mode">The selection mode for the collection.</param>
/// <typeparam name="T">The type of items in the collection.</typeparam>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "This class wrap a collection.")]
public sealed class SelectableCollection<T>(
    ExtendedCollection<T> collection,
    SelectionMode mode = SelectionMode.Multiple)
    : IDisposable
    where T : notnull
{
    private readonly SelectionEngine<T> _selection = new(collection.Connect(), mode);

    /// <summary>
    /// Gets the selection mode of the collection, indicating whether multiple items can be selected simultaneously or only a single item can be selected at a time.
    /// </summary>
    public SelectionMode Mode => _selection.Mode;

    /// <summary>
    /// Gets the collection of wrappers for the items in the selectable collection. Each wrapper provides information about the selection state of its corresponding item.
    /// </summary>
    public ReadOnlyObservableCollection<T> Items => collection.Items;

    /// <summary>
    /// Gets the collection of selected items in the selectable collection.
    /// </summary>
    public IEnumerable<T> SelectedItems => _selection.SelectedItems;

    /// <summary>
    /// Gets the number of selected items in the selectable collection.
    /// </summary>
    public int SelectedCount => _selection.Count;

    /// <summary>
    /// Selects the specified item in the selectable collection.
    /// </summary>
    /// <param name="item">The item to select.</param>
    public void Select(T item) => _selection.Select(item);

    /// <summary>
    /// Unselects the specified item in the selectable collection.
    /// </summary>
    /// <param name="item">The item to unselect.</param>
    public void Unselect(T item) => _selection.Unselect(item);

    /// <summary>
    /// Toggles the selection state of the specified item in the selectable collection.
    /// </summary>
    /// <param name="item">The item to toggle.</param>
    public void Toggle(T item) => _selection.Toggle(item);

    /// <summary>
    /// Clears the selection in the selectable collection.
    /// </summary>
    public void ClearSelection() => _selection.Clear();

    /// <summary>
    /// Sets the selection to the specified items in the selectable collection.
    /// </summary>
    /// <param name="items">The items to select.</param>
    public void SetSelection(IEnumerable<T> items) => _selection.Set(items);

    /// <summary>
    /// Determines whether the specified item is selected in the selectable collection.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is selected; otherwise, false.</returns>
    public bool IsSelected(T item) => _selection.Contains(item);

    /// <summary>
    /// Disposes the selectable collection and releases all resources.
    /// </summary>
    public void Dispose() => _selection.Dispose();
}
