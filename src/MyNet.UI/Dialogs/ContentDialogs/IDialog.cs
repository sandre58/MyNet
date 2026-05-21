// -----------------------------------------------------------------------
// <copyright file="IDialog.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Defines the contract for a dialog view model in the MVVM pattern.
/// Includes lifecycle hooks and closability support.
/// </summary>
public interface IDialog : IClosable, IDialogAware
{
    /// <summary>
    /// Gets the title of the dialog.
    /// </summary>
    string? Title { get; }
}

/// <summary>
/// Defines the contract for a dialog view model that can return a typed result of type <typeparamref name="TResult"/> when closed.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the dialog.</typeparam>
public interface IDialog<TResult> : IDialog
{
    /// <summary>
    /// Returns a task that completes when the dialog closes, carrying the typed <see cref="DialogResult{TResult}"/>.
    /// </summary>
    Task<DialogResult<TResult>> GetResultAsync();
}
