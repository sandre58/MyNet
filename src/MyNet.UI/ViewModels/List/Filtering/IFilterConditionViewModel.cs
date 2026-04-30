// -----------------------------------------------------------------------
// <copyright file="IFilterConditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.List.Filtering;

/// <summary>
/// Defines a view model for a filter condition that can be applied to items of type T. It includes a key to identify the condition, a method to get the predicate function, and a method to reset the condition.
/// </summary>
/// <typeparam name="T">The type of the items being filtered.</typeparam>
public interface IFilterConditionViewModel<T> : IFilterNodeViewModel<T>
{
    /// <summary>
    /// Gets the key that identifies this filter condition.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets a value indicating whether this filter condition is empty.
    /// </summary>
    bool IsEmpty { get; }
}
