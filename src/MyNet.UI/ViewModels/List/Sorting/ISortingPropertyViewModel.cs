// -----------------------------------------------------------------------
// <copyright file="ISortingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a view model for a sorting property that can be used to configure how a collection is sorted.
/// </summary>
public interface ISortingPropertyViewModel : INotifyPropertyChanged, ICloneable
{
    /// <summary>
    /// Gets the name of the property to sort by.
    /// This should correspond to a property name on the items being sorted.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    /// Gets or sets the sort direction (ascending or descending).
    /// </summary>
    ListSortDirection Direction { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this sorting property is currently enabled/active.
    /// When false, this property is available but not applied to the collection.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the sort order when multiple sorting properties are enabled.
    /// Lower values are applied first (primary sort), higher values are applied later (secondary, tertiary, etc.).
    /// Use -1 to indicate no specific order or disabled state.
    /// </summary>
    int Order { get; set; }
}
