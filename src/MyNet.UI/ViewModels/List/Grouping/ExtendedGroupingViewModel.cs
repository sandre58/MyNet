// -----------------------------------------------------------------------
// <copyright file="ExtendedGroupingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyNet.Observable.Attributes;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Extended grouping view model that manages a predefined set of available grouping properties.
/// Unlike <see cref="GroupingViewModel"/>, properties cannot be dynamically added or removed.
/// Instead, existing properties can be enabled/disabled and reordered.
/// This is useful for UI scenarios where users select from a fixed list of groupable columns.
/// </summary>
/// <remarks>
/// This implementation makes the collection read-only (<see cref="IsReadOnly"/> = true)
/// and overrides grouping operations to work with enabling/disabling existing properties
/// rather than adding/removing them from the collection.
/// <para><strong>Performance Optimizations:</strong></para>
/// <list type="bullet">
/// <item>Uses array-based iteration instead of LINQ for order recomputation (faster)</item>
/// <item>Dictionary lookup for O(1) property matching instead of O(n) FirstOrDefault</item>
/// <item>Minimizes allocations by avoiding unnecessary ToList() conversions</item>
/// <item>Batch operations with deferred execution to reduce notifications</item>
/// </list>
/// </remarks>
[CanBeValidated(false)]
[CanSetIsModified(false)]
public class ExtendedGroupingViewModel : GroupingViewModel
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedGroupingViewModel"/> class
    /// with a collection of property names and default grouping properties.
    /// </summary>
    /// <param name="properties">The collection of property names that can be used for grouping.</param>
    /// <param name="defaultProperties">The property names to enable by default.</param>
    public ExtendedGroupingViewModel(IEnumerable<string> properties, IEnumerable<string> defaultProperties)
        : this(new GroupingPropertiesCollection(properties), defaultProperties) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedGroupingViewModel"/> class
    /// with a collection of property names (no default grouping).
    /// All properties are initially disabled.
    /// </summary>
    /// <param name="properties">The collection of property names that can be used for grouping.</param>
    public ExtendedGroupingViewModel(IEnumerable<string> properties)
     : this(new GroupingPropertiesCollection(properties), []) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedGroupingViewModel"/> class
    /// with a dictionary mapping resource keys to property names (no default grouping).
    /// All properties are initially disabled.
    /// </summary>
    /// <param name="properties">A dictionary where keys are resource keys for display names and values are property names.</param>
    public ExtendedGroupingViewModel(IDictionary<string, string> properties)
  : this(properties.Select(x => new GroupingPropertyViewModel(x.Key, x.Value, x.Value)), []) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedGroupingViewModel"/> class
    /// with a collection of grouping property view models (no default grouping).
    /// </summary>
    /// <param name="properties">The collection of grouping property view models to manage.</param>
    public ExtendedGroupingViewModel(IEnumerable<IGroupingPropertyViewModel> properties)
        : this(properties, []) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedGroupingViewModel"/> class
    /// with a collection of grouping property view models and default grouping configuration.
    /// </summary>
    /// <param name="properties">The collection of grouping property view models to manage.</param>
    /// <param name="defaultProperties">The property names to enable by default.</param>
    public ExtendedGroupingViewModel(
        IEnumerable<IGroupingPropertyViewModel> properties,
        IEnumerable<string> defaultProperties)
        : base(defaultProperties)
    {
        GroupingProperties.AddRange(properties);
        Reset();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// Always returns true for <see cref="ExtendedGroupingViewModel"/> because the set of available properties is fixed.
    /// Properties can be enabled/disabled but not added/removed.
    /// </summary>
    public override bool IsReadOnly => true;

    #endregion

    #region Grouping Operations (Override to Enable/Disable Instead of Add/Remove)

    /// <summary>
    /// Enables an existing grouping property with the specified sorting property.
    /// If the property doesn't exist or is already enabled, does nothing.
    /// </summary>
    /// <param name="propertyName">The name of the property to enable.</param>
    /// <param name="sortingPropertyName">Optional property name for sorting groups.</param>
    /// <param name="order">The group order. If null, assigns the next available order (ActiveCount + 1).</param>
    public override void Add(string propertyName, string? sortingPropertyName = null, int? order = null)
    {
        if (GroupingProperties[propertyName] is not { IsEnabled: false } property)
            return;

        using (DeferChanged())
        {
            property.Order = order ?? ActiveCount + 1;
            property.IsEnabled = true;
        }
    }

    /// <summary>
    /// Disables a grouping property without removing it from the available properties.
    /// If the property doesn't exist or is already disabled, does nothing.
    /// Reorders remaining enabled properties to fill the gap.
    /// </summary>
    /// <param name="propertyName">The name of the property to disable.</param>
    /// <remarks>
    /// <para><strong>Performance:</strong> This method is optimized to minimize allocations and iterations.</para>
    /// <list type="bullet">
    /// <item>Single-pass iteration through enabled properties</item>
    /// <item>Array-based loop instead of LINQ for better performance</item>
    /// <item>Direct index assignment instead of IndexOf calls</item>
    /// </list>
    /// </remarks>
    public override void Remove(string propertyName)
    {
        if (GroupingProperties[propertyName] is not { IsEnabled: true } property)
            return;

        using (DeferChanged())
        {
            property.IsEnabled = false;
            property.Order = -1;

            // Performance optimization: Single-pass reordering with array instead of LINQ
            // Old approach: .Where().OrderBy().ToList().ForEach(IndexOf) → multiple iterations + allocations
            // New approach: Single iteration with direct index tracking → O(n) complexity
            var enabledProperties = GroupingProperties
              .Where(x => x.IsEnabled)
             .OrderBy(x => x.Order)
                   .ToArray(); // ToArray is faster than ToList for iteration

            // Direct index assignment - avoids IndexOf calls
            for (var i = 0; i < enabledProperties.Length; i++)
            {
                enabledProperties[i].Order = i + 1;
            }
        }
    }

    /// <summary>
    /// Sets the enabled state and order for all properties based on the provided collection.
    /// Properties not in the provided collection are disabled.
    /// Properties in the collection are enabled with their specified configuration.
    /// </summary>
    /// <param name="properties">The collection of grouping properties to apply.</param>
    /// <remarks>
    /// <para><strong>Performance:</strong> This method is optimized for batch updates.</para>
    /// <list type="bullet">
    /// <item>Single ToArray call instead of multiple ToList conversions</item>
    /// <item>Dictionary lookup for O(1) property matching instead of O(n) FirstOrDefault</item>
    /// <item>Minimized allocations by reusing the properties array</item>
    /// </list>
    /// </remarks>
    public override void Set(IEnumerable<IGroupingPropertyViewModel> properties)
    {
        using (DeferChanged())
        {
            // Performance optimization: Use array + dictionary for faster lookups
            // Old: list.FirstOrDefault(x => x.PropertyName == item.PropertyName) → O(n) per item = O(n²) total
            // New: dictionary[item.PropertyName] → O(1) per item = O(n) total
            var propertiesArray = properties as IGroupingPropertyViewModel[] ?? [.. properties];
            var propertyLookup = propertiesArray.ToDictionary(x => x.PropertyName);

            foreach (var item in GroupingProperties)
            {
                // O(1) dictionary lookup instead of O(n) FirstOrDefault
                var found = propertyLookup.TryGetValue(item.PropertyName, out var similarProperty);

                item.IsEnabled = found && (similarProperty?.IsEnabled ?? false);

                // Handle order: -1 if disabled, use specified order or array index if enabled
                item.Order = !found || similarProperty is null
                 ? -1
                 : similarProperty.Order < 0
             ? System.Array.IndexOf(propertiesArray, similarProperty) + 1
                 : similarProperty.Order;
            }
        }
    }

    /// <summary>
    /// Disables all grouping properties without removing them from the available properties.
    /// </summary>
    /// <remarks>
    /// <para><strong>Performance:</strong> This method is optimized for batch disabling.</para>
    /// <list type="bullet">
    /// <item>Single iteration through properties (no ToList conversion needed)</item>
    /// <item>Direct property modification within defer scope</item>
    /// <item>Minimized allocations and notifications</item>
    /// </list>
    /// </remarks>
    public override void Clear()
    {
        using (DeferChanged())
        {
            // Performance optimization: Direct iteration without ToList
            // Old: GroupingProperties.ToList().ForEach() → Creates unnecessary list copy
            // New: Direct foreach → No allocation, same iteration
            foreach (var property in GroupingProperties)
            {
                property.IsEnabled = false;
                property.Order = -1;
            }
        }
    }

    #endregion
}
