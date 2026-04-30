// -----------------------------------------------------------------------
// <copyright file="FilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DynamicData.Binding;
using MyNet.Observable;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Abstract base class for filter view models.
/// Provides common functionality for evaluating filters against objects and their properties.
/// Supports nested property paths and collection filtering.
/// </summary>
/// <remarks>
/// <para><strong>Key Features:</strong></para>
/// <list type="bullet">
/// <item>Nested property access using dot notation (e.g., "Person.Address.City")</item>
/// <item>Collection filtering (filters items within collections)</item>
/// <item>Automatic change notification when filter criteria change</item>
/// <item>Clone and similarity comparison support</item>
/// </list>
/// <para><strong>Usage:</strong></para>
/// <code>
/// public class CustomFilter : FilterViewModel
/// {
///     public CustomFilter(string propertyName) : base(propertyName) { }
///     protected override bool IsMatchProperty(object? toCompare)
///     {
///         // Implement your matching logic
///         return true;
///     }
///     public override bool IsEmpty() => /* check if filter has criteria */;
///     public override void Reset() => /* reset to default state */;
/// }
/// </code>
/// </remarks>
public abstract class FilterViewModel : ObservableObject, IFilterViewModel, IFiltersViewModel
{
    private static readonly string[] Separator = ["."];

    private event EventHandler<FiltersChangedEventArgs>? FiltersChanged;

    /// <summary>
    /// Explicit interface implementation of FiltersChanged event.
    /// Use the protected <see cref="OnFilterChanged"/> method to raise this event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Use FiltersChanged")]
    event EventHandler<FiltersChangedEventArgs>? IFiltersViewModel.FiltersChanged
    {
        add => FiltersChanged += value;
        remove => FiltersChanged -= value;
    }

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterViewModel"/> class.
    /// Subscribes to property changes to automatically trigger filter re-evaluation.
    /// </summary>
    /// <param name="propertyName">The name of the property to filter on. Supports dot notation for nested properties (e.g., "Person.Address.City").</param>
    protected FilterViewModel(string propertyName)
    {
        PropertyName = propertyName;

        // Subscribe to any property change to trigger filter change notification
        Disposables.Add(this.WhenAnyPropertyChanged().Subscribe(_ => OnFilterChanged()));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the property this filter applies to.
    /// Supports dot notation for nested properties (e.g., "Person.Address.City").
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this filter is read-only.
    /// When true, the filter configuration cannot be modified.
    /// </summary>
    public bool IsReadOnly { get; set; }

    #endregion

    #region Matching Logic

    /// <summary>
    /// Determines whether the specified target object matches this filter.
    /// Navigates through nested properties using the <see cref="PropertyName"/> path.
    /// </summary>
    /// <param name="target">The object to evaluate against this filter.</param>
    /// <returns>True if the object matches the filter criteria; otherwise, false.</returns>
    /// <remarks>
    /// This method supports:
    /// <list type="bullet">
    /// <item>Simple properties: "Name"</item>
    /// <item>Nested properties: "Person.Address.City"</item>
    /// <item>Collections at any level: Will check if any item in the collection matches</item>
    /// </list>
    /// </remarks>
    public virtual bool IsMatch(object? target)
        => IsMatchInternal(target, PropertyName.Split(Separator, StringSplitOptions.RemoveEmptyEntries));

    /// <summary>
    /// Internal recursive method to navigate through property paths and evaluate the filter.
    /// </summary>
    /// <param name="target">The current object being evaluated.</param>
    /// <param name="propertyNames">The remaining property names to navigate.</param>
    /// <returns>True if the target matches; otherwise, false.</returns>
    private bool IsMatchInternal(object? target, IList<string> propertyNames)
    {
        if (target is null)
            return false;

        var toCompare = target;

        // Base case: no more properties to navigate
        if (!propertyNames.Any())
        {
            return toCompare is IList toCompareEnumerable1
                ? IsMatchPropertyList(toCompareEnumerable1.Cast<object>())
                : IsMatchProperty(toCompare);
        }

        var newPropertyNames = propertyNames.ToList();

        // Navigate through property path
        foreach (var propertyName in propertyNames)
        {
            var propertyInfo = toCompare?.GetType().GetProperty(propertyName);
            if (propertyInfo is null)
                return false;

            toCompare = propertyInfo.GetValue(toCompare, null);
            _ = newPropertyNames.Remove(propertyName);

            // Handle collections in the middle of the path
            if (newPropertyNames.Count > 0 && toCompare is IList toCompareEnumerableRecursive)
                return toCompareEnumerableRecursive.Cast<object>().Any(x => IsMatchInternal(x, newPropertyNames));
        }

        // Final property reached
        return toCompare is IList toCompareEnumerable
     ? IsMatchPropertyList(toCompareEnumerable.Cast<object>())
: IsMatchProperty(toCompare);
    }

    /// <summary>
    /// Determines whether any item in a collection matches the filter.
    /// Override this to change the collection matching behavior (e.g., all items must match).
    /// </summary>
    /// <param name="toCompareEnumerable">The collection of objects to evaluate.</param>
    /// <returns>True if any item matches; otherwise, false.</returns>
    protected virtual bool IsMatchPropertyList(IEnumerable<object> toCompareEnumerable)
 => toCompareEnumerable.Any(IsMatchProperty);

    /// <summary>
    /// Determines whether the specified property value matches the filter criteria.
    /// This is the core matching logic that derived classes must implement.
    /// </summary>
    /// <param name="toCompare">The property value to compare against the filter criteria.</param>
    /// <returns>True if the value matches; otherwise, false.</returns>
    protected abstract bool IsMatchProperty(object? toCompare);

    #endregion

    #region Filter State

    /// <summary>
    /// Determines whether this filter is in an empty state (no criteria specified).
    /// Derived classes must implement this to indicate when the filter has no active criteria.
    /// </summary>
    /// <returns>True if the filter has no active criteria; otherwise, false.</returns>
    /// <remarks>
    /// Empty filters typically match all items. For example:
    /// <list type="bullet">
    /// <item>StringFilter: IsEmpty when Value is null or whitespace</item>
    /// <item>IntegerFilter: IsEmpty when both Min and Max are null</item>
    /// <item>BooleanFilter: IsEmpty when Value is null</item>
    /// </list>
    /// </remarks>
    public abstract bool IsEmpty();

    /// <summary>
    /// Resets the filter to its default state.
    /// Derived classes must implement this to clear all filter criteria.
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// Refreshes the filter and triggers a re-evaluation.
    /// Raises the <see cref="FiltersChanged"/> event even if criteria haven't changed.
    /// </summary>
    public void Refresh() => OnFilterChanged();

    /// <summary>
    /// Explicit interface implementation of Clear.
    /// Delegates to <see cref="Reset"/> to maintain consistent behavior.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Use Reset")]
    void IFiltersViewModel.Clear() => Reset();

    #endregion

    #region Event Handling

    /// <summary>
    /// Raises the <see cref="FiltersChanged"/> event when filter criteria change.
    /// Automatically called when any property of the filter changes.
    /// </summary>
    [SuppressPropertyChangedWarnings]
    private void OnFilterChanged()
        => FiltersChanged?.Invoke(this, new FiltersChangedEventArgs([new CompositeFilterViewModel(this)]));

    #endregion

    #region Equality and Cloning

    /// <summary>
    /// Determines whether the specified object is equal to the current filter.
    /// Two filters are equal if they are of the same type and filter the same property.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
 => obj is FilterViewModel o && GetType() == obj.GetType() && PropertyName == o.PropertyName;

    /// <summary>
    /// Returns a hash code based on the property name.
    /// </summary>
    /// <returns>A hash code for this filter.</returns>
    public override int GetHashCode() => PropertyName.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a deep clone of this filter with the same criteria.
    /// </summary>
    /// <returns>A cloned filter instance.</returns>
    public object Clone()
    {
        using (PropertyChangedSuspender.Suspend())
        {
            var newEntity = CreateCloneInstance();
            newEntity.SetFrom(this);
            return newEntity;
        }
    }

    /// <summary>
    /// Creates a new instance of the filter for cloning.
    /// Derived classes must implement this to create the appropriate filter type.
    /// </summary>
    /// <returns>A new filter instance of the same type.</returns>
    protected abstract FilterViewModel CreateCloneInstance();

    /// <summary>
    /// Sets the filter criteria from another filter instance.
    /// Used during cloning to copy all criteria properties.
    /// </summary>
    /// <param name="from">The filter to copy criteria from.</param>
    public abstract void SetFrom(object? from);

    /// <summary>
    /// Determines whether this filter is similar to another filter.
    /// Filters are similar if they are of the same type and filter the same property.
    /// </summary>
    /// <param name="obj">The filter to compare with.</param>
    /// <returns>True if similar; otherwise, false.</returns>
    public virtual bool IsSimilar(IFilterViewModel? obj)
  => GetType() == obj?.GetType() && PropertyName == obj.PropertyName;

    #endregion
}
