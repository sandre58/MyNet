// -----------------------------------------------------------------------
// <copyright file="IDialogResultAware.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Dialog;

/// <summary>
/// Defines the contract for a dialog view model that can be aware of its result of type <typeparamref name="TResult"/> when the dialog is closed.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the dialog.</typeparam>
public interface IDialogResultAware<out TResult>
{
    /// <summary>
    /// Gets the result of the dialog.
    /// </summary>
    TResult? Result { get; }
}
