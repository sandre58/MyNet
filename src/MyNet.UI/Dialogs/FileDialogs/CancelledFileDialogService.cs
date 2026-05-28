// -----------------------------------------------------------------------
// <copyright file="CancelledFileDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Headless fallback that always returns a cancelled <see cref="FileDialogResult"/>.
/// Replace with a platform-specific <see cref="IFileDialogService"/> in GUI hosts.
/// </summary>
public sealed class CancelledFileDialogService : IFileDialogService
{
    private static readonly FileDialogResult Cancelled = new() { IsCancelled = true };

    /// <inheritdoc />
    public Task<FileDialogResult> ShowOpenFileDialogAsync(
        OpenFileDialogSettings settings,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Cancelled);
    }

    /// <inheritdoc />
    public Task<FileDialogResult> ShowSaveFileDialogAsync(
        SaveFileDialogSettings settings,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Cancelled);
    }

    /// <inheritdoc />
    public Task<FileDialogResult> ShowFolderDialogAsync(
        OpenFolderDialogSettings settings,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Cancelled);
    }
}
