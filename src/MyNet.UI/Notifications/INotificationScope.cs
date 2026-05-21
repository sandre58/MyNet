// -----------------------------------------------------------------------
// <copyright file="INotificationScope.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Notifications;

/// <summary>
/// Defines the contract for a notification scope, which can be used to categorize or group notifications.
/// </summary>
public interface INotificationScope
{
    /// <summary>
    /// Gets the scope identifier.
    /// </summary>
    string Name { get; }
}
