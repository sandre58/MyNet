// -----------------------------------------------------------------------
// <copyright file="FileNotification.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Legacy.Notifications;
using MyNet.UI.Resources;

namespace MyNet.UI.Services.Handlers;

/// <summary>
/// Represents a notification for a file operation, such as a file download or export.
/// Provides a command to open the file and customizes the notification message and title.
/// </summary>
public class FileNotification : ClosableNotification
{
    /// <summary>
    /// Gets the path of the file associated with the notification.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the command to open the file.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileNotification"/> class.
    /// </summary>
    /// <param name="filePath">The path of the file associated with the notification.</param>
    /// <param name="openAction">The action to execute when opening the file.</param>
    /// <param name="message">The notification message. If null, a default message is used.</param>
    /// <param name="title">The notification title. If null, a default title is used.</param>
    /// <param name="severity">The severity of the notification. Default is Success.</param>
    /// <param name="commandFactory">Optional command factory used to create the open-file command.</param>
    public FileNotification(string filePath, Action<string> openAction, string? message = null, string? title = null, NotificationSeverity severity = NotificationSeverity.Success, ICommandFactory? commandFactory = null)
        : base(message ?? MessageResources.DownloadFileSuccess, title ?? UiResources.DownloadFile, severity)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;
        FilePath = filePath;
        OpenFileCommand = commands.Create(() => openAction(FilePath));
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FileNotification other && Equals(FilePath, other.FilePath) && Equals(Title, other.Title);

    /// <inheritdoc/>
    public override int GetHashCode() => FilePath.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override string ToString() => FilePath;
}
