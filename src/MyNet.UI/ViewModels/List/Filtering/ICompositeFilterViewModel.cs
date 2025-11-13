// -----------------------------------------------------------------------
// <copyright file="ICompositeFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.Utilities;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents a composite filter that wraps an inner filter with additional UI state (enabled/disabled, logical operator).
/// Allows building complex filter hierarchies with AND/OR logic.
/// </summary>
public interface ICompositeFilterViewModel : IWrapper<IFilterViewModel>, INotifyPropertyChanged, ICloneable
{
    /// <summary>
    /// Gets or sets a value indicating whether this composite filter is currently enabled.
    /// When false, the wrapped filter is not applied even if it has criteria.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the logical operator used to combine this filter with others.
    /// </summary>
    LogicalOperator Operator { get; set; }

    /// <summary>
    /// Resets the composite filter and its wrapped filter to their default states.
    /// </summary>
    void Reset();
}
