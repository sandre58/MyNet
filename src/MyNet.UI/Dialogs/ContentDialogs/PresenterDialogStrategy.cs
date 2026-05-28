// -----------------------------------------------------------------------
// <copyright file="PresenterDialogStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Routes dialog display to the highest-priority registered <see cref="IDialogPresenter"/>.
/// </summary>
/// <param name="presenters">Presenters registered in the service collection.</param>
public sealed class PresenterDialogStrategy(IEnumerable<IDialogPresenter> presenters) : IDialogStrategy
{
    /// <summary>Default priority for presenter-based strategies (above <see cref="HeadlessDialogStrategy"/>).</summary>
    public const int DefaultPriority = 0;

    /// <inheritdoc />
    public int Priority => DefaultPriority;

    /// <inheritdoc />
    public bool CanHandle(IDialog dialog, DialogOptions? options) => SelectPresenter(dialog, options) is not null;

    /// <inheritdoc />
    public Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions options, CancellationToken ct = default)
    {
        var presenter = SelectPresenter(dialog, options)
                        ?? throw new InvalidOperationException(
                            $"No {nameof(IDialogPresenter)} found for '{dialog.GetType().Name}'.");

        return presenter.PresentAsync(dialog, options, ct);
    }

    /// <inheritdoc />
    public Task CloseAsync(IDialog dialog)
    {
        var presenter = SelectPresenter(dialog, null)
                        ?? throw new InvalidOperationException(
                            $"No {nameof(IDialogPresenter)} found for '{dialog.GetType().Name}'.");

        return presenter.CloseAsync(dialog);
    }

    private IDialogPresenter? SelectPresenter(IDialog dialog, DialogOptions? options)
        => presenters
            .Where(p => p.CanPresent(dialog, options))
            .OrderByDescending(p => p.Priority)
            .FirstOrDefault();
}
