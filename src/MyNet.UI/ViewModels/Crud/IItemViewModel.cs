// -----------------------------------------------------------------------
// <copyright file="IItemViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Defines a view model that represents an item of type T.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
public interface IItemViewModel<out T> : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the item represented by this view model.
    /// </summary>
    T? Item { get; }
}
