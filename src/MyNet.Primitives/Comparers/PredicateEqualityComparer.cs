// -----------------------------------------------------------------------
// <copyright file="PredicateEqualityComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MyNet.Primitives.Comparers;

/// <summary>
/// An equality comparer that uses a predicate function to determine equality.
/// </summary>
/// <typeparam name="T">Type of objects to compare.</typeparam>
public class PredicateEqualityComparer<T>(Func<T, T, bool> predicate) : IEqualityComparer, IEqualityComparer<T>
{
    /// <summary>
    /// Returns a hash code for the specified object.
    /// This implementation uses <see cref="RuntimeHelpers.GetHashCode(object)"/> which is based on object identity.
    /// </summary>
    public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);

    /// <summary>
    /// Determines whether the specified objects are equal using the provided predicate function.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
    bool IEqualityComparer.Equals(object? x, object? y) => x is not null && y is not null && !ReferenceEquals(x, y) && predicate.Invoke((T)x, (T)y);

    /// <summary>
    /// Determines whether the specified objects of type <typeparamref name="T"/> are equal using the provided predicate function.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
    bool IEqualityComparer<T>.Equals(T? x, T? y) => x is not null && y is not null && !ReferenceEquals(x, y) && predicate.Invoke(x, y);

    /// <summary>
    /// Returns a hash code for the specified object of type <typeparamref name="T"/>.
    /// This implementation uses <see cref="RuntimeHelpers.GetHashCode(object)"/> which is based on object identity.
    /// </summary>
    /// <param name="obj">The object for which to get the hash code.</param>
    /// <returns>A hash code for the specified object.</returns>
    int IEqualityComparer<T>.GetHashCode(T? obj) => RuntimeHelpers.GetHashCode(obj);
}
