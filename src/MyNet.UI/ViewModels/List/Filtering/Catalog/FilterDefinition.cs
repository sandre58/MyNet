// -----------------------------------------------------------------------
// <copyright file="FilterDefinition.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents a concrete implementation of <see cref="IFilterDefinition{T}"/> that defines how to create a filter condition view model for a specific filter type. It includes a unique key, a display name provider, and a factory function to create instances of the filter condition view model. This class is used to register available filters in the filter registry and to create filter conditions in the UI based on user selection.
/// </summary>
/// <param name="key">The unique key identifying the filter.</param>
/// <param name="displayName">The provider for the display name of the filter.</param>
/// <param name="factory">The factory function to create instances of the filter condition view model.</param>
/// <typeparam name="T">The type of items to be filtered.</typeparam>
/// <typeparam name="TFilter">The type of the filter condition view model.</typeparam>
public sealed class FilterDefinition<T, TFilter>(string key, IProvideValue<string> displayName, Func<TFilter> factory)
    : IFilterDefinition<T>
    where TFilter : IFilterConditionViewModel<T>
{
    /// <summary>
    /// Gets the unique key that identifies this filter definition. This key is used to distinguish different filter definitions and should be unique across all available filters for type T.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Gets the provider for the display name of this filter definition. This provider is used to obtain the display name that can be shown in the UI.
    /// </summary>
    public IProvideValue<string> DisplayName { get; } = displayName;

    /// <summary>
    /// Gets the type of the filter condition view model. This property returns the type of the filter condition view model associated with this filter definition.
    /// </summary>
    public Type FilterType => typeof(TFilter);

    /// <summary>
    /// Creates an instance of the filter condition view model using the factory function provided. This method allows clients to create new instances of the filter condition view model based on the current filter definition.
    /// </summary>
    /// <returns>A new instance of the filter condition view model.</returns>
    public IFilterConditionViewModel<T> Create() => factory();
}
