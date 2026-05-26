// -----------------------------------------------------------------------
// <copyright file="ReferenceEqualityComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MyNet.Primitives.Comparers;

/// <summary>
/// Provides a default instance of a reference equality comparer for objects.
/// </summary>
public static class ReferenceEqualityComparer
{
    /// <summary>
    /// Gets a singleton instance of <see cref="ReferenceEqualityComparer"/>.
    /// </summary>
    public static ReferenceEqualityComparer<object> Instance { get; } = new();
}

/// <summary>
/// An equality comparer that compares objects by reference rather than by value.
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
public class ReferenceEqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceEqualityComparer{T}"/> class.
    /// </summary>
    internal ReferenceEqualityComparer() { }

    /// <summary>
    /// Returns a hash code for the specified object based on its reference.
    /// </summary>
    public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);

    /// <summary>
    /// Determines whether the specified objects are the same instance (reference equality).
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns><c>true</c> if the specified objects are the same instance; otherwise, <c>false</c>.</returns>
    bool IEqualityComparer.Equals(object? x, object? y) => ReferenceEquals(x, y);

    /// <summary>
    /// Determines whether the specified objects of type <typeparamref name="T"/> are the same instance (reference equality).
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns><c>true</c> if the specified objects are the same instance; otherwise, <c>false</c>.</returns>
    bool IEqualityComparer<T>.Equals(T? x, T? y) => ReferenceEquals(x, y);

    /// <summary>
    /// Returns a hash code for the specified object of type <typeparamref name="T"/> based on its reference.
    /// </summary>
    /// <param name="obj">The object for which to get the hash code.</param>
    /// <returns>A hash code for the specified object.</returns>
    int IEqualityComparer<T>.GetHashCode(T? obj) => RuntimeHelpers.GetHashCode(obj);
}
