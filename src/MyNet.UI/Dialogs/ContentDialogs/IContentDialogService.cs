// -----------------------------------------------------------------------
// <copyright file="IContentDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Service for opening and managing custom content dialogs.
/// </summary>
public interface IContentDialogService
{
    /// <summary>
    /// Gets the collection of currently opened dialogs.
    /// </summary>
    IReadOnlyList<IDialog> OpenedDialogs { get; }

    /// <summary>
    /// Gets a value indicating whether at least one dialog is currently open.
    /// </summary>
    bool HasOpenedDialogs { get; }

    /// <summary>
    /// Displays a dialog and returns a simple boolean outcome (confirmed / cancelled / dismissed).
    /// </summary>
    /// <param name="dialog">The dialog view model.</param>
    /// <param name="options">Optional configuration overrides (modality, title, owner …).</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Displays a typed dialog and returns a strongly-typed <see cref="DialogResult{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the dialog.</typeparam>
    /// <param name="dialog">The typed dialog view model.</param>
    /// <param name="options">Optional configuration overrides.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task<DialogResult<TResult>> ShowAsync<TResult>(IDialog<TResult> dialog, DialogOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a fluent <see cref="IDialogBuilder"/> to configure and display the given dialog.
    /// </summary>
    IDialogBuilder Create(IDialog dialog);

    /// <summary>
    /// Creates a fluent <see cref="IDialogBuilder{TResult}"/> to configure and display the given typed dialog.
    /// </summary>
    IDialogBuilder<TResult> Create<TResult>(IDialog<TResult> dialog);

    /// <summary>
    /// Programmatically closes the specified open dialog.
    /// </summary>
    Task CloseAsync(IDialog dialog);
}
