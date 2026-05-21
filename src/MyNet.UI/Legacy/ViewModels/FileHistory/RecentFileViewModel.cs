// -----------------------------------------------------------------------
// <copyright file="RecentFileViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.Observable;
using MyNet.UI.Commands;
using MyNet.UI.Extensions;
using MyNet.UI.Helpers;
using MyNet.UI.Legacy.Dialogs;
using MyNet.UI.Legacy.Dialogs.MessageBox;
using MyNet.UI.Legacy.Loading;
using MyNet.UI.Resources;
using MyNet.UI.Services;
using MyNet.UI.Toasting;
using MyNet.Utilities;
using MyNet.Utilities.IO;
using MyNet.Utilities.IO.FileHistory;

namespace MyNet.UI.Legacy.ViewModels.FileHistory;

public class RecentFileViewModel : ObservableObject, IIdentifiable<Guid>
{
    private readonly IRecentFileCommandsService _recentFileCommandsService;
    private readonly RecentFilesManager _recentFilesManager;

    public Guid Id { get; } = Guid.NewGuid();

    public string? Name { get; private set; }

    public string Path { get; }

    public DateTime? LastAccessDate { get; private set; }

    public DateTime? ModificationDate { get; private set; }

    public bool IsPinned { get; private set; }

    public bool IsRecoveredFile { get; private set; }

    public byte[]? Image { get; private set; }

    public IBusyService BusyService { get; set; }

    public ICommand OpenFolderLocationCommand { get; }

    public ICommand RemoveCommand { get; }

    public ICommand RemoveFileCommand { get; }

    public ICommand PinCommand { get; }

    public ICommand PinOffCommand { get; }

    public ICommand OpenCopyCommand { get; }

    public ICommand OpenCommand { get; }

    public RecentFileViewModel(
        RecentFile recentFile,
        RecentFilesManager recentFilesManager,
        IRecentFileCommandsService recentFileCommandsService)
    {
        _recentFileCommandsService = recentFileCommandsService;
        _recentFilesManager = recentFilesManager;

        Path = recentFile.Path;
        Update(recentFile);
        BusyService = BusyManager.Create();

        OpenCommand = RelayCommandFactory.Default.Create(async () => await OpenAsync().ConfigureAwait(false), Exists);
        OpenCopyCommand = RelayCommandFactory.Default.Create(async () => await OpenCopyAsync().ConfigureAwait(false), () => !IsRecoveredFile && Exists());
        OpenFolderLocationCommand = RelayCommandFactory.Default.Create(() => IoHelper.OpenFolderLocation(Path), () => !IsRecoveredFile);
        RemoveCommand = RelayCommandFactory.Default.Create(Remove, () => !IsRecoveredFile);
        RemoveFileCommand = RelayCommandFactory.Default.Create(async () => await RemoveFileAsync().ConfigureAwait(false), Exists);
        PinCommand = RelayCommandFactory.Default.Create(() => SetIsPinned(true), () => !IsPinned && !IsRecoveredFile);
        PinOffCommand = RelayCommandFactory.Default.Create(() => SetIsPinned(false), () => IsPinned && !IsRecoveredFile);
    }

    public bool Exists() => File.Exists(Path);

    public async Task LoadImageAsync()
    {
        if (Exists())
            await BusyService.RunIndeterminateAsync(async () => Image = await _recentFileCommandsService.GetImageAsync(Path).ConfigureAwait(false)).ConfigureAwait(false);
    }

    public async Task OpenAsync()
    {
        if (IsRecoveredFile)
            await _recentFileCommandsService.OpenCopyAsync(Path).ConfigureAwait(false);
        else
            await _recentFileCommandsService.OpenAsync(Path).ConfigureAwait(false);
    }

    public async Task OpenCopyAsync() => await _recentFileCommandsService.OpenCopyAsync(Path).ConfigureAwait(false);

    public void Remove() => _recentFilesManager.Remove(Path);

    public async Task RemoveFileAsync()
    {
        if (await DialogManager.ShowQuestionAsync(MessageResources.FileRemovingQuestion, UiResources.Removing).ConfigureAwait(false) == MessageBoxResult.Yes)
        {
            _ = FileHelper.RemoveFile(Path);
            _recentFilesManager.Remove(Path);
            ToasterManager.ShowSuccess(MessageResources.FileXRemovedSuccess.FormatWith(Path));
        }
    }

    public void Update(RecentFile recentFile)
    {
        Name = recentFile.Name;
        LastAccessDate = recentFile.LastAccessDate;
        ModificationDate = recentFile.ModificationDate;
        IsPinned = recentFile.IsPinned;
        IsRecoveredFile = recentFile.IsRecoveredFile;
    }

    private void SetIsPinned(bool isPinned)
    {
        IsPinned = isPinned;
        _recentFilesManager.Update(Path, isPinned);
    }

    protected override void Cleanup()
    {
        Image = null;
        base.Cleanup();
    }
}
