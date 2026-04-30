// -----------------------------------------------------------------------
// <copyright file="FiltersViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using DynamicData.Binding;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.Observable.Extensions;
using MyNet.UI.Commands;
using MyNet.Utilities;
using MyNet.Utilities.Comparison;
using MyNet.Utilities.Deferring;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// View model for managing a collection of composite filters.
/// Provides commands to add, remove, and configure filters with automatic or manual application.
/// Supports multiple filter types combined with logical operators (AND/OR).
/// </summary>
/// <remarks>
/// This class implements <see cref="ICollection{T}"/> and <see cref="INotifyCollectionChanged"/>
/// to allow direct manipulation of filters while providing change notifications.
/// Changes are deferred and batched for optimal performance.
/// <para><strong>Key Features:</strong></para>
/// <list type="bullet">
/// <item>Automatic or manual filter application</item>
/// <item>Deferred execution for batch changes</item>
/// <item>Support for composite filters with logical operators</item>
/// <item>Commands for common filter operations</item>
/// </list>
/// </remarks>
[CanBeValidated(false)]
[CanSetIsModified(false)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "It's a viewModel")]
public class FiltersViewModel : EditableObject, IFiltersViewModel, ICollection<ICompositeFilterViewModel>, INotifyCollectionChanged
{
    private readonly Deferrer _filtersChangedDeferrer;
    private ICollection<ICompositeFilterViewModel>? _currentFilters;

    /// <summary>
    /// Gets the collection of composite filters managed by this view model.
    /// </summary>
    protected FiltersCollection CompositeFilters { get; } = [];

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether filters are automatically applied when changed.
    /// When true, filter changes immediately trigger <see cref="FiltersChanged"/> event.
    /// When false, filters must be manually applied using <see cref="Apply"/> or <see cref="ApplyCommand"/>.
    /// Default is true.
    /// </summary>
    public bool AutoFilter { get; set; } = true;

    /// <summary>
    /// Gets the count of currently active (enabled and non-empty) filters.
    /// </summary>
    public int ActiveCount => CompositeFilters.Count(x => x.IsEnabled && !x.Item.IsEmpty());

    #endregion

    #region Commands

    /// <summary>
    /// Gets the command to add a new filter.
    /// Override <see cref="Add()"/> to implement custom filter creation.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets the command to remove a specific composite filter.
    /// Parameter: ICompositeFilterViewModel (the filter to remove).
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets the command to clear all filters.
    /// Enabled when there are filters in the collection.
    /// </summary>
    public ICommand ClearCommand { get; }

    /// <summary>
    /// Gets the command to clear dirty filters.
    /// Override <see cref="ClearDirtyFilters"/> to implement custom logic.
    /// </summary>
    public ICommand ClearDirtyFiltersCommand { get; }

    /// <summary>
    /// Gets the command to refresh filters with the last applied state.
    /// </summary>
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// Gets the command to reset all filters to their default state.
    /// </summary>
    public ICommand ResetCommand { get; }

    /// <summary>
    /// Gets the command to manually apply filters.
    /// Useful when <see cref="AutoFilter"/> is false.
    /// </summary>
    public ICommand ApplyCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the filter configuration has changed and filters are applied.
    /// Subscribers can react to apply the new filters to their collections.
    /// Fired after all pending changes are applied (deferred execution).
    /// </summary>
    public event EventHandler<FiltersChangedEventArgs>? FiltersChanged;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersViewModel"/> class.
    /// </summary>
    public FiltersViewModel()
    {
        _filtersChangedDeferrer = new Deferrer(OnFiltersChanged);

        // Initialize commands
        ClearCommand = CommandsManager.Create(Clear, () => Count > 0);
        ClearDirtyFiltersCommand = CommandsManager.Create(ClearDirtyFilters, () => Count > 0);
        AddCommand = CommandsManager.Create(Add);
        RefreshCommand = CommandsManager.Create(Refresh);
        ApplyCommand = CommandsManager.Create(Apply);
        ResetCommand = CommandsManager.Create(Reset);
        RemoveCommand = CommandsManager.CreateNotNull<ICompositeFilterViewModel>(x => Remove(x));

        // Subscribe to changes with deferred execution
        Disposables.AddRange(
        [
    CompositeFilters.ToObservableChangeSet().SubscribeAll(() => _filtersChangedDeferrer.DeferOrExecute()),
        this.WhenPropertyChanged(x => x.AutoFilter).Subscribe(_ => _filtersChangedDeferrer.DeferOrExecute())
        ]);

        CompositeFilters.CollectionChanged += HandleCollectionChanged;
    }

    #endregion

    #region Filter Operations

    /// <summary>
    /// Creates a deferral scope for batch changes.
    /// Changes made within the scope are applied and notified only when the scope is disposed.
    /// </summary>
    /// <returns>A disposable deferral scope.</returns>
    protected IDisposable Defer() => _filtersChangedDeferrer.Defer();

    /// <summary>
    /// Defers the filter changed notification or executes it immediately if not already deferred.
    /// </summary>
    protected void DeferOrExecute() => _filtersChangedDeferrer.DeferOrExecute();

    /// <summary>
    /// Creates a composite filter view model instance that wraps a filter with UI state.
    /// Override this method to create custom composite filter view models.
    /// </summary>
    /// <param name="filter">The filter to wrap.</param>
    /// <param name="logicalOperator">The logical operator for combining with other filters. Default is AND.</param>
    /// <returns>A new composite filter view model instance.</returns>
    protected virtual ICompositeFilterViewModel CreateCompositeFilter(IFilterViewModel filter, LogicalOperator logicalOperator = LogicalOperator.And)
        => new CompositeFilterViewModel(filter, logicalOperator);

    /// <summary>
    /// Adds a new filter to the collection.
    /// Override this method to implement custom filter creation logic.
    /// </summary>
    /// <exception cref="NotImplementedException">The base implementation must be overridden.</exception>
    public virtual void Add() => throw new NotImplementedException();

    /// <summary>
    /// Adds a filter to the collection with a specified logical operator.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    /// <param name="logicalOperator">The logical operator for combining with other filters. Default is AND.</param>
    public virtual void Add(IFilterViewModel filter, LogicalOperator logicalOperator = LogicalOperator.And)
        => CompositeFilters.Add(CreateCompositeFilter(filter, logicalOperator));

    /// <summary>
    /// Adds multiple filters to the collection.
    /// </summary>
    /// <param name="filters">The filters to add.</param>
    public virtual void AddRange(IEnumerable<IFilterViewModel> filters)
        => CompositeFilters.AddRange(filters);

    /// <summary>
    /// Clears all filters from the collection.
    /// If <see cref="AutoFilter"/> is false, manually applies the cleared state.
    /// </summary>
    public virtual void Clear()
    {
        CompositeFilters.Clear();

        if (!AutoFilter)
            Apply();
    }

    /// <summary>
    /// Clears dirty filters from the collection.
    /// Override this method to implement custom dirty filter detection logic.
    /// </summary>
    public virtual void ClearDirtyFilters() => CompositeFilters.Clear();

    /// <summary>
    /// Refreshes the filters to the last applied state.
    /// Restores filters from the cached <see cref="_currentFilters"/> collection.
    /// </summary>
    public virtual void Refresh() => CompositeFilters.Set(_currentFilters);

    /// <summary>
    /// Replaces all filters with the specified collection.
    /// Creates composite filters for each provided filter.
    /// </summary>
    /// <param name="filters">The new collection of filters.</param>
    public virtual void Set(IEnumerable<IFilterViewModel> filters)
    {
        using (_filtersChangedDeferrer.Defer())
            CompositeFilters.Set(filters.Select(x => CreateCompositeFilter(x)));
    }

    /// <summary>
    /// Resets all filters to their default state.
    /// Calls <see cref="ICompositeFilterViewModel.Reset"/> on each composite filter.
    /// </summary>
    public virtual void Reset()
    {
        using (_filtersChangedDeferrer.Defer())
            CompositeFilters.ForEach(x => x.Reset());
    }

    /// <summary>
    /// Manually applies the current filter configuration.
    /// Triggers the <see cref="FiltersChanged"/> event.
    /// </summary>
    private void Apply() => ApplyFilters(CompositeFilters);

    /// <summary>
    /// Applies the specified filters and raises the <see cref="FiltersChanged"/> event.
    /// Caches the current filter state for <see cref="Refresh"/> operation.
    /// </summary>
    /// <param name="compositeFilters">The composite filters to apply.</param>
    protected virtual void ApplyFilters(IEnumerable<ICompositeFilterViewModel> compositeFilters)
    {
        // Cache current state for refresh
        var list = compositeFilters.ToList();
        _currentFilters = [.. list];

        // Raise changed event
        FiltersChanged?.Invoke(this, new FiltersChangedEventArgs(list));
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when the filter configuration has changed.
    /// Updates dependent properties and applies filters if <see cref="AutoFilter"/> is true.
    /// </summary>
    [SuppressPropertyChangedWarnings]
    protected virtual void OnFiltersChanged()
    {
        OnPropertyChanged(nameof(ActiveCount));
        OnPropertyChanged(nameof(Count));

        if (AutoFilter)
            Apply();
    }

    #endregion

    #region ICollection Implementation

    /// <summary>
    /// Gets the total number of composite filters in the collection (enabled and disabled).
    /// </summary>
    public int Count => CompositeFilters.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// Returns false by default, can be overridden in derived classes.
    /// </summary>
    public virtual bool IsReadOnly => false;

    /// <summary>
    /// Adds a composite filter to the collection.
    /// </summary>
    /// <param name="item">The composite filter to add.</param>
    public virtual void Add(ICompositeFilterViewModel item)
  => IsReadOnly.IfFalse(() => CompositeFilters.Add(item));

    /// <summary>
    /// Removes a composite filter from the collection.
    /// </summary>
    /// <param name="item">The composite filter to remove.</param>
    /// <returns>True if the item was removed; otherwise, false.</returns>
    public virtual bool Remove(ICompositeFilterViewModel item)
  => !IsReadOnly && CompositeFilters.Remove(item);

    /// <summary>
    /// Determines whether the collection contains a specific composite filter.
    /// </summary>
    /// <param name="item">The composite filter to locate.</param>
    /// <returns>True if the item is found; otherwise, false.</returns>
    public bool Contains(ICompositeFilterViewModel item) => CompositeFilters.Contains(item);

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(ICompositeFilterViewModel[] array, int arrayIndex)
   => CompositeFilters.CopyTo(array, arrayIndex);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator<ICompositeFilterViewModel> GetEnumerator() => CompositeFilters.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => CompositeFilters.GetEnumerator();

    #endregion

    #region INotifyCollectionChanged Implementation

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
    {
        add => CollectionChanged += value;
        remove => CollectionChanged -= value;
    }

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "INotifyCollectionChanged implementation")]
    protected event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    [SuppressPropertyChangedWarnings]
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        => CollectionChanged?.Invoke(this, args);

    /// <summary>
    /// Handles collection change events from the underlying composite filters collection.
    /// </summary>
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
      => OnCollectionChanged(e);

    #endregion

    #region Cleanup

    /// <summary>
    /// Releases resources and performs cleanup operations.
    /// </summary>
    protected override void Cleanup()
    {
        CompositeFilters.CollectionChanged -= HandleCollectionChanged;
        base.Cleanup();
    }

    #endregion
}
