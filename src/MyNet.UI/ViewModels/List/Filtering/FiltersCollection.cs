// -----------------------------------------------------------------------
// <copyright file="FiltersCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyNet.UI.Collections;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents an observable collection of composite filter view models.
/// Provides convenience methods for adding filters directly without wrapping them in composite view models.
/// </summary>
public class FiltersCollection : UiObservableCollection<ICompositeFilterViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersCollection"/> class with an empty collection.
    /// </summary>
    public FiltersCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersCollection"/> class
    /// with a collection of composite filter view models.
    /// </summary>
    /// <param name="filters">The collection of composite filter view models.</param>
    public FiltersCollection(IEnumerable<ICompositeFilterViewModel> filters)
        : base(filters) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersCollection"/> class
    /// with a collection of filter view models that are automatically wrapped in composite filters.
    /// </summary>
    /// <param name="properties">The collection of filter view models to wrap.</param>
    public FiltersCollection(IEnumerable<IFilterViewModel> properties)
        : base(properties.Select(x => new CompositeFilterViewModel(x))) { }

    /// <summary>
    /// Adds a filter view model to the collection by automatically wrapping it in a composite filter.
    /// </summary>
    /// <param name="filter">The filter view model to add.</param>
    public void Add(IFilterViewModel filter) => Add(new CompositeFilterViewModel(filter));

    /// <summary>
    /// Adds multiple filter view models to the collection by automatically wrapping them in composite filters.
    /// </summary>
    /// <param name="filters">The collection of filter view models to add.</param>
    public void AddRange(IEnumerable<IFilterViewModel> filters)
        => AddRange(filters.Select(x => new CompositeFilterViewModel(x)));
}
