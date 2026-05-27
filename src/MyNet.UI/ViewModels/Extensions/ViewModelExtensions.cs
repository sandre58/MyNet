// -----------------------------------------------------------------------
// <copyright file="ViewModelExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Disposables;
using MyNet.UI.ViewModels.Crud;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels;
#pragma warning restore IDE0130

/// <summary>
/// Provides extension methods for view models, particularly for observing changes to items in IItemViewModel instances.
/// </summary>
public static class ViewModelExtensions
{
    /// <summary>
    /// Observes changes to the item of an IItemViewModel and invokes the provided handler with the new item value whenever it changes.
    /// </summary>
    /// <param name="vm">The IItemViewModel to observe.</param>
    /// <param name="handler">The action to invoke when the item changes.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <returns>An IDisposable that can be used to unsubscribe from the item changes.</returns>
    public static IDisposable ObserveItemChanged<T>(this IItemViewModel<T> vm, Action<T?> handler)
    {
        vm.ItemChanged += localHandler;

        return Disposable.Create(() => vm.ItemChanged -= localHandler);

        void localHandler(object? sender, ItemChangedEventArgs<T> e) => handler(e.NewItem);
    }
}
