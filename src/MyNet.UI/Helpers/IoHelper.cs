// -----------------------------------------------------------------------
// <copyright file="IoHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.IO;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Resources;
using MyNet.Utilities;
using MyNet.Utilities.IO;

namespace MyNet.UI.Helpers;

/// <summary>
/// Provides helper methods for input/output operations, such as opening folders and handling file paths.
/// </summary>
public static class IoHelper
{
    /// <summary>
    /// Opens the folder containing the specified file path. If the directory does not exist, shows an error message.
    /// </summary>
    /// <param name="filePath">The file path whose containing folder should be opened.</param>
    /// <param name="notificationPublisher">Optional notification publisher used to report UI errors without static dependencies.</param>
    /// <returns><c>true</c> when the folder was opened; otherwise <c>false</c>.</returns>
    public static bool OpenFolderLocation(string filePath, INotificationPublisher? notificationPublisher = null)
    {
        var directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
        {
            ProcessHelper.OpenFolder(directory);
            return true;
        }

        var message = MessageResources.FileXNotFoundError.FormatWith(CultureInfo.CurrentCulture, filePath);
        notificationPublisher?.Publish(new MessageNotification(message, severity: NotificationSeverity.Error));
        return false;
    }
}
