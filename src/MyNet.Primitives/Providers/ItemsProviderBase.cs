// -----------------------------------------------------------------------
// <copyright file="ItemsProviderBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyNet.Primitives.Providers;

/// <summary>
/// Defines an abstract base class for implementing the <see cref="IItemsProvider{T}"/> interface, providing a default implementation of the asynchronous item retrieval method. This class requires derived classes to implement the synchronous <see cref="GetItems"/> method, while it handles the conversion to an asynchronous enumerable using the <see cref="System.Linq.AsyncEnumerable"/> extension method. This allows for a consistent and simplified approach to creating item providers that can be used in both synchronous and asynchronous contexts without requiring duplicate code for item retrieval logic.
/// </summary>
/// <typeparam name="T">The type of items provided by the implementing class.</typeparam>
public abstract class ItemsProviderBase<T> : IItemsProvider<T>
{
    /// <summary>
    /// Provides a collection of items of type T. This method must be implemented by derived classes to return the items that they provide. The returned collection can be from any source, such as an in-memory list, a database query, or an external API call. The base class will handle the conversion of this synchronous collection into an asynchronous stream when the <see cref="GetItemsAsync"/> method is called, allowing for efficient retrieval of items in asynchronous contexts without requiring additional implementation effort from derived classes.
    /// </summary>
    /// <returns>A collection of items of type T.</returns>
    public abstract IEnumerable<T> GetItems();

    /// <summary>
    /// Asynchronously retrieves a stream of items of type T by converting the collection provided by the <see cref="GetItems"/> method into an asynchronous enumerable. This method uses the <see cref="System.Linq.AsyncEnumerable"/> extension method to convert the synchronous collection into an asynchronous stream, allowing for efficient retrieval of items that may be produced or fetched asynchronously. The method accepts an optional CancellationToken parameter, allowing callers to cancel the operation if needed. Implementations of this method can provide items from various sources, such as in-memory collections, databases, or external APIs, and can support asynchronous data retrieval patterns to improve performance and resource management when dealing with large datasets or slow data sources.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An asynchronous stream of items of type T.</returns>
    public IAsyncEnumerable<T> GetItemsAsync(CancellationToken cancellationToken = default) => GetItems().ToAsyncEnumerable();
}
