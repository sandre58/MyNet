// -----------------------------------------------------------------------
// <copyright file="ChildItemViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Represents a view model that is a child of another IItemViewModel, allowing it to automatically update its item based on the parent view model's item changes.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
public abstract class ChildItemViewModel<T> :
    WorkspaceViewModel,
    IItemViewModel<T>
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
    private IDisposable? _parentSubscription;

    /// <summary>
    /// Gets the item represented by this view model, which is automatically updated based on the parent IItemViewModel's item changes.
    /// </summary>
    public T? Item { get; private set; }

    /// <summary>
    /// Event raised when the current item changes, providing the old and new values of the item.
    /// </summary>
    public event EventHandler<ItemChangedEventArgs<T>>? ItemChanged;

    /// <summary>
    /// Attaches this child view model to a parent IItemViewModel, allowing it to automatically update its item whenever the parent's item changes.
    /// </summary>
    /// <param name="parent">The parent IItemViewModel to attach to.</param>
    /// <exception cref="ArgumentNullException">Thrown if the parent is null.</exception>
    public virtual void Attach(IItemViewModel<T> parent)
    {
        ArgumentNullException.ThrowIfNull(parent);

        Detach();

        SetItem(parent.Item);

        _parentSubscription = parent.ObserveItemChanged(OnParentItemChanged);
    }

    /// <summary>
    /// Detaches this child view model from its parent IItemViewModel, stopping it from automatically updating its item based on the parent's item changes.
    /// </summary>
    public virtual void Detach()
    {
        _parentSubscription?.Dispose();
        _parentSubscription = null;
    }

    /// <summary>
    /// Called when the parent IItemViewModel's item changes, allowing this child view model to update its own item accordingly.
    /// </summary>
    /// <param name="item">The new item value from the parent view model.</param>
    protected virtual void OnParentItemChanged(T? item) => SetItem(item);

    /// <summary>
    /// Sets the current item for this child view model, raising the ItemChanged event if the item has changed and allowing derived classes to react to the change.
    /// </summary>
    /// <param name="item">The new item value to set.</param>
    protected virtual void SetItem(T? item)
    {
        if (EqualityComparer<T?>.Default.Equals(Item, item))
            return;

        var previous = Item;

        Item = item;

        OnItemChanged(previous, item);
    }

    /// <summary>
    /// Handles Fody property change notifications for <see cref="Item"/>.
    /// </summary>
    /// <param name="oldValue">The previous item value.</param>
    /// <param name="newValue">The new item value.</param>
    protected virtual void OnItemChanged(T? oldValue, T? newValue)
    {
        HandleItemChanged();

        ItemChanged?.Invoke(this, new(oldValue, newValue));
    }

    /// <summary>
    /// Called when the current item changes.
    /// </summary>
    protected virtual void HandleItemChanged()
    {
    }

    /// <summary>
    /// Cleans up resources used by this view model, including detaching from the parent IItemViewModel to stop receiving item change notifications. This method is called when the view model is being disposed.
    /// </summary>
    protected override void Cleanup()
    {
        Detach();
        base.Cleanup();
    }
}
