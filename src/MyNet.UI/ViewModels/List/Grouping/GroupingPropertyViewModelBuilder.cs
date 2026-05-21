// -----------------------------------------------------------------------
// <copyright file="GroupingPropertyViewModelBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Observable;
using MyNet.Observable.Translatables;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Fluent builder used to configure a grouping property view model.
/// </summary>
/// <typeparam name="T">The type of the items to group.</typeparam>
public sealed class GroupingPropertyViewModelBuilder<T>(Expression<Func<T, object?>> expression)
{
    private string? _key;
    private IProvideValue<string>? _displayName;
    private GroupingDefaultState? _defaultState;
    private int _order;

    /// <summary>
    /// Sets the unique key of the grouping property.
    /// </summary>
    public GroupingPropertyViewModelBuilder<T> WithKey(string key)
    {
        _key = key;
        return this;
    }

    /// <summary>
    /// Sets the display name provider used by the UI.
    /// </summary>
    public GroupingPropertyViewModelBuilder<T> WithDisplayName(IProvideValue<string> displayName)
    {
        _displayName = displayName;
        return this;
    }

    /// <summary>
    /// Configures this grouping property to be active by default when the grouping view model is created. The optional order parameter can be used to specify the order in which this property should be activated relative to other properties that are also active by default. If no order is specified, the builder will assign an incremental order based on the sequence of calls to this method, ensuring that properties are activated in the order they were configured. This allows for a predictable and consistent default grouping configuration when the view model is instantiated.
    /// </summary>
    /// <param name="order">The order in which this grouping property should be activated relative to other properties.</param>
    public GroupingPropertyViewModelBuilder<T> ByDefault(int? order = null)
    {
        _defaultState = new(order ?? _order++);
        return this;
    }

    /// <summary>
    /// Builds the grouping property configuration based on the current state of the builder. This method validates that a key has been provided and constructs a <see cref="GroupingPropertyConfiguration{T}"/> instance that encapsulates all the configured properties, including the key, expression, display name, and default grouping direction. The resulting configuration can then be used to create instances of <see cref="GroupingPropertyViewModel{T}"/> or to define grouping behavior in the UI.
    /// </summary>
    /// <returns>The constructed <see cref="GroupingPropertyConfiguration{T}"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a grouping property key has not been provided.</exception>
    internal GroupingPropertyDefinition<T> Build()
    {
        var key = ResolveKey();
        var displayName = _displayName ?? new Translatable<string>(() => key);

        return new(key, expression, displayName, _defaultState);
    }

    /// <summary>
    /// Resolves the key for the grouping property. If a key has been explicitly set using the builder, it returns that key. Otherwise, it attempts to infer the key from the provided expression using the GetKey extension method. If the inferred key is null or whitespace, it throws an InvalidOperationException, indicating that a grouping property key must be provided. This method ensures that every grouping property has a valid and unique key that can be used to identify it within the collection of grouping properties.
    /// </summary>
    /// <returns>The resolved key for the grouping property.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a grouping property key cannot be resolved.</exception>
    private string ResolveKey()
    {
        if (!string.IsNullOrWhiteSpace(_key))
            return _key;

        var inferredKey = expression.GetKey();

        return string.IsNullOrWhiteSpace(inferredKey) ? throw new InvalidOperationException("A grouping property key must be provided.") : inferredKey;
    }
}

/// <summary>
/// Represents the configuration for a grouping property, including its key, expression, display name, and default grouping direction. This configuration is used to create instances of <see cref="GroupingPropertyViewModel{T}"/> based on the defined properties. The configuration encapsulates all necessary information to define how a grouping property should behave and be displayed in the UI, allowing for a clear separation between the configuration phase and the instantiation of view models.
/// </summary>
/// <param name="Key">The unique key of the grouping property.</param>
/// <param name="Expression">The expression used to extract the value for grouping.</param>
/// <param name="DisplayName">The display name provider for the UI.</param>
/// <param name="DefaultState">The default configuration for the grouping property, if any.</param>
/// <typeparam name="T">The type of the items being grouped.</typeparam>
internal sealed record GroupingPropertyDefinition<T>(string Key, Expression<Func<T, object?>> Expression, IProvideValue<string> DisplayName, GroupingDefaultState? DefaultState);

/// <summary>
/// Represents the active configuration for a grouping property, including its default order in which it was activated. This record is used to track the state of active grouping properties, allowing the system to determine how to apply grouping based on the defined configuration and the order of activation. The DefaultDirection property indicates the initial grouping direction for the property when it becomes active, while the Order property can be used to determine the sequence in which multiple active grouping properties should be applied to a collection.
/// </summary>
/// <param name="Order">The order in which the grouping property was activated.</param>
internal sealed record GroupingDefaultState(int Order);
