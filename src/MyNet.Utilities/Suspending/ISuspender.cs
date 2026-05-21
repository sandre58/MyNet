// -----------------------------------------------------------------------
// <copyright file="ISuspender.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Suspending;

/// <summary>
/// Defines a contract for objects that can suspend and allow operations, typically used to temporarily disable certain behaviors or notifications.
/// </summary>
public interface ISuspender
{
    /// <summary>
    /// Gets a value indicating whether operations are currently suspended.
    /// </summary>
    bool IsSuspended { get; }

    /// <summary>
    /// Suspends operations and returns an <see cref="IDisposable"/> that can be used to resume them.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, will resume operations.</returns>
    IDisposable Suspend();

    /// <summary>
    /// Allows operations and returns an <see cref="IDisposable"/> that can be used to suspend them again.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, will suspend operations.</returns>
    IDisposable Resume();
}
