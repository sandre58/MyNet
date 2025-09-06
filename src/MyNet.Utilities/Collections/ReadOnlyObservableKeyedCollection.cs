// -----------------------------------------------------------------------
// <copyright file="ReadOnlyObservableKeyedCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace MyNet.Utilities.Collections;

/// <summary>
/// Read-only wrapper around <see cref="ObservableKeyedCollection{TKey, T}"/> exposing key lookup.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="T">The type of items.</typeparam>
public class ReadOnlyObservableKeyedCollection<TKey, T>(ObservableKeyedCollection<TKey, T> list) : ReadOnlyObservableCollection<T>(list)
    where TKey : notnull
{
    /// <summary>
    /// Gets the item associated with the specified key, or null if not found.
    /// </summary>
    /// <param name="key">The key of the item to get.</param>
    public T? this[TKey key] => ((ObservableKeyedCollection<TKey, T>)Items)[key];
}
