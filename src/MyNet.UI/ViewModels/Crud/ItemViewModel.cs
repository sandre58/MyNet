// -----------------------------------------------------------------------
// <copyright file="ItemViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Provides a reusable base implementation for item view models.
/// </summary>
/// <typeparam name="T">The wrapped item type.</typeparam>
public abstract class ItemViewModel<T> : ViewModelBase, IItemViewModel<T>
{
    /// <summary>
    /// Gets or sets the current item.
    /// </summary>
    public T? Item { get; protected set; }

    /// <summary>
    /// Sets the current item.
    /// </summary>
    /// <param name="item">The item to expose.</param>
    public virtual void SetItem(T? item) => Item = item;

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
    protected override void Cleanup()
    {
        UnsubscribeFromItem(Item);
        base.Cleanup();
    }
}
