// -----------------------------------------------------------------------
// <copyright file="ISettable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities;

/// <summary>
/// Defines a contract for objects that can be set from another object of the same type.
/// </summary>
/// <typeparam name="T">The type of object to set from.</typeparam>
public interface ISettable<in T>
{
    /// <summary>
    /// Sets the current object's properties from the specified source object.
    /// </summary>
    /// <param name="from">The source object to copy properties from, or null to reset to defaults.</param>
    void SetFrom(T? from);
}

/// <summary>
/// Defines a contract for objects that can be set from another object.
/// </summary>
public interface ISettable
{
    /// <summary>
    /// Sets the current object's properties from the specified source object.
    /// </summary>
    /// <param name="from">The source object to copy properties from, or null to reset to defaults.</param>
    void SetFrom(object? from);
}
