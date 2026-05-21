// -----------------------------------------------------------------------
// <copyright file="IDialogFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Defines a factory interface for creating dialog instances based on specified options.
/// </summary>
public interface IDialogFactory
{
    /// <summary>
    /// Creates a dialog instance based on the provided <see cref="DialogOptions"/>.
    /// </summary>
    /// <param name="options">The options for configuring the dialog.</param>
    /// <returns>A new instance of a dialog.</returns>
    IDialog Create(DialogOptions options);
}
