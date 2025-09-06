// -----------------------------------------------------------------------
// <copyright file="IModifiable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities;

/// <summary>
/// Defines a contract for objects that can track whether they have been modified.
/// </summary>
public interface IModifiable
{
    /// <summary>
    /// Resets the modification state, marking the object as unmodified.
    /// </summary>
    void ResetIsModified();

    /// <summary>
    /// Determines whether the object has been modified since the last reset.
    /// </summary>
    /// <returns><c>true</c> if the object has been modified; otherwise <c>false</c>.</returns>
    bool IsModified();
}
