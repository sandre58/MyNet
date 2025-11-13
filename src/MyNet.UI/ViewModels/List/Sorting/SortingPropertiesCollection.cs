// -----------------------------------------------------------------------
// <copyright file="SortingPropertiesCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities.Collections;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a keyed collection of sorting property view models, indexed by property name.
/// Provides efficient lookup and management of sorting properties for UI scenarios.
/// </summary>
public class SortingPropertiesCollection : ObservableKeyedCollection<string, ISortingPropertyViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertiesCollection"/> class with an empty collection.
    /// </summary>
    public SortingPropertiesCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertiesCollection"/> class
    /// from a dictionary mapping resource keys to property names.
    /// All properties are initially disabled (IsEnabled = false).
    /// </summary>
    /// <param name="properties">A dictionary where keys are resource keys for display names and values are property names.</param>
    public SortingPropertiesCollection(IDictionary<string, string> properties)
        : base(properties.Select(x => new SortingPropertyViewModel(x.Key, x.Value) { IsEnabled = false })) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertiesCollection"/> class
    /// from a collection of property names.
    /// Property names are used as both the property name and display name.
    /// All properties are initially disabled (IsEnabled = false).
    /// </summary>
    /// <param name="properties">The collection of property names to create sorting properties for.</param>
    public SortingPropertiesCollection(IEnumerable<string> properties)
        : base(properties.Select(x => new SortingPropertyViewModel(x) { IsEnabled = false })) { }

    /// <summary>
    /// Gets the key (property name) for a sorting property view model item.
    /// </summary>
    /// <param name="item">The sorting property view model.</param>
    /// <returns>The property name that serves as the unique key for this item.</returns>
    protected override string GetKeyForItem(ISortingPropertyViewModel item) => item.PropertyName;
}
