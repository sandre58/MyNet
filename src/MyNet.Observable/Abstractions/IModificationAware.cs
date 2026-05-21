// -----------------------------------------------------------------------
// <copyright file="IModificationAware.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for objects that can track whether they have been modified, with a read-only property.
/// </summary>
public interface IModificationAware
{
    /// <summary>
    /// Gets a value indicating whether the object has been modified since the last reset.
    /// </summary>
    bool IsModified { get; }

    /// <summary>
    /// Resets the modification state of the object.
    /// </summary>
    void ResetModified();
}
