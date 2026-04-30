// -----------------------------------------------------------------------
// <copyright file="IItemEditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// View model for an item edition. It allows to track the original item, the current item and whether the item has been modified (IsDirty).
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
public interface IItemEditionViewModel<T> : IItemViewModel<T>
{
    /// <summary>
    /// Gets the original item before any modifications.
    /// </summary>
    T? OriginalItem { get; }

    /// <summary>
    /// Gets a value indicating whether the item has been modified.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Sets the original item.
    /// </summary>
    /// <param name="item">The original item.</param>
    void SetOriginalItem(T? item);

    /// <summary>
    /// Resets the item to its original state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Applies the changes to the item.
    /// </summary>
    void Apply();
}
