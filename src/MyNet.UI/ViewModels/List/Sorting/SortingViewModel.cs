// -----------------------------------------------------------------------
// <copyright file="SortingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MyNet.Observable;
using MyNet.Observable.Collections.Sorting;
using MyNet.Utilities;
using MyNet.Utilities.Deferring;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a view model for managing sorting configuration of a collection.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class SortingViewModel<T> : EditableObject, ISortingViewModel<T>
{
    private readonly Deferrer _deferrer;

    /// <summary>
    /// Creates a fluent builder used to configure and instantiate a <see cref="SortingViewModel{T}"/>.
    /// </summary>
    public static SortingViewModelBuilder<T> CreateBuilder() => new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingViewModel{T}"/> class with the specified sorting properties and optional default sorting configuration. The provided properties are wrapped in a read-only observable collection for UI binding, and the default sorting is stored for resetting purposes. The constructor also sets up event handlers to react to changes in the properties and trigger sorting updates accordingly.
    /// </summary>
    /// <param name="properties">The sorting properties for the collection.</param>
    /// <param name="defaultSorting">The optional default sorting configuration.</param>
    public SortingViewModel(IEnumerable<ISortingPropertyViewModel<T>> properties, IEnumerable<ISortingProperty<T>>? defaultSorting = null)
    {
        Properties = new(new(properties));
        DefaultSorting = defaultSorting?.ToList() ?? [];

        _deferrer = new(RaiseSortingChanged);

        Reset();

        foreach (var property in Properties)
            property.PropertyChanged += HandlePropertyChanged;
    }

    /// <summary>
    /// Gets the collection of sorting property view models that can be configured for sorting. This collection is read-only and observable, allowing UI components to bind to it and react to changes in the properties. Each property view model represents a specific sorting option that can be enabled or disabled, and may include additional information such as display names and sort directions.
    /// </summary>
    public ReadOnlyObservableCollection<ISortingPropertyViewModel<T>> Properties { get; }

    /// <summary>
    /// Gets the current sorting configuration built from the UI. This collection is read-only and represents the active sorting properties based on the user's configuration. It is computed from the enabled properties in the Properties collection and ordered by their activation time to reflect the order in which sorting options were applied. Subscribers can use this collection to apply the sorting configuration to their collections when the SortingChanged event is raised.
    /// </summary>
    public IReadOnlyList<ISortingProperty<T>> CurrentSorting { get; private set; } = [];

    /// <summary>
    /// Gets the default sorting configuration for the collection. This collection is read-only and represents the initial sorting properties that are applied when the view model is first created or when the Reset method is called.
    /// </summary>
    public IReadOnlyList<ISortingProperty<T>> DefaultSorting { get; }

    /// <summary>
    /// Gets a value indicating whether there are any active sorting properties in the current configuration. This property returns true if there is at least one sorting property that is enabled and contributing to the sorting configuration, and false if there are no active sorting properties. This can be used by UI components to determine whether to display sorting indicators or to enable/disable sorting-related actions based on the presence of active sorting properties.
    /// </summary>
    public bool HasActiveSorting => CurrentSorting.Any();

    /// <summary>
    /// Occurs when the sorting configuration has changed. Subscribers can react to this event to apply the new sorting configuration to their collections. The event provides data in the form of <see cref="SortingChangedEventArgs{T}"/>, which contains the current sorting properties that define the active sorting configuration based on the user's selections in the UI.
    /// </summary>
    public event EventHandler<SortingChangedEventArgs<T>>? SortingChanged;

    /// <summary>
    /// Applies the current sorting configuration. This method triggers the sorting update process by invoking the deferrer, which will call the OnSortingChanged method to compute the current sorting configuration and raise the SortingChanged event. Consumers can call this method after making changes to the sorting properties to ensure that the new configuration is applied to their collections.
    /// </summary>
    public void Apply() => _deferrer.DeferOrExecute();

    /// <summary>
    /// Clears all active sorting by disabling all sorting properties. This method iterates through the Properties collection and sets the IsEnabled property of each sorting property view model to false, effectively removing all sorting criteria from the current configuration. After clearing the sorting properties, it triggers the sorting update process to reflect the changes in the UI and notify subscribers of the new (empty) sorting configuration.
    /// </summary>
    public void Clear()
    {
        using (_deferrer.Defer())
            Properties.ForEach(p => p.IsEnabled = false);
    }

    /// <summary>
    /// Resets the sorting configuration to its default state. This method first clears all active sorting properties and then iterates through the DefaultSorting collection to find matching sorting properties in the Properties collection. For each matching property, it enables it and sets its direction according to the default configuration. After resetting the sorting properties, it triggers the sorting update process to reflect the changes in the UI and notify subscribers of the new sorting configuration based on the default settings.
    /// </summary>
    public void Reset()
    {
        using (_deferrer.Defer())
        {
            Clear();

            foreach (var def in DefaultSorting)
            {
                var vm = FindMatching(def);

                if (vm != null)
                {
                    vm.IsEnabled = true;
                    vm.Direction = def.Direction;
                }
            }
        }
    }

    /// <summary>
    /// Sets the active state of a sorting property identified by the specified key. This method finds the sorting property view model with the given key in the Properties collection and updates its IsEnabled property based on the provided isActive parameter. If the property is found, it triggers the sorting update process to reflect the changes in the UI and notify subscribers of the new sorting configuration. If no property with the specified key is found, the method does nothing.
    /// </summary>
    /// <param name="key">The key of the sorting property to update.</param>
    /// <param name="isActive">A value indicating whether the sorting property should be active.</param>
    public void SetActive(string key, bool isActive)
    {
        var property = Find(key);

        if (property is null) return;

        using (_deferrer.Defer())
            property.IsEnabled = isActive;
    }

    /// <summary>
    /// Toggles the sort direction of a sorting property identified by the specified key. If the sorting property is currently disabled, this method enables it and sets its direction to ascending. If the sorting property is already enabled, this method toggles its direction between ascending and descending. After updating the sorting property, it triggers the sorting update process to reflect the changes in the UI and notify subscribers of the new sorting configuration. If no property with the specified key is found, the method does nothing.
    /// </summary>
    /// <param name="key">The key of the sorting property to toggle.</param>
    public void Toggle(string key)
    {
        var property = Find(key);

        if (property is null) return;

        using (_deferrer.Defer())
        {
            if (!property.IsEnabled)
            {
                property.IsEnabled = true;
                property.Direction = ListSortDirection.Ascending;
            }
            else
            {
                property.Direction = property.Direction == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
        }
    }

    /// <summary>
    /// Sets the sort direction of a sorting property identified by the specified key. This method finds the sorting property view model with the given key in the Properties collection and updates its Direction property based on the provided direction parameter. If the property is found, it also ensures that the property is enabled (active) before setting the direction. After updating the sorting property, it triggers the sorting update process to reflect the changes in the UI and notify subscribers of the new sorting configuration. If no property with the specified key is found, the method does nothing.
    /// </summary>
    /// <param name="key">The key of the sorting property to update.</param>
    /// <param name="direction">The direction to set for the sorting property.</param>
    public void SetDirection(string key, ListSortDirection direction)
    {
        var property = Find(key);

        if (property is null) return;

        using (_deferrer.Defer())
        {
            property.IsEnabled = true;
            property.Direction = direction;
        }
    }

    /// <summary>
    /// Computes the current sorting configuration based on the enabled sorting properties in the Properties collection. This method filters the Properties collection to include only those properties that are currently enabled (IsEnabled is true), orders them by their activation time (ActivatedAt) to maintain the order in which they were activated, and then builds the corresponding <see cref="ISortingProperty{T}"/> instances for each active property. The resulting list represents the current sorting configuration that should be applied to the collection based on the user's selections in the UI.
    /// </summary>
    /// <returns>A read-only list of the current sorting properties.</returns>
    private IReadOnlyList<ISortingProperty<T>> ComputeCurrentSorting() =>
    [
        .. Properties
            .Where(p => p.IsEnabled)
            .OrderBy(p => p.ActivatedAt)
            .Select(p => p.Build())
    ];

    /// <summary>
    /// Handles the PropertyChanged event for the sorting property view models. When any property of a sorting property view model changes (such as IsEnabled or Direction), this method is called to trigger the sorting update process. It uses the deferrer to ensure that multiple changes are batched together and that the sorting update is performed efficiently after all changes have been processed. This allows the UI to react to changes in the sorting properties and update the sorting configuration accordingly.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e) => _deferrer.DeferOrExecute();

    /// <summary>
    /// Handles the logic for when the sorting configuration has changed. This method computes the current sorting configuration by calling ComputeCurrentSorting, updates the CurrentSorting property, and raises the SortingChanged event to notify subscribers of the new sorting configuration. This method is called by the deferrer after changes to the sorting properties have been processed, ensuring that the sorting update is performed efficiently and that subscribers receive the latest sorting configuration based on the user's selections in the UI.
    /// </summary>
    private void RaiseSortingChanged()
    {
        CurrentSorting = ComputeCurrentSorting();
        SortingChanged?.Invoke(this, new(CurrentSorting));
    }

    /// <summary>
    /// Finds a sorting property view model in the Properties collection by its unique key. This method searches through the Properties collection to find the first sorting property view model that has a Key property matching the specified key parameter. If a matching property is found, it is returned; otherwise, the method returns null. This method is used by other methods in the class to locate specific sorting properties for updating their active state or sort direction based on user interactions in the UI.
    /// </summary>
    /// <param name="key">The unique key of the sorting property to find.</param>
    /// <returns>The sorting property view model with the specified key, or null if not found.</returns>
    private ISortingPropertyViewModel<T>? Find(string key)
        => Properties.FirstOrDefault(p => p.Key == key);

    /// <summary>
    /// Finds a sorting property view model in the Properties collection that matches the specified sorting property. This method searches through the Properties collection to find the first sorting property view model whose built sorting property (obtained by calling Build()) has an expression that is equal to the expression of the provided sortingProperty parameter. If a matching property is found, it is returned; otherwise, the method returns null. This method is used by the Reset method to locate the corresponding view models for the default sorting properties and update their active state and direction accordingly.
    /// </summary>
    /// <param name="property">The sorting property to match.</param>
    /// <returns>The sorting property view model that matches the specified sorting property, or null if not found.</returns>
    private ISortingPropertyViewModel<T>? FindMatching(ISortingProperty<T> property)
        => Properties.FirstOrDefault(p => p.Matches(property));
}
