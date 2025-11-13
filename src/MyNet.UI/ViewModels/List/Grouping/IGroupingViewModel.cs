// -----------------------------------------------------------------------
// <copyright file="IGroupingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a view model for managing grouping configuration of a collection.
/// Provides methods and events to configure, apply, and reset grouping properties.
/// </summary>
public interface IGroupingViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Resets the grouping configuration to its default state.
    /// The default state is defined when the view model is created.
    /// </summary>
    void Reset();

    /// <summary>
    /// Occurs when the grouping configuration has changed.
    /// Subscribers can react to apply the new grouping to their collections.
    /// </summary>
    event EventHandler<GroupingChangedEventArgs>? GroupingChanged;
}
