// -----------------------------------------------------------------------
// <copyright file="HeadlessDialogStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Dialogs.MessageBox;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Fallback strategy for headless environments (unit tests, services without UI).
/// Message boxes complete immediately with <see cref="IMessageBox.DefaultResult"/>.
/// Other dialogs complete with a dismissed outcome.
/// </summary>
public sealed class HeadlessDialogStrategy : IDialogStrategy
{
    /// <summary>Priority used so platform presenters override this fallback.</summary>
    public const int FallbackPriority = -1000;

    /// <inheritdoc />
    public int Priority => FallbackPriority;

    /// <inheritdoc />
    public bool CanHandle(IDialog dialog, DialogOptions? options) => true;

    /// <inheritdoc />
    public Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (dialog is MessageBoxViewModel messageBox)
        {
            messageBox.ApplyResult(messageBox.DefaultResult);
            return Task.FromResult(DialogResult.Ok());
        }

        return Task.FromResult(DialogResult.Dismiss());
    }

    /// <inheritdoc />
    public Task CloseAsync(IDialog dialog) => Task.CompletedTask;
}
