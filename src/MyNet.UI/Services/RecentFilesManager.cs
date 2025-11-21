// -----------------------------------------------------------------------
// <copyright file="RecentFilesManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Messages;
using MyNet.Utilities.IO.FileHistory;
using MyNet.Utilities.Logging;
using MyNet.Utilities.Messaging;

namespace MyNet.UI.Services;

/// <summary>
/// Manages the list of recent files, providing methods to add, remove, and update recent files.
/// Sends notifications when the recent files list changes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RecentFilesManager"/> class.
/// </remarks>
/// <param name="recentFilesService">The service managing recent files.</param>
public sealed class RecentFilesManager(RecentFilesService recentFilesService)
{
    /// <summary>
    /// Adds a file to the recent files list and sends a notification.
    /// </summary>
    /// <param name="name">The display name of the file.</param>
    /// <param name="path">The path of the file.</param>
    public void Add(string name, string path)
    {
        using (LogManager.MeasureTime($"Add recent File : {name} | {path}", PerformanceTraceLevel.Debug))
        {
            _ = recentFilesService.Add(name, path);
            Messenger.Default?.Send(new RecentFilesChangedMessage());
        }
    }

    /// <summary>
    /// Removes a file from the recent files list and sends a notification.
    /// </summary>
    /// <param name="file">The path of the file to remove.</param>
    public void Remove(string file)
    {
        recentFilesService.Remove(file);
        Messenger.Default?.Send(new RecentFilesChangedMessage());
        LogManager.Debug($"Recent file removed : {file}");
    }

    /// <summary>
    /// Updates the pinned state of a file in the recent files list.
    /// </summary>
    /// <param name="file">The path of the file to update.</param>
    /// <param name="isPinned">True to pin the file; false to unpin.</param>
    public void Update(string file, bool isPinned) => recentFilesService.Update(file, isPinned);
}
