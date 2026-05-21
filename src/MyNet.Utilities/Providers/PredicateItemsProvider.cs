// -----------------------------------------------------------------------
// <copyright file="PredicateItemsProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MyNet.Utilities.Providers;

/// <summary>
/// Defines a class that implements the <see cref="IItemsProvider{T}"/> interface and provides a filtered collection of items based on a specified predicate. This class takes an existing <see cref="IItemsProvider{T}"/> and a predicate function, and when the <see cref="GetItemsAsync"/> method is called, it returns only the items that satisfy the predicate condition. This allows for dynamic filtering of items from any underlying provider without modifying the original data source.
/// </summary>
/// <param name="provider">The underlying items provider.</param>
/// <param name="predicate">The predicate function used to filter items.</param>
/// <typeparam name="T">The type of items provided by the implementing class.</typeparam>
public class PredicateItemsProvider<T>(IItemsProvider<T> provider, Func<T, bool> predicate) : IItemsProvider<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateItemsProvider{T}"/> class with the specified items and predicate. This constructor allows for creating a <see cref="PredicateItemsProvider{T}"/> directly from an enumerable collection of items and a predicate function, without needing to first create an <see cref="IItemsProvider{T}"/>. The provided items will be filtered based on the predicate when the <see cref="GetItemsAsync"/> method is called.
    /// </summary>
    /// <param name="items">The collection of items to be filtered.</param>
    /// <param name="predicate">The predicate function used to filter items.</param>
    public PredicateItemsProvider(IEnumerable<T> items, Func<T, bool> predicate)
        : this(new ItemsProvider<T>(items), predicate) { }

    /// <inheritdoc/>
    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "This method is intended to be used in a context where ConfigureAwait(false) is not necessary, such as in a library or when the caller is expected to handle synchronization context appropriately.")]
    public async IAsyncEnumerable<T> GetItemsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in provider.GetItemsAsync(cancellationToken))
        {
            if (predicate(item))
                yield return item;
        }
    }
}
