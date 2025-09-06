// -----------------------------------------------------------------------
// <copyright file="IIdentifiable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities;

/// <summary>
/// Represents an entity that exposes an identifier.
/// </summary>
/// <typeparam name="T">The type of the identifier.</typeparam>
public interface IIdentifiable<out T>
{
    /// <summary>
    /// Gets the identifier of the entity.
    /// </summary>
    T Id { get; }
}
