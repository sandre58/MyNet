// -----------------------------------------------------------------------
// <copyright file="SortingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using MyNet.Observable;
using MyNet.Observable.Translatables;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a view model for a single sorting property with display name, direction, and order information.
/// Extends <see cref="DisplayWrapper{T}"/> to provide localization support for the display name.
/// </summary>
/// <param name="displayName">The provider for the localized display name.</param>
/// <param name="propertyName">The name of the property to sort by.</param>
/// <param name="direction">The initial sort direction. Default is ascending.</param>
/// <param name="order">The initial sort order. Use -1 for no specific order. Default is -1.</param>
public class SortingPropertyViewModel(
    IProvideValue<string> displayName,
    string propertyName,
    ListSortDirection direction = ListSortDirection.Ascending,
    int order = -1)
    : DisplayWrapper<string>(propertyName, displayName), ISortingPropertyViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertyViewModel"/> class
    /// with the property name used as both the property name and display name.
    /// </summary>
    /// <param name="propertyName">The name of the property to sort by (also used as display name).</param>
    /// <param name="direction">The initial sort direction. Default is ascending.</param>
    /// <param name="order">The initial sort order. Use -1 for no specific order. Default is -1.</param>
    public SortingPropertyViewModel(
        string propertyName,
        ListSortDirection direction = ListSortDirection.Ascending,
        int order = -1)
        : this(propertyName, propertyName, direction, order) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingPropertyViewModel"/> class
    /// with a translatable resource key for the display name.
    /// </summary>
    /// <param name="resourceKey">The resource key for localization of the display name.</param>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <param name="direction">The initial sort direction. Default is ascending.</param>
    /// <param name="order">The initial sort order. Use -1 for no specific order. Default is -1.</param>
    public SortingPropertyViewModel(
        string resourceKey,
        string propertyName,
        ListSortDirection direction = ListSortDirection.Ascending,
        int order = -1)
        : this(new StringTranslatable(resourceKey), propertyName, direction, order) { }

    /// <summary>
    /// Gets the name of the property to sort by.
    /// This corresponds to a property name on the items being sorted.
    /// </summary>
    public string PropertyName => Item;

    /// <summary>
    /// Gets or sets the sort direction (ascending or descending).
    /// </summary>
    public ListSortDirection Direction { get; set; } = direction;

    /// <summary>
    /// Gets or sets a value indicating whether this sorting property is currently enabled/active.
    /// When true, this property contributes to the collection's sorting.
    /// When false, this property is available but not applied.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the sort order when multiple sorting properties are enabled.
    /// Lower values are applied first (primary sort), higher values are applied later.
    /// Use -1 to indicate no specific order or disabled state.
    /// </summary>
    public int Order { get; set; } = order;

    /// <summary>
    /// Creates a clone of this sorting property view model.
    /// </summary>
    /// <param name="item">The property name for the cloned instance.</param>
    /// <returns>A new instance with the same configuration as this instance.</returns>
    protected override DisplayWrapper<string> CreateCloneInstance(string item)
        => new SortingPropertyViewModel(DisplayName, item, Direction)
        {
            IsEnabled = IsEnabled,
            Order = Order
        };
}
