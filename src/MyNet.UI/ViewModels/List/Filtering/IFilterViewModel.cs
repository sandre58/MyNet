// -----------------------------------------------------------------------
// <copyright file="IFilterViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.Observable.Collections.Filters;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents a single filter that can be applied to a collection.
/// Extends <see cref="IFilter"/> with UI-specific functionality like reset and empty state checking.
/// </summary>
public interface IFilterViewModel : IFilter, INotifyPropertyChanged, ICloneable, ISettable, ISimilar<IFilterViewModel>
{
    /// <summary>
    /// Resets the filter to its default state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Determines whether this filter is in an empty state (no criteria specified).
    /// </summary>
    /// <returns>True if the filter has no active criteria; otherwise, false.</returns>
    bool IsEmpty();
}
