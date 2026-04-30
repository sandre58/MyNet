// -----------------------------------------------------------------------
// <copyright file="ItemEditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Provides a reusable base implementation for editable item view models.
/// </summary>
/// <typeparam name="T">The edited item type.</typeparam>
public abstract class ItemEditionViewModel<T> : ItemViewModel<T>, IItemEditionViewModel<T>
{
    /// <summary>
    /// Gets the original item snapshot.
    /// </summary>
    public T? OriginalItem { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the edited item differs from the original snapshot.
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Sets the original item and initializes the current editable item from it.
    /// </summary>
    /// <param name="item">The original item.</param>
    public virtual void SetOriginalItem(T? item)
    {
        OriginalItem = CloneItem(item);
        SetItem(CloneItem(item));
        UpdateIsDirty();
    }

    /// <summary>
    /// Restores the current item from the original snapshot.
    /// </summary>
    public virtual void Reset()
    {
        SetItem(CloneItem(OriginalItem));
        UpdateIsDirty();
    }

    /// <summary>
    /// Applies the current item as the new original snapshot.
    /// </summary>
    public virtual void Apply()
    {
        OriginalItem = CloneItem(Item);
        UpdateIsDirty();
    }

    /// <summary>
    /// Creates an editable copy of the specified item.
    /// </summary>
    /// <param name="item">The item to clone.</param>
    /// <returns>A copy suitable for edition, or the original instance when cloning is not available.</returns>
    protected virtual T? CloneItem(T? item)
        => item switch
        {
            null => default,
            ICloneable cloneable => (T?)cloneable.Clone(),
            _ => item
        };

    /// <inheritdoc />
    protected override void HandleItemChanged()
    {
        base.HandleItemChanged();
        UpdateIsDirty();
    }

    /// <inheritdoc />
    protected override void HandleCurrentItemPropertyChanged(PropertyChangedEventArgs e)
    {
        base.HandleCurrentItemPropertyChanged(e);
        UpdateIsDirty();
    }

    /// <summary>
    /// Recomputes the dirty state.
    /// </summary>
    protected void UpdateIsDirty() => IsDirty = !Equals(Item, OriginalItem);
}
