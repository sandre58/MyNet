// -----------------------------------------------------------------------
// <copyright file="IDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Defines the contract for a dialog view model, including title, result, and loading behavior.
/// </summary>
public interface IDialogViewModel : IClosable
{
    /// <summary>
    /// Gets or sets the title of the dialog.
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// Gets the result of the dialog (true for OK, false for Cancel, null for none).
    /// </summary>
    bool? DialogResult { get; }

    /// <summary>
    /// Gets a value indicating whether the dialog should load when opening.
    /// </summary>
    bool LoadWhenDialogOpening { get; }

    /// <summary>
    /// Loads the dialog content or data.
    /// </summary>
    Task LoadAsync();
}
