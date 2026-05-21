// -----------------------------------------------------------------------
// <copyright file="FileNotificationHandler.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Legacy.Notifications;
using MyNet.UI.Messages;
using MyNet.UI.Toasting;
using MyNet.UI.Toasting.Settings;
using MyNet.Utilities.Messaging;

namespace MyNet.UI.Services.Handlers;

/// <summary>
/// Handles notifications related to file operations, such as file export events.
/// Displays toast notifications and notifies subscribers when a file is exported.
/// </summary>
public sealed class FileNotificationHandler : NotificationHandlerBase
{
    /// <summary>
    /// The category name for export file notifications.
    /// </summary>
    public static readonly string ExportFileCategory = "ExportFile";

    /// <summary>
    /// Initializes a new instance of the <see cref="FileNotificationHandler"/> class and subscribes to <see cref="FileExportedMessage"/> events.
    /// </summary>
    public FileNotificationHandler() => Messenger.Default?.Register<FileExportedMessage>(this, OnFileExportedMessage);

    /// <summary>
    /// Handles the <see cref="FileExportedMessage"/> by displaying a toast notification and notifying subscribers.
    /// </summary>
    /// <param name="obj">The file exported message containing file path and open action.</param>
    private void OnFileExportedMessage(FileExportedMessage obj)
    {
        var notification = new FileNotification(obj.FilePath, obj.OpenAction);
        ToasterManager.Show(notification, new() { ClosingStrategy = ToastClosingStrategy.AutoClose }, true, _ => obj.OpenAction(obj.FilePath));

        Notify(notification);
    }

    /// <summary>
    /// Cleans up resources and unregisters from <see cref="FileExportedMessage"/> events.
    /// </summary>
    protected override void Cleanup() => Messenger.Default?.Unregister(this);
}
