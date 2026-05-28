// -----------------------------------------------------------------------
// <copyright file="IDialogPresenter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Platform-specific presenter that displays a dialog and waits until it closes.
/// Register implementations via <c>AddDialogPresenter&lt;TPresenter&gt;()</c> on <see cref="IServiceCollection"/>.
/// </summary>
public interface IDialogPresenter
{
    /// <summary>
    /// Gets the priority of this presenter. When multiple presenters match, the highest wins.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines whether this presenter can display the given dialog.
    /// </summary>
    bool CanPresent(IDialog dialog, DialogOptions? options);

    /// <summary>
    /// Displays the dialog and waits until it is closed.
    /// </summary>
    Task<DialogResult<bool>> PresentAsync(IDialog dialog, DialogOptions options, CancellationToken cancellationToken);

    /// <summary>
    /// Programmatically closes a dialog previously shown by this presenter.
    /// </summary>
    Task CloseAsync(IDialog dialog);
}
