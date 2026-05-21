// -----------------------------------------------------------------------
// <copyright file="ICrudService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.ViewModels.List.Services;

/// <summary>
/// Provides CRUD operations for list-oriented view models.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface ICrudService<T>
    where T : notnull
{
    /// <summary>
    /// Gets details for an existing item.
    /// </summary>
    Task<T?> GetAsync(T item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new item.
    /// </summary>
    Task<T?> CreateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    Task<bool> UpdateAsync(T item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing item.
    /// </summary>
    Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple items.
    /// </summary>
    Task<bool> DeleteRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
}
