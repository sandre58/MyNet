// -----------------------------------------------------------------------
// <copyright file="ISortingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a view model for managing sorting configuration of a collection.
/// Provides commands and events to configure, apply, and reset sorting properties.
/// </summary>
public interface ISortingViewModel
{
    /// <summary>
    /// Gets the command to apply a specific sorting configuration.
    /// Typically accepts a collection of (propertyName, direction) tuples.
    /// </summary>
    ICommand ApplyCommand { get; }

    /// <summary>
    /// Resets the sorting configuration to its default state.
    /// The default state is defined when the view model is created.
    /// </summary>
    void Reset();

    /// <summary>
    /// Occurs when the sorting configuration has changed.
    /// Subscribers can react to apply the new sorting to their collections.
    /// </summary>
    event EventHandler<SortingChangedEventArgs>? SortingChanged;
}
