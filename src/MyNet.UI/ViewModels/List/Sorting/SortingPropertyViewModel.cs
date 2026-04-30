// -----------------------------------------------------------------------
// <copyright file="SortingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using MyNet.Observable;
using MyNet.Observable.Collections.Sorting;
using MyNet.Observable.Translatables;
using MyNet.Utilities.Comparers;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Represents a view model for a sorting property that can be used to configure how a collection is sorted.
/// </summary>
/// <param name="expression">The expression used to access the property to sort by.</param>
/// <param name="key">The unique identifier for the sorting property.</param>
/// <param name="displayName">The provider for the localized display name.</param>
/// <param name="direction">The initial sort direction. Default is ascending.</param>
/// <typeparam name="T">The type of the items being sorted.</typeparam>
public class SortingPropertyViewModel<T>(
    Expression<Func<T, object?>> expression,
    string key,
    IProvideValue<string> displayName,
    ListSortDirection direction = ListSortDirection.Ascending)
    : DisplayWrapper<string>(key, displayName), ISortingPropertyViewModel<T>
{
    /// <summary>
    /// Gets the unique identifier for the sorting property. This key is used to identify the sorting property within the collection of sorting properties. It should be unique among all sorting properties to avoid conflicts when applying sorting to a collection. The key can be used by consumers to reference this specific sorting property when configuring sorting behavior or when building the collection of active sorting properties.
    /// </summary>
    string ISortingPropertyViewModel<T>.Key { get; } = key;

    /// <summary>
    /// Gets the expression that defines the sorting property. This expression is used to determine the value of the property by which the items will be sorted. The expression should return a value that can be compared for sorting purposes, such as a property or a computed value based on the properties of T.
    /// </summary>
    public Expression<Func<T, object?>> Expression { get; } = expression;

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
    /// Gets the date and time when this sorting property was activated (enabled). This property is null if the sorting property is not currently active. When the sorting property is enabled, this property is set to the current date and time, indicating when it became active. This information can be used to determine the order in which sorting properties were activated, which can be relevant when multiple sorting properties are applied to a collection, as it may affect the overall sorting behavior.
    /// </summary>
    public DateTime? ActivatedAt { get; private set; }

    /// <summary>
    /// Builds the core sorting property based on the current state of the view model. If the sorting property is active (enabled), this method returns an instance of <see cref="ISortingProperty{T}"/> that encapsulates the expression and direction defined in the view model. If the sorting property is not active, this method returns null, indicating that it should not be applied to the collection's sorting configuration.
    /// </summary>
    /// <returns>An instance of <see cref="ISortingProperty{T}"/> if the sorting property is active; otherwise, null.</returns>
    public ISortingProperty<T> Build() => new ExpressionSortingProperty<T>(Expression, Direction);

    /// <summary>
    /// Determines whether the provided sorting property matches the expression of this view model. This method compares the expression of this view model with the expression provided by the given sorting property using an expression equality comparer. If the expressions are considered equal, it returns true, indicating that the sorting property corresponds to this view model; otherwise, it returns false. This can be useful for identifying which sorting properties in a collection correspond to which view models when applying sorting configurations.
    /// </summary>
    /// <param name="property">The sorting property to compare with this view model.</param>
    /// <returns>True if the expressions match; otherwise, false.</returns>
    public bool Matches(ISortingProperty<T> property) => ExpressionEqualityComparer.Equals(Expression, property.ProvideExpression());

    /// <summary>
    /// Handles changes to the IsEnabled property. When the IsEnabled property changes, this method updates the ActivatedAt property to reflect the current date and time if the sorting property is enabled, or sets it to null if it is disabled. This allows consumers to track when each sorting property was activated, which can be important for determining the order of sorting when multiple properties are active.
    /// </summary>
    protected virtual void OnIsEnabledChanged() => ActivatedAt = IsEnabled ? DateTime.UtcNow : null;
}
