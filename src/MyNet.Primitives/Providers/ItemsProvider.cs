// -----------------------------------------------------------------------
// <copyright file="ItemsProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Primitives.Providers;

/// <summary>
/// A simple implementation of the <see cref="IItemsProvider{T}"/> interface that takes an <see cref="IEnumerable{T}"/> as a constructor parameter and provides it through the <see cref="GetItems"/> method.
/// </summary>
/// <param name="items">The collection of items to be provided.</param>
/// <typeparam name="T">The type of items provided by the implementing class.</typeparam>
public class ItemsProvider<T>(IEnumerable<T> items) : ItemsProviderBase<T>
{
    /// <summary>
    /// Provides the collection of items that was passed to the constructor. This method simply returns the original collection of items without any modifications or filtering. It serves as a basic implementation of the <see cref="IItemsProvider{T}"/> interface, allowing for easy provision of a predefined set of items when the <see cref="GetItems"/> method is called.
    /// </summary>
    /// <returns>The collection of items provided by this instance.</returns>
    public override IEnumerable<T> GetItems() => items ?? throw new ArgumentNullException(nameof(items));
}
