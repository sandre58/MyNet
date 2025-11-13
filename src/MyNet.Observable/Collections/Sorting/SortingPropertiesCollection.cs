// -----------------------------------------------------------------------
// <copyright file="SortingPropertiesCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyNet.Utilities.Collections;
using MyNet.Utilities.Deferring;
using PropertyChanged;

namespace MyNet.Observable.Collections.Sorting;

/// <summary>
/// An optimized observable collection for managing sorting properties with efficient batch operations.
/// </summary>
public class SortingPropertiesCollection : OptimizedObservableCollection<SortingProperty>
{
    private readonly Deferrer _sortChangedDeferrer;

    /// <summary>
    /// Occurs when sorting properties in the collection have changed.
    /// </summary>
    public event EventHandler? SortChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertiesCollection"/> class.
    /// </summary>
    public SortingPropertiesCollection()
     => _sortChangedDeferrer = new Deferrer(OnSortChanged);

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertiesCollection"/> class with initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity to pre-allocate.</param>
    public SortingPropertiesCollection(int capacity)
     : base(capacity)
      => _sortChangedDeferrer = new Deferrer(OnSortChanged);

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertiesCollection"/> class with existing properties.
    /// </summary>
    /// <param name="properties">The initial sorting properties to add.</param>
    public SortingPropertiesCollection(IEnumerable<SortingProperty> properties)
      : base(properties)
          => _sortChangedDeferrer = new Deferrer(OnSortChanged);

    [SuppressPropertyChangedWarnings]
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        _sortChangedDeferrer.DeferOrExecute();
    }

    /// <summary>
    /// Replaces all sorting properties with the specified collection.
    /// Optimized to send a single notification for the entire operation.
    /// </summary>
    /// <param name="properties">The sorting properties to set.</param>
    public void Set(IEnumerable<SortingProperty> properties)
    {
        // Optimize: Use Load for better performance when replacing all items
        using (_sortChangedDeferrer.Defer())
        {
            Load(properties);
        }
    }

    /// <summary>
    /// Adds multiple sorting properties at once.
    /// </summary>
    /// <param name="sort">The sorting properties to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public new SortingPropertiesCollection AddRange(IEnumerable<SortingProperty> sort)
    {
        using (_sortChangedDeferrer.Defer())
            base.AddRange(sort);

        return this;
    }

    /// <summary>
    /// Adds a sorting property with the specified direction.
    /// </summary>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <param name="sortDirection">The sort direction.</param>
    /// <returns>This instance for method chaining.</returns>
    public SortingPropertiesCollection Add(string propertyName, ListSortDirection sortDirection = ListSortDirection.Ascending)
    {
        Add(new SortingProperty(propertyName, sortDirection));
        return this;
    }

    /// <summary>
    /// Adds a property with ascending sort order.
    /// </summary>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <returns>This instance for method chaining.</returns>
    public SortingPropertiesCollection Ascending(string propertyName)
        => Add(propertyName, ListSortDirection.Ascending);

    /// <summary>
    /// Adds a property with descending sort order.
    /// </summary>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <returns>This instance for method chaining.</returns>
    public SortingPropertiesCollection Descending(string propertyName)
        => Add(propertyName, ListSortDirection.Descending);

    /// <summary>
    /// Adds multiple properties with ascending sort order.
    /// Optimized for batch operations.
    /// </summary>
    /// <param name="propertyNames">The property names to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public SortingPropertiesCollection AscendingRange(IEnumerable<string> propertyNames)
    {
        // Optimize: Convert to array once and use AddRange for better performance
        var sortingProperties = propertyNames.Select(x => new SortingProperty(x, ListSortDirection.Ascending));

        using (_sortChangedDeferrer.Defer())
            base.AddRange(sortingProperties);

        return this;
    }

    /// <summary>
    /// Adds multiple properties with descending sort order.
    /// Optimized for batch operations.
    /// </summary>
    /// <param name="propertyNames">The property names to add.</param>
    /// <returns>This instance for method chaining.</returns>
    public SortingPropertiesCollection DescendingRange(IEnumerable<string> propertyNames)
    {
        // Optimize: Convert to array once and use AddRange for better performance
        var sortingProperties = propertyNames.Select(x => new SortingProperty(x, ListSortDirection.Descending));

        using (_sortChangedDeferrer.Defer())
            base.AddRange(sortingProperties);

        return this;
    }

    /// <summary>
    /// Removes all sorting properties matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to test properties.</param>
    /// <returns>The number of properties removed.</returns>
    public int RemoveSortingProperties(Func<SortingProperty, bool> predicate)
    {
        using (_sortChangedDeferrer.Defer())
            return RemoveAll(predicate);
    }

    /// <summary>
    /// Toggles the sort direction for a specific property.
    /// If the property exists, its direction is reversed.
    /// If it doesn't exist, it's added with the specified direction.
    /// </summary>
    /// <param name="propertyName">The property name to toggle.</param>
    /// <param name="defaultDirection">The default direction if the property doesn't exist.</param>
    /// <returns>This instance for method chaining.</returns>
    public SortingPropertiesCollection Toggle(string propertyName, ListSortDirection defaultDirection = ListSortDirection.Ascending)
    {
        using (_sortChangedDeferrer.Defer())
        {
            var existingProperty = this.FirstOrDefault(x => x.PropertyName == propertyName);

            if (existingProperty != null)
            {
                var index = IndexOf(existingProperty);
                var newDirection = existingProperty.Direction == ListSortDirection.Ascending
                         ? ListSortDirection.Descending
                                : ListSortDirection.Ascending;

                this[index] = new SortingProperty(propertyName, newDirection);
            }
            else
            {
                Add(new SortingProperty(propertyName, defaultDirection));
            }
        }

        return this;
    }

    [SuppressPropertyChangedWarnings]
    public void OnSortChanged() => SortChanged?.Invoke(this, EventArgs.Empty);
}
