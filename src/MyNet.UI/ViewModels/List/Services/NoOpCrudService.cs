// -----------------------------------------------------------------------
// <copyright file="NoOpCrudService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.ViewModels.List.Services;

/// <summary>
/// Default no-op implementation for <see cref="ICrudService{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed class NoOpCrudService<T> : ICrudService<T>
    where T : notnull
{
    /// <inheritdoc />
    public Task<T?> GetAsync(T item, CancellationToken cancellationToken = default) => Task.FromResult<T?>(item);

    /// <inheritdoc />
    public Task<T?> CreateAsync(CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);

    /// <inheritdoc />
    public Task<bool> UpdateAsync(T item, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc />
    public Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc />
    public Task<bool> DeleteRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default) => Task.FromResult(true);
}
