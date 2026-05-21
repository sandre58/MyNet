// -----------------------------------------------------------------------
// <copyright file="IDialogStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Defines the contract for a dialog strategy, which determines how to display and manage dialogs.
/// Each strategy is platform-specific (Window, Popup, Overlay, …) and registered via DI.
/// </summary>
public interface IDialogStrategy
{
    /// <summary>
    /// Gets the priority of this strategy. When multiple strategies match, the one with
    /// the highest priority is selected.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines whether this strategy can handle the given dialog and options.
    /// </summary>
    /// <param name="dialog">The dialog view model to evaluate.</param>
    /// <param name="options">The dialog options to consider (maybe null for close-only queries).</param>
    /// <returns><see langword="true"/> if this strategy can handle the dialog; otherwise, <see langword="false"/>.</returns>
    bool CanHandle(IDialog dialog, DialogOptions? options);

    /// <summary>
    /// Displays the dialog using this strategy and waits until it is closed.
    /// </summary>
    /// <param name="dialog">The dialog view model to display.</param>
    /// <param name="options">The dialog options (modality, owner, …).</param>
    /// <param name="ct">A token to observe for cancellation requests.</param>
    /// <returns>
    /// A <see cref="DialogResult{T}"/> of <see cref="bool"/> where <c>true</c> means the user
    /// confirmed, <c>false</c> means the user explicitly canceled, and <see cref="DialogOutcome.Dismissed"/>
    /// means the dialog was closed without an explicit action.
    /// </returns>
    Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions options, CancellationToken ct = default);

    /// <summary>
    /// Programmatically closes the dialog managed by this strategy.
    /// </summary>
    /// <param name="dialog">The dialog view model to close.</param>
    Task CloseAsync(IDialog dialog);
}
