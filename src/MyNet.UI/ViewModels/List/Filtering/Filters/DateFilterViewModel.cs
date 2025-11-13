// -----------------------------------------------------------------------
// <copyright file="DateFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for DateTime properties.
/// Supports date range filtering (from/to) and various comparison operators.
/// </summary>
/// <remarks>
/// <para><strong>Usage Examples:</strong></para>
/// <code>
/// // Events between two dates
/// var dateRangeFilter = new DateFilterViewModel("EventDate")
/// {
///     Operator = ComplexComparableOperator.Between,
///     From = new DateTime(2024, 1, 1),
///     To = new DateTime(2024, 12, 31)
/// };
/// // Created after a specific date
/// var afterFilter = new DateFilterViewModel("CreatedDate")
/// {
///     Operator = ComplexComparableOperator.GreaterThan,
///     From = new DateTime(2024, 6, 1)
/// };
/// // Due before today
/// var beforeFilter = new DateFilterViewModel("DueDate")
/// {
///     Operator = ComplexComparableOperator.LessThan,
///   To = DateTime.Today
/// };
/// </code>
/// </remarks>
public class DateFilterViewModel : ComparableFilterViewModel<DateTime>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateFilterViewModel"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the DateTime property to filter on.</param>
    public DateFilterViewModel(string propertyName)
        : base(propertyName) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateFilterViewModel"/> class
    /// with a specific comparison operator and date range.
    /// </summary>
    /// <param name="propertyName">The name of the DateTime property to filter on.</param>
 /// <param name="comparison">The comparison operator (e.g., Between, GreaterThan, LessThan).</param>
    /// <param name="from">The start date (inclusive). Can be null for no lower bound.</param>
    /// <param name="to">The end date (inclusive). Can be null for no upper bound.</param>
    public DateFilterViewModel(string propertyName, ComplexComparableOperator comparison, DateTime? from, DateTime? to)
        : base(propertyName, comparison, from, to) { }

    /// <summary>
    /// Creates a new instance of <see cref="DateFilterViewModel"/> for cloning.
    /// </summary>
    /// <returns>A new date filter instance with the same configuration.</returns>
    protected override FilterViewModel CreateCloneInstance()
      => new DateFilterViewModel(PropertyName, Operator, From, To)
        {
            Minimum = Minimum,
            Maximum = Maximum
        };
}
