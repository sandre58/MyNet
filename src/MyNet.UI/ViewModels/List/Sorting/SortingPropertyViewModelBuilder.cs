// -----------------------------------------------------------------------
// <copyright file="SortingPropertyViewModelBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using MyNet.Observable;
using MyNet.Observable.Translatables;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.List.Sorting;

/// <summary>
/// Fluent builder used to configure a sorting property view model.
/// </summary>
/// <typeparam name="T">The type of the items to sort.</typeparam>
public sealed class SortingPropertyViewModelBuilder<T>(Expression<Func<T, object?>> expression)
{
    private string? _key;
    private IProvideValue<string>? _displayName;
    private SortingDefaultState? _defaultState;
    private int _order;

    /// <summary>
    /// Sets the unique key of the sorting property.
    /// </summary>
    public SortingPropertyViewModelBuilder<T> WithKey(string key)
    {
        _key = key;
        return this;
    }

    /// <summary>
    /// Sets the display name provider used by the UI.
    /// </summary>
    public SortingPropertyViewModelBuilder<T> WithDisplayName(IProvideValue<string> displayName)
    {
        _displayName = displayName;
        return this;
    }

    /// <summary>
    /// Sets the display name using a resource key.
    /// </summary>
    public SortingPropertyViewModelBuilder<T> WithDisplayName(string resourceKey) => WithDisplayName(new StringTranslatable(resourceKey));

    /// <summary>
    /// Sets the default sorting direction to ascending. This means that when this sorting property is applied, it will sort in ascending order by default unless explicitly changed by the user. This method is a convenience method for setting the default direction without having to specify the enum value directly.
    /// </summary>
    /// <param name="order">The order in which this sorting property should be applied relative to other sorting properties.</param>
    /// <returns>The current instance of <see cref="SortingPropertyViewModelBuilder{T}"/> for fluent configuration.</returns>
    public SortingPropertyViewModelBuilder<T> Ascending(int? order = null)
    {
        _defaultState = new(ListSortDirection.Ascending, order ?? _order++);
        return this;
    }

    /// <summary>
    /// Sets the default sorting direction to descending. This means that when this sorting property is applied, it will sort in descending order by default unless explicitly changed by the user. This method is a convenience method for setting the default direction without having to specify the enum value directly.
    /// </summary>
    /// <param name="order">The order in which this sorting property should be applied relative to other sorting properties.</param>
    /// <returns>The current instance of <see cref="SortingPropertyViewModelBuilder{T}"/> for fluent configuration.</returns>
    public SortingPropertyViewModelBuilder<T> Descending(int? order = null)
    {
        _defaultState = new(ListSortDirection.Descending, order ?? _order++);
        return this;
    }

    /// <summary>
    /// Builds the sorting property configuration based on the current state of the builder. This method validates that a key has been provided and constructs a <see cref="SortingPropertyConfiguration{T}"/> instance that encapsulates all the configured properties, including the key, expression, display name, and default sorting direction. The resulting configuration can then be used to create instances of <see cref="SortingPropertyViewModel{T}"/> or to define sorting behavior in the UI.
    /// </summary>
    /// <returns>The constructed <see cref="SortingPropertyConfiguration{T}"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a sorting property key has not been provided.</exception>
    internal SortingPropertyDefinition<T> Build()
    {
        var key = ResolveKey();
        var displayName = _displayName ?? new Translatable<string>(() => key);

        return new(key, expression, displayName, _defaultState);
    }

    /// <summary>
    /// Resolves the key for the sorting property. If a key has been explicitly set using the builder, it returns that key. Otherwise, it attempts to infer the key from the provided expression using the GetKey extension method. If the inferred key is null or whitespace, it throws an InvalidOperationException, indicating that a sorting property key must be provided. This method ensures that every sorting property has a valid and unique key that can be used to identify it within the collection of sorting properties.
    /// </summary>
    /// <returns>The resolved key for the sorting property.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a sorting property key cannot be resolved.</exception>
    private string ResolveKey()
    {
        if (!string.IsNullOrWhiteSpace(_key))
            return _key;

        var inferredKey = expression.GetKey();

        return string.IsNullOrWhiteSpace(inferredKey) ? throw new InvalidOperationException("A sorting property key must be provided.") : inferredKey;
    }
}

/// <summary>
/// Represents the configuration for a sorting property, including its key, expression, display name, and default sorting direction. This configuration is used to create instances of <see cref="SortingPropertyViewModel{T}"/> based on the defined properties. The configuration encapsulates all necessary information to define how a sorting property should behave and be displayed in the UI, allowing for a clear separation between the configuration phase and the instantiation of view models.
/// </summary>
/// <param name="Key">The unique key of the sorting property.</param>
/// <param name="Expression">The expression used to extract the value for sorting.</param>
/// <param name="DisplayName">The display name provider for the UI.</param>
/// <param name="DefaultState">The default configuration for the sorting property, if any.</param>
/// <typeparam name="T">The type of the items being sorted.</typeparam>
internal sealed record SortingPropertyDefinition<T>(string Key, Expression<Func<T, object?>> Expression, IProvideValue<string> DisplayName, SortingDefaultState? DefaultState);

/// <summary>
/// Represents the active configuration for a sorting property, including its default sorting direction and the order in which it was activated. This record is used to track the state of active sorting properties, allowing the system to determine how to apply sorting based on the defined configuration and the order of activation. The DefaultDirection property indicates the initial sorting direction for the property when it becomes active, while the Order property can be used to determine the sequence in which multiple active sorting properties should be applied to a collection.
/// </summary>
/// <param name="Direction">The default sorting direction for the active sorting property.</param>
/// <param name="Order">The order in which the sorting property was activated.</param>
internal sealed record SortingDefaultState(ListSortDirection Direction, int Order);
