// -----------------------------------------------------------------------
// <copyright file="ItemsFileProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Utilities.Providers;

namespace MyNet.Utilities.IO;

/// <summary>
/// Base class for items providers that read items from a file.
/// </summary>
/// <param name="filename">The file path.</param>
/// <typeparam name="T">The type of items.</typeparam>
public abstract class ItemsFileProvider<T>(string? filename = null) : IValidatedItemsProvider<T>
{
    /// <summary>
    /// Gets the source file path.
    /// </summary>
    public string? Filename { get; private set; } = filename;

    /// <summary>
    /// Sets the source file path.
    /// </summary>
    /// <param name="filename">The file path.</param>
    public void SetFilename(string? filename) => Filename = filename;

    /// <inheritdoc/>
    public async IAsyncEnumerable<T> GetItemsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var filename = FileHelper.EnsureFileExists(Filename.OrEmpty());

        await foreach (var item in GetItemsCoreAsync(filename, cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return item;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ItemLoadError>> ValidateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var filename = FileHelper.EnsureFileExists(Filename.OrEmpty());

        var errors = await ValidateCoreAsync(filename, cancellationToken).ConfigureAwait(false);

        return errors.AsReadOnly();
    }

    /// <summary>
    /// When implemented in a derived class, retrieves items from the specified file path.
    /// </summary>
    /// <param name="filename">The file path.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An asynchronous stream of items.</returns>
    protected abstract IAsyncEnumerable<T> GetItemsCoreAsync(string filename, CancellationToken cancellationToken);

    /// <summary>
    /// Validates the specified file and returns discovered errors.
    /// </summary>
    /// <param name="filename">The source filename.</param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    protected virtual Task<List<ItemLoadError>> ValidateCoreAsync(string filename, CancellationToken cancellationToken) => Task.FromResult<List<ItemLoadError>>([]);
}
