// -----------------------------------------------------------------------
// <copyright file="IToast.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Windows.Input;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Settings;

namespace MyNet.UI.Toasting.Models;

/// <summary>
/// Defines the contract for a toast notification, which includes the notification content, settings, visibility status, and associated commands for user interactions such as clicking or closing the toast.
/// </summary>
public interface IToast
{
    /// <summary>
    /// Gets the notification associated with this toast, which contains the message, title, severity, and any additional data relevant to the notification being displayed as a toast.
    /// </summary>
    INotification Notification { get; }

    /// <summary>
    /// Gets the settings for this toast, which may include properties such as duration, position, animation, and other visual or behavioral configurations that determine how the toast is displayed and interacts with the user.
    /// </summary>
    ToastSettings Settings { get; }

    /// <summary>
    /// Gets a value indicating whether the toast is currently visible.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// Gets the command to execute when the toast is clicked.
    /// </summary>
    ICommand? ClickCommand { get; }

    /// <summary>
    /// Gets the command to execute when the toast is requested to be closed.
    /// </summary>
    ICommand? CloseCommand { get; }
}
