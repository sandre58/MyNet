// -----------------------------------------------------------------------
// <copyright file="IItemsProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;

namespace MyNet.Utilities.Providers;

/// <summary>
/// Defines an interface for providing a collection of items of type T. This interface includes a method that returns an asynchronous stream of items, allowing for efficient retrieval of items that may be produced or fetched asynchronously. Implementations of this interface can provide items from various sources, such as in-memory collections, databases, or external APIs, and can support cancellation through the provided CancellationToken parameter. The use of IAsyncEnumerable allows for consuming items in a streaming fashion, which can be beneficial for performance and resource management when dealing with large datasets or slow data sources.
/// </summary>
/// <typeparam name="T">The type of items provided by the implementing class.</typeparam>
public interface IItemsProvider<out T>
{
    /// <summary>
    /// Asynchronously retrieves a stream of items of type T. This method returns an IAsyncEnumerable, which allows for consuming items in a streaming fashion, enabling efficient retrieval of items that may be produced or fetched asynchronously. The method accepts an optional CancellationToken parameter, allowing callers to cancel the operation if needed. Implementations of this method can provide items from various sources, such as in-memory collections, databases, or external APIs, and can support asynchronous data retrieval patterns to improve performance and resource management when dealing with large datasets or slow data sources.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An asynchronous stream of items of type T.</returns>
    IAsyncEnumerable<T> GetItemsAsync(CancellationToken cancellationToken = default);
}
