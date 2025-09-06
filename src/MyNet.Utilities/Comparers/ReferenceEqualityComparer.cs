// -----------------------------------------------------------------------
// <copyright file="ReferenceEqualityComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MyNet.Utilities.Comparers;

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
    internal ReferenceEqualityComparer() { }

    /// <summary>
    /// Returns a hash code for the specified object based on its reference.
    /// </summary>
    public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);

    bool IEqualityComparer.Equals(object? x, object? y) => ReferenceEquals(x, y);

    bool IEqualityComparer<T>.Equals(T? x, T? y) => ReferenceEquals(x, y);

    int IEqualityComparer<T>.GetHashCode(T? obj) => RuntimeHelpers.GetHashCode(obj);
}
