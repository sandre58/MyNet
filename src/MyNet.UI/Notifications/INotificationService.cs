// -----------------------------------------------------------------------
// <copyright file="INotificationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Notifications;

/// <summary>
/// Defines the contract for a notification service that combines both publishing and streaming capabilities for notifications.
/// </summary>
public interface INotificationService : INotificationPublisher, INotificationStream;
