// -----------------------------------------------------------------------
// <copyright file="IContentDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyNet.UI.ViewModels;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Service for opening and managing custom dialogs.
/// </summary>
public interface IContentDialogService
{
    /// <summary>
    /// Gets the collection of currently opened dialogs.
    /// </summary>
    IList<IDialogViewModel> OpenedDialogs { get; }

    /// <summary>
    /// Displays a non-modal custom dialog of specified type.
    /// </summary>
    /// <param name="viewModel">The view model of the new custom dialog.</param>
    /// <param name="options">The options for the new custom dialog.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the dialog was accepted (true), canceled (false), or closed without a result (null).</returns>
    Task<bool?> ShowAsync<T>(T viewModel, DialogOptions? options = null)
        where T : IDialogViewModel;

    /// <summary>
    /// Displays a non-modal custom dialog and executes a callback when closed.
    /// </summary>
    Task<bool?> ShowAsync<T>(T viewModel, Action<T>? closeAction)
        where T : IDialogViewModel;

    /// <summary>
    /// Displays a modal custom dialog.
    /// </summary>
    Task<bool?> ShowModalAsync<T>(T viewModel)
        where T : IDialogViewModel;

    /// <summary>
    /// Closes the specified dialog.
    /// </summary>
    /// <param name="dialog">The dialog to close.</param>
    Task<bool?> CloseAsync(IDialogViewModel dialog);
}
