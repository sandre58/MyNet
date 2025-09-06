// -----------------------------------------------------------------------
// <copyright file="NullableComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Comparers;

/// <summary>
/// Provides comparison logic for nullable value types following the MSDN null comparison rules.
/// </summary>
/// <typeparam name="T">The underlying non-nullable value type that implements <see cref="IComparable{T}"/>.</typeparam>
public class NullableComparer<T> : IComparer<T?>, IComparer<IComparable<T>?>
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Compares two nullable values of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="x">The first value to compare.</param>
    /// <param name="y">The second value to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of x and y:
    /// - Less than zero: x is less than y.
    /// - Zero: x equals y.
    /// - Greater than zero: x is greater than y.
    /// </returns>
    public int Compare(T? x, T? y)
    {
        // Compare nulls according MSDN specification

        // Two nulls are equal
        if (x == null && y == null)
            return 0;

        // Any object is greater than null
        if (x != null && y == null)
            return 1;

        if (x == null)
            return -1;

        // Otherwise compare the two values
        return x.Value.CompareTo(y!.Value);
    }

    /// <summary>
    /// Compares two objects that implement <see cref="IComparable{T}"/>, allowing nulls.
    /// </summary>
    /// <param name="x">The first comparable object to compare.</param>
    /// <param name="y">The second comparable object to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of x and y:
    /// - Less than zero: x is less than y.
    /// - Zero: x equals y.
    /// - Greater than zero: x is greater than y.
    /// </returns>
    public int Compare(IComparable<T>? x, IComparable<T>? y)
    {
        // Compare nulls according MSDN specification

        // Two nulls are equal
        if (x == null && y == null)
            return 0;

        // Any object is greater than null
        if (x != null && y == null)
            return 1;

        if (x == null)
            return -1;

        // Otherwise compare the two values
        return x.CompareTo((T)y!);
    }
}
