// -----------------------------------------------------------------------
// <copyright file="UnknownValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Represents a singleton object used to indicate an unknown value in scenarios where a specific value is not available or cannot be determined. This can be useful in various contexts, such as when working with data that may have missing or undefined values, allowing developers to use this instance as a placeholder for unknown or unavailable information.
/// </summary>
public static class UnknownValue
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="UnknownValue"/> class, which can be used to represent an unknown value in various contexts. This instance is immutable and can be safely shared across different parts of an application to indicate the presence of an unknown or undefined value without the need for null references or other special handling.
    /// </summary>
    public static readonly object Instance = new();
}
