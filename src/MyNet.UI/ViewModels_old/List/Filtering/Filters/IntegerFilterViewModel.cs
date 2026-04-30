// -----------------------------------------------------------------------
// <copyright file="IntegerFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Comparison;
using MyNet.Utilities.Sequences;

namespace MyNet.UI.ViewModels.List.Filtering.Filters;

/// <summary>
/// Filter view model for integer properties.
/// Supports range filtering (from/to) and various comparison operators (less than, greater than, between, etc.).
/// </summary>
/// <remarks>
/// <para><strong>Usage Examples:</strong></para>
/// <code>
/// // Age between 18 and 65
/// var ageFilter = new IntegerFilterViewModel("Age")
/// {
///     Operator = ComplexComparableOperator.Between,
///     From = 18,
///     To = 65
/// };
/// // Quantity greater than 10
/// var qtyFilter = new IntegerFilterViewModel("Quantity")
/// {
///     Operator = ComplexComparableOperator.GreaterThan,
///     From = 10
/// };
/// // With acceptable range constraints
/// var rangeFilter = new IntegerFilterViewModel("Score",
///     ComplexComparableOperator.Between,
///     new AcceptableValueRange&lt;int&gt;(0, 100));
/// </code>
/// </remarks>
public class IntegerFilterViewModel : ComparableFilterViewModel<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerFilterViewModel"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the integer property to filter on.</param>
    public IntegerFilterViewModel(string propertyName)
        : base(propertyName) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerFilterViewModel"/> class
    /// with a specific comparison operator and range.
    /// </summary>
    /// <param name="propertyName">The name of the integer property to filter on.</param>
    /// <param name="comparison">The comparison operator (e.g., Between, GreaterThan, LessThan).</param>
    /// <param name="from">The minimum value (inclusive). Can be null for no lower bound.</param>
    /// <param name="to">The maximum value (inclusive). Can be null for no upper bound.</param>
    public IntegerFilterViewModel(string propertyName, ComplexComparableOperator comparison, int? from, int? to)
        : base(propertyName, comparison, from, to) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerFilterViewModel"/> class
    /// with an acceptable value range that defines the minimum and maximum allowed values.
    /// </summary>
    /// <param name="propertyName">The name of the integer property to filter on.</param>
    /// <param name="comparison">The comparison operator.</param>
    /// <param name="range">The acceptable range that defines constraints for From and To values.</param>
    public IntegerFilterViewModel(string propertyName, ComplexComparableOperator comparison, AcceptableValueRange<int> range)
        : base(propertyName, comparison, range.Min, range.Max)
        => (Minimum, Maximum) = (range.Min, range.Max);

    /// <summary>
    /// Creates a new instance of <see cref="IntegerFilterViewModel"/> for cloning.
    /// </summary>
    /// <returns>A new integer filter instance with the same configuration.</returns>
    protected override FilterViewModel CreateCloneInstance()
     => new IntegerFilterViewModel(PropertyName, Operator, From, To)
     {
         Maximum = Maximum,
         Minimum = Minimum
     };
}
