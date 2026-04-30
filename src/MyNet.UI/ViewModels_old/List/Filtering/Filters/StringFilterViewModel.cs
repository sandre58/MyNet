// -----------------------------------------------------------------------
// <copyright file="StringFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for string properties.
/// Supports multiple comparison operators (Contains, StartsWith, EndsWith, Is, IsNot) with case-sensitive options.
/// </summary>
/// <param name="propertyName">The name of the string property to filter on.</param>
/// <param name="filterMode">The string comparison operator. Default is Contains.</param>
/// <param name="caseSensitive">Whether the comparison should be case-sensitive. Default is false.</param>
/// <remarks>
/// <para><strong>Usage Examples:</strong></para>
/// <code>
/// // Simple contains filter (case-insensitive)
/// var filter = new StringFilterViewModel("Name")
/// {
///     Value = "John"
/// };
/// // Exact match (case-sensitive)
/// var exactFilter = new StringFilterViewModel("Email", StringOperator.Is, caseSensitive: true)
/// {
///     Value = "user@example.com"
/// };
/// // Starts with filter
/// var startsWithFilter = new StringFilterViewModel("LastName", StringOperator.StartsWith)
/// {
///     Value = "Smi"
/// };
/// </code>
/// </remarks>
public class StringFilterViewModel(
    string propertyName,
    StringOperator filterMode = StringOperator.Contains,
    bool caseSensitive = false)
    : FilterViewModel(propertyName)
{
    /// <summary>
    /// Gets or sets the string comparison operator used for matching.
    /// </summary>
    public StringOperator Operator { get; set; } = filterMode;

    /// <summary>
    /// Gets or sets the search value to match against.
    /// When null or empty, the filter matches all items (empty filter).
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the string comparison should be case-sensitive.
    /// When false (default), comparisons are case-insensitive.
    /// </summary>
    public bool CaseSensitive { get; set; } = caseSensitive;

    /// <summary>
    /// Determines whether the specified string value matches the filter criteria.
    /// </summary>
    /// <param name="toCompare">The string value to compare.</param>
    /// <returns>True if the value matches according to the <see cref="Operator"/> and <see cref="CaseSensitive"/> settings; otherwise, false.</returns>
    /// <remarks>
    /// Supports the following operators:
    /// <list type="bullet">
    /// <item><see cref="StringOperator.Contains"/>: Value appears anywhere in the string</item>
    /// <item><see cref="StringOperator.StartsWith"/>: Value matches the beginning of the string</item>
    /// <item><see cref="StringOperator.EndsWith"/>: Value matches the end of the string</item>
    /// <item><see cref="StringOperator.Is"/>: Exact match</item>
    /// <item><see cref="StringOperator.IsNot"/>: Does not exactly match</item>
    /// </list>
    /// </remarks>
    protected override bool IsMatchProperty(object? toCompare)
    {
        // Handle null target value
        if (toCompare is null)
            return Operator == StringOperator.Is && Value is null;

        var toStringCompare = toCompare.ToString() ?? string.Empty;

        // Empty filter matches nothing
        if (Value is null)
            return false;

        var value = Value;

        // Case-sensitive comparison
        if (CaseSensitive)
        {
            return Operator switch
            {
                StringOperator.Contains => toStringCompare.Contains(value, StringComparison.OrdinalIgnoreCase),

                StringOperator.StartsWith => toStringCompare.StartsWith(value, StringComparison.InvariantCultureIgnoreCase),

                StringOperator.EndsWith => toStringCompare.EndsWith(value, StringComparison.InvariantCultureIgnoreCase),

                StringOperator.Is => toStringCompare.Equals(value, StringComparison.OrdinalIgnoreCase),

                StringOperator.IsNot => !toStringCompare.Equals(value, StringComparison.OrdinalIgnoreCase),

                _ => throw new NotImplementedException($"Operator {Operator} is not implemented")
            };
        }

        // Case-insensitive comparison (default)
        value = Value.ToUpperInvariant();
        toStringCompare = toStringCompare.ToUpperInvariant();

        return Operator switch
        {
            StringOperator.Contains => toStringCompare.Contains(value, StringComparison.OrdinalIgnoreCase),

            StringOperator.StartsWith => toStringCompare.StartsWith(value, StringComparison.InvariantCultureIgnoreCase),

            StringOperator.EndsWith => toStringCompare.EndsWith(value, StringComparison.InvariantCultureIgnoreCase),

            StringOperator.Is => toStringCompare.Equals(value, StringComparison.OrdinalIgnoreCase),

            StringOperator.IsNot => !toStringCompare.Equals(value, StringComparison.OrdinalIgnoreCase),

            _ => throw new NotImplementedException($"Operator {Operator} is not implemented")
        };
    }

    /// <summary>
    /// Determines whether this filter is in an empty state.
    /// A string filter is empty when <see cref="Value"/> is null or empty.
    /// </summary>
    /// <returns>True if Value is null or empty; otherwise, false.</returns>
    public override bool IsEmpty() => string.IsNullOrEmpty(Value);

    /// <summary>
    /// Resets the filter to its default state by clearing the value.
    /// </summary>
    public override void Reset() => Value = string.Empty;

    /// <summary>
    /// Determines whether the specified object is equal to the current filter.
    /// Two string filters are equal if they have the same property name, operator, and case sensitivity.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
        => obj is StringFilterViewModel o
        && base.Equals(obj)
        && Operator == o.Operator
        && CaseSensitive == o.CaseSensitive;

    /// <summary>
    /// Returns a hash code based on the current value.
    /// </summary>
    /// <returns>A hash code for this filter.</returns>
    public override int GetHashCode() => Value.OrEmpty().GetHashCode(StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Sets the filter criteria from another string filter instance.
    /// </summary>
    /// <param name="from">The filter to copy criteria from. Must be a <see cref="StringFilterViewModel"/>.</param>
    public override void SetFrom(object? from)
    {
        if (from is not StringFilterViewModel other)
            return;

        Operator = other.Operator;
        CaseSensitive = other.CaseSensitive;
        Value = other.Value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="StringFilterViewModel"/> for cloning.
    /// </summary>
    /// <returns>A new string filter instance with the same operator and case sensitivity settings.</returns>
    protected override FilterViewModel CreateCloneInstance()
        => new StringFilterViewModel(PropertyName, Operator, CaseSensitive);
}
