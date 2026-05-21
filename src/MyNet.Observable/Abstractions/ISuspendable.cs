// -----------------------------------------------------------------------
// <copyright file="ISuspendable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for objects that can be temporarily suspended, allowing for the suppression of notifications or events during the suspension period. The <see cref="Suspend"/> method returns an <see cref="IDisposable"/> that, when disposed, will resume the normal operation of the object and re-enable any suppressed notifications or events.
/// </summary>
public interface ISuspendable
{
    /// <summary>
    /// Suspends the object, suppressing notifications or events until the returned <see cref="IDisposable"/> is disposed. This method allows for temporary suspension of the object's normal behavior, which can be useful in scenarios where multiple changes need to be made without triggering notifications for each change. Once the returned <see cref="IDisposable"/> is disposed, the object will resume its normal operation and any suppressed notifications or events will be re-enabled.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, will resume the normal operation of the object and re-enable any suppressed notifications or events.</returns>
    IDisposable Suspend();
}
