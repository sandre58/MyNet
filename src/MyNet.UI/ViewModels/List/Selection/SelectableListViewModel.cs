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
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Selection;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;

namespace MyNet.UI.ViewModels.List.Selection;

/// <summary>
/// List view model with selection backed by <see cref="SelectableCollection{T}"/> (no UI wrappers).
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class SelectableListViewModel<T> : ListViewModelBase<T, ExtendedCollection<T>>, ISelectableListViewModel<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListViewModel{T}"/> class.
    /// </summary>
    public SelectableListViewModel(
        ExtendedCollection<T> collection,
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
    /// Initializes a new instance of the <see cref="SelectableListViewModel{T}"/> class.
    /// </summary>
    public SelectableListViewModel(
        ExtendedCollection<T> collection,
        SelectionMode mode,
        ListViewModelOptions<T>? options)
        : this(collection, mode, options?.Filters, options?.Sorting, options?.Grouping, options?.Paging, options?.BusyService, options?.Scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListViewModel{T}"/> class.
    /// </summary>
    public SelectableListViewModel(
        ExtendedCollection<T> collection,
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

    /// <inheritdoc />
    public SelectionMode SelectionMode => _selection.Mode;

    /// <inheritdoc />
    public IReadOnlyList<T> SelectedItems => _selection.SelectedItems;

    /// <inheritdoc />
    public int SelectedCount => _selection.SelectedCount;

    /// <inheritdoc />
    public T? SelectedItem => SelectedItems.FirstOrDefault();

    /// <inheritdoc />
    public bool IsSelected(T item) => _selection.IsSelected(item);

    /// <inheritdoc />
    public void Select(T item) => _selection.Select(item);

    /// <inheritdoc />
    public void Unselect(T item) => _selection.Unselect(item);

    /// <inheritdoc />
    public void Toggle(T item) => _selection.Toggle(item);

    /// <inheritdoc />
    public void ClearSelection() => _selection.ClearSelection();

    /// <inheritdoc />
    public void SetSelection(IEnumerable<T> items) => _selection.SetSelection(items);

    private void HandleSelectionChanged(object? sender, EventArgs e)
    {
        NotifyPropertyChanged(nameof(SelectedItems));
        NotifyPropertyChanged(nameof(SelectedCount));
        NotifyPropertyChanged(nameof(SelectedItem));
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        _selection.SelectionChanged -= HandleSelectionChanged;
        _selection.Dispose();
        base.DisposeManagedResources();
    }
}
