// -----------------------------------------------------------------------
// <copyright file="IValidatedItemsProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.Utilities.Providers;

/// <summary>
/// Defines a contract for providers that can validate items before loading them.
/// </summary>
/// <typeparam name="T">The type of items.</typeparam>
public interface IValidatedItemsProvider<out T> : IItemsProvider<T>
{
    /// <summary>
    /// Validates the items before loading them. This method should perform necessary checks and return a result indicating whether the items are valid and can be loaded.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains the validation result.</returns>
    Task<IReadOnlyList<ItemLoadError>> ValidateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an error that occurred during the loading of an item, including the exception that was thrown and an optional context message providing additional information about the error.
/// </summary>
/// <param name="Exception">The exception that was thrown.</param>
/// <param name="Context">An optional context message providing additional information about the error.</param>
public sealed record ItemLoadError(Exception Exception, string? Context = null);
