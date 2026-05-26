// -----------------------------------------------------------------------
// <copyright file="ReverseComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Primitives.Comparers;

/// <summary>
/// A comparer that reverses the order of an inner comparer.
/// </summary>
/// <param name="inner">The inner comparer to reverse.</param>
/// <typeparam name="T">The type of elements to compare.</typeparam>
public sealed class ReverseComparer<T>(IComparer<T> inner) : IComparer<T>
{
    /// <inheritdoc/>
    public int Compare(T? x, T? y) => inner.Compare(y, x);
}
