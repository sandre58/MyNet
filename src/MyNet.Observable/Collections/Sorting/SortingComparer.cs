// -----------------------------------------------------------------------
// <copyright file="SortingComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MyNet.Observable.Collections.Sorting;

/// <summary>
/// Implements a comparer that compares objects of type T based on multiple sorting properties defined in an array of <see cref="ISortingProperty{T}"/>.
/// </summary>
/// <param name="sorting">An array of sorting properties to define the comparison logic.</param>
/// <typeparam name="T">The type of objects to compare.</typeparam>
public class SortingComparer<T>(ISortingProperty<T>[] sorting) : IComparer, IComparer<T>
{
    private readonly Func<T, object?>[] _selectors = [.. sorting.Select(x => x.ProvideExpression().Compile())];
    private readonly ListSortDirection[] _directions = [.. sorting.Select(x => x.Direction)];

    /// <summary>
    /// Compares two objects of type T based on the sorting properties defined in the sortCollection.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>A signed integer that indicates the relative values of x and y.</returns>
    public int Compare(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        for (var i = 0; i < _selectors.Length; i++)
        {
            var selector = _selectors[i];

            var vx = selector(x);
            var vy = selector(y);

            var result = Comparer<object?>.Default.Compare(vx, vy);

            if (result != 0)
            {
                return _directions[i] == ListSortDirection.Ascending
                    ? result
                    : -result;
            }
        }

        return 0;
    }

    /// <summary>
    /// Compares two objects based on the sorting properties defined in the sortCollection.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>A signed integer that indicates the relative values of x and y.</returns>
    public int Compare(object? x, object? y) => Compare((T?)x, (T?)y);
}
