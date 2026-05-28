// -----------------------------------------------------------------------
// <copyright file="RecentFilesOperations.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyNet.IO.FileHistory;

namespace MyNet.UI.Services.FileHistory;

/// <summary>
/// Wraps <see cref="IRecentFilesService"/> for UI scenarios and notifies subscribers when data changes.
/// </summary>
public sealed class RecentFilesOperations(IRecentFilesService recentFilesService) : IRecentFilesOperations
{
    /// <inheritdoc />
    public event EventHandler? Changed;

    /// <inheritdoc />
    public Task<IReadOnlyList<RecentFile>> GetAllAsync(CancellationToken cancellationToken = default) =>
        recentFilesService.GetAllAsync();

    /// <inheritdoc />
    public async Task<RecentFile?> AddAsync(string name, string path, bool isRecovered = false, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var result = await recentFilesService.AddAsync(name, path, isRecovered).ConfigureAwait(false);
        NotifyChanged();
        return result;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveAsync(string path, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var removed = await recentFilesService.RemoveAsync(path).ConfigureAwait(false);

        if (removed)
            NotifyChanged();

        return removed;
    }

    /// <inheritdoc />
    public async Task<RecentFile?> SetPinnedAsync(string path, bool isPinned, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var result = await recentFilesService.PinAsync(path, isPinned).ConfigureAwait(false);
        NotifyChanged();
        return result;
    }

    private void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);
}
