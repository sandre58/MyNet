// -----------------------------------------------------------------------
// <copyright file="IContentDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Service for opening and managing custom dialogs.
/// </summary>
public interface IContentDialogService
{
    /// <summary>
    /// Occurs when a dialog is opened.
    /// </summary>
    event EventHandler<ContentDialogEventArgs> DialogOpened;

    /// <summary>
    /// Occurs when a dialog is closed.
    /// </summary>
    event EventHandler<ContentDialogEventArgs> DialogClosed;

    /// <summary>
    /// Gets the collection of currently opened dialogs.
    /// </summary>
    ObservableCollection<IDialogViewModel> OpenedDialogs { get; }

    /// <summary>
    /// Displays a non-modal custom dialog of specified type.
    /// </summary>
    /// <param name="view">The type of the custom dialog to show.</param>
    /// <param name="viewModel">The view model of the new custom dialog.</param>
    Task ShowAsync(object view, IDialogViewModel viewModel);

    /// <summary>
    /// Displays a modal custom dialog of specified type.
    /// </summary>
    /// <param name="view">The type of the custom dialog to show.</param>
    /// <param name="viewModel">The view model of the new custom dialog.</param>
    Task<bool?> ShowModalAsync(object view, IDialogViewModel viewModel);

    /// <summary>
    /// Closes the specified dialog.
    /// </summary>
    /// <param name="dialog">The dialog to close.</param>
    Task<bool?> CloseAsync(IDialogViewModel dialog);
}
