// -----------------------------------------------------------------------
// <copyright file="DialogOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Represents the options for configuring a content dialog (modality, title, owner, overlay behaviour …).
/// </summary>
public sealed class DialogOptions
{
    /// <summary>
    /// Gets or sets the dialog associated with these options.
    /// </summary>
    public IDialog Dialog { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is modal.
    /// </summary>
    public bool IsModal { get; set; }

    /// <summary>
    /// Gets or sets the title override for the dialog.
    /// When set, takes precedence over <see cref="IDialog.Title"/>.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the owner of the dialog (e.g. the parent window).
    /// </summary>
    public object? Owner { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether clicking on the overlay closes the dialog.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool CloseOnOverlayClick { get; set; } = true;

    /// <summary>
    /// Creates a detached copy of <paramref name="options"/> bound to <paramref name="dialog"/>
    /// without mutating the caller's instance.
    /// </summary>
    /// <param name="dialog">The dialog instance to associate with the resolved options.</param>
    /// <param name="options">Optional source options; when <see langword="null"/>, defaults are used.</param>
    /// <returns>A new <see cref="DialogOptions"/> instance.</returns>
    public static DialogOptions Resolve(IDialog dialog, DialogOptions? options = null)
        => options is null
            ? new DialogOptions { Dialog = dialog }
            : new DialogOptions
            {
                Dialog = dialog,
                IsModal = options.IsModal,
                Title = options.Title,
                Owner = options.Owner,
                CloseOnOverlayClick = options.CloseOnOverlayClick
            };
}
