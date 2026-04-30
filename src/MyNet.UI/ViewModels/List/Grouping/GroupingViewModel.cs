// -----------------------------------------------------------------------
// <copyright file="GroupingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MyNet.Observable;
using MyNet.Observable.Collections.Grouping;
using MyNet.Utilities;
using MyNet.Utilities.Deferring;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a view model for managing grouping configuration of a collection.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class GroupingViewModel<T> : EditableObject, IGroupingViewModel<T>
{
    private readonly Deferrer _deferrer;

    /// <summary>
    /// Creates a fluent builder used to configure and instantiate a <see cref="GroupingViewModel{T}"/>.
    /// </summary>
    public static GroupingViewModelBuilder<T> CreateBuilder() => new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingViewModel{T}"/> class with the specified grouping properties and optional default grouping configuration. The provided properties are wrapped in a read-only observable collection for UI binding, and the default grouping is stored for resetting purposes. The constructor also sets up event handlers to react to changes in the properties and trigger grouping updates accordingly.
    /// </summary>
    /// <param name="properties">The grouping properties for the collection.</param>
    /// <param name="defaultGrouping">The optional default grouping configuration.</param>
    public GroupingViewModel(IEnumerable<IGroupingPropertyViewModel<T>> properties, IEnumerable<IGroupingProperty<T>>? defaultGrouping = null)
    {
        Properties = new(new(properties));
        DefaultGrouping = defaultGrouping?.ToList() ?? [];

        _deferrer = new(RaiseGroupingChanged);

        Reset();

        foreach (var property in Properties)
            property.PropertyChanged += HandlePropertyChanged;
    }

    /// <summary>
    /// Gets the collection of grouping property view models that can be configured for grouping. This collection is read-only and observable, allowing UI components to bind to it and react to changes in the properties. Each property view model represents a specific grouping option that can be enabled or disabled, and may include additional information such as display names.
    /// </summary>
    public ReadOnlyObservableCollection<IGroupingPropertyViewModel<T>> Properties { get; }

    /// <summary>
    /// Gets the current grouping configuration built from the UI. This collection is read-only and represents the active grouping properties based on the user's configuration. It is computed from the enabled properties in the Properties collection and ordered by their activation time to reflect the order in which grouping options were applied. Subscribers can use this collection to apply the grouping configuration to their collections when the GroupingChanged event is raised.
    /// </summary>
    public IReadOnlyList<IGroupingProperty<T>> CurrentGrouping { get; private set; } = [];

    /// <summary>
    /// Gets the default grouping configuration for the collection. This collection is read-only and represents the initial grouping properties that are applied when the view model is first created or when the Reset method is called.
    /// </summary>
    public IReadOnlyList<IGroupingProperty<T>> DefaultGrouping { get; }

    /// <summary>
    /// Gets a value indicating whether there are any active grouping properties in the current configuration. This property returns true if there is at least one grouping property that is enabled and contributing to the grouping configuration, and false if there are no active grouping properties. This can be used by UI components to determine whether to display grouping indicators or to enable/disable grouping-related actions based on the presence of active grouping properties.
    /// </summary>
    public bool HasActiveGrouping => CurrentGrouping.Any();

    /// <summary>
    /// Occurs when the grouping configuration has changed. Subscribers can react to this event to apply the new grouping configuration to their collections. The event provides data in the form of <see cref="GroupingChangedEventArgs{T}"/>, which contains the current grouping properties that define the active grouping configuration based on the user's selections in the UI.
    /// </summary>
    public event EventHandler<GroupingChangedEventArgs<T>>? GroupingChanged;

    /// <summary>
    /// Applies the current grouping configuration. This method triggers the grouping update process by invoking the deferrer, which will call the OnGroupingChanged method to compute the current grouping configuration and raise the GroupingChanged event. Consumers can call this method after making changes to the grouping properties to ensure that the new configuration is applied to their collections.
    /// </summary>
    public void Apply() => _deferrer.DeferOrExecute();

    /// <summary>
    /// Clears all active grouping by disabling all grouping properties. This method iterates through the Properties collection and sets the IsEnabled property of each grouping property view model to false, effectively removing all grouping criteria from the current configuration. After clearing the grouping properties, it triggers the grouping update process to reflect the changes in the UI and notify subscribers of the new (empty) grouping configuration.
    /// </summary>
    public void Clear()
    {
        using (_deferrer.Defer())
            Properties.ForEach(p => p.IsEnabled = false);
    }

    /// <summary>
    /// Resets the grouping configuration to its default state. This method first clears all active grouping properties and then iterates through the DefaultGrouping collection to find matching grouping properties in the Properties collection. For each matching property, it enables it according to the default configuration. After resetting the grouping properties, it triggers the grouping update process to reflect the changes in the UI and notify subscribers of the new grouping configuration based on the default settings.
    /// </summary>
    public void Reset()
    {
        using (_deferrer.Defer())
        {
            Clear();

            foreach (var def in DefaultGrouping)
            {
                var vm = FindMatching(def);

                vm?.IsEnabled = true;
            }
        }
    }

    /// <summary>
    /// Sets the active state of a grouping property identified by the specified key. This method finds the grouping property view model with the given key in the Properties collection and updates its IsEnabled property based on the provided isActive parameter. If the property is found, it triggers the grouping update process to reflect the changes in the UI and notify subscribers of the new grouping configuration. If no property with the specified key is found, the method does nothing.
    /// </summary>
    /// <param name="key">The key of the grouping property to update.</param>
    /// <param name="isActive">A value indicating whether the grouping property should be active.</param>
    public void SetActive(string key, bool isActive)
    {
        var property = Find(key);

        if (property is null) return;

        using (_deferrer.Defer())
            property.IsEnabled = isActive;
    }

    /// <summary>
    /// Computes the current grouping configuration based on the enabled grouping properties in the Properties collection. This method filters the Properties collection to include only those properties that are currently enabled (IsEnabled is true), orders them by their activation time (ActivatedAt) to maintain the order in which they were activated, and then builds the corresponding <see cref="IGroupingProperty{T}"/> instances for each active property. The resulting list represents the current grouping configuration that should be applied to the collection based on the user's selections in the UI.
    /// </summary>
    /// <returns>A read-only list of the current grouping properties.</returns>
    private IReadOnlyList<IGroupingProperty<T>> ComputeCurrentGrouping() =>
    [
        .. Properties
            .Where(p => p.IsEnabled)
            .OrderBy(p => p.ActivatedAt)
            .Select(p => p.Build())
    ];

    /// <summary>
    /// Handles the PropertyChanged event for the grouping property view models. When any property of a grouping property view model changes (such as IsEnabled or Direction), this method is called to trigger the grouping update process. It uses the deferrer to ensure that multiple changes are batched together and that the grouping update is performed efficiently after all changes have been processed. This allows the UI to react to changes in the grouping properties and update the grouping configuration accordingly.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e) => _deferrer.DeferOrExecute();

    /// <summary>
    /// Handles the logic for when the grouping configuration has changed. This method computes the current grouping configuration by calling ComputeCurrentGrouping, updates the CurrentGrouping property, and raises the GroupingChanged event to notify subscribers of the new grouping configuration. This method is called by the deferrer after changes to the grouping properties have been processed, ensuring that the grouping update is performed efficiently and that subscribers receive the latest grouping configuration based on the user's selections in the UI.
    /// </summary>
    private void RaiseGroupingChanged()
    {
        CurrentGrouping = ComputeCurrentGrouping();
        GroupingChanged?.Invoke(this, new(CurrentGrouping));
    }

    /// <summary>
    /// Finds a grouping property view model in the Properties collection by its unique key. This method searches through the Properties collection to find the first grouping property view model that has a Key property matching the specified key parameter. If a matching property is found, it is returned; otherwise, the method returns null. This method is used by other methods in the class to locate specific grouping properties for updating their active state based on user interactions in the UI.
    /// </summary>
    /// <param name="key">The unique key of the grouping property to find.</param>
    /// <returns>The grouping property view model with the specified key, or null if not found.</returns>
    private IGroupingPropertyViewModel<T>? Find(string key)
        => Properties.FirstOrDefault(p => p.Key == key);

    /// <summary>
    /// Finds a grouping property view model in the Properties collection that matches the specified grouping property. This method searches through the Properties collection to find the first grouping property view model whose built grouping property (obtained by calling Build()) has an expression that is equal to the expression of the provided groupingProperty parameter. If a matching property is found, it is returned; otherwise, the method returns null. This method is used by the Reset method to locate the corresponding view models for the default grouping properties and update their active state accordingly.
    /// </summary>
    /// <param name="property">The grouping property to match.</param>
    /// <returns>The grouping property view model that matches the specified grouping property, or null if not found.</returns>
    private IGroupingPropertyViewModel<T>? FindMatching(IGroupingProperty<T> property)
        => Properties.FirstOrDefault(p => p.Matches(property));
}
