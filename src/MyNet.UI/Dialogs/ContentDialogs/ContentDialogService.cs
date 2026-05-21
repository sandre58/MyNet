// -----------------------------------------------------------------------
// <copyright file="ContentDialogService.cs" company="Stéphane ANDRE">
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
/// Default implementation of <see cref="IContentDialogService"/>.
/// Selects the appropriate <see cref="IDialogStrategy"/> via <c>CanHandle</c> / <c>Priority</c>
/// and manages the full dialog lifecycle (OnOpenedAsync → show → OnClosedAsync).
/// </summary>
public sealed class ContentDialogService(IEnumerable<IDialogStrategy> strategies) : IContentDialogService
{
    private readonly List<IDialog> _openedDialogs = [];

    /// <inheritdoc />
    public IReadOnlyList<IDialog> OpenedDialogs => _openedDialogs.AsReadOnly();

    /// <inheritdoc />
    public bool HasOpenedDialogs => _openedDialogs.Count > 0;

    /// <inheritdoc />
    public async Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions? options = null, CancellationToken cancellationToken = default)
    {
        var resolvedOptions = options ?? new DialogOptions { Dialog = dialog };
        resolvedOptions.Dialog = dialog;

        var strategy = GetStrategy(dialog, resolvedOptions);

        _openedDialogs.Add(dialog);
        await dialog.OnOpenedAsync().ConfigureAwait(false);

        try
        {
            return await strategy.ShowAsync(dialog, resolvedOptions, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _openedDialogs.Remove(dialog);
            await dialog.OnClosedAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<DialogResult<TResult>> ShowAsync<TResult>(IDialog<TResult> dialog, DialogOptions? options = null, CancellationToken cancellationToken = default)
    {
        var resolvedOptions = options ?? new DialogOptions { Dialog = dialog };
        resolvedOptions.Dialog = dialog;

        var strategy = GetStrategy(dialog, resolvedOptions);

        _openedDialogs.Add(dialog);
        await dialog.OnOpenedAsync().ConfigureAwait(false);

        try
        {
            await strategy.ShowAsync(dialog, resolvedOptions, cancellationToken).ConfigureAwait(false);
            return await dialog.GetResultAsync().ConfigureAwait(false);
        }
        finally
        {
            _openedDialogs.Remove(dialog);
            await dialog.OnClosedAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public IDialogBuilder Create(IDialog dialog) => new DialogBuilder(this, dialog);

    /// <inheritdoc />
    public IDialogBuilder<TResult> Create<TResult>(IDialog<TResult> dialog) => new DialogBuilder<TResult>(this, dialog);

    /// <inheritdoc />
    public async Task CloseAsync(IDialog dialog)
    {
        var strategy = GetStrategy(dialog, null);
        await strategy.CloseAsync(dialog).ConfigureAwait(false);
    }

    /// <summary>
    /// Selects the appropriate dialog strategy based on the dialog type and options.
    /// </summary>
    /// <param name="dialog">The dialog for which to select a strategy.</param>
    /// <param name="options">The dialog options to consider (may be null for close-only queries).</param>
    /// <returns>The selected dialog strategy.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no suitable dialog strategy is found.</exception>
    private IDialogStrategy GetStrategy(IDialog dialog, DialogOptions? options)
        => strategies
            .Where(s => s.CanHandle(dialog, options))
            .OrderByDescending(s => s.Priority)
            .FirstOrDefault()
           ?? throw new InvalidOperationException($"No dialog strategy found for '{dialog.GetType().Name}'. Register an IDialogStrategy implementation.");
}
