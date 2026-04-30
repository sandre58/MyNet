// -----------------------------------------------------------------------
// <copyright file="IDialogStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using MyNet.UI.ViewModels;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Defines the contract for a dialog strategy, which determines how to display and manage dialogs based on their view models and options.
/// </summary>
public interface IDialogStrategy
{
    /// <summary>
    /// Determines whether this strategy can handle the given dialog view model and options, allowing for flexible dialog management based on different criteria.
    /// </summary>
    /// <param name="viewModel">The dialog view model to evaluate.</param>
    /// <param name="options">The dialog options to consider.</param>
    /// <returns>True if this strategy can handle the dialog; otherwise, false.</returns>
    bool CanHandle(IDialogViewModel viewModel, DialogOptions? options);

    /// <summary>
    /// Displays the dialog using this strategy.
    /// </summary>
    /// <param name="viewModel">The dialog view model to display.</param>
    /// <param name="options">The dialog options to consider.</param>
    Task ShowAsync(IDialogViewModel viewModel, DialogOptions? options);

    /// <summary>
    /// Closes the dialog using this strategy.
    /// </summary>
    /// <param name="viewModel">The dialog view model to close.</param>
    Task CloseAsync(IDialogViewModel viewModel);
}
