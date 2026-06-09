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
    private readonly Lock _sync = new();
    private readonly List<IDialog> _openedDialogs = [];
    private readonly Dictionary<IDialog, ActiveDialog> _activeDialogs = [];

    /// <inheritdoc />
    public event EventHandler<ContentDialogLifecycleEventArgs>? DialogOpened;

    /// <inheritdoc />
    public event EventHandler<ContentDialogLifecycleEventArgs>? DialogClosed;

    /// <inheritdoc />
    public IReadOnlyList<IDialog> OpenedDialogs
    {
        get
        {
            lock (_sync)
            {
                return _openedDialogs.ToList().AsReadOnly();
            }
        }
    }

    /// <inheritdoc />
    public bool HasOpenedDialogs
    {
        get
        {
            lock (_sync)
            {
                return _openedDialogs.Count > 0;
            }
        }
    }

    /// <inheritdoc />
    public async Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dialog);
        cancellationToken.ThrowIfCancellationRequested();

        var resolvedOptions = DialogOptions.Resolve(dialog, options);
        var strategy = GetStrategy(dialog, resolvedOptions);
        BeginShow(dialog, strategy);

        await dialog.OnOpenedAsync().ConfigureAwait(false);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await strategy.ShowAsync(dialog, resolvedOptions, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await CompleteLifecycleAsync(dialog).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<DialogResult<TResult>> ShowAsync<TResult>(IDialog<TResult> dialog, DialogOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dialog);
        cancellationToken.ThrowIfCancellationRequested();

        var resolvedOptions = DialogOptions.Resolve(dialog, options);
        var strategy = GetStrategy(dialog, resolvedOptions);
        BeginShow(dialog, strategy);

        await dialog.OnOpenedAsync().ConfigureAwait(false);
        var resultTask = dialog.GetResultAsync();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await strategy.ShowAsync(dialog, resolvedOptions, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await CompleteLifecycleAsync(dialog).ConfigureAwait(false);
        }

        return await resultTask.ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IDialogBuilder Create(IDialog dialog) => new DialogBuilder(this, dialog);

    /// <inheritdoc />
    public IDialogBuilder<TResult> Create<TResult>(IDialog<TResult> dialog) => new DialogBuilder<TResult>(this, dialog);

    /// <inheritdoc />
    public async Task CloseAsync(IDialog dialog)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        IDialogStrategy strategy;
        lock (_sync)
        {
            strategy = _activeDialogs.TryGetValue(dialog, out var active)
                ? active.Strategy
                : GetStrategy(dialog, null);
        }

        await strategy.CloseAsync(dialog).ConfigureAwait(false);
        await CompleteLifecycleAsync(dialog).ConfigureAwait(false);
    }

    private void BeginShow(IDialog dialog, IDialogStrategy strategy)
    {
        lock (_sync)
        {
            _openedDialogs.Add(dialog);
            _activeDialogs[dialog] = new() { Strategy = strategy };
        }

        DialogOpened?.Invoke(this, new(dialog));
    }

    private async Task CompleteLifecycleAsync(IDialog dialog)
    {
        bool shouldNotifyClosed;
        lock (_sync)
        {
            if (!_activeDialogs.TryGetValue(dialog, out var active) || active.LifecycleCompleted)
            {
                return;
            }

            active.LifecycleCompleted = true;
            _openedDialogs.Remove(dialog);
            _activeDialogs.Remove(dialog);
            shouldNotifyClosed = true;
        }

        if (shouldNotifyClosed)
        {
            await dialog.OnClosedAsync().ConfigureAwait(false);
            DialogClosed?.Invoke(this, new(dialog));
        }
    }

    private IDialogStrategy GetStrategy(IDialog dialog, DialogOptions? options)
        => strategies
            .Where(s => s.CanHandle(dialog, options))
            .OrderByDescending(s => s.Priority)
            .FirstOrDefault()
           ?? throw new InvalidOperationException(
               $"No dialog strategy found for '{dialog.GetType().Name}'. Register an IDialogStrategy via AddDialogs() or AddDialogStrategy<T>().");

    private sealed class ActiveDialog
    {
        public required IDialogStrategy Strategy { get; init; }

        public bool LifecycleCompleted { get; set; }
    }
}
