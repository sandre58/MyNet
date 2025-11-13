// -----------------------------------------------------------------------
// <copyright file="IGroupingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a view model for a single grouping property that can be used to configure how a collection is grouped.
/// </summary>
public interface IGroupingPropertyViewModel : INotifyPropertyChanged, ICloneable
{
    /// <summary>
    /// Gets the name of the property to group by.
    /// This should correspond to a property name on the items being grouped.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    /// Gets the name of the property used for sorting groups.
    /// If different from <see cref="PropertyName"/>, allows sorting groups by a different property.
    /// </summary>
    string SortingPropertyName { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this grouping property is currently enabled/active.
    /// When false, this property is available but not applied to the collection.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the grouping order when multiple grouping properties are enabled.
    /// Lower values are applied first (primary group), higher values are applied later (sub-groups).
    /// Use -1 to indicate no specific order or disabled state.
    /// </summary>
    int Order { get; set; }
}
