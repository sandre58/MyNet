// -----------------------------------------------------------------------
// <copyright file="ICloneable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities;

/// <summary>
/// Represents a type that can create a copy of itself.
/// </summary>
/// <typeparam name="T">The concrete type returned by <see cref="Clone"/>.</typeparam>
public interface ICloneable<out T>
{
    /// <summary>
    /// Creates a deep or shallow copy of the current instance, depending on the implementation.
    /// </summary>
    /// <returns>A copy of the current instance.</returns>
    T Clone();
}
