// -----------------------------------------------------------------------
// <copyright file="ISortingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyNet.Observable.Collections.Sorting;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a view model for managing sorting configuration of a collection.
/// Provides commands and events to configure, apply, and reset sorting properties.
/// </summary>
public interface ISortingViewModel<T>
{
    /// <summary>
    /// Gets the collection of sorting property view models that can be configured for sorting.
    /// </summary>
    ReadOnlyObservableCollection<ISortingPropertyViewModel<T>> Properties { get; }

    /// <summary>
    /// Gets the current sorting configuration built from the UI.
    /// </summary>
    IReadOnlyList<ISortingProperty<T>> CurrentSorting { get; }

    /// <summary>
    /// Applies the current sorting configuration.
    /// </summary>
    void Apply();

    /// <summary>
    /// Resets the sorting configuration to its default state.
    /// The default state is defined when the view model is created.
    /// </summary>
    void Reset();

    /// <summary>
    /// Clears all active sorting.
    /// </summary>
    void Clear();

    /// <summary>
    /// Occurs when the sorting configuration has changed.
    /// Subscribers can react to apply the new sorting to their collections.
    /// </summary>
    event EventHandler<SortingChangedEventArgs<T>>? SortingChanged;
}
