// -----------------------------------------------------------------------
// <copyright file="DefaultCollectionSorter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MyNet.Collections;

/// <summary>
/// Current sorting strategy using comparer-based ordering.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed class DefaultCollectionSorter<T> : ICollectionSorter<T>
{
    /// <summary>
    /// Gets the default instance of the collection sorter that uses comparer-based ordering.
    /// </summary>
    public static DefaultCollectionSorter<T> Default { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultCollectionSorter{T}"/> class.
    /// </summary>
    private DefaultCollectionSorter() { }

    /// <summary>
    /// Returns a sorted snapshot of source items based on the specified selector and sort direction.
    /// </summary>
    /// <param name="source">The source collection to sort.</param>
    /// <param name="selector">A function to extract the key for each element.</param>
    /// <param name="direction">The direction to sort the elements.</param>
    /// <returns>A sorted list of elements.</returns>
    public IReadOnlyList<T> Sort(IEnumerable<T> source, Func<T, object> selector, ListSortDirection direction = ListSortDirection.Ascending)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return direction == ListSortDirection.Ascending
            ? [.. source.OrderBy(selector)]
            : source.OrderByDescending(selector).ToList();
    }

    /// <summary>
    /// Finds the insertion index for the specified item in the sorted source collection based on the selector and sort direction.
    /// </summary>
    /// <param name="source">The sorted source collection.</param>
    /// <param name="item">The item to find the insertion index for.</param>
    /// <param name="selector">A function to extract the key for each element.</param>
    /// <param name="direction">The direction to sort the elements.</param>
    /// <returns>The index at which the item should be inserted.</returns>
    public int FindInsertIndex(IReadOnlyList<T> source, T item, Func<T, object> selector, ListSortDirection direction = ListSortDirection.Ascending)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        if (source.Count == 0)
        {
            return 0;
        }

        var itemKey = selector(item);
        var left = 0;
        var right = source.Count;

        while (left < right)
        {
            var mid = (left + right) / 2;
            var midKey = selector(source[mid]);
            var comparison = Comparer<object>.Default.Compare(midKey, itemKey);

            if (direction == ListSortDirection.Descending)
            {
                comparison = -comparison;
            }

            if (comparison < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }

        return left;
    }
}
