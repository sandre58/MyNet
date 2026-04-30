// -----------------------------------------------------------------------
// <copyright file="ISortingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.Observable.Collections.Sorting;
using MyNet.UI.ViewModels.Common;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a view model for a sorting property that can be used to configure how a collection is sorted.
/// </summary>
public interface ISortingPropertyViewModel<T> : INotifyPropertyChanged, IActivable
{
    /// <summary>
    /// Gets unique identifier for the sorting property.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets or sets the sort direction (ascending or descending).
    /// </summary>
    ListSortDirection Direction { get; set; }

    /// <summary>
    /// Gets the date and time when this sorting property was activated (enabled). This property is null if the sorting property is not currently active. When the sorting property is enabled, this property is set to the current date and time, indicating when it became active. This information can be used to determine the order in which sorting properties were activated, which can be relevant when multiple sorting properties are applied to a collection, as it may affect the overall sorting behavior.
    /// </summary>
    DateTime? ActivatedAt { get; }

    /// <summary>
    /// Determines if the given sorting property is currently part of the active sorting configuration.
    /// </summary>
    /// <param name="property">The sorting property to check.</param>
    /// <returns>True if the sorting property is part of the active sorting configuration; otherwise, false.</returns>
    bool Matches(ISortingProperty<T> property);

    /// <summary>
    /// Builds the core sorting property.
    /// Returns null if not active.
    /// </summary>
    ISortingProperty<T> Build();
}
