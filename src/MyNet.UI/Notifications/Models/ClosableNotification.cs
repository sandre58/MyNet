// -----------------------------------------------------------------------
// <copyright file="ClosableNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Represents a notification that can be closed by the user or programmatically.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ClosableNotification"/> class.
/// </remarks>
/// <param name="message">The message content.</param>
/// <param name="title">The title of the notification.</param>
/// <param name="severity">The severity of the notification.</param>
/// <param name="isClosable">Indicates whether the notification can be closed.</param>
public class ClosableNotification(string message, string title, NotificationSeverity severity, bool isClosable = true) : NotificationBase(message, title, severity), IClosableNotification
{
    /// <summary>
    /// Gets a value indicating whether the notification can be closed.
    /// </summary>
    public bool IsClosable { get; } = isClosable;

    /// <summary>
    /// Event raised when a request to close the notification is made, either by the user or programmatically.
    /// </summary>
    public event EventHandler<CloseRequestedEventArgs>? CloseRequested;

    /// <summary>
    /// Determines asynchronously whether the notification can be closed.
    /// </summary>
    /// <returns>A task that returns true if the notification can be closed; otherwise, false.</returns>
    public Task<bool> CanCloseAsync() => Task.FromResult(IsClosable);

    /// <summary>
    /// Closes the notification and raises the <see cref="CloseRequested"/> event.
    /// </summary>
    public void RequestClose() => CloseRequested?.Invoke(this, new());
}
