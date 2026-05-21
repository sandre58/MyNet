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
}
