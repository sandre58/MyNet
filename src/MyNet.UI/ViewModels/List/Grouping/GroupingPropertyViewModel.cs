// -----------------------------------------------------------------------
// <copyright file="GroupingPropertyViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Observable;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Translatables;
using MyNet.Utilities.Comparers;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a view model for a grouping property that can be used to configure how a collection is grouped.
/// </summary>
/// <param name="expression">The expression used to access the property to group by.</param>
/// <param name="key">The unique identifier for the grouping property.</param>
/// <param name="displayName">The provider for the localized display name.</param>
/// <typeparam name="T">The type of the items being grouped.</typeparam>
public class GroupingPropertyViewModel<T>(
    Expression<Func<T, object?>> expression,
    string key,
    IProvideValue<string> displayName)
    : DisplayWrapper<string>(key, displayName), IGroupingPropertyViewModel<T>
{
    /// <summary>
    /// Gets the unique identifier for the grouping property. This key is used to identify the grouping property within the collection of grouping properties. It should be unique among all grouping properties to avoid conflicts when applying grouping to a collection. The key can be used by consumers to reference this specific grouping property when configuring grouping behavior or when building the collection of active grouping properties.
    /// </summary>
    string IGroupingPropertyViewModel<T>.Key { get; } = key;

    /// <summary>
    /// Gets the expression that defines the grouping property. This expression is used to determine the value of the property by which the items will be grouped. The expression should return a value that can be compared for grouping purposes, such as a property or a computed value based on the properties of T.
    /// </summary>
    public Expression<Func<T, object?>> Expression { get; } = expression;

    /// <summary>
    /// Gets or sets a value indicating whether this grouping property is currently enabled/active.
    /// When true, this property contributes to the collection's grouping.
    /// When false, this property is available but not applied.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets the date and time when this grouping property was activated (enabled). This property is null if the grouping property is not currently active. When the grouping property is enabled, this property is set to the current date and time, indicating when it became active. This information can be used to determine the order in which grouping properties were activated, which can be relevant when multiple grouping properties are applied to a collection, as it may affect the overall grouping behavior.
    /// </summary>
    public DateTime? ActivatedAt { get; private set; }

    /// <summary>
    /// Builds the core grouping property based on the current state of the view model. If the grouping property is active (enabled), this method returns an instance of <see cref="IGroupingProperty{T}"/> that encapsulates the expression defined in the view model. If the grouping property is not active, this method returns null, indicating that it should not be applied to the collection's grouping configuration.
    /// </summary>
    /// <returns>An instance of <see cref="IGroupingProperty{T}"/> if the grouping property is active; otherwise, null.</returns>
    public IGroupingProperty<T> Build() => new ExpressionGroupingProperty<T>(Expression);

    /// <summary>
    /// Determines whether the provided grouping property matches the expression of this view model. This method compares the expression of this view model with the expression provided by the given grouping property using an expression equality comparer. If the expressions are considered equal, it returns true, indicating that the grouping property corresponds to this view model; otherwise, it returns false. This can be useful for identifying which grouping properties in a collection correspond to which view models when applying grouping configurations.
    /// </summary>
    /// <param name="property">The grouping property to compare with this view model.</param>
    /// <returns>True if the expressions match; otherwise, false.</returns>
    public bool Matches(IGroupingProperty<T> property) => ExpressionEqualityComparer.Equals(Expression, property.ProvideExpression());

    /// <summary>
    /// Handles changes to the IsEnabled property. When the IsEnabled property changes, this method updates the ActivatedAt property to reflect the current date and time if the grouping property is enabled, or sets it to null if it is disabled. This allows consumers to track when each grouping property was activated, which can be important for determining the order of grouping when multiple properties are active.
    /// </summary>
    protected virtual void OnIsEnabledChanged() => ActivatedAt = IsEnabled ? DateTime.UtcNow : null;
}
