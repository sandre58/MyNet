// -----------------------------------------------------------------------
// <copyright file="ListViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Operators;
using MyNet.Humanizer;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Sorting;
using MyNet.UI.Collections;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.MessageBox;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;
using MyNet.UI.Resources;
using MyNet.UI.Threading;
using MyNet.UI.ViewModels.Display;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.Utilities;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Base view model for managing lists with filtering, sorting, grouping, paging, and CRUD operations.
/// Provides a comprehensive foundation for list-based UI components.
/// </summary>
/// <typeparam name="T">The type of items in the list. Must be a reference type.</typeparam>
/// <typeparam name="TCollection">The collection type managing the items. Must derive from <see cref="ExtendedCollection{T}"/>.</typeparam>
[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public abstract class ListViewModelBase<T, TCollection> : ViewModelBase, IListViewModel<T>, IEnumerable<T>
    where TCollection : ExtendedCollection<T>
    where T : notnull
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
    private readonly BehaviorSubject<PageRequest> _pager = new(new PageRequest(1, int.MaxValue));
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
    private readonly UiObservableCollection<T> _pagedItems = [];
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
    private IDisposable? _pagedDisposable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewModelBase{T, TCollection}"/> class.
    /// </summary>
    /// <param name="collection">The underlying collection managing the data.</param>
    /// <param name="parametersProvider">Optional provider for list parameters (filters, sorting, etc.). Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    protected ListViewModelBase(TCollection collection, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : base(busyService)
    {
        // Initialize list parameters (filters, sorting, display, grouping, paging)
        var parameters = parametersProvider ?? ListParametersProvider.Default;
        Filters = parameters.ProvideFilters();
        Sorting = parameters.ProvideSorting();
        Display = parameters.ProvideDisplay();
        Grouping = parameters.ProvideGrouping();
        Paging = parameters.ProvidePaging();

        // Initialize collection and paged items
        Collection = collection;
        PagedItems = new(_pagedItems);

        // Initialize commands
        ShowFiltersCommand = CommandsManager.Create(ToggleFilters);
        OpenCommand = CommandsManager.Create<T>(x => Open(x), CanOpenItem);
        ClearCommand = CommandsManager.Create(async () => await ClearAsync().ConfigureAwait(false), () => CanRemoveItems(Collection.Source));
        AddCommand = CommandsManager.Create(async () => await AddAsync().ConfigureAwait(false), () => CanAdd);
        EditCommand = CommandsManager.Create<T>(async x => await EditAsync(x).ConfigureAwait(false), CanEditItem);
        EditRangeCommand = CommandsManager.CreateNotNull<IEnumerable<T>>(async x => await EditRangeAsync(x).ConfigureAwait(false), CanEditItems);
        RemoveCommand = CommandsManager.Create<T>(async x => await RemoveAsync(x).ConfigureAwait(false), x => CanRemoveItems(new[] { x }.NotNull()));
        RemoveRangeCommand = CommandsManager.CreateNotNull<IEnumerable<T>>(async x => await RemoveRangeAsync(x).ConfigureAwait(false), CanRemoveItems);
        PreviousCommand = CommandsManager.Create(() => SelectedItem = GetPrevious(), () => GetPrevious() is not null);
        NextCommand = CommandsManager.Create(() => SelectedItem = GetNext(), () => GetNext() is not null);
        FirstCommand = CommandsManager.Create(() => SelectedItem = Items.FirstOrDefault(), () => Items.FirstOrDefault() is not null);
        LastCommand = CommandsManager.Create(() => SelectedItem = Items.LastOrDefault(), () => Items.LastOrDefault() is not null);

        // Subscribe to reactive events for filters, sorting, grouping, paging, and collection changes
        Disposables.AddRange(
        [
            System.Reactive.Linq.Observable.FromEventPattern<FiltersChangedEventArgs>(x => Filters.FiltersChanged += x, x => Filters.FiltersChanged -= x)
                                           .Subscribe(x => OnFiltersChanged(x.EventArgs.Filters)),
            System.Reactive.Linq.Observable.FromEventPattern<SortingChangedEventArgs>(x => Sorting.SortingChanged += x, x => Sorting.SortingChanged -= x)
                                           .Subscribe(x => OnSortChanged(x.EventArgs.SortingProperties)),
            System.Reactive.Linq.Observable.FromEventPattern<GroupingChangedEventArgs>(x => Grouping.GroupingChanged += x, x => Grouping.GroupingChanged -= x)
                                           .Subscribe(x => OnGroupChanged(x.EventArgs.GroupProperties)),
            System.Reactive.Linq.Observable.FromEventPattern<PagingChangedEventArgs>(x => Paging.PagingChanged += x, x => Paging.PagingChanged -= x)
                                           .Subscribe(x => OnPagingChanged(x.EventArgs.Page, x.EventArgs.PageSize)),
            Collection.WhenPropertyChanged(x => x.Count).Subscribe(_ => OnPropertyChanged(nameof(Count))),
            Collection.WhenPropertyChanged(x => x.SourceCount)
                      .Subscribe(_ =>
                      {
                          OnPropertyChanged(nameof(SourceCount));

                          // Mark as modified when source count changes (items added/removed)
                          if (!IsModifiedSuspender.IsSuspended)
                             SetIsModified();
                      }),
            Collection
        ]);

        // Connect filter dialog close request to ShowFilters property
        if (Filters is IDialogViewModel dialog)
            dialog.CloseRequest += (_, _) => ShowFilters = false;

        // Reset all parameters to default state
        Filters.Reset();
        Sorting.Reset();
        Grouping.Reset();
    }

    /// <summary>
    /// Gets the underlying collection managing the data.
    /// This collection handles filtering, sorting, and data transformations.
    /// </summary>
    [CanSetIsModified]
    [CanBeValidated]
    protected TCollection Collection { get; }

    /// <summary>
    /// Gets the filtered and sorted items ready for display.
    /// </summary>
    public ReadOnlyObservableCollection<T> Items => Collection.Items;

    /// <summary>
    /// Gets the source items before any filtering or sorting is applied.
    /// </summary>
    public ReadOnlyObservableCollection<T> Source => Collection.Source;

    /// <summary>
    /// Gets the paged subset of items when paging is enabled.
    /// Contains only the items for the current page.
    /// </summary>
    public ReadOnlyObservableCollection<T> PagedItems { get; }

    /// <summary>
    /// Gets the total number of items in the source collection.
    /// </summary>
    public int SourceCount => Source.Count;

    /// <summary>
    /// Gets or sets the currently selected item in the list.
    /// </summary>
    [CanSetIsModified]
    [DoNotCheckEquality]
    public T? SelectedItem { get; set; }

    /// <summary>
    /// Gets the filters view model for managing list filtering.
    /// </summary>
    [CanNotify(false)]
    public IFiltersViewModel Filters { get; }

    /// <summary>
    /// Gets the sorting view model for managing list sorting.
    /// </summary>
    [CanNotify(false)]
    public ISortingViewModel Sorting { get; }

    /// <summary>
    /// Gets the grouping view model for managing list grouping.
    /// </summary>
    [CanNotify(false)]
    public IGroupingViewModel Grouping { get; }

    /// <summary>
    /// Gets the paging view model for managing list pagination.
    /// </summary>
    [CanNotify(false)]
    public IPagingViewModel Paging { get; }

    /// <summary>
    /// Gets the display view model for managing display modes (grid, list, etc.).
    /// </summary>
    [CanNotify(false)]
    public IDisplayViewModel Display { get; }

    /// <summary>
    /// Gets the currently active filters applied to the collection.
    /// </summary>
    public ICollection CurrentFilters => Collection.Filters;

    /// <summary>
    /// Gets the currently active sorting properties applied to the collection.
    /// </summary>
    public ICollection CurrentSorting => Collection.SortingProperties;

    /// <summary>
    /// Gets the currently active grouping properties.
    /// </summary>
    public UiObservableCollection<IGroupingPropertyViewModel> CurrentGroups { get; } = [];

    /// <summary>
    /// Gets the currently active grouping properties (explicit interface implementation).
    /// </summary>
    ICollection IListViewModel.CurrentGroups => CurrentGroups;

    /// <summary>
    /// Gets a value indicating whether any filters are currently applied.
    /// </summary>
    public bool IsFiltered => Collection.Filters.Any();

    /// <summary>
    /// Gets a value indicating whether any sorting is currently applied.
    /// </summary>
    public bool IsSorted => Collection.SortingProperties.Any();

    /// <summary>
    /// Gets a value indicating whether any grouping is currently applied.
    /// </summary>
    public bool IsGrouped => CurrentGroups.Count > 0;

    /// <summary>
    /// Gets a value indicating whether the list is currently paged (has multiple pages).
    /// </summary>
    public bool IsPaged => CanPage && Paging.TotalPages > 1;

    /// <summary>
    /// Gets or sets a value indicating whether the filters panel is visible.
    /// </summary>
    public bool ShowFilters { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether filtering is enabled for this list.
    /// </summary>
    public virtual bool CanFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether sorting is enabled for this list.
    /// </summary>
    public virtual bool CanSort { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether grouping is enabled for this list.
    /// </summary>
    public virtual bool CanGroup { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether paging is enabled for this list.
    /// </summary>
    public virtual bool CanPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether items can be opened/viewed.
    /// </summary>
    public virtual bool CanOpen { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether new items can be added to the list.
    /// </summary>
    public virtual bool CanAdd { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether items can be edited.
    /// </summary>
    public virtual bool CanEdit { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether items can be removed from the list.
    /// </summary>
    public virtual bool CanRemove { get; set; } = true;

    /// <summary>
    /// Gets the command to toggle the visibility of the filters panel.
    /// </summary>
    public ICommand ShowFiltersCommand { get; }

    /// <summary>
    /// Gets the command to open/view a specific item.
    /// </summary>
    public ICommand OpenCommand { get; }

    /// <summary>
    /// Gets the command to add a new item to the list.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets the command to edit a specific item.
    /// </summary>
    public ICommand EditCommand { get; }

    /// <summary>
    /// Gets the command to edit multiple items at once.
    /// </summary>
    public ICommand EditRangeCommand { get; }

    /// <summary>
    /// Gets the command to remove a specific item from the list.
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets the command to remove multiple items at once.
    /// </summary>
    public ICommand RemoveRangeCommand { get; }

    /// <summary>
    /// Gets the command to clear all items from the list.
    /// </summary>
    public ICommand ClearCommand { get; }

    /// <summary>
    /// Gets the command to select the previous item in the list.
    /// </summary>
    public ICommand PreviousCommand { get; }

    /// <summary>
    /// Gets the command to select the next item in the list.
    /// </summary>
    public ICommand NextCommand { get; }

    /// <summary>
    /// Gets the command to select the first item in the list.
    /// </summary>
    public ICommand FirstCommand { get; }

    /// <summary>
    /// Gets the command to select the last item in the list.
    /// </summary>
    public ICommand LastCommand { get; }

    /// <summary>
    /// Occurs when the sorting has changed.
    /// </summary>
    public event EventHandler<SortedEventArgs>? Sorted;

    /// <summary>
    /// Occurs when the filtering has changed.
    /// </summary>
    public event EventHandler<FilteredEventArgs>? Filtered;

    /// <summary>
    /// Occurs when the grouping has changed.
    /// </summary>
    public event EventHandler<EventArgs>? Grouped;

    /// <summary>
    /// Occurs when the paging has changed (page number or page size).
    /// </summary>
    public event EventHandler<EventArgs>? Paged;

    #region Filtering

    /// <summary>
    /// Called when filters have changed. Applies the new filters to the collection.
    /// </summary>
    /// <param name="filters">The collection of filter view models to apply.</param>
    [SuppressPropertyChangedWarnings]
    protected virtual void OnFiltersChanged(IEnumerable<ICompositeFilterViewModel> filters)
    {
        if (!CanFilter) return;

        // Apply only enabled filters that are not empty
        Collection.Filters.Set(filters.Where(x => x.IsEnabled && !x.Item.IsEmpty()).Select(x => new CompositeFilter(x.Item, x.Operator)));

        OnPropertyChanged(nameof(IsFiltered));
        Filtered?.Invoke(this, new FilteredEventArgs(Collection.Filters));
    }

    /// <summary>
    /// Toggles the visibility of the filters panel.
    /// </summary>
    protected virtual void ToggleFilters() => ShowFilters = !ShowFilters;

    #endregion

    #region Sorting

    /// <summary>
    /// Called when sorting has changed. Applies the new sorting to the collection.
    /// </summary>
    /// <param name="sort">The collection of sorting properties to apply.</param>
    [SuppressPropertyChangedWarnings]
    private void OnSortChanged(IEnumerable<ISortingPropertyViewModel> sort)
    {
        if (!CanSort) return;

        // Apply only enabled sorting properties in their specified order
        Collection.SortingProperties.Set(sort.Where(x => x.IsEnabled).OrderBy(x => x.Order).Select(x => new SortingProperty(x.PropertyName, x.Direction)));

        OnPropertyChanged(nameof(IsSorted));
        Sorted?.Invoke(this, new SortedEventArgs(Collection.SortingProperties));
    }

    #endregion

    #region Grouping

    /// <summary>
    /// Called when grouping has changed. Applies the new grouping to the collection and updates sorting.
    /// </summary>
    /// <param name="groupProperties">The collection of grouping properties to apply.</param>
    [SuppressPropertyChangedWarnings]
    private void OnGroupChanged(IEnumerable<IGroupingPropertyViewModel> groupProperties)
    {
        if (!CanGroup) return;

        // Get active group properties (enabled and with valid property name)
        var activeGroupProperties = groupProperties
            .Where(x => x.IsEnabled && !string.IsNullOrEmpty(x.PropertyName)).OrderBy(x => x.Order).ToList();

        // Early exit if no change
        if (CurrentGroups.Count == 0 && activeGroupProperties.Count == 0) return;

        // Update sorting: group properties first, then existing sorting
        OnSortChanged(activeGroupProperties.Select((x, index) => new SortingPropertyViewModel(x.SortingPropertyName, order: index))
                                           .Concat(Collection.SortingProperties.Select((x, index) => new SortingPropertyViewModel(x.PropertyName, x.Direction, index + activeGroupProperties.Count))));

        CurrentGroups.Set(activeGroupProperties);
        OnPropertyChanged(nameof(IsGrouped));
        Grouped?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Paging

    /// <summary>
    /// Called when paging has changed (page number or page size).
    /// </summary>
    /// <param name="page">The new page number.</param>
    /// <param name="pageSize">The new page size.</param>
    [SuppressPropertyChangedWarnings]
    private void OnPagingChanged(int page, int pageSize)
    {
        if (!CanPage) return;

        _pager.OnNext(new PageRequest(page, pageSize));
        OnPropertyChanged(nameof(IsPaged));
        Paged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Updates the paging view model with the response from the paging operation.
    /// </summary>
    /// <param name="response">The page response containing total size, pages, and current page.</param>
    private void UpdatePaging(IPageResponse response)
        => Paging.Update(new(response.TotalSize == 0 ? 1 : response.TotalSize, response.Pages, response.Page));

    /// <summary>
    /// Called when the CanPage property changes. Initializes or disposes the paging pipeline.
    /// </summary>
    protected virtual void OnCanPageChanged()
    {
        _pagedDisposable?.Dispose();

        if (!CanPage)
            return;

        _pagedItems.Clear();
        _pagedDisposable = SubscribePager(_pager);
        OnPagingChanged(Paging.CurrentPage, Paging.PageSize);
    }

    /// <summary>
    /// Subscribes to the pager observable and creates the paging pipeline.
    /// </summary>
    /// <param name="pager">The observable page request stream.</param>
    /// <returns>A disposable subscription to the paging pipeline.</returns>
    protected virtual IDisposable SubscribePager(IObservable<PageRequest> pager)
        => Collection.Connect().Page(pager).Do(x => UpdatePaging(x.Response)).Bind(_pagedItems).Subscribe();

    #endregion

    #region Navigation

    /// <summary>
    /// Gets the previous item relative to the currently selected item.
    /// </summary>
    /// <returns>The previous item, or null if there is no previous item.</returns>
    public T? GetPrevious()
    {
        if (SelectedItem is null) return default;

        var currentIndex = Items.IndexOf(SelectedItem);
        return Items.GetByIndex(currentIndex - 1);
    }

    /// <summary>
    /// Gets the next item relative to the currently selected item.
    /// </summary>
    /// <returns>The next item, or null if there is no next item.</returns>
    public T? GetNext()
    {
        if (SelectedItem is null) return default;

        var currentIndex = Items.IndexOf(SelectedItem);
        return Items.GetByIndex(currentIndex + 1);
    }

    #endregion

    #region Open

    /// <summary>
    /// Determines whether the specified item can be opened.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>true if the item can be opened; otherwise, false.</returns>
    protected virtual bool CanOpenItem(T? item) => CanOpen && item is not null;

    /// <summary>
    /// Opens the specified item for viewing.
    /// </summary>
    /// <param name="item">The item to open.</param>
    /// <param name="selectedTab">Optional tab index to select when opening.</param>
    public void Open(T? item, int? selectedTab = null)
    {
        if (!CanOpen || item is null) return;

        OpenCore(item, selectedTab);
    }

    /// <summary>
    /// Core implementation for opening an item. Override in derived classes to provide custom behavior.
    /// </summary>
    /// <param name="item">The item to open.</param>
    /// <param name="selectedTab">Optional tab index to select when opening.</param>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Used by children classes")]
    protected virtual void OpenCore(T item, int? selectedTab = null) { }

    #endregion

    #region Add

    /// <summary>
    /// Asynchronously adds a new item to the list.
    /// Creates a new item, then adds it to the collection.
    /// </summary>
    public virtual async Task AddAsync()
    {
        if (!CanAdd) return;

        var item = await CreateNewItemAsync().ConfigureAwait(false);

        if (item is not null)
        {
            await ExecuteAsync(() =>
               {
                   AddItemCore(item);
                   OnAddCompleted(item);
               }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new item to be added to the list. Override to customize item creation.
    /// </summary>
    /// <returns>The newly created item, or null if creation was cancelled.</returns>
    protected virtual async Task<T?> CreateNewItemAsync()
      => await Task.FromResult(Activator.CreateInstance<T>()).ConfigureAwait(false);

    /// <summary>
    /// Adds the specified item to the underlying collection. Override to customize the add behavior.
    /// </summary>
    /// <param name="item">The item to add.</param>
    protected virtual void AddItemCore(T item) => Collection.Add(item);

    /// <summary>
    /// Called after an item has been successfully added. Override to perform post-add operations.
    /// </summary>
    /// <param name="item">The item that was added.</param>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Used by children classes")]
    protected virtual void OnAddCompleted(T item) { }

    #endregion

    #region Edit

    /// <summary>
    /// Determines whether the specified item can be edited.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>true if the item can be edited; otherwise, false.</returns>
    protected virtual bool CanEditItem(T? item) => CanEdit && item is not null;

    /// <summary>
    /// Determines whether the specified items can be edited.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <returns>true if the items can be edited; otherwise, false.</returns>
    protected virtual bool CanEditItems(IEnumerable<T> items) => CanEdit && items.Any();

    /// <summary>
    /// Asynchronously edits the specified item.
    /// Updates the item, then replaces it in the collection.
    /// </summary>
    /// <param name="oldItem">The item to edit.</param>
    public virtual async Task EditAsync(T? oldItem)
    {
        if (!CanEditItem(oldItem) || oldItem is null) return;

        var newItem = await UpdateItemAsync(oldItem).ConfigureAwait(false);

        if (newItem is not null)
        {
            await ExecuteAsync(() =>
               {
                   EditItemCore(oldItem, newItem);
                   OnEditCompleted(oldItem, newItem);
               }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Replaces the old item with the new item in the collection. Override to customize the edit behavior.
    /// </summary>
    /// <param name="oldItem">The item to replace.</param>
    /// <param name="newItem">The replacement item.</param>
    protected virtual void EditItemCore(T oldItem, T newItem)
    {
        _ = Collection.Remove(oldItem);
        Collection.Add(newItem);
    }

    /// <summary>
    /// Updates the specified item. Override to show an edit dialog or perform custom update logic.
    /// </summary>
    /// <param name="oldItem">The item to update.</param>
    /// <returns>The updated item, or null if the update was cancelled.</returns>
    protected virtual async Task<T?> UpdateItemAsync(T oldItem)
        => await Task.FromResult(oldItem).ConfigureAwait(false);

    /// <summary>
    /// Called after an item has been successfully edited. Override to perform post-edit operations.
    /// </summary>
    /// <param name="oldItem">The original item before editing.</param>
    /// <param name="newItem">The updated item after editing.</param>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Used by children classes")]
    protected virtual void OnEditCompleted(T oldItem, T newItem) { }

    /// <summary>
    /// Asynchronously edits multiple items at once.
    /// </summary>
    /// <param name="oldItems">The items to edit.</param>
    public virtual async Task EditRangeAsync(IEnumerable<T> oldItems)
    {
        var oldItemsList = oldItems.ToList();
        if (!CanEditItems(oldItemsList)) return;

        var newItems = await UpdateRangeAsync(oldItemsList).ConfigureAwait(false);

        var newItemsList = newItems.ToList();
        if (newItemsList.Count != 0)
        {
            await ExecuteAsync(() => OnEditRangeCompleted(oldItemsList, newItemsList)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Updates multiple items. Override to show a bulk edit dialog or perform custom update logic.
    /// </summary>
    /// <param name="oldItems">The items to update.</param>
    /// <returns>The updated items.</returns>
    protected virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> oldItems)
        => await Task.FromResult(oldItems).ConfigureAwait(false);

    /// <summary>
    /// Called after multiple items have been successfully edited. Override to perform post-edit operations.
    /// </summary>
    /// <param name="oldItems">The original items before editing.</param>
    /// <param name="newItems">The updated items after editing.</param>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Used by children classes")]
    protected virtual void OnEditRangeCompleted(IEnumerable<T> oldItems, IEnumerable<T> newItems) { }

    #endregion

    #region Remove

    /// <summary>
    /// Determines whether the specified items can be removed.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <returns>true if the items can be removed; otherwise, false.</returns>
    protected virtual bool CanRemoveItems(IEnumerable<T> items) => CanRemove && items.Any();

    /// <summary>
    /// Asynchronously clears all items from the list.
    /// </summary>
    protected virtual async Task ClearAsync() => await RemoveRangeAsync(Collection.Source).ConfigureAwait(false);

    /// <summary>
    /// Asynchronously removes the specified item from the list.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public async Task RemoveAsync(T? item)
    {
        var items = new[] { item }.NotNull();
        var itemsList = items.ToList();

        if (!CanRemoveItems(itemsList)) return;

        await RemoveRangeAsync(itemsList).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously removes multiple items from the list.
    /// Shows a confirmation dialog before removing.
    /// </summary>
    /// <param name="oldItems">The items to remove.</param>
    public virtual async Task RemoveRangeAsync(IEnumerable<T> oldItems)
    {
        if (!CanRemove) return;

        var cancelEventArgs = new CancelEventArgs(false);
        var oldItemsList = oldItems.ToList();

        // Request confirmation before removing
        await OnRemovingRequestedAsync(oldItemsList, cancelEventArgs).ConfigureAwait(false);

        if (!cancelEventArgs.Cancel && oldItemsList.Count != 0)
        {
            await ExecuteAsync(() =>
                {
                    RemoveItemsCore(oldItemsList);
                    OnRemoveCompleted(oldItemsList);
                }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes the specified items from the underlying collection. Override to customize the remove behavior.
    /// </summary>
    /// <param name="oldItems">The items to remove.</param>
    protected virtual void RemoveItemsCore(IEnumerable<T> oldItems) => Collection.RemoveMany(oldItems);

    /// <summary>
    /// Called before items are removed. Shows a confirmation dialog by default.
    /// Override to customize the confirmation behavior or skip confirmation.
    /// </summary>
    /// <param name="oldItems">The items to be removed.</param>
    /// <param name="cancelEventArgs">Event args to cancel the removal if needed.</param>
    protected virtual async Task OnRemovingRequestedAsync(IEnumerable<T> oldItems, CancelEventArgs cancelEventArgs)
        => cancelEventArgs.Cancel = await DialogManager.ShowQuestionAsync(nameof(MessageResources.XItemsRemovingQuestion).TranslateAndFormatWithCount(oldItems.Count()), UiResources.Removing).ConfigureAwait(false) != MessageBoxResult.Yes;

    /// <summary>
    /// Called after items have been successfully removed. Override to perform post-remove operations.
    /// </summary>
    /// <param name="oldItems">The items that were removed.</param>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Used by children classes")]
    protected virtual void OnRemoveCompleted(IEnumerable<T> oldItems) { }

    #endregion

    #region Refresh

    /// <summary>
    /// Synchronously refreshes the list. Calls RefreshAsync synchronously.
    /// </summary>
    public void Refresh() => RefreshAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Asynchronously refreshes the list. Override to provide custom refresh logic.
    /// </summary>
    public virtual async Task RefreshAsync() => await ExecuteAsync(RefreshCore).ConfigureAwait(false);

    /// <summary>
    /// Core refresh logic. Override in derived classes to implement custom refresh behavior.
    /// </summary>
    protected virtual void RefreshCore() { }

    #endregion

    #region ICollection Implementation

    /// <summary>
    /// Gets the number of items in the filtered collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets a value indicating whether access to the collection is synchronized (thread safe).
    /// </summary>
    public bool IsSynchronized => true;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the collection.
    /// </summary>
    public object SyncRoot { get; } = new();

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The zero-based index in array at which copying begins.</param>
    public void CopyTo(Array array, int index) => Collection.CopyTo((T[])array, index);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator GetEnumerator() => Collection.GetEnumerator();

    /// <summary>
    /// Returns a generic enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A generic enumerator for the collection.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => Collection.GetEnumerator();

    #endregion

    #region Cleanup

    /// <summary>
    /// Releases resources and performs cleanup operations.
    /// </summary>
    protected override void Cleanup()
    {
        _pager.Dispose();
        _pagedDisposable?.Dispose();
        _pagedItems.Dispose();
        base.Cleanup();
    }

    #endregion
}
