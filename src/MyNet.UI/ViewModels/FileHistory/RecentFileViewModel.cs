// -----------------------------------------------------------------------
// <copyright file="RecentFileViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.IO;
using MyNet.IO.FileHistory;
using MyNet.Primitives;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.MessageBox;
using MyNet.UI.Helpers;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Resources;
using MyNet.UI.Services;
using MyNet.UI.Services.FileHistory;

namespace MyNet.UI.ViewModels.FileHistory;

/// <summary>
/// View model for a single recent file entry.
/// </summary>
public sealed class RecentFileViewModel : ViewModelBase
{
    private readonly IRecentFilesOperations _recentFilesOperations;
    private readonly IRecentFileCommandsService _recentFileCommandsService;
    private readonly IDialogService _dialogService;
    private readonly INotificationPublisher _notificationPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecentFileViewModel"/> class.
    /// </summary>
    public RecentFileViewModel(
        RecentFile recentFile,
        IRecentFilesOperations recentFilesOperations,
        IRecentFileCommandsService recentFileCommandsService,
        IDialogService dialogService,
        INotificationPublisher notificationPublisher,
        ICommandFactory? commandFactory = null)
    {
        ArgumentNullException.ThrowIfNull(recentFile);

        _recentFilesOperations = recentFilesOperations ?? throw new ArgumentNullException(nameof(recentFilesOperations));
        _recentFileCommandsService = recentFileCommandsService ?? throw new ArgumentNullException(nameof(recentFileCommandsService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));

        Path = recentFile.Path;
        Update(recentFile);

        var commands = commandFactory ?? RelayCommandFactory.Default;
        OpenCommand = commands.Create(() => OpenAsync(), FileExists);
        OpenCopyCommand = commands.Create(() => OpenCopyAsync(), () => !IsRecoveredFile && FileExists());
        OpenFolderLocationCommand = commands.Create(() => IoHelper.OpenFolderLocation(Path, _notificationPublisher), () => !IsRecoveredFile);
        RemoveFromListCommand = commands.Create(() => RemoveFromListAsync(), () => !IsRecoveredFile);
        RemoveFileCommand = commands.Create(() => RemoveFileAsync(), FileExists);
        PinCommand = commands.Create(() => SetPinnedAsync(true), () => !IsPinned && !IsRecoveredFile);
        PinOffCommand = commands.Create(() => SetPinnedAsync(false), () => IsPinned && !IsRecoveredFile);
    }

    /// <summary>
    /// Gets the file path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string? DisplayName { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the last access date.
    /// </summary>
    public DateTime? LastAccessDate { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the last modification date.
    /// </summary>
    public DateTime? ModificationDate { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets a value indicating whether the file is pinned.
    /// </summary>
    public bool IsPinned { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets a value indicating whether the file is a recovered document.
    /// </summary>
    public bool IsRecoveredFile { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the preview image bytes.
    /// </summary>
    public byte[]? Image { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the command that opens the file.
    /// </summary>
    public ICommand OpenCommand { get; }

    /// <summary>
    /// Gets the command that opens a copy of the file.
    /// </summary>
    public ICommand OpenCopyCommand { get; }

    /// <summary>
    /// Gets the command that opens the containing folder.
    /// </summary>
    public ICommand OpenFolderLocationCommand { get; }

    /// <summary>
    /// Gets the command that removes the entry from the recent-files list.
    /// </summary>
    public ICommand RemoveFromListCommand { get; }

    /// <summary>
    /// Gets the command that deletes the file from disk and removes it from the list.
    /// </summary>
    public ICommand RemoveFileCommand { get; }

    /// <summary>
    /// Gets the command that pins the file.
    /// </summary>
    public ICommand PinCommand { get; }

    /// <summary>
    /// Gets the command that unpins the file.
    /// </summary>
    public ICommand PinOffCommand { get; }

    /// <summary>
    /// Gets a value indicating whether the file exists on disk.
    /// </summary>
    public bool FileExists() => File.Exists(Path);

    /// <summary>
    /// Loads the preview image asynchronously.
    /// </summary>
    public async Task LoadImageAsync(CancellationToken cancellationToken = default)
    {
        if (!FileExists())
            return;

        await ExecuteSafeAsync(
            async _ => Image = await _recentFileCommandsService.GetImageAsync(Path).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens the file.
    /// </summary>
    public async Task OpenAsync()
    {
        if (IsRecoveredFile)
            await _recentFileCommandsService.OpenCopyAsync(Path).ConfigureAwait(false);
        else
            await _recentFileCommandsService.OpenAsync(Path).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens a copy of the file.
    /// </summary>
    public async Task OpenCopyAsync() => await _recentFileCommandsService.OpenCopyAsync(Path).ConfigureAwait(false);

    /// <summary>
    /// Updates the view model from the domain model.
    /// </summary>
    public void Update(RecentFile recentFile)
    {
        ArgumentNullException.ThrowIfNull(recentFile);

        DisplayName = recentFile.Name;
        LastAccessDate = recentFile.LastAccessedAt?.LocalDateTime;
        ModificationDate = recentFile.LastModifiedAt?.LocalDateTime;
        IsPinned = recentFile.IsPinned;
        IsRecoveredFile = recentFile.IsRecovered;
    }

    private async Task RemoveFromListAsync() => await _recentFilesOperations.RemoveAsync(Path).ConfigureAwait(false);

    private async Task RemoveFileAsync()
    {
        var result = await _dialogService
            .ShowQuestionAsync(MessageResources.FileRemovingQuestion, UiResources.Removing)
            .ConfigureAwait(false);

        if (result != MessageBoxResult.Yes)
            return;

        if (FileHelper.TryDeleteFile(Path))
        {
            await _recentFilesOperations.RemoveAsync(Path).ConfigureAwait(false);
            _notificationPublisher.Publish(
                new MessageNotification(
                    MessageResources.FileXRemovedSuccess.FormatWith(CultureInfo.CurrentCulture, Path),
                    severity: NotificationSeverity.Success));
        }
    }

    private async Task SetPinnedAsync(bool isPinned)
    {
        var updated = await _recentFilesOperations.SetPinnedAsync(Path, isPinned).ConfigureAwait(false);

        if (updated is not null)
            Update(updated);
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        Image = null;
        base.DisposeManagedResources();
    }
}
