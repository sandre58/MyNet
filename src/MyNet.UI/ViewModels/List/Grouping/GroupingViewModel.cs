// -----------------------------------------------------------------------
// <copyright file="GroupingViewModel.cs" company="Stéphane ANDRE">
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
using DynamicData.Kernel;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.Observable.Extensions;
using MyNet.UI.Commands;
using MyNet.Utilities;
using MyNet.Utilities.Deferring;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// View model for managing grouping configuration of a collection.
/// Provides commands to add, remove, and reset grouping properties.
/// Supports hierarchical grouping (primary, secondary, tertiary groups, etc.).
/// </summary>
/// <remarks>
/// This class implements <see cref="ICollection{T}"/> and <see cref="INotifyCollectionChanged"/>
/// to allow direct manipulation of grouping properties while providing change notifications.
/// Changes are deferred and batched for optimal performance.
/// </remarks>
[CanBeValidated(false)]
[CanSetIsModified(false)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "It's a viewModel")]
public class GroupingViewModel : EditableObject, IGroupingViewModel, ICollection<IGroupingPropertyViewModel>, INotifyCollectionChanged
{
    private readonly IReadOnlyCollection<string> _defaultGroupingProperties;
    private readonly Deferrer _groupingChangedDeferrer;

    /// <summary>
    /// Gets the collection of grouping properties managed by this view model.
    /// </summary>
    protected GroupingPropertiesCollection GroupingProperties { get; } = [];

    #region Properties

    /// <summary>
    /// Gets the currently active (enabled) grouping property with the lowest order number.
    /// This represents the primary group if multiple groups are enabled.
    /// Returns null if no grouping properties are active.
    /// </summary>
    public IGroupingPropertyViewModel? ActiveGroupingProperty
     => GroupingProperties.OrderBy(x => x.Order).FirstOrDefault(x => x.IsEnabled);

    /// <summary>
    /// Gets the count of currently active (enabled) grouping properties.
    /// </summary>
    public int ActiveCount => GroupingProperties.Count(x => x.IsEnabled);

    #endregion

    #region Commands

    /// <summary>
    /// Gets the command to add a grouping property by name.
    /// Parameter: string (property name).
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets the command to apply a specific grouping configuration.
    /// Parameter: string (property name) to set as the sole active grouping.
    /// </summary>
    public ICommand ApplyCommand { get; }

    /// <summary>
    /// Gets the command to remove a grouping property by name.
    /// Parameter: string (property name).
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets the command to reset grouping to the default configuration.
    /// </summary>
    public ICommand ResetCommand { get; }

    /// <summary>
    /// Gets the command to clear all active grouping properties.
    /// </summary>
    public ICommand ClearCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the grouping configuration has changed.
    /// Fired after all pending changes are applied (deferred execution).
    /// </summary>
    public event EventHandler<GroupingChangedEventArgs>? GroupingChanged;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingViewModel"/> class with no default grouping.
    /// </summary>
    public GroupingViewModel()
      : this([]) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingViewModel"/> class with default grouping properties.
    /// </summary>
    /// <param name="defaultProperties">The list of property names to group by default.</param>
    public GroupingViewModel(IEnumerable<string> defaultProperties)
    {
        _groupingChangedDeferrer = new Deferrer(OnSortChanged);
        _defaultGroupingProperties = defaultProperties.AsList().AsReadOnly();

        // Initialize commands
        ClearCommand = CommandsManager.Create(Clear);
        AddCommand = CommandsManager.CreateNotNull<string>(x => Add(x));
        ApplyCommand = CommandsManager.CreateNotNull<string>(Set);
        RemoveCommand = CommandsManager.CreateNotNull<string>(Remove);
        ResetCommand = CommandsManager.Create(Reset);

        // Apply default grouping
        Reset();

        // Subscribe to property changes with deferred execution
        Disposables.Add(GroupingProperties
          .ToObservableChangeSet(x => x.PropertyName)
                  .SubscribeAll(() => _groupingChangedDeferrer.DeferOrExecute()));

        GroupingProperties.CollectionChanged += HandleCollectionChanged;
    }

    #endregion

    #region Grouping Operations

    /// <summary>
    /// Creates a deferral scope for batch changes.
    /// Changes made within the scope are applied and notified only when the scope is disposed.
    /// </summary>
    /// <returns>A disposable deferral scope.</returns>
    protected IDisposable DeferChanged() => _groupingChangedDeferrer.Defer();

    /// <summary>
    /// Creates a grouping property view model instance.
    /// Override this method to create custom grouping property view models.
    /// </summary>
    /// <param name="propertyName">The name of the property to group by.</param>
    /// <param name="sortingPropertyName">Optional property name for sorting groups. If null, uses <paramref name="propertyName"/>.</param>
    /// <param name="order">The group order. If null, uses the next available order (ActiveCount + 1).</param>
    /// <returns>A new grouping property view model instance.</returns>
    protected virtual IGroupingPropertyViewModel CreateGroupingProperty(
        string propertyName,
        string? sortingPropertyName = null,
        int? order = null)
        => new GroupingPropertyViewModel(propertyName, propertyName, sortingPropertyName, order ?? ActiveCount + 1)
        {
            IsEnabled = true
        };

    /// <summary>
    /// Adds a grouping property to the collection.
    /// If a property with the same name exists, it replaces it.
    /// </summary>
    /// <param name="propertyName">The name of the property to group by.</param>
    /// <param name="sortingPropertyName">Optional property name for sorting groups.</param>
    /// <param name="order">The group order. If null, assigns the next available order.</param>
    public virtual void Add(string propertyName, string? sortingPropertyName = null, int? order = null)
     => GroupingProperties.TryAdd(CreateGroupingProperty(propertyName, sortingPropertyName, order));

    /// <summary>
    /// Removes a grouping property from the collection by name.
    /// </summary>
    /// <param name="propertyName">The name of the property to remove.</param>
    public virtual void Remove(string propertyName) => GroupingProperties.Remove(propertyName);

    /// <summary>
    /// Sets a single property as the sole active grouping.
    /// Clears all existing groups and adds the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property to set as the sole grouping.</param>
    public void Set(string propertyName)
    {
        using (_groupingChangedDeferrer.Defer())
        {
            Clear();
            Add(propertyName);
        }
    }

    /// <summary>
    /// Replaces all grouping properties with the specified collection.
    /// </summary>
    /// <param name="properties">The new collection of grouping properties.</param>
    public virtual void Set(IEnumerable<IGroupingPropertyViewModel> properties)
    {
        using (_groupingChangedDeferrer.Defer())
            GroupingProperties.Set(properties);
    }

    /// <summary>
    /// Clears all grouping properties from the collection.
    /// </summary>
    public virtual void Clear() => GroupingProperties.Clear();

    /// <summary>
    /// Resets the grouping configuration to its default state.
    /// The default state is defined by the properties passed to the constructor.
    /// </summary>
    public void Reset()
         => Set(_defaultGroupingProperties.Select((x, index) => CreateGroupingProperty(x, order: index + 1)));

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when the grouping configuration has changed.
    /// Raises the <see cref="GroupingChanged"/> event and updates dependent properties.
    /// </summary>
    [SuppressPropertyChangedWarnings]
    protected virtual void OnSortChanged()
    {
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(ActiveCount));
        OnPropertyChanged(nameof(ActiveGroupingProperty));
        GroupingChanged?.Invoke(this, new GroupingChangedEventArgs(GroupingProperties));
    }

    #endregion

    #region ICollection Implementation

    /// <summary>
    /// Gets the total number of grouping properties in the collection (enabled and disabled).
    /// </summary>
    public int Count => GroupingProperties.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// Returns false by default, can be overridden in derived classes.
    /// </summary>
    public virtual bool IsReadOnly => false;

    /// <summary>
    /// Adds a grouping property view model to the collection.
    /// </summary>
    /// <param name="item">The grouping property view model to add.</param>
    public virtual void Add(IGroupingPropertyViewModel item)
        => IsReadOnly.IfFalse(() => GroupingProperties.Add(item));

    /// <summary>
    /// Removes a grouping property view model from the collection.
    /// </summary>
    /// <param name="item">The grouping property view model to remove.</param>
    /// <returns>True if the item was removed; otherwise, false.</returns>
    public virtual bool Remove(IGroupingPropertyViewModel item)
        => !IsReadOnly && GroupingProperties.Remove(item);

    /// <summary>
    /// Determines whether the collection contains a specific grouping property view model.
    /// </summary>
    /// <param name="item">The grouping property view model to locate.</param>
    /// <returns>True if the item is found; otherwise, false.</returns>
    public bool Contains(IGroupingPropertyViewModel item) => GroupingProperties.Contains(item);

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(IGroupingPropertyViewModel[] array, int arrayIndex)
        => GroupingProperties.CopyTo(array, arrayIndex);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator<IGroupingPropertyViewModel> GetEnumerator() => GroupingProperties.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GroupingProperties.GetEnumerator();

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
    /// Handles collection change events from the underlying grouping properties collection.
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
        GroupingProperties.CollectionChanged -= HandleCollectionChanged;
        base.Cleanup();
    }

    #endregion
}
