// -----------------------------------------------------------------------
// <copyright file="IFilterDefinition.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Defines a filter definition for type T, which describes how to create a filter condition view model and build an expression for filtering items of type T. Each filter definition has a unique key and a display name for use in the UI.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
public interface IFilterDefinition<T>
{
    /// <summary>
    /// Gets the unique key that identifies this filter definition. This key is used to distinguish different filter definitions and should be unique across all available filters for type T.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the display name for this filter definition, which is used in the UI to represent this filter option to the user. The display name can be a static string or a dynamic value that provides more context about the filter condition.
    /// </summary>
    IProvideValue<string> DisplayName { get; }

    /// <summary>
    /// Gets the type of the filter (for UI templating).
    /// </summary>
    Type FilterType { get; }

    /// <summary>
    /// Creates a new instance of the filter condition view model for this filter definition.
    /// </summary>
    /// <returns>A new instance of <see cref="IFilterConditionViewModel{T}"/>.</returns>
    IFilterConditionViewModel<T> Create();
}
