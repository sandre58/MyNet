// -----------------------------------------------------------------------
// <copyright file="SortingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DynamicData.Binding;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.Observable.Extensions;
using MyNet.UI.Commands;
using MyNet.Utilities;
using MyNet.Utilities.Deferring;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// View model for managing sorting configuration of a collection.
/// Provides commands to add, remove, toggle, and reset sorting properties.
/// Supports multiple sorting levels (primary, secondary, tertiary, etc.) with configurable order.
/// </summary>
/// <remarks>
/// This class implements <see cref="ICollection{T}"/> and <see cref="INotifyCollectionChanged"/>
/// to allow direct manipulation of sorting properties while providing change notifications.
/// Changes are deferred and batched for optimal performance.
/// </remarks>
[CanBeValidated(false)]
[CanSetIsModified(false)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "It's a viewModel")]
public class SortingViewModel : EditableObject, ISortingViewModel, ICollection<ISortingPropertyViewModel>, INotifyCollectionChanged
{
    private readonly IReadOnlyDictionary<string, ListSortDirection> _defaultSortingProperties;
    private readonly Deferrer _sortingChangedDeferrer;

    /// <summary>
    /// Gets the collection of sorting properties managed by this view model.
    /// </summary>
    protected SortingPropertiesCollection SortingProperties { get; } = [];

    #region Properties

    /// <summary>
    /// Gets the currently active (enabled) sorting property with the lowest order number.
    /// This represents the primary sort if multiple sorts are enabled.
    /// Returns null if no sorting properties are active.
    /// </summary>
    public ISortingPropertyViewModel? ActiveSortingProperty
   => SortingProperties.OrderBy(x => x.Order).FirstOrDefault(x => x.IsEnabled);

    /// <summary>
    /// Gets the count of currently active (enabled) sorting properties.
    /// </summary>
    public int ActiveCount => SortingProperties.Count(x => x.IsEnabled);

    #endregion

    #region Commands

    /// <summary>
    /// Gets the command to add a sorting property by name.
    /// Parameter: string (property name).
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets the command to switch (toggle) the sort direction of a property.
    /// If the property is disabled, it will be enabled first.
    /// Parameter: string (property name).
    /// </summary>
    public ICommand SwitchCommand { get; }

    /// <summary>
    /// Gets the command to toggle a property as the sole active sort.
    /// Clears all other sorts, adds the specified property, and switches direction if already ascending.
    /// Parameter: string (property name).
    /// </summary>
    public ICommand ToggleCommand { get; }

    /// <summary>
    /// Gets the command to remove a sorting property by name.
    /// Parameter: string (property name).
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets the command to reset sorting to the default configuration.
    /// </summary>
    public ICommand ResetCommand { get; }

    /// <summary>
    /// Gets the command to clear all active sorting properties.
    /// </summary>
    public ICommand ClearCommand { get; }

    /// <summary>
    /// Gets the command to apply a specific sorting configuration.
    /// </summary>
    public ICommand ApplyCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the sorting configuration has changed.
    /// Fired after all pending changes are applied (deferred execution).
    /// </summary>
    public event EventHandler<SortingChangedEventArgs>? SortingChanged;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingViewModel"/> class with no default sorting.
    /// </summary>
    public SortingViewModel()
      : this([]) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingViewModel"/> class with a single default sorting property.
    /// </summary>
    /// <param name="defaultProperty">The property name to sort by default.</param>
    /// <param name="listSortDirection">The default sort direction. Default is ascending.</param>
    public SortingViewModel(string defaultProperty, ListSortDirection listSortDirection = ListSortDirection.Ascending)
       : this(new Dictionary<string, ListSortDirection> { { defaultProperty, listSortDirection } }) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingViewModel"/> class with multiple default sorting properties (ascending).
    /// </summary>
    /// <param name="defaultProperties">The list of property names to sort by default (all ascending).</param>
    public SortingViewModel(IList<string> defaultProperties)
        : this(defaultProperties.ToDictionary(x => x, _ => ListSortDirection.Ascending)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingViewModel"/> class with multiple default sorting properties and directions.
    /// </summary>
    /// <param name="defaultProperties">A dictionary mapping property names to their default sort directions.</param>
    public SortingViewModel(IDictionary<string, ListSortDirection> defaultProperties)
    {
        _sortingChangedDeferrer = new Deferrer(OnSortChanged);
        _defaultSortingProperties = defaultProperties.AsReadOnly();

        // Initialize commands
        ClearCommand = CommandsManager.Create(Clear);
        AddCommand = CommandsManager.CreateNotNull<string>(x => Add(x));
        ToggleCommand = CommandsManager.CreateNotNull<string>(Toggle);
        SwitchCommand = CommandsManager.CreateNotNull<string>(Switch);
        ApplyCommand = CommandsManager.CreateNotNull<List<(string, ListSortDirection)>>(
  x => Set(x.Select((y, index) => CreateSortingProperty(y.Item1, y.Item2, index + 1))));
        RemoveCommand = CommandsManager.CreateNotNull<string>(Remove);
        ResetCommand = CommandsManager.Create(Reset);

        // Apply default sorting
        Reset();

        // Subscribe to property changes with deferred execution
        Disposables.Add(SortingProperties
   .ToObservableChangeSet(x => x.PropertyName)
  .SubscribeAll(() => _sortingChangedDeferrer.DeferOrExecute()));

        SortingProperties.CollectionChanged += HandleCollectionChanged;
    }

    #endregion

    #region Sorting Operations

    /// <summary>
    /// Creates a deferral scope for batch changes.
    /// Changes made within the scope are applied and notified only when the scope is disposed.
    /// </summary>
    /// <returns>A disposable deferral scope.</returns>
    protected IDisposable DeferChanged() => _sortingChangedDeferrer.Defer();

    /// <summary>
    /// Creates a sorting property view model instance.
    /// Override this method to create custom sorting property view models.
    /// </summary>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <param name="listSortDirection">The sort direction. Default is ascending.</param>
    /// <param name="order">The sort order. If null, uses the next available order (ActiveCount + 1).</param>
    /// <returns>A new sorting property view model instance.</returns>
    protected virtual ISortingPropertyViewModel CreateSortingProperty(
        string propertyName,
        ListSortDirection listSortDirection = ListSortDirection.Ascending,
        int? order = null)
        => new SortingPropertyViewModel(propertyName, listSortDirection, order ?? ActiveCount + 1);

    /// <summary>
    /// Adds a sorting property to the collection.
    /// If a property with the same name exists, it replaces it.
    /// </summary>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <param name="listSortDirection">The sort direction. Default is ascending.</param>
    /// <param name="order">The sort order. If null, assigns the next available order.</param>
    public virtual void Add(
        string propertyName,
        ListSortDirection listSortDirection = ListSortDirection.Ascending,
        int? order = null)
    => SortingProperties.TryAdd(CreateSortingProperty(propertyName, listSortDirection, order));

    /// <summary>
    /// Removes a sorting property from the collection by name.
    /// </summary>
    /// <param name="propertyName">The name of the property to remove.</param>
    public virtual void Remove(string propertyName) => SortingProperties.Remove(propertyName);

    /// <summary>
    /// Switches (toggles) the sort direction of a property.
    /// If the property is not enabled, it is enabled first with the next available order.
    /// If enabled, the direction is reversed (Ascending ↔ Descending).
    /// </summary>
    /// <param name="propertyName">The name of the property to switch.</param>
    public virtual void Switch(string propertyName)
    {
        if (SortingProperties[propertyName] is not { } property)
            return;

        using (_sortingChangedDeferrer.Defer())
        {
            if (!property.IsEnabled)
                property.Order = ActiveCount + 1;

            property.IsEnabled = true;
            property.Direction = property.Direction == ListSortDirection.Ascending
         ? ListSortDirection.Descending
       : ListSortDirection.Ascending;
        }
    }

    /// <summary>
    /// Toggles a property as the sole active sort.
    /// Clears all existing sorts, adds the specified property, and switches direction if it was already ascending.
    /// </summary>
    /// <param name="propertyName">The name of the property to toggle.</param>
    /// <remarks>
    /// This is commonly used for single-column sorting in data grids:
    /// - First click: Sort ascending
    /// - Second click: Sort descending
    /// - Third click: Back to ascending.
    /// </remarks>
    public void Toggle(string propertyName)
    {
        using (_sortingChangedDeferrer.Defer())
        {
            var switchDirection = SortingProperties[propertyName] is { IsEnabled: true, Direction: ListSortDirection.Ascending };
            Clear();
            Add(propertyName);

            if (switchDirection)
                Switch(propertyName);
        }
    }

    /// <summary>
    /// Replaces all sorting properties with the specified collection.
    /// </summary>
    /// <param name="properties">The new collection of sorting properties.</param>
    public virtual void Set(IEnumerable<ISortingPropertyViewModel> properties)
    {
        using (_sortingChangedDeferrer.Defer())
            SortingProperties.Set(properties);
    }

    /// <summary>
    /// Clears all sorting properties from the collection.
    /// </summary>
    public virtual void Clear() => SortingProperties.Clear();

    /// <summary>
    /// Resets the sorting configuration to its default state.
    /// The default state is defined by the properties passed to the constructor.
    /// </summary>
    public void Reset()
    => Set(_defaultSortingProperties.Select((x, index) => CreateSortingProperty(x.Key, x.Value, index + 1)));

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when the sorting configuration has changed.
    /// Raises the <see cref="SortingChanged"/> event and updates dependent properties.
    /// </summary>
    [SuppressPropertyChangedWarnings]
    protected virtual void OnSortChanged()
    {
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(ActiveCount));
        OnPropertyChanged(nameof(ActiveSortingProperty));
        SortingChanged?.Invoke(this, new SortingChangedEventArgs(SortingProperties));
    }

    #endregion

    #region ICollection Implementation

    /// <summary>
    /// Gets the total number of sorting properties in the collection (enabled and disabled).
    /// </summary>
    public int Count => SortingProperties.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// Returns false by default, can be overridden in derived classes.
    /// </summary>
    public virtual bool IsReadOnly => false;

    /// <summary>
    /// Adds a sorting property view model to the collection.
    /// </summary>
    /// <param name="item">The sorting property view model to add.</param>
    public virtual void Add(ISortingPropertyViewModel item)
        => IsReadOnly.IfFalse(() => SortingProperties.Add(item));

    /// <summary>
    /// Removes a sorting property view model from the collection.
    /// </summary>
    /// <param name="item">The sorting property view model to remove.</param>
    /// <returns>True if the item was removed; otherwise, false.</returns>
    public virtual bool Remove(ISortingPropertyViewModel item)
        => !IsReadOnly && SortingProperties.Remove(item);

    /// <summary>
    /// Determines whether the collection contains a specific sorting property view model.
    /// </summary>
    /// <param name="item">The sorting property view model to locate.</param>
    /// <returns>True if the item is found; otherwise, false.</returns>
    public bool Contains(ISortingPropertyViewModel item) => SortingProperties.Contains(item);

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(ISortingPropertyViewModel[] array, int arrayIndex)
        => SortingProperties.CopyTo(array, arrayIndex);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator<ISortingPropertyViewModel> GetEnumerator() => SortingProperties.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => SortingProperties.GetEnumerator();

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
    /// Handles collection change events from the underlying sorting properties collection.
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
        SortingProperties.CollectionChanged -= HandleCollectionChanged;
        base.Cleanup();
    }

    #endregion
}
