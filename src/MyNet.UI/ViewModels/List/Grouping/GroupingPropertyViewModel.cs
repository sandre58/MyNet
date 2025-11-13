// -----------------------------------------------------------------------
// <copyright file="GroupingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable;
using MyNet.Observable.Translatables;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a view model for a single grouping property with display name, sorting configuration, and order information.
/// Extends <see cref="DisplayWrapper{T}"/> to provide localization support for the display name.
/// </summary>
/// <param name="displayName">The provider for the localized display name.</param>
/// <param name="propertyName">The name of the property to group by.</param>
/// <param name="sortingPropertyName">The property name used for sorting groups.</param>
/// <param name="order">The initial grouping order. Use -1 for no specific order. Default is -1.</param>
public class GroupingPropertyViewModel(
    IProvideValue<string> displayName,
    string propertyName,
    string sortingPropertyName,
    int order = -1)
    : DisplayWrapper<string>(propertyName, displayName), IGroupingPropertyViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingPropertyViewModel"/> class
/// with a translatable resource key for the display name.
    /// </summary>
    /// <param name="resourceKey">The resource key for localization of the display name.</param>
    /// <param name="propertyName">The name of the property to group by.</param>
    /// <param name="sortingPropertyName">The property name for sorting groups. If null, uses <paramref name="propertyName"/>.</param>
    /// <param name="order">The initial grouping order. Use -1 for no specific order. Default is -1.</param>
    public GroupingPropertyViewModel(
        string resourceKey,
        string propertyName,
        string? sortingPropertyName = null,
        int order = -1)
   : this(new StringTranslatable(resourceKey), propertyName, sortingPropertyName ?? propertyName, order) { }

    /// <summary>
    /// Gets the name of the property to group by.
    /// This corresponds to a property name on the items being grouped.
    /// </summary>
    public string PropertyName => Item;

    /// <summary>
    /// Gets the name of the property used for sorting groups.
    /// Can be different from <see cref="PropertyName"/> to allow sorting groups by a different criterion.
    /// </summary>
    public string SortingPropertyName { get; } = sortingPropertyName;

    /// <summary>
    /// Gets or sets a value indicating whether this grouping property is currently enabled/active.
    /// When false, this property is available but not applied to the collection.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the grouping order when multiple grouping properties are enabled.
    /// Lower values are applied first (primary group), higher values create sub-groups (secondary, tertiary, etc.).
    /// Use -1 to indicate no specific order or disabled state.
    /// </summary>
    public int Order { get; set; } = order;
}
