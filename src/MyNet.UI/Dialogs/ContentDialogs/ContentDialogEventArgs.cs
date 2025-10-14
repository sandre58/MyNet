// -----------------------------------------------------------------------
// <copyright file="ContentDialogEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Provides data for dialog events such as opening and closing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ContentDialogEventArgs"/> class.
/// </remarks>
/// <param name="dialog">The dialog view model associated with the event.</param>
public class ContentDialogEventArgs(IDialogViewModel dialog) : EventArgs
{
    /// <summary>
    /// Gets the dialog view model associated with the event.
    /// </summary>
    public IDialogViewModel Dialog { get; } = dialog;
}
