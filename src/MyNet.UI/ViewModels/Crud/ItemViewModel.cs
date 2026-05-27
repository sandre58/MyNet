// -----------------------------------------------------------------------
// <copyright file="ItemViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using MyNet.UI.Commands;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Provides a reusable base implementation for item view models.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public abstract class ItemViewModel<T>(IBusyService? busyService = null, ICommandFactory? commandFactory = null) : WorkspaceViewModel(busyService, commandFactory), IItemViewModel<T>
{
    /// <summary>
    /// Gets or sets the current item.
    /// </summary>
    public T? Item { get; protected set => SetProperty(ref field, value); }

    /// <summary>
    /// Event raised when the current item changes, providing the old and new values of the item.
    /// </summary>
    public event EventHandler<ItemChangedEventArgs<T>>? ItemChanged;

    /// <summary>
    /// Sets the current item and raises <see cref="ItemChanged"/> when the value changes.
    /// </summary>
    /// <param name="item">The item to expose.</param>
    public virtual void SetItem(T? item)
    {
        if (EqualityComparer<T?>.Default.Equals(Item, item))
            return;

        var previous = Item;
        Item = item;
        OnItemChanged(previous, item);
    }

    /// <summary>
    /// Called when the current item changes.
    /// </summary>
    protected virtual void HandleItemChanged()
    {
    }

    /// <summary>
    /// Handles Fody property change notifications for <see cref="Item"/>.
    /// </summary>
    /// <param name="oldValue">The previous item value.</param>
    /// <param name="newValue">The new item value.</param>
    protected virtual void OnItemChanged(T? oldValue, T? newValue)
    {
        UnsubscribeFromItem(oldValue);
        SubscribeToItem(newValue);
        HandleItemChanged();

        ItemChanged?.Invoke(this, new(oldValue, newValue));
    }

    private void SubscribeToItem(T? item)
    {
        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged += HandleItemPropertyChanged;
    }

    private void UnsubscribeFromItem(T? item)
    {
        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged -= HandleItemPropertyChanged;
    }

    private void HandleItemPropertyChanged(object? sender, PropertyChangedEventArgs e) => HandleCurrentItemPropertyChanged(e);

    /// <summary>
    /// Called when a property of the current item changes.
    /// </summary>
    /// <param name="e">The property change arguments.</param>
    protected virtual void HandleCurrentItemPropertyChanged(PropertyChangedEventArgs e)
    {
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        UnsubscribeFromItem(Item);
        base.DisposeManagedResources();
    }
}
