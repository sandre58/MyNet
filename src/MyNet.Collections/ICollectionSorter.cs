// -----------------------------------------------------------------------
// <copyright file="ICollectionSorter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyNet.Collections;

/// <summary>
/// Defines external sorting behavior for collection items.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface ICollectionSorter<T>
{
    /// <summary>
    /// Returns a sorted snapshot of source items.
    /// </summary>
    IReadOnlyList<T> Sort(IEnumerable<T> source, Func<T, object> selector, ListSortDirection direction = ListSortDirection.Ascending);

    /// <summary>
    /// Finds the insertion index to keep the sequence sorted.
    /// </summary>
    int FindInsertIndex(IReadOnlyList<T> source, T item, Func<T, object> selector, ListSortDirection direction = ListSortDirection.Ascending);
}
