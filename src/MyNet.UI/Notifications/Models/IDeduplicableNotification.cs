// -----------------------------------------------------------------------
// <copyright file="IDeduplicableNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Notifications.Models;

/// <summary>
/// Defines the contract for a notification that can be deduplicated, allowing the system to identify and handle duplicate notifications appropriately.
/// </summary>
public interface IDeduplicableNotification : INotification
{
    /// <summary>
    /// Determines whether this notification is a duplicate of another.
    /// </summary>
    bool IsDuplicateOf(INotification other);
}
