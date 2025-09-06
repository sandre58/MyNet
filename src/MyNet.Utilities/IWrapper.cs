// -----------------------------------------------------------------------
// <copyright file="IWrapper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities;

/// <summary>
/// Represents a wrapper that exposes an inner item.
/// </summary>
/// <typeparam name="T">The type of the wrapped item.</typeparam>
public interface IWrapper<out T>
{
    /// <summary>
    /// Gets the wrapped item instance.
    /// </summary>
    T Item { get; }
}
