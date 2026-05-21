// -----------------------------------------------------------------------
// <copyright file="FilterConditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Observable;

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Represents a view model for a filter condition that can be applied to items of type T. It includes a key to identify the condition, a method to get the predicate function, and a method to reset the condition.
/// </summary>
/// <param name="key">The key that identifies this filter condition.</param>
/// <param name="isReadOnly">A value indicating whether this filter condition is read-only.</param>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
public abstract class FilterConditionViewModel<T>(string key, bool isReadOnly = false) : EditableObject, IFilterConditionViewModel<T>
{
    /// <summary>
    /// Gets the key that identifies this filter condition.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Gets a value indicating whether this filter condition is read-only.
    /// </summary>
    public bool IsReadOnly { get; } = isReadOnly;

    /// <summary>
    /// Gets or sets a value indicating whether this filter condition is enabled. When disabled, the condition is ignored in the filtering logic.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets a value indicating whether this filter condition is empty, meaning it has no criteria defined and should not affect the filtering results. An empty condition is typically ignored in the filtering logic.
    /// </summary>
    public abstract bool IsEmpty { get; }

    /// <summary>
    /// Builds the expression representing this filter condition. If the condition is empty, it returns an expression that always evaluates to true, effectively not filtering out any items. Otherwise, it calls the BuildExpressionCore method to get the specific filtering logic defined by the concrete implementation.
    /// </summary>
    /// <returns>An expression representing the filter condition.</returns>
    public Expression<Func<T, bool>>? BuildExpression() => IsEmpty || !IsEnabled ? null : BuildExpressionCore();

    /// <summary>
    /// When overridden in a derived class, builds the specific expression representing this filter condition based on its criteria. This method is called by the BuildExpression method when the condition is not empty. The implementation should return an expression that evaluates to true for items that match the filter criteria and false for those that do not.
    /// </summary>
    /// <returns>An expression representing the specific filter condition.</returns>
    protected abstract Expression<Func<T, bool>> BuildExpressionCore();

    /// <summary>
    /// Resets the filter condition to its default state. This method should be implemented by derived classes to clear any criteria or values associated with the filter condition, effectively making it empty and not affecting the filtering results until new criteria are defined.
    /// </summary>
    public abstract void Reset();
}
