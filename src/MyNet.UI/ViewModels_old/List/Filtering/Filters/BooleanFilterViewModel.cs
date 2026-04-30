// -----------------------------------------------------------------------
// <copyright file="BooleanFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for boolean properties.
/// Supports nullable boolean values with optional null value filtering.
/// </summary>
/// <param name="propertyName">The name of the boolean property to filter on.</param>
/// <remarks>
/// <para><strong>Usage:</strong></para>
/// <code>
/// // Filter for active items
/// var filter = new BooleanFilterViewModel("IsActive")
/// {
///   Value = true
/// };
/// // Allow null values in filter
/// var filterWithNull = new BooleanFilterViewModel("IsDeleted")
/// {
///     AllowNullValue = true,
///     Value = null // Matches items where IsDeleted is null
/// };
/// </code>
/// </remarks>
public class BooleanFilterViewModel(string propertyName) : FilterViewModel(propertyName)
{
    /// <summary>
    /// Gets or sets the boolean value to filter by.
    /// When null, the filter behavior depends on <see cref="AllowNullValue"/>.
    /// </summary>
    public bool? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether null values are allowed in the filter.
    /// When true, <see cref="Reset"/> sets Value to null instead of false.
    /// Default is false.
    /// </summary>
    public bool AllowNullValue { get; set; }

    /// <summary>
    /// Determines whether the specified boolean value matches the filter criteria.
    /// </summary>
    /// <param name="toCompare">The boolean value to compare.</param>
    /// <returns>True if the value matches <see cref="Value"/>; otherwise, false.</returns>
    protected override bool IsMatchProperty(object? toCompare) => (bool?)toCompare == Value;

    /// <summary>
    /// Determines whether this filter is in an empty state.
    /// A boolean filter is empty when <see cref="Value"/> is null.
    /// </summary>
    /// <returns>True if Value is null; otherwise, false.</returns>
    public override bool IsEmpty() => Value is null;

    /// <summary>
    /// Resets the filter to its default state.
    /// If <see cref="AllowNullValue"/> is true, sets Value to null; otherwise, sets it to false.
    /// </summary>
    public override void Reset() => Value = AllowNullValue ? null : false;

    /// <summary>
    /// Sets the filter criteria from another boolean filter instance.
    /// </summary>
    /// <param name="from">The filter to copy criteria from. Must be a <see cref="BooleanFilterViewModel"/>.</param>
    public override void SetFrom(object? from)
    {
        if (from is not BooleanFilterViewModel other)
            return;
        AllowNullValue = other.AllowNullValue;
        Value = other.Value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="BooleanFilterViewModel"/> for cloning.
    /// </summary>
    /// <returns>A new boolean filter instance with the same AllowNullValue setting.</returns>
    protected override FilterViewModel CreateCloneInstance() => new BooleanFilterViewModel(PropertyName) { AllowNullValue = AllowNullValue };
}
