// -----------------------------------------------------------------------
// <copyright file="IDeferrer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Deferring;

/// <summary>
/// Defines a contract for objects that can defer execution of operations until a later time.
/// </summary>
public interface IDeferrer
{
    /// <summary>
    /// Gets a value indicating whether operations are currently being deferred.
    /// </summary>
    bool IsDeferred { get; }

    /// <summary>
    /// Starts deferring operations and returns a disposable that will execute deferred operations when disposed.
    /// </summary>
    /// <returns>A disposable object that triggers execution of deferred operations when disposed.</returns>
    IDisposable Defer();

    /// <summary>
    /// Executes all deferred operations immediately.
    /// </summary>
    void Execute();

    /// <summary>
    /// Executes deferred operations if not currently deferring, otherwise adds the operation to the deferred queue.
    /// </summary>
    void DeferOrExecute();
}
