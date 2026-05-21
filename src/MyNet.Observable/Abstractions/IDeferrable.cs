// -----------------------------------------------------------------------
// <copyright file="IDeferrable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for objects that support deferring notifications. This interface allows implementing classes to provide a mechanism for temporarily postponing the execution of notifications, which can be useful in scenarios where multiple changes are made in quick succession and you want to avoid sending out notifications for each individual change, instead sending a single notification after all changes are complete. The Defer method returns an IDisposable that, when disposed, will trigger the execution of all deferred notifications. This pattern is commonly used in observable objects to optimize performance and reduce unnecessary notifications, especially when making batch updates to properties or collections. Implementing this interface allows for more efficient notification handling and can improve the responsiveness of applications that rely on observable patterns, such as those using data binding in UI frameworks.
/// </summary>
public interface IDeferrable
{
    /// <summary>
    /// Defers the execution of notifications until the returned <see cref="IDisposable"/> is disposed. This allows batching multiple changes together and sending a single notification when the batch is complete. The exact behavior of how notifications are deferred and sent may depend on the implementation, but typically, while the returned <see cref="IDisposable"/> is not disposed, notifications will be queued or suppressed, and once it is disposed, all queued notifications will be sent out in a single batch.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, will trigger the execution of all deferred notifications.</returns>
    IDisposable Defer();
}
