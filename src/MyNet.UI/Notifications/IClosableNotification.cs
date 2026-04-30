// -----------------------------------------------------------------------
// <copyright file="IClosableNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.UI.ViewModels.Common;

namespace MyNet.UI.Notifications;

/// <summary>
/// Defines the contract for a notification that can be closed by the user or programmatically.
/// </summary>
public interface IClosableNotification : INotification, IClosable
{
    /// <summary>
    /// Occurs when a close request is raised by legacy notification consumers.
    /// </summary>
    event EventHandler<CancelEventArgs>? CloseRequest;

    /// <summary>
    /// Gets a value indicating whether the notification can be closed.
    /// </summary>
    bool IsClosable { get; }

    /// <summary>
    /// Requests notification closure.
    /// </summary>
    void Close();
}
