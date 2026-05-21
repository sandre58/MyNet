// -----------------------------------------------------------------------
// <copyright file="SuspendableBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Disposables;
using MyNet.Utilities.Suspending;

namespace MyNet.Observable.Behaviors;

public abstract class SuspendableBehavior(ObservableObject owner) : ObservableBehavior(owner), ISuspendable
{
    private readonly Suspender _suspender = new();

    /// <summary>
    /// Gets a value indicating whether property changed notifications for culture changes are currently suspended. This can be useful to check if notifications are currently suspended before performing certain actions that may trigger property changed notifications, to avoid unnecessary processing or to ensure that certain operations are performed only when notifications are not suspended. When this property returns true, it indicates that notifications for culture changes are currently suspended, and any property changed notifications that would normally be raised in response to culture changes will be deferred until the suspension is lifted.
    /// </summary>
    public bool IsSuspended => _suspender.IsSuspended;

    /// <summary>
    /// Suspends property changed notifications for culture changes until the returned scope is disposed. This can be useful when multiple properties need to be updated in response to a culture change, to prevent multiple notifications from being raised. When the returned scope is disposed, any deferred property changed notifications that were raised during the suspension will be replayed, allowing the object to react to the culture change and update its display accordingly.
    /// </summary>
    /// <returns>A disposable suspension scope.</returns>
    public IDisposable Suspend() => IsDisposed ? Disposable.Empty : _suspender.Suspend();
}
