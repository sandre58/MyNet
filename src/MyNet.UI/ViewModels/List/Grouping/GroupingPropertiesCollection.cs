// -----------------------------------------------------------------------
// <copyright file="GroupingPropertiesCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities.Collections;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a keyed collection of grouping property view models, indexed by property name.
/// Provides efficient lookup and management of grouping properties for UI scenarios.
/// </summary>
public class GroupingPropertiesCollection : ObservableKeyedCollection<string, IGroupingPropertyViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingPropertiesCollection"/> class with an empty collection.
    /// </summary>
    public GroupingPropertiesCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingPropertiesCollection"/> class
    /// from a dictionary mapping resource keys to property names.
    /// Property names are used for both grouping and sorting.
    /// </summary>
    /// <param name="properties">A dictionary where keys are resource keys for display names and values are property names.</param>
    public GroupingPropertiesCollection(IDictionary<string, string> properties)
        : base(properties.Select(x => new GroupingPropertyViewModel(x.Key, x.Value, x.Value))) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingPropertiesCollection"/> class
    /// from a collection of property names.
    /// Property names are used as both the property name, display name, and sorting property.
    /// </summary>
    /// <param name="properties">The collection of property names to create grouping properties for.</param>
    public GroupingPropertiesCollection(IEnumerable<string> properties)
        : base(properties.Select(x => new GroupingPropertyViewModel(x, x, x))) { }

    /// <summary>
    /// Gets the key (property name) for a grouping property view model item.
    /// </summary>
    /// <param name="item">The grouping property view model.</param>
    /// <returns>The property name that serves as the unique key for this item.</returns>
    protected override string GetKeyForItem(IGroupingPropertyViewModel item) => item.PropertyName;
}
