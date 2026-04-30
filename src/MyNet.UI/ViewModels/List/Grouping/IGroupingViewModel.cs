// -----------------------------------------------------------------------
// <copyright file="IGroupingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyNet.Observable.Collections.Grouping;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a view model for managing grouping configuration of a collection.
/// Provides commands and events to configure, apply, and reset grouping properties.
/// </summary>
public interface IGroupingViewModel<T>
{
    /// <summary>
    /// Gets the collection of grouping property view models that can be configured for grouping.
    /// </summary>
    ReadOnlyObservableCollection<IGroupingPropertyViewModel<T>> Properties { get; }

    /// <summary>
    /// Gets the current grouping configuration built from the UI.
    /// </summary>
    IReadOnlyList<IGroupingProperty<T>> CurrentGrouping { get; }

    /// <summary>
    /// Applies the current grouping configuration.
    /// </summary>
    void Apply();

    /// <summary>
    /// Resets the grouping configuration to its default state.
    /// The default state is defined when the view model is created.
    /// </summary>
    void Reset();

    /// <summary>
    /// Clears all active grouping.
    /// </summary>
    void Clear();

    /// <summary>
    /// Occurs when the grouping configuration has changed.
    /// Subscribers can react to apply the new grouping to their collections.
    /// </summary>
    event EventHandler<GroupingChangedEventArgs<T>>? GroupingChanged;
}
