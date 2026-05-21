// -----------------------------------------------------------------------
// <copyright file="ItemChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Event arguments for an item change, containing the old and new values of the item.
/// </summary>
/// <param name="oldItem">The previous item value.</param>
/// <param name="newItem">The new item value.</param>
/// <typeparam name="T">The type of the item.</typeparam>
public class ItemChangedEventArgs<T>(T? oldItem, T? newItem) : EventArgs
{
    /// <summary>
    /// Gets the previous item value before the change.
    /// </summary>
    public T? OldItem { get; } = oldItem;

    /// <summary>
    /// Gets the new item value after the change.
    /// </summary>
    public T? NewItem { get; } = newItem;
}
