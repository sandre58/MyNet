// -----------------------------------------------------------------------
// <copyright file="IDialogBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Fluent builder for configuring and displaying a content dialog.
/// </summary>
public interface IDialogBuilder
{
    /// <summary>Configures the dialog to be displayed as a modal dialog.</summary>
    IDialogBuilder AsModal();

    /// <summary>Configures the dialog to be displayed as a non-modal dialog.</summary>
    IDialogBuilder AsNonModal();

    /// <summary>Sets the title of the dialog, overriding <see cref="IDialog.Title"/>.</summary>
    IDialogBuilder WithTitle(string title);

    /// <summary>Specifies the owner window or element for the dialog.</summary>
    IDialogBuilder WithOwner(object owner);

    /// <summary>Configures whether the dialog closes when the overlay is clicked.</summary>
    IDialogBuilder CloseOnOverlayClick(bool value = true);

    /// <summary>
    /// Displays the dialog asynchronously and returns a <see cref="DialogResult{T}"/> of <see cref="bool"/>.
    /// </summary>
    Task<DialogResult<bool>> ShowAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Fluent builder for configuring and displaying a content dialog that returns a typed result.
/// </summary>
/// <typeparam name="TResult">The type of the value returned by the dialog.</typeparam>
public interface IDialogBuilder<TResult> : IDialogBuilder
{
    /// <summary>Configures the typed dialog to be displayed as a modal dialog.</summary>
    new IDialogBuilder<TResult> AsModal();

    /// <summary>Configures the typed dialog to be displayed as a non-modal dialog.</summary>
    new IDialogBuilder<TResult> AsNonModal();

    /// <summary>Sets the title of the typed dialog, overriding <see cref="IDialog.Title"/>.</summary>
    new IDialogBuilder<TResult> WithTitle(string title);

    /// <summary>Specifies the owner window or element for the typed dialog.</summary>
    new IDialogBuilder<TResult> WithOwner(object owner);

    /// <summary>Configures whether the typed dialog closes when the overlay is clicked.</summary>
    new IDialogBuilder<TResult> CloseOnOverlayClick(bool value = true);

    /// <summary>
    /// Displays the dialog asynchronously and returns a strongly-typed <see cref="DialogResult{TResult}"/>.
    /// </summary>
    new Task<DialogResult<TResult>> ShowAsync(CancellationToken cancellationToken = default);
}
