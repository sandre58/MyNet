// -----------------------------------------------------------------------
// <copyright file="ISimilar.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities;

/// <summary>
/// Defines a contract for objects that can determine similarity with other objects.
/// </summary>
public interface ISimilar
{
    /// <summary>
    /// Determines whether the current object is similar to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the objects are similar; otherwise <c>false</c>.</returns>
    bool IsSimilar(object? obj);
}

/// <summary>
/// Defines a contract for objects that can determine similarity with other objects of the same type.
/// </summary>
/// <typeparam name="T">The type of objects to compare for similarity.</typeparam>
public interface ISimilar<in T>
{
    /// <summary>
    /// Determines whether the current object is similar to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the objects are similar; otherwise <c>false</c>.</returns>
    bool IsSimilar(T? obj);
}
