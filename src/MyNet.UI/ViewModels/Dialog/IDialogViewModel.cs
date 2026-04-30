// -----------------------------------------------------------------------
// <copyright file="IDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.ViewModels.Common;

namespace MyNet.UI.ViewModels.Dialog;

/// <summary>
/// Defines the contract for a dialog view model, which represents the data and behavior of a dialog in the MVVM pattern.
/// </summary>
public interface IDialogViewModel : IClosable
{
    /// <summary>
    /// Gets or sets the title of the dialog.
    /// </summary>
    string? Title { get; set; }
}

/// <summary>
/// Defines the contract for a dialog view model that can return a result of type <typeparamref name="TResult"/> when the dialog is closed.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the dialog.</typeparam>
public interface IDialogViewModel<out TResult> : IDialogViewModel, IDialogResultAware<TResult>;
