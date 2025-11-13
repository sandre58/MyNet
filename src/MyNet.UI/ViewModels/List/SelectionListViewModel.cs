// -----------------------------------------------------------------------
// <copyright file="SelectionListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using MyNet.Observable.Attributes;
using MyNet.Observable.Collections.Providers;
using MyNet.UI.Commands;
using MyNet.UI.Loading;
using MyNet.UI.Selection;
using MyNet.UI.Selection.Models;
using MyNet.UI.Threading;
using MyNet.Utilities;
using MyNet.Utilities.Providers;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Base view model for managing lists with item selection support.
/// Extends <see cref="WrapperListViewModel{T, TWrapper, TCollection}"/> to provide selection-specific functionality
/// including single/multiple selection modes, selection commands, and selection state management.
/// </summary>
/// <typeparam name="T">The type of items in the list. Must be a reference type.</typeparam>
/// <typeparam name="TCollection">The collection type managing selectable items. Must derive from <see cref="SelectableCollection{T}"/>.</typeparam>
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
[CanBeValidatedForDeclaredClassOnly(false)]
public abstract class SelectionListViewModel<T, TCollection> : WrapperListViewModel<T, SelectedWrapper<T>, TCollection>
    where TCollection : SelectableCollection<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionListViewModel{T, TCollection}"/> class.
    /// </summary>
    /// <param name="collection">The underlying selectable collection managing the data.</param>
    /// <param name="parametersProvider">Optional provider for list parameters (filters, sorting, etc.). Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    protected SelectionListViewModel(
        TCollection collection,
        IListParametersProvider? parametersProvider = null,
        IBusyService? busyService = null)
      : base(collection, parametersProvider, busyService)
    {
        // Selection commands
        SelectCommand = CommandsManager.CreateNotNull<T>(Collection.Select, CanSelect);
        SelectItemsCommand = CommandsManager.CreateNotNull<IEnumerable<T>>(Collection.Select, CanSelect);
        UnselectCommand = CommandsManager.CreateNotNull<T>(Collection.Unselect, CanUnselect);
        UnselectItemsCommand = CommandsManager.CreateNotNull<IEnumerable<T>>(Collection.Unselect, CanUnselect);
        SelectAllCommand = CommandsManager.Create(SelectAll, () => Wrappers.Any(x => CanChangeSelectedState(x, true)));
        UnselectAllCommand = CommandsManager.Create(UnselectAll, () => Wrappers.Any(x => CanChangeSelectedState(x, false)));
        ClearSelectionCommand = CommandsManager.Create(ClearSelection, () => WrappersSource.Any(x => CanChangeSelectedState(x, false)));

        // Item operation commands on selected items
        OpenSelectedItemCommand = CommandsManager.Create(
            () => Open(SelectedItem),
            () => CanOpenItem(SelectedItem) && SelectedItems.Count() == 1);

        OpenTabSelectedItemCommand = CommandsManager.CreateNotNull<object>(
            x => Open(SelectedItem, (int)x),
            _ => CanOpenItem(SelectedItem) && SelectedItems.Count() == 1);

        EditSelectedItemCommand = CommandsManager.Create(
            async () => await EditAsync(SelectedItem).ConfigureAwait(false),
            () => CanEditItem(SelectedItem) && SelectedItems.Count() == 1);

        EditSelectedItemsCommand = CommandsManager.Create(
            async () => await EditRangeAsync(SelectedItems).ConfigureAwait(false),
            () => CanEditItems(SelectedItems) && SelectedWrappers.Count > 0);

        RemoveSelectedItemCommand = CommandsManager.Create(
            async () => await RemoveAsync(SelectedItem).ConfigureAwait(false),
            () => SelectedItems.Count() == 1 && CanRemoveItems(SelectedItems));

        RemoveSelectedItemsCommand = CommandsManager.Create(
            async () => await RemoveRangeAsync(SelectedItems).ConfigureAwait(false),
            () => CanRemoveItems(SelectedItems) && SelectedWrappers.Count > 0);

        // Subscribe to selection changes and wrapper removals
        Disposables.AddRange(
        [
            System.Reactive.Linq.Observable
        .FromEventPattern(x => Collection.SelectionChanged += x, x => Collection.SelectionChanged -= x)
                .Subscribe(_ => OnSelectionChanged()),

            Collection.Wrappers
      .ToObservableChangeSet()
          .OnItemRemoved(x => x.IsSelected = false)
             .Subscribe()
        ]);
    }

    #region Properties

    /// <summary>
    /// Gets or sets the selection mode (single or multiple selection).
    /// </summary>
    public SelectionMode SelectionMode
    {
        get => Collection.SelectionMode;
        set => Collection.SelectionMode = value;
    }

    /// <summary>
    /// Gets the dictionary of preset selection commands.
    /// Use this to define common selection presets (e.g., "All Adults", "Active Items").
    /// </summary>
    public IDictionary<string, ICommand> PresetSelections { get; } = new Dictionary<string, ICommand>();

    /// <summary>
    /// Gets the collection of selected wrappers.
    /// Use this when you need access to wrapper-specific properties (IsSelectable, etc.).
    /// </summary>
    public ReadOnlyObservableCollection<SelectedWrapper<T>> SelectedWrappers => Collection.SelectedWrappers;

    /// <summary>
    /// Gets the collection of selected items (unwrapped).
    /// This is the primary collection to use for working with selected data.
    /// </summary>
    public IEnumerable<T> SelectedItems => Collection.SelectedItems;

    /// <summary>
    /// Gets the first selected wrapper, or null if no items are selected.
    /// </summary>
    public SelectedWrapper<T>? SelectedWrapper => SelectedWrappers.FirstOrDefault();

    /// <summary>
    /// Gets the first selected item, or null if no items are selected.
    /// Hides the base <see cref="ListViewModelBase{T, TCollection}.SelectedItem"/> property
    /// to provide selection-specific behavior.
    /// </summary>
    public new T? SelectedItem => SelectedItems.FirstOrDefault();

    /// <summary>
    /// Gets or sets whether all items are selected.
    /// - true: All items are selected
    /// - false: No items are selected
    /// - null: Some items are selected (indeterminate state)
    /// Useful for "Select All" checkbox with three states.
    /// </summary>
    public bool? AreAllSelected
    {
        get
        {
            var selected = Wrappers.Select(item => item.IsSelected).Distinct().ToList();
            return selected.Count == 1 ? selected.Single() : null;
        }

        set
        {
            if (value.HasValue)
                Collection.SelectAll(value.Value);
        }
    }

    #endregion

    #region Commands

    /// <summary>
    /// Gets the command to open the selected item.
    /// Enabled only when exactly one item is selected.
    /// </summary>
    public ICommand OpenSelectedItemCommand { get; }

    /// <summary>
    /// Gets the command to open the selected item in a specific tab.
    /// Enabled only when exactly one item is selected.
    /// </summary>
    public ICommand OpenTabSelectedItemCommand { get; }

    /// <summary>
    /// Gets the command to edit the selected item.
    /// Enabled only when exactly one item is selected.
    /// </summary>
    public ICommand EditSelectedItemCommand { get; }

    /// <summary>
    /// Gets the command to edit all selected items.
    /// Enabled only when one or more items are selected.
    /// </summary>
    public ICommand EditSelectedItemsCommand { get; }

    /// <summary>
    /// Gets the command to remove the selected item.
    /// Enabled only when exactly one item is selected.
    /// </summary>
    public ICommand RemoveSelectedItemCommand { get; }

    /// <summary>
    /// Gets the command to remove all selected items.
    /// Enabled only when one or more items are selected.
    /// </summary>
    public ICommand RemoveSelectedItemsCommand { get; }

    /// <summary>
    /// Gets the command to select a specific item.
    /// </summary>
    public ICommand SelectCommand { get; }

    /// <summary>
    /// Gets the command to select multiple items.
    /// </summary>
    public ICommand SelectItemsCommand { get; }

    /// <summary>
    /// Gets the command to unselect a specific item.
    /// </summary>
    public ICommand UnselectCommand { get; }

    /// <summary>
    /// Gets the command to unselect multiple items.
    /// </summary>
    public ICommand UnselectItemsCommand { get; }

    /// <summary>
    /// Gets the command to select all visible (filtered) items.
    /// </summary>
    public ICommand SelectAllCommand { get; }

    /// <summary>
    /// Gets the command to unselect all visible (filtered) items.
    /// </summary>
    public ICommand UnselectAllCommand { get; }

    /// <summary>
    /// Gets the command to clear the entire selection (all items, including those not visible).
    /// </summary>
    public ICommand ClearSelectionCommand { get; }

    #endregion

    #region Selection Logic

    /// <summary>
    /// Determines whether the selection meets the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to test against all selected items.</param>
    /// <returns>True if any items are selected and all selected items match the predicate; otherwise, false.</returns>
    protected virtual bool SelectionIsAvailable(Func<T, bool> predicate)
   => SelectedWrappers.Count > 0 && SelectedItems.All(predicate);

    /// <summary>
    /// Determines whether the selection state of a wrapper can be changed to the specified value.
    /// </summary>
    /// <param name="item">The wrapper to check.</param>
    /// <param name="value">The desired selection state.</param>
    /// <returns>True if the wrapper is selectable and its current state differs from the desired state; otherwise, false.</returns>
    protected virtual bool CanChangeSelectedState(SelectedWrapper<T>? item, bool value)
            => item?.IsSelectable == true && item.IsSelected != value;

    /// <summary>
    /// Determines whether the selection state of an item can be changed to the specified value.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <param name="value">The desired selection state.</param>
    /// <returns>True if the item's wrapper is selectable and its current state differs from the desired state; otherwise, false.</returns>
    protected virtual bool CanChangeSelectedState(T item, bool value)
        => CanChangeSelectedState(WrappersSource.First(x => ReferenceEquals(x.Item, item)), value);

    /// <summary>
    /// Determines whether the specified item can be selected.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item can be selected; otherwise, false.</returns>
    protected virtual bool CanSelect(T item) => CanChangeSelectedState(item, true);

    /// <summary>
    /// Determines whether any of the specified items can be selected.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <returns>True if at least one item can be selected; otherwise, false.</returns>
    protected virtual bool CanSelect(IEnumerable<T> items) => items.Any(CanSelect);

    /// <summary>
    /// Determines whether the specified item can be unselected.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item can be unselected; otherwise, false.</returns>
    protected virtual bool CanUnselect(T item) => CanChangeSelectedState(item, false);

    /// <summary>
    /// Determines whether any of the specified items can be unselected.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <returns>True if at least one item can be unselected; otherwise, false.</returns>
    protected virtual bool CanUnselect(IEnumerable<T> items) => items.Any(CanUnselect);

    /// <summary>
    /// Updates the selection to match the specified items.
    /// Clears the current selection and selects only the specified items.
    /// </summary>
    /// <param name="items">The items to select.</param>
    public void UpdateSelection(IEnumerable<T> items) => Collection.SetSelection(items);

    /// <summary>
    /// Selects all visible (filtered) items.
    /// </summary>
    protected virtual void SelectAll() => Collection.SelectAll(true);

    /// <summary>
    /// Unselects all visible (filtered) items.
    /// </summary>
    protected virtual void UnselectAll() => Collection.SelectAll(false);

    /// <summary>
    /// Clears the entire selection (all items, including those not visible due to filtering).
    /// </summary>
    protected virtual void ClearSelection() => Collection.ClearSelection();

    /// <summary>
    /// Applies an action to all currently selected items.
    /// </summary>
    /// <param name="action">The action to apply to each selected item.</param>
    protected virtual void ApplyOnSelection(Action<T> action) => SelectedItems.ForEach(action);

    /// <summary>
    /// Called when the selection changes.
    /// Updates dependent properties and clears filters if selected items are not visible.
    /// </summary>
    [SuppressPropertyChangedWarnings]
    protected virtual void OnSelectionChanged()
    {
        // Notify property changes
        OnPropertyChanged(nameof(SelectedItems));
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(SelectedWrapper));
        OnPropertyChanged(nameof(AreAllSelected));

        // Clear filters if selected items are not visible due to current filters
        var selectedItemsNotInFilteredItems = SelectedItems.Where(x => !Items.Contains(x)).ToList();

        if (selectedItemsNotInFilteredItems.Count != 0)
            Filters.Clear();
    }

    #endregion
}

/// <summary>
/// Concrete implementation of <see cref="SelectionListViewModel{T, TCollection}"/>
/// using the default <see cref="SelectableCollection{T}"/> collection type.
/// Provides multiple constructors for different data source scenarios with built-in selection support.
/// </summary>
/// <typeparam name="T">The type of items in the list. Must be a reference type.</typeparam>
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
[CanBeValidatedForDeclaredClassOnly(false)]
public class SelectionListViewModel<T> : SelectionListViewModel<T, SelectableCollection<T>>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionListViewModel{T}"/> class
    /// from an existing collection.
    /// </summary>
    /// <param name="source">The source collection of items.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="selectionMode">The selection mode (single or multiple). Default is multiple.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public SelectionListViewModel(ICollection<T> source,
                                  IListParametersProvider? parametersProvider = null,
                                  SelectionMode selectionMode = SelectionMode.Multiple,
                                  IBusyService? busyService = null)
        : base(SelectableCollectionFactory.FromCollection(source, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent),
               parametersProvider,
               busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionListViewModel{T}"/> class
    /// from an items provider.
    /// </summary>
    /// <param name="source">The items provider.</param>
    /// <param name="loadItems">Whether to load items immediately. Default is true.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="selectionMode">The selection mode (single or multiple). Default is multiple.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public SelectionListViewModel(IItemsProvider<T> source,
                                  bool loadItems = true,
                                  IListParametersProvider? parametersProvider = null,
                                  SelectionMode selectionMode = SelectionMode.Multiple,
                                  IBusyService? busyService = null)
        : base(SelectableCollectionFactory.FromItemsProvider(source, loadItems, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent),
               parametersProvider,
               busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionListViewModel{T}"/> class
    /// from a source provider.
    /// </summary>
    /// <param name="source">The source provider.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="selectionMode">The selection mode (single or multiple). Default is multiple.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public SelectionListViewModel(ISourceProvider<T> source,
                                  IListParametersProvider? parametersProvider = null,
                                  SelectionMode selectionMode = SelectionMode.Multiple,
                                  IBusyService? busyService = null)
        : base(SelectableCollectionFactory.FromSourceProvider(source, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent),
               parametersProvider,
               busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionListViewModel{T}"/> class
    /// from an observable change set.
    /// </summary>
    /// <param name="source">The observable change set of items.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="selectionMode">The selection mode (single or multiple). Default is multiple.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public SelectionListViewModel(IObservable<IChangeSet<T>> source,
                                  IListParametersProvider? parametersProvider = null,
                                  SelectionMode selectionMode = SelectionMode.Multiple,
                                  IBusyService? busyService = null)
  : base(SelectableCollectionFactory.FromObservable(source, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent),
         parametersProvider,
         busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionListViewModel{T}"/> class
    /// with an empty collection.
    /// </summary>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="selectionMode">The selection mode (single or multiple). Default is multiple.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public SelectionListViewModel(IListParametersProvider? parametersProvider = null,
                                  SelectionMode selectionMode = SelectionMode.Multiple,
                                  IBusyService? busyService = null)
        : base(SelectableCollectionFactory.Empty<T>(selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent),
               parametersProvider,
               busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionListViewModel{T}"/> class
    /// with a pre-configured selectable collection.
    /// </summary>
    /// <param name="collection">The selectable collection to use.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    protected SelectionListViewModel(SelectableCollection<T> collection,
                                     IListParametersProvider? parametersProvider = null,
                                     IBusyService? busyService = null)
        : base(collection, parametersProvider, busyService) { }
}
