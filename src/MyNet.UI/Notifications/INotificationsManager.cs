// -----------------------------------------------------------------------
// <copyright file="INotificationsManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using MyNet.UI.Notifications.Models;

namespace MyNet.UI.Notifications;

/// <summary>
/// Defines the contract for a manager that handles notifications and their lifecycle.
/// </summary>
public interface INotificationsManager
{
    /// <summary>
    /// Gets the collection of notifications managed by this manager.
    /// </summary>
    ReadOnlyObservableCollection<INotification> Notifications { get; }

    /// <summary>
    /// Removes a specific notification from the manager.
    /// </summary>
    /// <param name="notification">The notification to remove.</param>
    void Remove(INotification notification);

    /// <summary>
    /// Clears all notifications.
    /// </summary>
    void Clear();
}
