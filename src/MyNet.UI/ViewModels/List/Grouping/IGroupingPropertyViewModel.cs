// -----------------------------------------------------------------------
// <copyright file="IGroupingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.Observable.Collections.Grouping;
using MyNet.UI.ViewModels.Common;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a view model for a grouping property that can be used to configure how a collection is grouped.
/// </summary>
public interface IGroupingPropertyViewModel<T> : INotifyPropertyChanged, IActivable
{
    /// <summary>
    /// Gets unique identifier for the grouping property.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the date and time when this grouping property was activated (enabled). This property is null if the grouping property is not currently active. When the grouping property is enabled, this property is set to the current date and time, indicating when it became active. This information can be used to determine the order in which grouping properties were activated, which can be relevant when multiple grouping properties are applied to a collection, as it may affect the overall grouping behavior.
    /// </summary>
    DateTime? ActivatedAt { get; }

    /// <summary>
    /// Determines if the given grouping property is currently part of the active grouping configuration.
    /// </summary>
    /// <param name="property">The grouping property to check.</param>
    bool Matches(IGroupingProperty<T> property);

    /// <summary>
    /// Builds the core grouping property.
    /// Returns null if not active.
    /// </summary>
    IGroupingProperty<T> Build();
}
