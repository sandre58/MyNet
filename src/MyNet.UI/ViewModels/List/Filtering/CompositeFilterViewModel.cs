// -----------------------------------------------------------------------
// <copyright file="CompositeFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable;
using MyNet.Observable.Translatables;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents a composite filter that wraps an inner filter with additional UI state.
/// Provides enable/disable functionality and logical operator configuration for combining with other filters.
/// </summary>
/// <param name="displayName">The provider for the localized display name.</param>
/// <param name="filter">The inner filter to wrap.</param>
/// <param name="logicalOperator">The logical operator for combining with other filters. Default is AND.</param>
public class CompositeFilterViewModel(
    IProvideValue<string> displayName,
    IFilterViewModel filter,
    LogicalOperator logicalOperator = LogicalOperator.And)
    : DisplayWrapper<IFilterViewModel>(filter, displayName), ICompositeFilterViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether this composite filter is currently enabled.
    /// When false, the wrapped filter is not evaluated even if it has active criteria.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the logical operator used to combine this filter with others in a collection.
    /// </summary>
    public LogicalOperator Operator { get; set; } = logicalOperator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeFilterViewModel"/> class
    /// using the filter's property name as the display name.
    /// </summary>
    /// <param name="filter">The inner filter to wrap.</param>
    /// <param name="logicalOperator">The logical operator for combining with other filters. Default is AND.</param>
    public CompositeFilterViewModel(IFilterViewModel filter, LogicalOperator logicalOperator = LogicalOperator.And)
        : this(filter.PropertyName, filter, logicalOperator) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeFilterViewModel"/> class
    /// with a translatable resource key for the display name.
    /// </summary>
    /// <param name="resourceKey">The resource key for localization of the display name.</param>
    /// <param name="filter">The inner filter to wrap.</param>
    /// <param name="logicalOperator">The logical operator for combining with other filters. Default is AND.</param>
    public CompositeFilterViewModel(string resourceKey, IFilterViewModel filter, LogicalOperator logicalOperator = LogicalOperator.And)
   : this(new StringTranslatable(resourceKey), filter, logicalOperator) { }

    /// <summary>
    /// Resets the composite filter and its wrapped inner filter to their default states.
    /// </summary>
    public void Reset() => Item.Reset();

    /// <summary>
    /// Creates a clone instance of this composite filter.
    /// </summary>
    /// <param name="item">The inner filter item for the clone.</param>
    /// <returns>A new composite filter view model with the same configuration.</returns>
    protected override Wrapper<IFilterViewModel> CreateCloneInstance(IFilterViewModel item)
        => new CompositeFilterViewModel(DisplayName, Item, Operator) { IsEnabled = IsEnabled };
}
