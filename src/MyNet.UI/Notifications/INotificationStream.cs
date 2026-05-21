// -----------------------------------------------------------------------
// <copyright file="INotificationStream.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Notifications;

/// <summary>
/// Defines the contract for a notification stream, which provides an observable stream of notifications that subscribers can subscribe to in order to receive notifications as they are published.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "The suffix 'Stream' is appropriate to indicate that this is a stream of notifications.")]
public interface INotificationStream
{
    /// <summary>
    /// Gets an observable stream of notifications that subscribers can subscribe to in order to receive notifications as they are published.
    /// </summary>
    IObservable<INotification> Notifications { get; }
}
