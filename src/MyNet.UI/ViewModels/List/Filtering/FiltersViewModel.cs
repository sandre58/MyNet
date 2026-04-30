// -----------------------------------------------------------------------
// <copyright file="FiltersViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MyNet.Observable;
using MyNet.Observable.Collections.Filters;
using MyNet.Utilities.Deferring;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents the view model for managing filters applied to a collection of items of type T. It maintains a tree of filter conditions and groups, computes the current composite filter based on the active conditions, and raises events when the filters change. The FiltersViewModel allows for applying, clearing, and resetting filters, and supports automatic application of filters when changes occur. It also tracks whether there are active filters and whether the current filter configuration is dirty (i.e., has unsaved changes).
/// </summary>
/// <typeparam name="T">The type of items to be filtered.</typeparam>
public class FiltersViewModel<T> : ObservableObject, IFiltersViewModel<T>
{
    private readonly Deferrer _deferrer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersViewModel{T}"/> class with the specified root filter group view model. The constructor sets up the necessary event subscriptions to track changes in the filter configuration and initializes the deferrer to manage deferred execution of filter change notifications.
    /// </summary>
    /// <param name="root">The root filter group view model.</param>
    /// <exception cref="ArgumentNullException">Thrown if the root parameter is null.</exception>
    public FiltersViewModel(IFilterGroupViewModel<T> root)
    {
        Root = root ?? throw new ArgumentNullException(nameof(root));

        _deferrer = new(HandleFiltersChanged);

        Disposables.Add(Subscribe(Root));
    }

    /// <summary>
    /// Gets the root filter group view model, which represents the full filter tree configured in the UI. This property provides access to the top-level filter group, allowing clients to navigate and manipulate the filter hierarchy as needed. The root filter group serves as the entry point for building the composite filter based on the active conditions defined in the filter tree.
    /// </summary>
    public IFilterGroupViewModel<T> Root { get; }

    /// <summary>
    /// Gets the current filter built from the UI configuration. This property returns an instance of <see cref="IFilter{T}"/> that represents the composite filter constructed based on the active conditions defined in the filter tree. If no filters are active or if the filter tree is empty, this property returns null. The CurrentFilter property allows clients to retrieve the effective filter that can be applied to a collection of items of type T, based on the current configuration of filters in the UI.
    /// </summary>
    public IFilter<T>? CurrentFilter { get; private set; }

    /// <summary>
    /// Gets a value indicating whether there are any active filters in the current configuration. This property returns true if the CurrentFilter property is not null, indicating that there are active filter conditions defined in the filter tree. If CurrentFilter is null, it means that there are no active filters, and this property will return false. The HasActiveFilters property provides a convenient way for clients to check if any filters are currently applied without needing to directly inspect the CurrentFilter property.
    /// </summary>
    public bool HasActiveFilters => CurrentFilter is not null;

    /// <summary>
    /// Gets or sets a value indicating whether the filters should be automatically applied when they change. If set to true, the filters will be applied automatically whenever a change occurs in the filter configuration. If set to false, the filters will need to be applied manually by calling the Apply method.
    /// </summary>
    public bool AutoApply { get; set; } = true;

    /// <summary>
    /// Gets a value indicating whether the filter configuration has been modified since the last application. This property returns true if there are unsaved changes in the filter configuration, allowing clients to determine if the filters need to be reapplied.
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Occurs when the filter configuration changes and a new filter is produced. This event is raised whenever the filters are applied, either automatically or manually, and provides subscribers with the new filter that has been generated based on the current configuration of the filter tree. The FiltersChanged event allows clients to react to changes in the filter configuration, such as updating the displayed data or performing other actions based on the new filter criteria.
    /// </summary>
    public event EventHandler<FiltersChangedEventArgs<T>>? FiltersChanged;

    /// <summary>
    /// Observes collection changes on the specified source collection and returns an observable sequence of collection changed event arguments. This method uses Reactive Extensions to create an observable that listens for changes to the collection and emits the corresponding event arguments whenever a change occurs. The ObserveCollectionChanges method provides a way to react to changes in the collection, allowing subscribers to update their state or perform actions based on the specific changes that occur in the collection, while still maintaining encapsulation and integrity of the data.
    /// </summary>
    /// <param name="source">The source collection to observe for changes.</param>
    /// <returns>An observable sequence of collection changed event arguments.</returns>
    private static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanges(
        INotifyCollectionChanged source) =>
        System.Reactive.Linq.Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h)
            .Select(x => x.EventArgs);

    /// <summary>
    /// Applies the current filter configuration and raises the FiltersChanged event. This method computes the current filter based on the active conditions defined in the filter tree, updates the CurrentFilter property, and sets the IsDirty flag to false. After applying the filters, it raises the FiltersChanged event to notify subscribers of the new filter that has been generated. The Apply method allows clients to manually trigger the application of filters when AutoApply is set to false or when they want to ensure that changes are applied immediately.
    /// </summary>
    public void Apply()
    {
        CurrentFilter = ComputeCurrentFilter();
        IsDirty = false;

        FiltersChanged?.Invoke(this, new(CurrentFilter));
    }

    /// <summary>
    /// Clears all filters by setting them to their empty state. This method traverses the filter tree and resets each filter condition to its default state, effectively clearing any criteria defined in the filters. After clearing the filters, it raises the necessary events to indicate that the filter configuration has changed. The Clear method allows clients to quickly remove all filter criteria and return to an unfiltered state without needing to manually reset each individual filter condition.
    /// </summary>
    public void Clear()
    {
        using (_deferrer.Defer())
            Root.Clear();
    }

    /// <summary>
    /// Resets the filters to their default state. This method traverses the filter tree and resets each filter condition to its default state, similar to the Clear method. However, while Clear typically sets filters to an empty state, Reset may restore filters to their initial configuration or default values as defined by the implementation of each filter condition. After resetting the filters, it raises the necessary events to indicate that the filter configuration has changed. The Reset method allows clients to return the filters to a predefined default state, which may be different from simply clearing all criteria.
    /// </summary>
    public void Reset()
    {
        using (_deferrer.Defer())
            Root.Reset();
    }

    /// <summary>
    /// Handles changes to the filter configuration by marking the state as dirty and applying the filters if AutoApply is enabled. This method is called whenever a change occurs in the filter tree, such as when a filter condition is modified, added, or removed. It sets the IsDirty flag to true to indicate that there are unsaved changes in the filter configuration. If AutoApply is set to true, it then calls the Apply method to immediately apply the new filter configuration and raise the FiltersChanged event. The OnFiltersChanged method ensures that changes to the filter configuration are properly tracked and applied based on the user's preferences for automatic application of filters.
    /// </summary>
    private void HandleFiltersChanged()
    {
        IsDirty = true;

        if (AutoApply)
            Apply();
    }

    /// <summary>
    /// Computes the current filter based on the active conditions defined in the filter tree. This method calls the BuildExpression method on the root filter group to construct a composite expression that represents the combined filter criteria from all active filters in the tree. If the resulting expression is not null, it creates and returns a new instance of <see cref="ExpressionFilter{T}"/> using the computed expression. If there are no active filters or if the filter tree is empty, it returns null, indicating that there are no filters to apply. The ComputeCurrentFilter method encapsulates the logic for generating the effective filter based on the current configuration of filters in the UI.
    /// </summary>
    /// <returns>The current filter based on the active conditions, or null if there are no active filters.</returns>
    private ExpressionFilter<T>? ComputeCurrentFilter() => Root.BuildExpression() is { } expr ? new ExpressionFilter<T>(expr) : null;

    /// <summary>
    /// Subscribes to property changes and collection changes in the filter tree starting from the specified node. This method recursively traverses the filter tree, subscribing to property change notifications for each filter node that implements INotifyPropertyChanged, as well as collection change notifications for any filter groups that contain child nodes. The subscriptions are managed using a CompositeDisposable, which allows for easy cleanup of all subscriptions when the FiltersViewModel is disposed. By subscribing to these changes, the FiltersViewModel can react to modifications in the filter configuration and trigger the necessary updates to the current filter and raise events accordingly.
    /// </summary>
    /// <param name="node">The filter node to start subscribing from.</param>
    /// <returns>A CompositeDisposable containing all subscriptions for the specified node and its children.</returns>
    private CompositeDisposable Subscribe(IFilterNodeViewModel<T> node)
    {
        var disposable = new CompositeDisposable();
        if (node is INotifyPropertyChanged npc)
        {
            disposable.Add(System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => npc.PropertyChanged += h,
                    h => npc.PropertyChanged -= h)
                .Select(x => x.EventArgs)
                .Subscribe(_ => _deferrer.DeferOrExecute()));
        }

        if (node is IFilterGroupViewModel<T> group)
        {
            disposable.Add(ObserveCollectionChanges(group.Children).Subscribe(_ => _deferrer.DeferOrExecute()));

            foreach (var child in group.Children)
                disposable.Add(Subscribe(child));
        }

        return disposable;
    }
}
