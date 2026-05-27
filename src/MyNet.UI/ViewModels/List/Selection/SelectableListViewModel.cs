// -----------------------------------------------------------------------
// <copyright file="SelectableListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Concurrency;
using MyNet.Observable.Collections.Selection;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.UI.ViewModels.List.Wrappers;

namespace MyNet.UI.ViewModels.List.Selection;

/// <summary>
/// Provides a list view model that supports item selection, allowing for single or multiple selection modes.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class SelectableListViewModel<T> : WrapperListViewModel<T, SelectedWrapper<T>>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListViewModel{T}"/> class.
    /// </summary>
    public SelectableListViewModel(
        ExtendedWrapperCollection<T, SelectedWrapper<T>> collection,
        SelectionMode mode = SelectionMode.Multiple,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
        : this(
            collection,
            new SelectableCollectionSelectionManager<T>(collection, mode),
            filters,
            sorting,
            grouping,
            paging,
            busyService,
            scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListViewModel{T}"/> class with options.
    /// </summary>
    public SelectableListViewModel(ExtendedWrapperCollection<T, SelectedWrapper<T>> collection, SelectionMode mode, ListViewModelOptions<T>? options)
        : this(collection, mode, options?.Filters, options?.Sorting, options?.Grouping, options?.Paging, options?.BusyService, options?.Scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListViewModel{T}"/> class with a custom selection manager.
    /// </summary>
    public SelectableListViewModel(
        ExtendedWrapperCollection<T, SelectedWrapper<T>> collection,
        ISelectionManager<T> selectionManager,
        IFiltersViewModel<T>? filters = null,
        ISortingViewModel<T>? sorting = null,
        IGroupingViewModel<T>? grouping = null,
        IPagingViewModel? paging = null,
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
        : base(collection, filters, sorting, grouping, paging, busyService, scheduler)
    {
        _selection = selectionManager ?? throw new ArgumentNullException(nameof(selectionManager));
        _selection.SelectionChanged += HandleSelectionChanged;
    }

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup.")]
    private readonly ISelectionManager<T> _selection;

    /// <summary>
    /// Gets the selection mode enforced by the selection engine. This property indicates whether the selection engine allows for single selection (only one item can be selected at a time) or multiple selection (multiple items can be selected simultaneously). The selection mode determines how the selection state is managed and updated when items are selected, deselected, or toggled, ensuring that the defined selection rules are consistently applied throughout the lifecycle of the selection engine.
    /// </summary>
    public SelectionMode SelectionMode => _selection.Mode;

    /// <summary>
    /// Gets the list of selected items in the collection.
    /// </summary>
    public IReadOnlyList<T> SelectedItems => _selection.SelectedItems;

    /// <summary>
    /// Gets the count of selected items in the collection.
    /// </summary>
    public int SelectedCount => _selection.SelectedCount;

    /// <summary>
    /// Gets the currently selected item in the collection.
    /// </summary>
    public T? SelectedItem => SelectedItems.FirstOrDefault();

    /// <summary>
    /// Selects the specified item in the collection.
    /// </summary>
    public void Select(T item) => _selection.Select(item);

    /// <summary>
    /// Toggles the selection state of the specified item in the collection.
    /// </summary>
    public void Toggle(T item) => _selection.Toggle(item);

    /// <summary>
    /// Clears the selection in the collection.
    /// </summary>
    public void ClearSelection() => _selection.ClearSelection();

    /// <summary>
    /// Sets the selection to the specified items in the collection.
    /// </summary>
    public void SetSelection(IEnumerable<T> items) => _selection.SetSelection(items);

    private void HandleSelectionChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SelectedItems));
        OnPropertyChanged(nameof(SelectedCount));
        OnPropertyChanged(nameof(SelectedItem));
    }

    /// <summary>
    /// Cleans up resources used by the view model.
    /// </summary>
    protected override void Cleanup()
    {
        _selection.SelectionChanged -= HandleSelectionChanged;
        _selection.Dispose();
        base.Cleanup();
    }
}
