// -----------------------------------------------------------------------
// <copyright file="DialogServiceListExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Observable.Collections.Selection;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.ViewModels.Dialog;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Selection;

namespace MyNet.UI.Dialogs;

/// <summary>
/// Provides fluent helpers to create list-based dialogs from existing list view models or raw item sequences.
/// </summary>
public static class DialogServiceListExtensions
{
    extension(IDialogService dialogService)
    {
        /// <summary>
        /// Creates a fluent builder for a list dialog backed by an existing list view model.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <typeparam name="TList">The concrete list view model type.</typeparam>
        /// <param name="list">The list view model displayed by the dialog.</param>
        /// <param name="commandFactory">Optional command factory used by the dialog view model.</param>
        /// <returns>A typed fluent builder for the dialog.</returns>
        public IDialogBuilder<IReadOnlyCollection<T>> CreateList<T, TList>(TList list, ICommandFactory? commandFactory = null)
            where T : notnull
            where TList : IListViewModel<T>
        {
            ArgumentNullException.ThrowIfNull(dialogService);
            ArgumentNullException.ThrowIfNull(list);

            return dialogService.Create(new ListDialogViewModel<T, TList>(list, commandFactory));
        }

        /// <summary>
        /// Creates a fluent builder for a list dialog from a raw sequence of items.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items to display.</param>
        /// <param name="listOptions">Optional configuration for the created list view model.</param>
        /// <param name="commandFactory">Optional command factory used by the dialog view model.</param>
        /// <returns>A typed fluent builder for the dialog.</returns>
        public IDialogBuilder<IReadOnlyCollection<T>> CreateList<T>(
            IEnumerable<T> items,
            ListViewModelOptions<T>? listOptions = null,
            ICommandFactory? commandFactory = null)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(dialogService);
            ArgumentNullException.ThrowIfNull(items);

            var list = ListViewModelFactory.Create(items, listOptions);
            return dialogService.Create(new ListDialogViewModel<T, ListViewModel<T>>(list, commandFactory));
        }

        /// <summary>
        /// Creates a fluent builder for a selection dialog backed by an existing selectable list view model.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="list">The selectable list view model displayed by the dialog.</param>
        /// <param name="commandFactory">Optional command factory used by the dialog view model.</param>
        /// <returns>A typed fluent builder for the dialog.</returns>
        public IDialogBuilder<IReadOnlyCollection<T>> CreateSelection<T>(SelectableListViewModel<T> list, ICommandFactory? commandFactory = null)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(dialogService);
            ArgumentNullException.ThrowIfNull(list);

            return dialogService.Create(new SelectionDialogViewModel<T>(list, commandFactory));
        }

        /// <summary>
        /// Creates a fluent builder for a selection dialog from a raw sequence of items.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items to display.</param>
        /// <param name="selectionMode">The selection mode used by the generated list view model.</param>
        /// <param name="listOptions">Optional configuration for the created list view model.</param>
        /// <param name="commandFactory">Optional command factory used by the dialog view model.</param>
        /// <returns>A typed fluent builder for the dialog.</returns>
        public IDialogBuilder<IReadOnlyCollection<T>> CreateSelection<T>(
            IEnumerable<T> items,
            SelectionMode selectionMode = SelectionMode.Multiple,
            ListViewModelOptions<T>? listOptions = null,
            ICommandFactory? commandFactory = null)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(dialogService);
            ArgumentNullException.ThrowIfNull(items);

            var list = ListViewModelFactory.CreateSelection(items, listOptions, selectionMode);
            return dialogService.Create(new SelectionDialogViewModel<T>(list, commandFactory));
        }
    }
}
