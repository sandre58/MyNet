// -----------------------------------------------------------------------
// <copyright file="IClosableNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Defines the contract for a notification that can be closed by the user or programmatically.
/// </summary>
public interface IClosableNotification : INotification, IClosable
{
    /// <summary>
    /// Gets a value indicating whether the notification can be closed.
    /// </summary>
    bool IsClosable { get; }

    /// <summary>
    /// Requests that the notification is closed.
    /// </summary>
    void RequestClose();
}
